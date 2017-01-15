/*
 * DRBDBReader
 * Copyright (C) 2016-2017, Kyle Repinski
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.IO;
using DRBDBReader.DB.Records;

namespace DRBDBReader.DB
{
	public class Table
	{
		public uint offset;
		private ushort id;
		public ushort rowCount;
		public ushort rowSize;
		public byte colCount;
		public byte[] colSizes;
		public Record[] records;
		public Database db;

		// This saves us a ridiculous amount of memory.
		// GC catches it otherwise of course, but without this
		// we were making 1GB of allocations. Now it's not even 20MB.
		private byte[] empty = new byte[8];
		private byte[] scratch = new byte[8];

		public Table( Database db, ushort id, uint offset, ushort rowCount, ushort rowSize, byte colCount, byte[] colSizes )
		{
			this.db = db;
			this.id = id;
			this.offset = offset;
			this.rowCount = rowCount;
			this.rowSize = rowSize;
			this.colCount = colCount;
			this.colSizes = colSizes;
			this.records = new Record[rowCount];
		}

		public void readRecords()
		{
			long tempPos = this.db.reader.BaseStream.Position;

			this.db.reader.BaseStream.Seek( this.offset, SeekOrigin.Begin );

			// NOTE: loop unrolled purposely
			switch( this.id )
			{
				case Database.TABLE_DES_INFO:
					for( ushort i = 0; i < this.rowCount; ++i )
					{
						records[i] = new DESRecord( this, this.db.reader.ReadBytes( this.rowSize ) );
					}
					break;
				case Database.TABLE_SERIVCE_CAT_STUFFS:
					for( ushort i = 0; i < this.rowCount; ++i )
					{
						records[i] = new ServiceCatRecord( this, this.db.reader.ReadBytes( this.rowSize ) );
					}
					break;
				case Database.TABLE_TRANSMIT:
					for( ushort i = 0; i < this.rowCount; ++i )
					{
						records[i] = new TXRecord( this, this.db.reader.ReadBytes( this.rowSize ) );
					}
					break;
				case Database.TABLE_MODULE:
					for( ushort i = 0; i < this.rowCount; ++i )
					{
						records[i] = new ModuleRecord( this, this.db.reader.ReadBytes( this.rowSize ) );
					}
					break;
				default:
					for( ushort i = 0; i < this.rowCount; ++i )
					{
						records[i] = new Record( this, this.db.reader.ReadBytes( this.rowSize ) );
					}
					break;
			}

			this.db.reader.BaseStream.Seek( tempPos, SeekOrigin.Begin );
		}

		public Record getRecord( long key, byte idcol = 0 )
		{
			int min = 0;
			int max = this.rowCount - 1;
			int mid = ( min + max ) / 2;
			long val = 0L;

			do
			{
				val = this.readField( this.records[mid], idcol );

				if( val == key )
				{
					return this.records[mid];
				}

				if( val < key )
				{
					min = mid + 1;
				}
				else
				{
					max = mid - 1;
				}

				mid = ( min + max ) / 2;
			} while( min <= max );

			return null;
		}

		public int getColumnOffset( byte col )
		{
			int colOffset = 0;

			for( int i = 0; i < col; ++i )
			{
				colOffset += this.colSizes[i];
			}

			return colOffset;
		}

		public long readField( Record rec, byte col )
		{
			return this.readInternal( rec.record, this.getColumnOffset( col ), this.colSizes[col] );
		}
		
		public byte[] readFieldRaw( Record rec, byte col )
		{
			byte[] ret = new byte[this.colSizes[col]];
			Array.Copy( rec.record, this.getColumnOffset( col ), ret, 0, ret.Length );
			return ret;
		}

		public long readInternal( byte[] record, int colOffset, int colSize )
		{
			/* Previously, this method did something like this:
			 * byte[] scratch = new byte[8];
			 * The problem is, this method is called nearly 200,000 times.
			 * That was generating an obscene amount of garbage memory.
			 * By making 'scratch' an instance variable of this method's
			 * containing class, it can avoid all that garbage.
			 *
			 * Since 'scratch' needs to start out empty though, we also
			 * need to memcpy a spare empty array to it to clear it out.
			 * This is a million percent cheaper than allocating a new array.
			 */
			this.empty.CopyTo( this.scratch, 0 );

			if( !this.db.isStarScanDB )
			{
				/* While Array.Copy should be faster due to presumably being a native memcpy,
				 * it turns out that the combo of needing Array.Reverse as well is awful.
				 * So although the following code is slower than a memcpy, the fact that it
				 * avoids Array.Reverse makes it much much faster, especially for smaller fields.
				 */
				int idx = colOffset + colSize - 1;
				for( int i = 0; i < colSize; ++i )
				{
					this.scratch[i] = record[idx - i];
				}

				/* Old code, for reference. */
				//Array.Copy( rec.record, colOffset, this.scratch, 8 - sizze, sizze );
				//Array.Reverse( this.scratch );
			}
			else
			{
				/* StarSCAN's database already has endianness taken care of,
				 * so it's OK to just do a direct copy of the data.
				 */
				Array.Copy( record, colOffset, this.scratch, 0, colSize );
			}

			return BitConverter.ToInt64( this.scratch, 0 );
		}

		public List<Record> selectRecords( byte field, long key, bool sorted = false )
		{
			List<Record> ret = new List<Record>();
			long val;
			for( int i = 0; i < this.records.Length; ++i )
			{
				val = this.readField( this.records[i], field );
				if( val == key )
				{
					ret.Add( this.records[i] );
				}
				else if( sorted && ( val > key ) )
				{
					break;
				}
			}
			return ret;
		}

		public List<ushort> selectRecordsReturnIDs( byte field, long key, bool sorted = false )
		{
			List<ushort> ret = new List<ushort>();
			long val;
			for( ushort i = 0; i < this.rowCount; ++i )
			{
				val = this.readField( this.records[i], field );
				if( val == key )
				{
					ret.Add( i );
				}
				else if( sorted && ( val > key ) )
				{
					break;
				}
			}
			return ret;
		}
	}
}
