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
using DRBDBReader.DB.Records;

namespace DRBDBReader.DB.Converters
{
	public class NumericConverter : Converter
	{
		public NCRecord ncRecord;
		public NDSRecord ndsRecord;

		public NumericConverter( Database db, byte[] record, ushort cfid, ushort dsid ) : base( db, record, cfid, dsid )
		{
			Table numConvTable = this.db.tables[Database.TABLE_CONVERTERS_NUMERIC];
			this.ncRecord = (NCRecord)numConvTable.getRecord( this.cfid );

			Table ndsTable = this.db.tables[Database.TABLE_NUMERIC_DATA_SPECIFIER];
			this.ndsRecord = (NDSRecord)ndsTable.getRecord( this.dsid );
		}

		public override string processData( long data, bool outputMetric = false )
		{
			decimal result = data * (decimal)this.ncRecord.slope + (decimal)this.ncRecord.offset;
			string unit = this.ndsRecord.imperialUnitString;
			if( outputMetric )
			{
				result = result * (decimal)this.ndsRecord.metricConvSlope + (decimal)this.ndsRecord.metricConvOffset;
				unit = this.ndsRecord.metricUnitString;
			}
			return result + " " + unit;
		}
	}
}
