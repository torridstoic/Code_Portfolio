namespace MoneyManager.Forms.v2
{
	partial class Reports
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.walletGroup = new System.Windows.Forms.GroupBox();
			this.walletGrid = new System.Windows.Forms.DataGridView();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.startDatePicker = new System.Windows.Forms.DateTimePicker();
			this.endDatePicker = new System.Windows.Forms.DateTimePicker();
			this.periodGroup = new System.Windows.Forms.GroupBox();
			this.datesGroup = new System.Windows.Forms.GroupBox();
			this.monthlyRadio = new System.Windows.Forms.RadioButton();
			this.customRadio = new System.Windows.Forms.RadioButton();
			this.monthGroup = new System.Windows.Forms.GroupBox();
			this.label4 = new System.Windows.Forms.Label();
			this.monthPicker = new System.Windows.Forms.DateTimePicker();
			this.walletGroup.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.walletGrid)).BeginInit();
			this.periodGroup.SuspendLayout();
			this.datesGroup.SuspendLayout();
			this.monthGroup.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(237, 15);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(66, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "Reports";
			// 
			// walletGroup
			// 
			this.walletGroup.Controls.Add(this.walletGrid);
			this.walletGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.walletGroup.Location = new System.Drawing.Point(39, 146);
			this.walletGroup.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.walletGroup.Name = "walletGroup";
			this.walletGroup.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.walletGroup.Size = new System.Drawing.Size(506, 379);
			this.walletGroup.TabIndex = 1;
			this.walletGroup.TabStop = false;
			this.walletGroup.Text = "Wallets";
			// 
			// walletGrid
			// 
			this.walletGrid.AllowUserToAddRows = false;
			this.walletGrid.AllowUserToDeleteRows = false;
			this.walletGrid.AllowUserToResizeColumns = false;
			this.walletGrid.AllowUserToResizeRows = false;
			this.walletGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.walletGrid.BackgroundColor = System.Drawing.SystemColors.Control;
			this.walletGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.walletGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.walletGrid.Location = new System.Drawing.Point(4, 24);
			this.walletGrid.MultiSelect = false;
			this.walletGrid.Name = "walletGrid";
			this.walletGrid.Size = new System.Drawing.Size(498, 350);
			this.walletGrid.TabIndex = 0;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(7, 30);
			this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(87, 20);
			this.label2.TabIndex = 2;
			this.label2.Text = "Start Date:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(13, 62);
			this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(81, 20);
			this.label3.TabIndex = 3;
			this.label3.Text = "End Date:";
			// 
			// startDatePicker
			// 
			this.startDatePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.startDatePicker.Location = new System.Drawing.Point(101, 25);
			this.startDatePicker.Name = "startDatePicker";
			this.startDatePicker.Size = new System.Drawing.Size(134, 26);
			this.startDatePicker.TabIndex = 4;
			this.startDatePicker.ValueChanged += new System.EventHandler(this.DateChanged);
			// 
			// endDatePicker
			// 
			this.endDatePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.endDatePicker.Location = new System.Drawing.Point(101, 57);
			this.endDatePicker.Name = "endDatePicker";
			this.endDatePicker.Size = new System.Drawing.Size(134, 26);
			this.endDatePicker.TabIndex = 5;
			this.endDatePicker.ValueChanged += new System.EventHandler(this.DateChanged);
			// 
			// periodGroup
			// 
			this.periodGroup.Controls.Add(this.customRadio);
			this.periodGroup.Controls.Add(this.monthlyRadio);
			this.periodGroup.Location = new System.Drawing.Point(70, 47);
			this.periodGroup.Name = "periodGroup";
			this.periodGroup.Size = new System.Drawing.Size(146, 91);
			this.periodGroup.TabIndex = 6;
			this.periodGroup.TabStop = false;
			this.periodGroup.Text = "Viewing Period";
			// 
			// datesGroup
			// 
			this.datesGroup.Controls.Add(this.endDatePicker);
			this.datesGroup.Controls.Add(this.label2);
			this.datesGroup.Controls.Add(this.startDatePicker);
			this.datesGroup.Controls.Add(this.label3);
			this.datesGroup.Location = new System.Drawing.Point(260, 47);
			this.datesGroup.Name = "datesGroup";
			this.datesGroup.Size = new System.Drawing.Size(250, 102);
			this.datesGroup.TabIndex = 0;
			this.datesGroup.TabStop = false;
			// 
			// monthlyRadio
			// 
			this.monthlyRadio.AutoSize = true;
			this.monthlyRadio.Checked = true;
			this.monthlyRadio.Location = new System.Drawing.Point(14, 25);
			this.monthlyRadio.Name = "monthlyRadio";
			this.monthlyRadio.Size = new System.Drawing.Size(82, 24);
			this.monthlyRadio.TabIndex = 0;
			this.monthlyRadio.TabStop = true;
			this.monthlyRadio.Text = "Monthly";
			this.monthlyRadio.UseVisualStyleBackColor = true;
			this.monthlyRadio.CheckedChanged += new System.EventHandler(this.radioChange);
			// 
			// customRadio
			// 
			this.customRadio.AutoSize = true;
			this.customRadio.Location = new System.Drawing.Point(14, 58);
			this.customRadio.Name = "customRadio";
			this.customRadio.Size = new System.Drawing.Size(129, 24);
			this.customRadio.TabIndex = 1;
			this.customRadio.Text = "Custom Dates";
			this.customRadio.UseVisualStyleBackColor = true;
			this.customRadio.CheckedChanged += new System.EventHandler(this.radioChange);
			// 
			// monthGroup
			// 
			this.monthGroup.Controls.Add(this.monthPicker);
			this.monthGroup.Controls.Add(this.label4);
			this.monthGroup.Location = new System.Drawing.Point(260, 47);
			this.monthGroup.Name = "monthGroup";
			this.monthGroup.Size = new System.Drawing.Size(250, 102);
			this.monthGroup.TabIndex = 1;
			this.monthGroup.TabStop = false;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(6, 46);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(58, 20);
			this.label4.TabIndex = 0;
			this.label4.Text = "Month:";
			// 
			// monthPicker
			// 
			this.monthPicker.CustomFormat = "MMMM, yyyy";
			this.monthPicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.monthPicker.Location = new System.Drawing.Point(70, 41);
			this.monthPicker.Name = "monthPicker";
			this.monthPicker.ShowUpDown = true;
			this.monthPicker.Size = new System.Drawing.Size(174, 26);
			this.monthPicker.TabIndex = 1;
			this.monthPicker.ValueChanged += new System.EventHandler(this.DateChanged);
			// 
			// Reports
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.datesGroup);
			this.Controls.Add(this.periodGroup);
			this.Controls.Add(this.walletGroup);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.monthGroup);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.Name = "Reports";
			this.Size = new System.Drawing.Size(584, 561);
			this.walletGroup.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.walletGrid)).EndInit();
			this.periodGroup.ResumeLayout(false);
			this.periodGroup.PerformLayout();
			this.datesGroup.ResumeLayout(false);
			this.datesGroup.PerformLayout();
			this.monthGroup.ResumeLayout(false);
			this.monthGroup.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox walletGroup;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.DataGridView walletGrid;
		private System.Windows.Forms.DateTimePicker startDatePicker;
		private System.Windows.Forms.DateTimePicker endDatePicker;
		private System.Windows.Forms.GroupBox periodGroup;
		private System.Windows.Forms.RadioButton customRadio;
		private System.Windows.Forms.RadioButton monthlyRadio;
		private System.Windows.Forms.GroupBox datesGroup;
		private System.Windows.Forms.GroupBox monthGroup;
		private System.Windows.Forms.DateTimePicker monthPicker;
		private System.Windows.Forms.Label label4;
	}
}
