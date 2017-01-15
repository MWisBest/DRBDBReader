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
using DRBDBReader.DB.Converters;

namespace DRBDBReader.DB.Records
{
	public class TXRecord : Record
	{
		private const byte FIELD_ID = 0;
		private const byte FIELD_CONVERSION = 1;
		private const byte FIELD_DATA_AQU_DESC_ID = 2;
		private const byte FIELD_DATA_ELEM_SET_ID = 4;
		private const byte FIELD_TXBYTES = 6;
		private const byte FIELD_STRING_ID = 8;
		private const byte FIELD_SVCCAT_ID = 14;

		private const byte FIELD_DAD_REQUEST_LENGTH = 1;
		private const byte FIELD_DAD_RESPONSE_LENGTH = 3;
		private const byte FIELD_DAD_EXTRACT_OFFSET = 5;
		private const byte FIELD_DAD_EXTRACT_SIZE = 6;
		private const byte FIELD_DAD_PROTOCOL = 10;

		public long id;

		public ushort dadid;
		public byte dadreqlen;
		public byte dadresplen;
		public byte dadextroff;
		public byte dadextrsize;
		public ushort protocolid;

		public ushort dataelemsetid;

		public byte[] xmitbytes;
		public string xmitstring;

		public ushort nameid;
		public string name;

		public ushort scid;
		public string scname;

		public Converter converter;

		public TXRecord( Table table, byte[] record ) : base( table, record )
		{
			// get id
			this.id = this.table.readField( this, FIELD_ID );


			// get converter info
			byte[] convertfield = this.table.readFieldRaw( this, FIELD_CONVERSION );

			ushort dsid = (ushort)this.table.readInternal( convertfield, 2, 2 );
			ushort cfid = (ushort)this.table.readInternal( convertfield, 4, 2 );

			switch( convertfield[0] )
			{
				case 0:
					this.converter = new BinaryStateConverter( this.table.db, convertfield, cfid, dsid );
					break;
				case 17:
					this.converter = new NumericConverter( this.table.db, convertfield, cfid, dsid );
					break;
				case 32:
					this.converter = new StateConverter( this.table.db, convertfield, cfid, dsid );
					break;
				case 2:
				case 18:
				case 34:
					this.converter = new UnknownConverter( this.table.db, convertfield, cfid, dsid );
					break;
				default:
					this.converter = new Converter( this.table.db, convertfield, cfid, dsid );
					break;
			}

			// get protocol info
			this.dadid = (ushort)this.table.readField( this, FIELD_DATA_AQU_DESC_ID );
			Table dadTable = this.table.db.tables[Database.TABLE_DATA_ACQUISITION_DESCRIPTION];
			Record recordObjTwo = dadTable.getRecord( this.dadid );
			this.protocolid = (ushort)dadTable.readField( recordObjTwo, FIELD_DAD_PROTOCOL );
			this.dadreqlen = (byte)dadTable.readField( recordObjTwo, FIELD_DAD_REQUEST_LENGTH );
			this.dadresplen = (byte)dadTable.readField( recordObjTwo, FIELD_DAD_RESPONSE_LENGTH );
			this.dadextroff = (byte)dadTable.readField( recordObjTwo, FIELD_DAD_EXTRACT_OFFSET );
			this.dadextrsize = (byte)dadTable.readField( recordObjTwo, FIELD_DAD_EXTRACT_SIZE );


			// get data elem set stuff
			this.dataelemsetid = (ushort)this.table.readField( this, FIELD_DATA_ELEM_SET_ID );


			// get xmitbytes
			int index = this.table.getColumnOffset( FIELD_TXBYTES );
			this.xmitbytes = new byte[this.record[index]];
			Array.Copy( this.record, index + 1, this.xmitbytes, 0, this.xmitbytes.Length );

			this.xmitstring = BitConverter.ToString( this.xmitbytes );


			// get name
			this.nameid = (ushort)this.table.readField( this, FIELD_STRING_ID );
			string temp = this.table.db.getString( this.nameid );
			if( temp == null )
			{
				temp = "(null)";
			}
			this.name = temp;


			// get scid/scname
			this.scid = (ushort)((int)(this.table.readField( this, FIELD_SVCCAT_ID ) >> 8));
			this.scname = this.table.db.getServiceCatString( this.scid );
		}
	}
}
