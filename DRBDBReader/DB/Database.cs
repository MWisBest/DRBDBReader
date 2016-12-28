/*
 * DRBDBReader
 * Copyright (C) 2016, Kyle Repinski
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
using System.Linq;
using System.Text;

namespace DRBDBReader.DB
{
	public class Database
	{
		public const ushort TABLE_MODULE = 0;
		public const ushort TABLE_DES_INFO = 1;
		public const ushort TABLE_BINARY_DATA_SPECIFIER = 2;
		public const ushort TABLE_UNKNOWN_3 = 3;
		public const ushort TABLE_CONVERTERS_STATE = 4;
		public const ushort TABLE_CONVERTERS_NUMERIC = 5;
		public const ushort TABLE_SERIVCE_CAT_STUFFS = 6; // lolidk
		public const ushort TABLE_QUALIFIER = 7;
		public const ushort TABLE_DATA_ACQUISITION_DESCRIPTION = 8;
		public const ushort TABLE_UNKNOWN_9 = 9;
		public const ushort TABLE_MODULE_DATAELEMENT = 10;
		public const ushort TABLE_UNKNOWN_11 = 11;
		public const ushort TABLE_EMPTY_12 = 12;
		public const ushort TABLE_STATE_DATA_SPECIFIER = 13;
		public const ushort TABLE_UNKNOWN_14 = 14;
		public const ushort TABLE_STATE_ENTRY = 15;
		public const ushort TABLE_STATE = 16;
		public const ushort TABLE_NUMERIC_DATA_SPECIFIER = 17;
		public const ushort TABLE_DATAELEMENT_QUALIFIER = 18;
		public const ushort TABLE_UNKNOWN_19 = 19;
		public const ushort TABLE_UNKNOWN_20 = 20;
		public const ushort TABLE_UNKNOWN_21 = 21;
		public const ushort TABLE_UNKNOWN_22 = 22;
		public const ushort TABLE_TRANSMIT = 23;
		public const ushort TABLE_EMPTY_24 = 24;
		public const ushort TABLE_EMPTY_25 = 25;
		public const ushort TABLE_DBTEXT_1 = 26;
		public const ushort TABLE_DBTEXT_2 = 27;

		private FileInfo dbFile;
		private MemoryStream dbStream;
		public BinaryReader reader;
		public Table[] tables;

		private ushort[] recordReadOrder = {
			TABLE_DBTEXT_1,
			TABLE_DBTEXT_2,
			TABLE_STATE,

			TABLE_EMPTY_12,
			TABLE_EMPTY_24,
			TABLE_EMPTY_25,
			TABLE_UNKNOWN_3,
			TABLE_UNKNOWN_9,
			TABLE_UNKNOWN_11,
			TABLE_UNKNOWN_14,
			TABLE_UNKNOWN_19,
			TABLE_UNKNOWN_20,
			TABLE_UNKNOWN_21,
			TABLE_UNKNOWN_22,

			TABLE_BINARY_DATA_SPECIFIER,
			TABLE_NUMERIC_DATA_SPECIFIER,
			TABLE_STATE_DATA_SPECIFIER,

			TABLE_CONVERTERS_STATE,
			TABLE_CONVERTERS_NUMERIC,
			TABLE_DATA_ACQUISITION_DESCRIPTION,
			TABLE_STATE_ENTRY,
			TABLE_QUALIFIER,
			TABLE_DATAELEMENT_QUALIFIER,

			TABLE_DES_INFO, // must come before TABLE_TRANSMIT, maybe others?
			TABLE_SERIVCE_CAT_STUFFS, // must come before TABLE_TRANSMIT
			TABLE_TRANSMIT, // must come before TABLE_MODULE_DATAELEMENT
			TABLE_MODULE_DATAELEMENT, // must come before TABLE_MODULE
			TABLE_MODULE };

		public bool isStarScanDB;
		
		public Database( FileInfo dbFile )
		{
			this.dbFile = dbFile;

			/* Since we're going to need access to this data often, lets load it into a MemoryStream.
			 * With it being about 2.5MB it's fairly cheap.
			 */
			byte[] buffer;
			using( FileStream fs = new FileStream( this.dbFile.FullName, FileMode.Open, FileAccess.Read ) )
			{
				buffer = new byte[fs.Length];
				fs.Read( buffer, 0, buffer.Length );
			}
			this.dbStream = new MemoryStream( buffer );
			this.reader = new BinaryReader( this.dbStream );

			/* StarSCAN's database.mem has a different endianness;
			 * This detects and accounts for that as needed.
			 */
			this.isStarScanDB = this.checkStarScan();

			this.makeTables();
		}

		private bool checkStarScan()
		{
			bool ret;

			long tempPos = this.reader.BaseStream.Position;
			this.reader.BaseStream.Seek( this.reader.BaseStream.Length - 0x17, SeekOrigin.Begin );

			/* Rather than try and deal with converting this to a string etc.,
			 * it's cheaper to just work with and compare bytes directly. */
			byte[] starscanbytes = this.reader.ReadBytes( 8 );
			ret = starscanbytes.SequenceEqual( new byte[] { 0x53, 0x74, 0x61, 0x72, 0x53, 0x43, 0x41, 0x4E } );

			this.reader.BaseStream.Seek( tempPos, SeekOrigin.Begin );
			return ret;
		}

		private void makeTables()
		{
			uint fileSize = this.reader.ReadUInt32();
			ushort idk = this.reader.ReadUInt16();
			ushort numTables = this.reader.ReadUInt16();
			this.tables = new Table[numTables];
			for( ushort i = 0; i < numTables; ++i )
			{
				uint offset = this.reader.ReadUInt32();
				ushort rowCount = this.reader.ReadUInt16();
				ushort rowSize = this.reader.ReadUInt16();

				/* While technically the 'stated' code alone was correct, it had an issue:
				 * there are some columns with a size of 0! This is a waste.
				 * As such, empty columns are now removed and field IDs adjusted for that. */
				byte statedColCount = this.reader.ReadByte();
				byte[] statedColSizes = this.reader.ReadBytes( statedColCount );

				/* There's actually room reserved for 27 bytes after the statedColCount,
				 * so it is necessary to seek past whatever bytes that go unread. */
				this.reader.BaseStream.Seek( 27 - statedColCount, SeekOrigin.Current );

				List<byte> colSizes = new List<byte>();
				for( byte j = 0; j < statedColCount; ++j )
				{
					if( statedColSizes[j] != 0 )
					{
						colSizes.Add( statedColSizes[j] );
					}
				}

				this.tables[i] = new Table( this, i, offset, rowCount, rowSize, (byte)colSizes.Count, colSizes.ToArray<byte>() );
			}

			foreach( ushort x in recordReadOrder )
			{
				this.tables[x].readRecords();
			}
		}

		private StringBuilder cachedStateBuilder = new StringBuilder();
		private Dictionary<ushort, string> cachedStrings = new Dictionary<ushort, string>();

		public string getString( ushort id )
		{
			if( this.cachedStrings.ContainsKey( id ) )
			{
				return this.cachedStrings[id];
			}

			Table t = this.tables[TABLE_STATE];
			Record recordObj = t.getRecord( id );
			
			if( recordObj == null )
			{
				return "(null)";
			}

			cachedStateBuilder.Clear();

			int p = (int)t.readField( recordObj, 1 );
			Table txtTable = this.tables[(ushort)(TABLE_DBTEXT_1 + ( p >> 24 ))];

			int offset = ( p & 0xFFFFFF );
			int row = (int)Math.Floor( (double)( offset / txtTable.rowSize ) );
			int rowOffset = offset % txtTable.rowSize;
			Record txtRecord = txtTable.records[row];
			byte curByte;

			while( ( curByte = txtRecord.record[rowOffset] ) != 0 )
			{
				cachedStateBuilder.Append( Convert.ToChar( curByte ) );
				++rowOffset;
				if( rowOffset >= txtTable.rowSize )
				{
					rowOffset = 0;
					txtRecord = txtTable.records[++row];
				}
			}

			this.cachedStrings[id] = cachedStateBuilder.ToString();

			return this.cachedStrings[id];
		}

		public string getServiceCatString( ushort id )
		{
			Table t = this.tables[TABLE_SERIVCE_CAT_STUFFS];
			Record recordObj = t.getRecord( id, 3 );

			if( recordObj == null )
			{
				return "(null)";
			}

			ServiceCatRecord screc = (ServiceCatRecord)recordObj;

			return screc.name;
		}

		public string getDESString( ushort id )
		{
			Table t = this.tables[TABLE_DES_INFO];
			Record recordObj = t.getRecord( id, 0 );

			if( recordObj == null )
			{
				return "(null)";
			}

			DESRecord desrec = (DESRecord)recordObj;

			return desrec.name;
		}

		public string getProtocolText( ushort id )
		{
			switch( id )
			{
				case 1:
					return "J1850";
				case 53:
					return "CCD";
				case 60:
					return "SCI";
				case 159:
					return "Multimeter";
				case 160:
					return "J2190?";
				default:
					return "P" + id;
			}
		}

		public string getTX( long id )
		{	
			Table t = this.tables[TABLE_TRANSMIT];
			Record recordObj = t.getRecord( id );
			if( recordObj == null )
			{
				return null;
			}
			TXRecord txrec = (TXRecord)recordObj;

			string protocolTxt = this.getProtocolText( txrec.protocolid ) + "; ";

			return txrec.name + ": " + protocolTxt + "xmit: " + txrec.xmitstring + "; sc: " + txrec.scname;
		}

		public string getDetailedTX( long id )
		{
			Table t = this.tables[TABLE_TRANSMIT];
			Record recordObj = t.getRecord( id );
			if( recordObj == null )
			{
				return null;
			}
			TXRecord txrec = (TXRecord)recordObj;

			string protocolTxt = this.getProtocolText( txrec.protocolid ) + "; ";

			string detailText = "";
			detailText += Environment.NewLine + "dadreqlen: " + txrec.dadreqlen + "; dadresplen: " + txrec.dadresplen + ";";
			detailText += Environment.NewLine + "dadextroff: " + txrec.dadextroff + "; dadextrsize: " + txrec.dadextrsize + ";";
			detailText += Environment.NewLine + "desid: " + txrec.dataelemsetid + "; desname: " + this.getDESString( txrec.dataelemsetid ) + ";";
			detailText += Environment.NewLine + "record: " + BitConverter.ToString( txrec.record ) + ";";

			return txrec.name + ": " + protocolTxt + "xmit: " + txrec.xmitstring + "; sc: " + txrec.scname + ";" + detailText;
		}

		public string getModule( ushort id )
		{
			Table t = this.tables[TABLE_MODULE];
			Record recordObj = t.getRecord( id );
			if( recordObj == null )
			{
				return null;
			}
			ModuleRecord modrec = (ModuleRecord)recordObj;

			return modrec.name + "; sc: " + modrec.scname;
		}
	}
}
