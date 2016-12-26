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
	public class UnknownConverter : Converter
	{
		public Record dsrec;

		public UnknownConverter( Database db, byte[] record, ushort cfid, ushort dsid ) : base( db, record, cfid, dsid )
		{
			Table dstable;
			if( this.record[0] == 2 )
			{
				dstable = this.db.tables[Database.TABLE_BINARY_DATA_SPECIFIER];
				dsrec = dstable.getRecord( this.dsid );
			}
			else if( this.record[0] == 18 )
			{
				dstable = this.db.tables[Database.TABLE_NUMERIC_DATA_SPECIFIER];
				dsrec = dstable.getRecord( this.dsid );
			}
			else if( this.record[0] == 34 )
			{
				dstable = this.db.tables[Database.TABLE_STATE_DATA_SPECIFIER];
				dsrec = dstable.getRecord( this.dsid );
			}
		}

		public override string processData( byte[] data )
		{
			return "type: " + this.record[0] + "; rec: " + BitConverter.ToString( this.record ) + "; dsrec: " + BitConverter.ToString( this.dsrec.record );
		}
	}
}
