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

namespace DRBDBReader.DB
{
	public class ModuleRecord : Record
	{
		private const byte FIELD_ID = 0;
		private const byte FIELD_SCID = 1;
		private const byte FIELD_NAMEID = 3;

		public ushort id;
		public ushort scid;
		public string scname;
		public ushort nameid;
		public string name;
		public TXRecord[] dataelements;

		public ModuleRecord( Table table, byte[] record ) : base( table, record )
		{
			// get id
			this.id = (ushort)this.table.readField( this, FIELD_ID );


			// get scid/scname
			this.scid = (ushort)this.table.readField( this, FIELD_SCID );
			this.scname = this.table.db.getServiceCatString( this.scid );


			// get name
			this.nameid = (ushort)this.table.readField( this, FIELD_NAMEID );
			string temp = this.table.db.getStateString( this.nameid );
			if( temp == null )
			{
				temp = "(null)";
			}
			this.name = temp;


			// get dataelements
			Table t = this.table.db.tables[Database.TABLE_MODULE_DATAELEMENT];
			List<ushort> recordIds = t.selectRecordsReturnIDs( 0, this.id, true );
			this.dataelements = new TXRecord[recordIds.Count];
			for( int i = 0; i < dataelements.Length; ++i )
			{
				this.dataelements[i] = (TXRecord)( this.table.db.tables[Database.TABLE_TRANSMIT].getRecord( t.readField( t.records[recordIds[i]], 1 ) ) );
			}
		}
	}
}
