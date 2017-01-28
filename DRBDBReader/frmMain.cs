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
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using DRBDBReader.DB;
using DRBDBReader.DB.Converters;
using DRBDBReader.DB.Records;

namespace DRBDBReader
{
	public partial class frmMain : Form
	{
		FileInfo fi = new FileInfo( "database.mem" );
		Database db;
		List<string> cmdHistory = new List<string>();
		int cmdIdx = 0;
		string bulkConsole = "";

		public frmMain()
		{
			InitializeComponent();
			this.cmdHistory.Add( "" );
			this.cmdIdx = 0;
		}

		public void writeToConsole( string text )
		{
			if( this.txtConsole.Text != "" )
			{
				this.txtConsole.AppendText( Environment.NewLine );
			}
			this.txtConsole.AppendText( text );
			this.txtConsole.SelectionStart = this.txtConsole.Text.Length;
			this.txtConsole.Refresh();
		}

		public void writeBulkToConsoleStart()
		{
			this.txtConsole.AppendText( Environment.NewLine );
			this.txtConsole.SuspendLayout();
		}

		public void writeBulkToConsole( string text )
		{
			this.bulkConsole += text + Environment.NewLine;
		}

		public void writeBulkToConsoleEnd()
		{
			this.txtConsole.AppendText( this.bulkConsole );
			this.txtConsole.ResumeLayout();
			this.txtConsole.SelectionStart = this.txtConsole.Text.Length;
			this.txtConsole.Refresh();
			this.bulkConsole = "";
		}

		private void checkDB()
		{
			if( this.db == null )
			{
				this.writeToConsole( "Loading database, please wait..." );
				this.db = new Database( this.fi );
				// manually append text to avoid the automatic newline preceeding it
				this.txtConsole.AppendText( " ...done!" + Environment.NewLine );
				this.txtConsole.SelectionStart = this.txtConsole.Text.Length;
				this.txtConsole.Refresh();
			}
		}

		public void consoleCommandHandler( string cmd )
		{
			try
			{
				string tofind = "";
				string[] tofindall = null;
				ushort modid, stid;
				uint txid;
				cmd = cmd.Trim();
				this.cmdHistory.Add( cmd );
				cmdIdx = this.cmdHistory.Count - 1;
				string[] splitted = cmd.Split( new char[] { ' ' }, 2 );
				switch( splitted[0] )
				{
					case "readdb":
						if( splitted.Length > 1 && splitted[1].Trim() != "" )
						{
							this.fi = new FileInfo( splitted[1].Trim() );
						}
						this.checkDB();
						break;
					case "unloaddb":
						this.db = null;
						// We're taking out a huge chunk of memory, so let GC clear it out right away.
						GC.Collect();
						break;
					case "stringid":
						this.checkDB();

						stid = Util.parseUShort( splitted[1] );

						this.writeToConsole( this.db.getString( stid ) + Environment.NewLine );

						break;
					case "txid":
						this.checkDB();

						txid = Util.parseUInt( splitted[1] );

						this.writeToConsole( this.db.getDetailedTX( (uint)txid ) + Environment.NewLine );

						break;
					case "txrunconverter":
					case "txrunconvertermetric":
						this.checkDB();

						string[] txconvsplit = splitted[1].Split( new char[] { ' ' }, 2 );
						long convdata = 0;

						txid = Util.parseUInt( txconvsplit[0] );
						convdata = Util.parseLong( txconvsplit[1] );

						Table txconvtable = this.db.tables[Database.TABLE_TRANSMIT];
						TXRecord txconvrec = (TXRecord)txconvtable.getRecord( txid );

						string result = txconvrec.converter.processData( convdata, outputMetric: splitted[0].EndsWith( "metric" )  );

						this.writeToConsole( result + Environment.NewLine );

						break;
					case "txsearch":
						this.checkDB();

						tofind = splitted[1].ToLower();
						if( tofind.Contains( " && " ) )
						{
							tofindall = tofind.Split( new string[] { " && " }, StringSplitOptions.RemoveEmptyEntries );
						}

						this.writeBulkToConsoleStart();
						for( uint u = 0x80000000; u < 0x80009000; ++u )
						{
							try
							{
								string temp = this.db.getTX( u );
								if( temp != null )
								{
									string templower = temp.ToLower();

									if( tofindall != null )
									{
										foreach( string s in tofindall )
										{
											if( !templower.Contains( s ) )
											{
												goto SKIPTX;
											}
										}
										this.writeBulkToConsole( temp + "; 0x" + u.ToString( "x" ) );
									}
									else if( templower.Contains( tofind ) )
									{
										this.writeBulkToConsole( temp + "; 0x" + u.ToString( "x" ) );
									}
								}
							}
							catch
							{
								continue;
							}

						SKIPTX:
							continue;
						}
						this.writeBulkToConsoleEnd();

						break;
					case "dumpstateconverter":
						this.checkDB();

						txid = Util.parseUInt( splitted[1] );

						Table dtcdumptxconvtable = this.db.tables[Database.TABLE_TRANSMIT];
						TXRecord dtcdumptxconvrec = (TXRecord)dtcdumptxconvtable.getRecord( txid );

						if( dtcdumptxconvrec.converter is StateConverter )
						{
							StateConverter dtcdumpconv = (StateConverter)dtcdumptxconvrec.converter;
							this.writeBulkToConsoleStart();
							foreach( KeyValuePair<ushort, string> kvp in dtcdumpconv.entries )
							{
								this.writeBulkToConsole( kvp.Key.ToString() + ": " + kvp.Value );
							}
							this.writeBulkToConsoleEnd();
						}
						else
						{
							this.writeToConsole( "Not a StateConverter." );
						}

						break;
					case "dumptableinfo":
						this.checkDB();

						ushort tableNum = Util.parseUShort( splitted[1] );

						Table t = this.db.tables[tableNum];
						string toPrint = "";

						toPrint += "Table: " + tableNum + "; Columns: " + t.colCount + "; Rows: " + t.rowCount + ";" + Environment.NewLine;
						toPrint += "ColSizes: " + BitConverter.ToString( t.colSizes ) + "; RowSize: " + t.rowSize + ";" + Environment.NewLine;

						this.writeToConsole( toPrint );

						break;
					case "stringidfuzz":
						this.checkDB();

						string[] stringfuzzsplit = splitted[1].Split( new char[] { ' ' }, 2 );

						ushort stringfuzzTable = Util.parseUShort( stringfuzzsplit[0] );
						byte stringfuzzTableCol = (byte)Util.parseUShort( stringfuzzsplit[1] );
						Table tt = this.db.tables[stringfuzzTable];
						int stringfuzzhits = 0;
						int stringfuzzzeros = 0;

						foreach( Record stringfuzzrec in tt.records )
						{
							ushort stringfuzzfield = (ushort)tt.readField( stringfuzzrec, stringfuzzTableCol );
							string stringfuzzstring = this.db.getString( stringfuzzfield );
							if( stringfuzzstring != "(null)" )
							{
								++stringfuzzhits;
							}
							if( stringfuzzfield == 0 )
							{
								++stringfuzzzeros;
							}
						}

						this.writeToConsole( "Records: " + tt.records.Length + "; Hits: " + stringfuzzhits  + "; Zeros: " + stringfuzzzeros + Environment.NewLine );

						break;
					case "genericidfuzz":
						this.checkDB();

						string[] fuzzsplit = splitted[1].Split( new char[] { ' ' }, 4 );

						ushort fuzzerTableId = Util.parseUShort( fuzzsplit[0] );
						byte fuzzerTableCol = (byte)Util.parseUShort( fuzzsplit[1] );
						ushort fuzzingTableId = Util.parseUShort( fuzzsplit[2] );
						byte fuzzingTableCol = (byte)Util.parseUShort( fuzzsplit[3] );
						Table fuzzerTable = this.db.tables[fuzzerTableId];
						Table fuzzingTable = this.db.tables[fuzzingTableId];
						int fuzzhits = 0;
						int fuzzzeros = 0;

						foreach( Record fuzzingRec in fuzzingTable.records )
						{
							uint fuzzingRecID = (uint)fuzzingTable.readField( fuzzingRec, fuzzingTableCol );
							Record fuzzerRec = fuzzerTable.getRecord( fuzzingRecID, idcol: fuzzerTableCol, sorted: false );
							if( fuzzerRec != null )
							{
								++fuzzhits;
							}
							if( fuzzingRecID == 0 )
							{
								++fuzzzeros;
							}
						}

						this.writeToConsole( "Records: " + fuzzingTable.records.Length + "; Hits: " + fuzzhits + "; Zeros: " + fuzzzeros + Environment.NewLine );

						break;
					case "modid":
						this.checkDB();

						modid = Util.parseUShort( splitted[1] );

						string modresult = this.db.getModule( modid );

						if( modresult != null )
						{
							this.writeToConsole( modresult + Environment.NewLine );
						}
						else
						{
							this.writeToConsole( "No such module ID." + Environment.NewLine );
						}

						this.txtConsoleInput.Focus();
						this.txtConsoleInput.AppendText( "modid " + splitted[1] );

						break;
					case "modlist":
					case "modsearch":
						this.checkDB();

						if( splitted[0] != "modlist" )
						{
							tofind = splitted[1].ToLower();
							if( tofind.Contains( " && " ) )
							{
								tofindall = tofind.Split( new string[] { " && " }, StringSplitOptions.RemoveEmptyEntries );
							}
						}

						this.writeBulkToConsoleStart();
						for( ushort l = 0x0000; l < 0x2000; ++l )
						{
							try
							{
								string temp = this.db.getModule( l );

								if( temp != null )
								{
									if( splitted[0] != "modlist" )
									{
										string templower = temp.ToLower();

										if( tofindall != null )
										{
											foreach( string s in tofindall )
											{
												if( !templower.Contains( s ) )
												{
													goto SKIPMOD;
												}
											}
											this.writeBulkToConsole( temp + "; 0x" + l.ToString( "x" ) );
										}
										else if( templower.Contains( tofind ) )
										{
											this.writeBulkToConsole( temp + "; 0x" + l.ToString( "x" ) );
										}
									}
									else
									{
										this.writeBulkToConsole( temp + "; 0x" + l.ToString( "x" ) );
									}
								}
							}
							catch
							{
								continue;
							}

						SKIPMOD:
							continue;
						}
						this.writeBulkToConsoleEnd();

						break;
					case "modtxlist":
						this.checkDB();

						modid = Util.parseUShort( splitted[1] );

						Record rec = this.db.tables[Database.TABLE_MODULE].getRecord( modid );
						if( rec != null )
						{
							ModuleRecord modrec = (ModuleRecord)rec;
							this.writeBulkToConsoleStart();

							foreach( TXRecord txrec in modrec.dataelements )
							{
								string temp = this.db.getTX( txrec.id );
								this.writeBulkToConsole( temp + "; 0x" + txrec.id.ToString( "x" ) );
							}

							this.writeBulkToConsoleEnd();
						}
						else
						{
							this.writeToConsole( "No such module." + Environment.NewLine );
						}

						break;
				}
			}
			catch( Exception e )
			{
				this.writeToConsole( "Exception: " + e.ToString() + Environment.NewLine );
			}
		}

		private void txtConsoleInput_KeyPress( object sender, KeyPressEventArgs e )
		{
			if( e.KeyChar == '\r' )
			{
				string text = this.txtConsoleInput.Text;
				if( text != "" )
				{
					this.txtConsoleInput.Text = "";
					this.txtConsoleInput.Refresh();
					this.writeToConsole( "> " + text );
					this.consoleCommandHandler( text );
					this.txtConsoleInput.Focus();
					e.Handled = true;
				}
			}
		}

		private void txtConsoleInput_PreviewKeyDown( object sender, PreviewKeyDownEventArgs e )
		{
			switch( e.KeyCode )
			{
				case Keys.Down:
				case Keys.Up:
					e.IsInputKey = true;
					break;
			}
		}

		private void txtConsoleInput_KeyDown( object sender, KeyEventArgs e )
		{
			switch( e.KeyCode )
			{
				case Keys.Down:
					this.txtConsoleInput.Clear();
					if( this.cmdIdx < this.cmdHistory.Count - 1 )
					{
						++this.cmdIdx;
					}
					this.txtConsoleInput.AppendText( this.cmdHistory[this.cmdIdx] );
					e.Handled = true;
					break;
				case Keys.Up:
					this.txtConsoleInput.Clear();
					if( this.cmdIdx > 0 )
					{
						--this.cmdIdx;
					}
					this.txtConsoleInput.AppendText( this.cmdHistory[this.cmdIdx] );
					e.Handled = true;
					break;
			}
		}
	}
}
