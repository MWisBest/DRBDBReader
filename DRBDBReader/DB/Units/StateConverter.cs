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
using System.Collections.Generic;

namespace DRBDBReader.DB.Units
{
	public class StateConverter : Converter
	{
		private const byte FIELD_MASK = 1;
		private const byte FIELD_OP = 2;

		public Operator op;
		public ushort mask;

		public string defaultState;
		public Dictionary<ushort, string> entries;

		public StateConverter( Database db, byte[] record, ushort cfid, ushort dsid ) : base( db, record, cfid, dsid )
		{
			Table stateConvTable = this.db.tables[Database.TABLE_CONVERTERS_STATE];
			Record stateConvRecord = stateConvTable.getRecord( this.cfid );

			this.mask = (ushort)stateConvTable.readField( stateConvRecord, FIELD_MASK );
			this.op = (Operator)( (byte)( ( (ushort)stateConvTable.readField( stateConvRecord, FIELD_OP ) ) >> 8 ) );
			this.entries = new Dictionary<ushort, string>();

			Table sdsTable = this.db.tables[Database.TABLE_STATE_DATA_SPECIFIER];
			Record sdsRecord = sdsTable.getRecord( this.dsid );
			int defaultid = (int)sdsTable.readField( sdsRecord, 1 );
			this.defaultState = ( defaultid != 0 ? this.db.getString( (ushort)defaultid ) : "" );

			Table stateTable = this.db.tables[Database.TABLE_STATE_ENTRY];
			List<ushort> recordIds = stateTable.selectRecordsReturnIDs( 3, this.dsid );
			for( ushort i = 0; i < recordIds.Count; ++i )
			{
				ushort value = (ushort)stateTable.readField( stateTable.records[recordIds[i]], 1 );
				string name = this.db.getString( (ushort)stateTable.readField( stateTable.records[recordIds[i]], 0 ) );

				this.entries.Add( value, name );
			}
		}
	}
}
