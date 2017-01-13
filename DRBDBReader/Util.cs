/*
 * DRBDBReader
 * Copyright (C) 2017, Kyle Repinski
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

namespace DRBDBReader
{
	public static class Util
	{
		public static long parseLong( string input )
		{
			string lower = input.ToLower();
			long ret;

			if( lower.StartsWith( "0x" ) )
			{
				ret = Convert.ToInt64( lower.Substring( 2 ), 16 );
			}
			else if( lower.StartsWith( "0b" ) )
			{
				ret = Convert.ToInt64( lower.Substring( 2 ), 2 );
			}
			else if( lower.StartsWith( "0o" ) )
			{
				ret = Convert.ToInt64( lower.Substring( 2 ), 8 );
			}
			else
			{
				ret = Convert.ToInt64( lower );
			}

			return ret;
		}

		public static ushort parseUShort( string input )
		{
			string lower = input.ToLower();
			ushort ret;

			if( lower.StartsWith( "0x" ) )
			{
				ret = Convert.ToUInt16( lower.Substring( 2 ), 16 );
			}
			else if( lower.StartsWith( "0b" ) )
			{
				ret = Convert.ToUInt16( lower.Substring( 2 ), 2 );
			}
			else if( lower.StartsWith( "0o" ) )
			{
				ret = Convert.ToUInt16( lower.Substring( 2 ), 8 );
			}
			else
			{
				ret = Convert.ToUInt16( lower );
			}

			return ret;
		}
	}
}
