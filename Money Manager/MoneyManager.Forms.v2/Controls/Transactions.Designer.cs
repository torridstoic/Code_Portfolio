namespace MoneyManager.Forms.v2
{
	partial class Transactions
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
            this.titleLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.transactionGroup = new System.Windows.Forms.GroupBox();
            this.deleteButton = new System.Windows.Forms.Button();
            this.editButton = new System.Windows.Forms.Button();
            this.dataGridData = new System.Windows.Forms.DataGridView();
            this.addButton = new System.Windows.Forms.Button();
            this.startDatePicker = new System.Windows.Forms.DateTimePicker();
            this.endDatePicker = new System.Windows.Forms.DateTimePicker();
            this.recurringButton = new System.Windows.Forms.Button();
            this.periodGroup = new System.Windows.Forms.GroupBox();
            this.customRadio = new System.Windows.Forms.RadioButton();
            this.monthlyRadio = new System.Windows.Forms.RadioButton();
            this.datesGroup = new System.Windows.Forms.GroupBox();
            this.monthGroup = new System.Windows.Forms.GroupBox();
            this.monthPicker = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.transactionGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridData)).BeginInit();
            this.periodGroup.SuspendLayout();
            this.datesGroup.SuspendLayout();
            this.monthGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Location = new System.Drawing.Point(233, 14);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(46, 20);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "[Title]";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Start Date:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "End Date:";
            // 
            // transactionGroup
            // 
            this.transactionGroup.Controls.Add(this.deleteButton);
            this.transactionGroup.Controls.Add(this.editButton);
            this.transactionGroup.Controls.Add(this.dataGridData);
            this.transactionGroup.Controls.Add(this.addButton);
            this.transactionGroup.Location = new System.Drawing.Point(42, 151);
            this.transactionGroup.Name = "transactionGroup";
            this.transactionGroup.Size = new System.Drawing.Size(482, 492);
            this.transactionGroup.TabIndex = 5;
            this.transactionGroup.TabStop = false;
            this.transactionGroup.Text = "Your [Title]";
            // 
            // deleteButton
            // 
            this.deleteButton.Location = new System.Drawing.Point(254, 409);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(155, 29);
            this.deleteButton.TabIndex = 3;
            this.deleteButton.Text = "Delete [Title]";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // editButton
            // 
            this.editButton.Location = new System.Drawing.Point(77, 409);
            this.editButton.Name = "editButton";
            this.editButton.Size = new System.Drawing.Size(155, 29);
            this.editButton.TabIndex = 2;
            this.editButton.Text = "Edit [Title]";
            this.editButton.UseVisualStyleBackColor = true;
            this.editButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // dataGridData
            // 
            this.dataGridData.AllowUserToAddRows = false;
            this.dataGridData.AllowUserToDeleteRows = false;
            this.dataGridData.AllowUserToResizeColumns = false;
            this.dataGridData.AllowUserToResizeRows = false;
            this.dataGridData.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridData.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridData.Dock = System.Windows.Forms.DockStyle.Top;
            this.dataGridData.Location = new System.Drawing.Point(3, 22);
            this.dataGridData.MultiSelect = false;
            this.dataGridData.Name = "dataGridData";
            this.dataGridData.ReadOnly = true;
            this.dataGridData.Size = new System.Drawing.Size(476, 381);
            this.dataGridData.TabIndex = 1;
            this.dataGridData.SelectionChanged += new System.EventHandler(this.gridClicked);
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(153, 453);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(180, 28);
            this.addButton.TabIndex = 0;
            this.addButton.Text = "Add New [Title]";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // startDatePicker
            // 
            this.startDatePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.startDatePicker.Location = new System.Drawing.Point(103, 25);
            this.startDatePicker.Name = "startDatePicker";
            this.startDatePicker.Size = new System.Drawing.Size(122, 26);
            this.startDatePicker.TabIndex = 6;
            this.startDatePicker.ValueChanged += new System.EventHandler(this.DateChanged);
            // 
            // endDatePicker
            // 
            this.endDatePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.endDatePicker.Location = new System.Drawing.Point(103, 57);
            this.endDatePicker.Name = "endDatePicker";
            this.endDatePicker.Size = new System.Drawing.Size(122, 26);
            this.endDatePicker.TabIndex = 7;
            this.endDatePicker.ValueChanged += new System.EventHandler(this.DateChanged);
            // 
            // recurringButton
            // 
            this.recurringButton.Location = new System.Drawing.Point(425, 18);
            this.recurringButton.Name = "recurringButton";
            this.recurringButton.Size = new System.Drawing.Size(145, 48);
            this.recurringButton.TabIndex = 8;
            this.recurringButton.Text = "Your Recurring [Title] >>";
            this.recurringButton.UseVisualStyleBackColor = true;
            this.recurringButton.Click += new System.EventHandler(this.recurringButton_Click);
            // 
            // periodGroup
            // 
            this.periodGroup.Controls.Add(this.customRadio);
            this.periodGroup.Controls.Add(this.monthlyRadio);
            this.periodGroup.Location = new System.Drawing.Point(23, 47);
            this.periodGroup.Name = "periodGroup";
            this.periodGroup.Size = new System.Drawing.Size(148, 87);
            this.periodGroup.TabIndex = 9;
            this.periodGroup.TabStop = false;
            this.periodGroup.Text = "Viewing Period";
            // 
            // customRadio
            // 
            this.customRadio.AutoSize = true;
            this.customRadio.Location = new System.Drawing.Point(15, 55);
            this.customRadio.Name = "customRadio";
            this.customRadio.Size = new System.Drawing.Size(129, 24);
            this.customRadio.TabIndex = 1;
            this.customRadio.Text = "Custom Dates";
            this.customRadio.UseVisualStyleBackColor = true;
            this.customRadio.CheckedChanged += new System.EventHandler(this.radioChange);
            // 
            // monthlyRadio
            // 
            this.monthlyRadio.AutoSize = true;
            this.monthlyRadio.Checked = true;
            this.monthlyRadio.Location = new System.Drawing.Point(15, 25);
            this.monthlyRadio.Name = "monthlyRadio";
            this.monthlyRadio.Size = new System.Drawing.Size(82, 24);
            this.monthlyRadio.TabIndex = 0;
            this.monthlyRadio.TabStop = true;
            this.monthlyRadio.Text = "Monthly";
            this.monthlyRadio.UseVisualStyleBackColor = true;
            this.monthlyRadio.CheckedChanged += new System.EventHandler(this.radioChange);
            // 
            // datesGroup
            // 
            this.datesGroup.Controls.Add(this.label2);
            this.datesGroup.Controls.Add(this.label3);
            this.datesGroup.Controls.Add(this.endDatePicker);
            this.datesGroup.Controls.Add(this.startDatePicker);
            this.datesGroup.Location = new System.Drawing.Point(177, 47);
            this.datesGroup.Name = "datesGroup";
            this.datesGroup.Size = new System.Drawing.Size(242, 98);
            this.datesGroup.TabIndex = 10;
            this.datesGroup.TabStop = false;
            // 
            // monthGroup
            // 
            this.monthGroup.Controls.Add(this.monthPicker);
            this.monthGroup.Controls.Add(this.label4);
            this.monthGroup.Location = new System.Drawing.Point(177, 47);
            this.monthGroup.Name = "monthGroup";
            this.monthGroup.Size = new System.Drawing.Size(242, 98);
            this.monthGroup.TabIndex = 4;
            this.monthGroup.TabStop = false;
            // 
            // monthPicker
            // 
            this.monthPicker.CustomFormat = "MMMM, yyyy";
            this.monthPicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.monthPicker.Location = new System.Drawing.Point(68, 39);
            this.monthPicker.Name = "monthPicker";
            this.monthPicker.ShowUpDown = true;
            this.monthPicker.Size = new System.Drawing.Size(168, 26);
            this.monthPicker.TabIndex = 1;
            this.monthPicker.ValueChanged += new System.EventHandler(this.DateChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 44);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 20);
            this.label4.TabIndex = 0;
            this.label4.Text = "Month:";
            // 
            // Transactions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.datesGroup);
            this.Controls.Add(this.periodGroup);
            this.Controls.Add(this.recurringButton);
            this.Controls.Add(this.transactionGroup);
            this.Controls.Add(this.titleLabel);
            this.Controls.Add(this.monthGroup);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Transactions";
            this.Size = new System.Drawing.Size(584, 661);
            this.transactionGroup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridData)).EndInit();
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

		private System.Windows.Forms.Label titleLabel;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox transactionGroup;
		private System.Windows.Forms.Button addButton;
		private System.Windows.Forms.Button deleteButton;
		private System.Windows.Forms.Button editButton;
		private System.Windows.Forms.DataGridView dataGridData;
		private System.Windows.Forms.DateTimePicker startDatePicker;
		private System.Windows.Forms.DateTimePicker endDatePicker;
		private System.Windows.Forms.Button recurringButton;
		private System.Windows.Forms.GroupBox periodGroup;
		private System.Windows.Forms.RadioButton customRadio;
		private System.Windows.Forms.RadioButton monthlyRadio;
		private System.Windows.Forms.GroupBox datesGroup;
		private System.Windows.Forms.GroupBox monthGroup;
		private System.Windows.Forms.DateTimePicker monthPicker;
		private System.Windows.Forms.Label label4;
	}
}
