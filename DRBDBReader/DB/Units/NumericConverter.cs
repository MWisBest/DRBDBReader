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

namespace DRBDBReader.DB.Units
{
	public class NumericConverter : Converter
	{
		private const byte FIELD_SLOPE = 1;
		private const byte FIELD_OFFSET = 2;
		private const byte FIELD_NDS_UNIT = 6;

		public float slope;

		public float offset;

		public ushort unitid;
		public string unit;

		public NumericConverter( Database db, byte[] record, ushort cfid, ushort dsid ) : base( db, record, cfid, dsid )
		{
			Table numConvTable = this.db.tables[Database.TABLE_CONVERTERS_NUMERIC];
			Record numConvRecord = numConvTable.getRecord( this.cfid );

			this.slope = BitConverter.ToSingle( BitConverter.GetBytes( (int)numConvTable.readField( numConvRecord, FIELD_SLOPE ) ), 0 );
			this.offset = BitConverter.ToSingle( BitConverter.GetBytes( (int)numConvTable.readField( numConvRecord, FIELD_OFFSET ) ), 0 );

			Table ndsTable = this.db.tables[Database.TABLE_NUMERIC_DATA_SPECIFIER];
			Record ndsRecord = ndsTable.getRecord( this.dsid );
			this.unitid = (ushort)ndsTable.readField( ndsRecord, FIELD_NDS_UNIT );
			this.unit = ( this.unitid != 0 ? this.db.getString( this.unitid ) : "" );
		}

		public override string processData( byte[] data )
		{
			int val = BitConverter.ToInt32( data, 0 );
			double result = val * this.slope + this.offset;
			return result + " " + this.unit;
		}
	}
}
