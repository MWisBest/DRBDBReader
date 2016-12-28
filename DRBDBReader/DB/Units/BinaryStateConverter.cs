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
	public class BinaryStateConverter : StateConverter
	{
		private const byte FIELD_BDS_TRUE_STR_ID  = 1;
		private const byte FIELD_BDS_FALSE_STR_ID = 2;

		public BinaryStateConverter( Database db, byte[] record, ushort cfid, ushort dsid ) : base( db, record, cfid, dsid )
		{
		}

		protected override void buildStateList()
		{
			Table bdsTable = this.db.tables[Database.TABLE_BINARY_DATA_SPECIFIER];
			Record bdsRecord = bdsTable.getRecord( this.dsid );

			string stateTrue = this.db.getString( (ushort)bdsTable.readField( bdsRecord, FIELD_BDS_TRUE_STR_ID ) );
			string stateFalse = this.db.getString( (ushort)bdsTable.readField( bdsRecord, FIELD_BDS_FALSE_STR_ID ) );

			this.entries.Add( 0, stateFalse );
			this.entries.Add( 1, stateTrue );
		}

		protected override ushort getEntryID( ushort val )
		{
			switch( this.op )
			{
				case Operator.GREATER:
					return (ushort)( val > this.mask ? 1 : 0 );
				case Operator.LESS:
					return (ushort)( val < this.mask ? 1 : 0 );
				case Operator.MASK_ZERO:
					return (ushort)( ( val & this.mask ) == 0 ? 1 : 0 );
				case Operator.MASK_NOT_ZERO:
					return (ushort)( ( val & this.mask ) != 0 ? 1 : 0 );
				case Operator.NOT_EQUAL:
					return (ushort)( val != this.mask ? 1 : 0 );
				case Operator.EQUAL:
				default:
					return (ushort)( val == this.mask ? 1 : 0 );
			}
		}
	}
}
