namespace DRBDBReader
{
	partial class frmMain
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if( disposing && ( components != null ) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.txtConsole = new System.Windows.Forms.TextBox();
			this.txtConsoleInput = new System.Windows.Forms.TextBox();
			this.spltTopBottom = new System.Windows.Forms.SplitContainer();
			this.spltLeftRight = new System.Windows.Forms.SplitContainer();
			this.tvMain = new System.Windows.Forms.TreeView();
			this.dgvIdk = new System.Windows.Forms.DataGridView();
			((System.ComponentModel.ISupportInitialize)(this.spltTopBottom)).BeginInit();
			this.spltTopBottom.Panel1.SuspendLayout();
			this.spltTopBottom.Panel2.SuspendLayout();
			this.spltTopBottom.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spltLeftRight)).BeginInit();
			this.spltLeftRight.Panel1.SuspendLayout();
			this.spltLeftRight.Panel2.SuspendLayout();
			this.spltLeftRight.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvIdk)).BeginInit();
			this.SuspendLayout();
			// 
			// txtConsole
			// 
			this.txtConsole.BackColor = System.Drawing.SystemColors.Window;
			this.txtConsole.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtConsole.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtConsole.Location = new System.Drawing.Point(0, 0);
			this.txtConsole.Multiline = true;
			this.txtConsole.Name = "txtConsole";
			this.txtConsole.ReadOnly = true;
			this.txtConsole.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtConsole.Size = new System.Drawing.Size(805, 78);
			this.txtConsole.TabIndex = 0;
			// 
			// txtConsoleInput
			// 
			this.txtConsoleInput.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.txtConsoleInput.Location = new System.Drawing.Point(0, 78);
			this.txtConsoleInput.Name = "txtConsoleInput";
			this.txtConsoleInput.Size = new System.Drawing.Size(805, 20);
			this.txtConsoleInput.TabIndex = 1;
			this.txtConsoleInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtConsoleInput_KeyDown);
			this.txtConsoleInput.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtConsoleInput_KeyPress);
			this.txtConsoleInput.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtConsoleInput_PreviewKeyDown);
			// 
			// spltTopBottom
			// 
			this.spltTopBottom.Dock = System.Windows.Forms.DockStyle.Fill;
			this.spltTopBottom.Location = new System.Drawing.Point(0, 0);
			this.spltTopBottom.Name = "spltTopBottom";
			this.spltTopBottom.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// spltTopBottom.Panel1
			// 
			this.spltTopBottom.Panel1.Controls.Add(this.spltLeftRight);
			// 
			// spltTopBottom.Panel2
			// 
			this.spltTopBottom.Panel2.Controls.Add(this.txtConsole);
			this.spltTopBottom.Panel2.Controls.Add(this.txtConsoleInput);
			this.spltTopBottom.Size = new System.Drawing.Size(805, 405);
			this.spltTopBottom.SplitterDistance = 303;
			this.spltTopBottom.TabIndex = 2;
			// 
			// spltLeftRight
			// 
			this.spltLeftRight.Dock = System.Windows.Forms.DockStyle.Fill;
			this.spltLeftRight.Location = new System.Drawing.Point(0, 0);
			this.spltLeftRight.Name = "spltLeftRight";
			// 
			// spltLeftRight.Panel1
			// 
			this.spltLeftRight.Panel1.Controls.Add(this.tvMain);
			// 
			// spltLeftRight.Panel2
			// 
			this.spltLeftRight.Panel2.Controls.Add(this.dgvIdk);
			this.spltLeftRight.Size = new System.Drawing.Size(805, 303);
			this.spltLeftRight.SplitterDistance = 205;
			this.spltLeftRight.TabIndex = 0;
			// 
			// tvMain
			// 
			this.tvMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tvMain.Location = new System.Drawing.Point(0, 0);
			this.tvMain.Name = "tvMain";
			this.tvMain.Size = new System.Drawing.Size(205, 303);
			this.tvMain.TabIndex = 0;
			// 
			// dgvIdk
			// 
			this.dgvIdk.AllowUserToAddRows = false;
			this.dgvIdk.AllowUserToDeleteRows = false;
			this.dgvIdk.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvIdk.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvIdk.Location = new System.Drawing.Point(0, 0);
			this.dgvIdk.Name = "dgvIdk";
			this.dgvIdk.ReadOnly = true;
			this.dgvIdk.Size = new System.Drawing.Size(596, 303);
			this.dgvIdk.TabIndex = 0;
			// 
			// frmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(805, 405);
			this.Controls.Add(this.spltTopBottom);
			this.Name = "frmMain";
			this.Text = "DRB DB Reader";
			this.spltTopBottom.Panel1.ResumeLayout(false);
			this.spltTopBottom.Panel2.ResumeLayout(false);
			this.spltTopBottom.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.spltTopBottom)).EndInit();
			this.spltTopBottom.ResumeLayout(false);
			this.spltLeftRight.Panel1.ResumeLayout(false);
			this.spltLeftRight.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.spltLeftRight)).EndInit();
			this.spltLeftRight.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvIdk)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TextBox txtConsole;
		private System.Windows.Forms.TextBox txtConsoleInput;
		private System.Windows.Forms.SplitContainer spltTopBottom;
		private System.Windows.Forms.SplitContainer spltLeftRight;
		private System.Windows.Forms.TreeView tvMain;
		private System.Windows.Forms.DataGridView dgvIdk;
	}
}

