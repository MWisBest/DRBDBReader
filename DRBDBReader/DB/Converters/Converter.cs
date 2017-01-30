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
using DRBDBReader.DB.Records;

namespace DRBDBReader.DB.Converters
{
	public class Converter
	{
		public Database db;
		public byte[] record;
		public ushort cfid;
		public ushort dsid;
		public Record dsRecord;

		public Converter( Database db, byte[] record, ushort cfid, ushort dsid )
		{
			this.db = db;
			this.record = record;
			this.cfid = cfid;
			this.dsid = dsid;
		}

		public virtual string processData( long data, bool outputMetric = false )
		{
			return "(null)";
		}

		public override string ToString()
		{
			string ret = "type: " + this.record[0] + "; rec: " + BitConverter.ToString( this.record );
			if( this.dsRecord != null )
			{
				ret += "; dsrec: " + BitConverter.ToString( this.dsRecord.record );
			}
			return ret;
		}
	}
}
