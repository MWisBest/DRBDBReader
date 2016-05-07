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

		public float slope = 0.0F;

		public float offset = 0.0F;

		public NumericConverter( Database db, byte[] record, ushort cfid, ushort dsid ) : base( db, record, cfid, dsid )
		{
			Table numConvTable = this.db.tables[Database.TABLE_CONVERTERS_NUMERIC];
			Record numConvRecord = numConvTable.getRecord( this.cfid );

			if( numConvRecord != null )
			{
				this.slope = BitConverter.ToSingle( BitConverter.GetBytes( (int)numConvTable.readField( numConvRecord, FIELD_SLOPE ) ), 0 );

				this.offset = BitConverter.ToSingle( BitConverter.GetBytes( (int)numConvTable.readField( numConvRecord, FIELD_OFFSET ) ), 0 );
			}
			else
			{
				;
			}
		}
	}
}
