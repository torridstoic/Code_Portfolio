namespace MoneyManager.Forms.v2
{
	partial class EditBudget
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.budgetGroup = new System.Windows.Forms.GroupBox();
			this.newWalletButton = new System.Windows.Forms.Button();
			this.amountUD = new System.Windows.Forms.NumericUpDown();
			this.walletCombo = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.recurringRadio = new System.Windows.Forms.RadioButton();
			this.customRadio = new System.Windows.Forms.RadioButton();
			this.typeGroup = new System.Windows.Forms.GroupBox();
			this.datesGroup = new System.Windows.Forms.GroupBox();
			this.endDatePicker = new System.Windows.Forms.DateTimePicker();
			this.startDatePicker = new System.Windows.Forms.DateTimePicker();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.periodGroup = new System.Windows.Forms.GroupBox();
			this.recurStartDatePicker = new System.Windows.Forms.DateTimePicker();
			this.label1 = new System.Windows.Forms.Label();
			this.periodCombo = new System.Windows.Forms.ComboBox();
			this.budgetGroup.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.amountUD)).BeginInit();
			this.typeGroup.SuspendLayout();
			this.datesGroup.SuspendLayout();
			this.periodGroup.SuspendLayout();
			this.SuspendLayout();
			// 
			// budgetGroup
			// 
			this.budgetGroup.Controls.Add(this.newWalletButton);
			this.budgetGroup.Controls.Add(this.amountUD);
			this.budgetGroup.Controls.Add(this.walletCombo);
			this.budgetGroup.Controls.Add(this.label3);
			this.budgetGroup.Controls.Add(this.label2);
			this.budgetGroup.Location = new System.Drawing.Point(12, 173);
			this.budgetGroup.Name = "budgetGroup";
			this.budgetGroup.Size = new System.Drawing.Size(402, 187);
			this.budgetGroup.TabIndex = 3;
			this.budgetGroup.TabStop = false;
			this.budgetGroup.Text = "Budget Details";
			// 
			// newWalletButton
			// 
			this.newWalletButton.Location = new System.Drawing.Point(140, 102);
			this.newWalletButton.Name = "newWalletButton";
			this.newWalletButton.Size = new System.Drawing.Size(144, 28);
			this.newWalletButton.TabIndex = 7;
			this.newWalletButton.Text = "Add New Wallet";
			this.newWalletButton.UseVisualStyleBackColor = true;
			this.newWalletButton.Click += new System.EventHandler(this.newWalletButton_Click);
			// 
			// amountUD
			// 
			this.amountUD.DecimalPlaces = 2;
			this.amountUD.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
			this.amountUD.Location = new System.Drawing.Point(141, 146);
			this.amountUD.Maximum = new decimal(new int[] {
            5000000,
            0,
            0,
            0});
			this.amountUD.Name = "amountUD";
			this.amountUD.Size = new System.Drawing.Size(120, 26);
			this.amountUD.TabIndex = 8;
			// 
			// walletCombo
			// 
			this.walletCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.walletCombo.FormattingEnabled = true;
			this.walletCombo.Location = new System.Drawing.Point(140, 68);
			this.walletCombo.Name = "walletCombo";
			this.walletCombo.Size = new System.Drawing.Size(192, 28);
			this.walletCombo.TabIndex = 6;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(66, 148);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(69, 20);
			this.label3.TabIndex = 2;
			this.label3.Text = "Amount:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(77, 71);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(57, 20);
			this.label2.TabIndex = 1;
			this.label2.Text = "Wallet:";
			// 
			// okButton
			// 
			this.okButton.Location = new System.Drawing.Point(94, 382);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(97, 31);
			this.okButton.TabIndex = 9;
			this.okButton.Text = "Save";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(235, 382);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(97, 31);
			this.cancelButton.TabIndex = 10;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// recurringRadio
			// 
			this.recurringRadio.AutoSize = true;
			this.recurringRadio.Location = new System.Drawing.Point(60, 25);
			this.recurringRadio.Name = "recurringRadio";
			this.recurringRadio.Size = new System.Drawing.Size(96, 24);
			this.recurringRadio.TabIndex = 0;
			this.recurringRadio.Text = "Recurring";
			this.recurringRadio.UseVisualStyleBackColor = true;
			this.recurringRadio.CheckedChanged += new System.EventHandler(this.radioChange);
			// 
			// customRadio
			// 
			this.customRadio.AutoSize = true;
			this.customRadio.Checked = true;
			this.customRadio.Location = new System.Drawing.Point(213, 25);
			this.customRadio.Name = "customRadio";
			this.customRadio.Size = new System.Drawing.Size(129, 24);
			this.customRadio.TabIndex = 1;
			this.customRadio.TabStop = true;
			this.customRadio.Text = "Custom Dates";
			this.customRadio.UseVisualStyleBackColor = true;
			this.customRadio.CheckedChanged += new System.EventHandler(this.radioChange);
			// 
			// typeGroup
			// 
			this.typeGroup.Controls.Add(this.recurringRadio);
			this.typeGroup.Controls.Add(this.customRadio);
			this.typeGroup.Location = new System.Drawing.Point(12, 12);
			this.typeGroup.Name = "typeGroup";
			this.typeGroup.Size = new System.Drawing.Size(402, 60);
			this.typeGroup.TabIndex = 0;
			this.typeGroup.TabStop = false;
			this.typeGroup.Text = "Budget Type";
			// 
			// datesGroup
			// 
			this.datesGroup.Controls.Add(this.endDatePicker);
			this.datesGroup.Controls.Add(this.startDatePicker);
			this.datesGroup.Controls.Add(this.label5);
			this.datesGroup.Controls.Add(this.label4);
			this.datesGroup.Location = new System.Drawing.Point(12, 78);
			this.datesGroup.Name = "datesGroup";
			this.datesGroup.Size = new System.Drawing.Size(402, 89);
			this.datesGroup.TabIndex = 2;
			this.datesGroup.TabStop = false;
			this.datesGroup.Text = "Budget Dates";
			// 
			// endDatePicker
			// 
			this.endDatePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.endDatePicker.Location = new System.Drawing.Point(202, 45);
			this.endDatePicker.Name = "endDatePicker";
			this.endDatePicker.Size = new System.Drawing.Size(156, 26);
			this.endDatePicker.TabIndex = 5;
			// 
			// startDatePicker
			// 
			this.startDatePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.startDatePicker.Location = new System.Drawing.Point(22, 45);
			this.startDatePicker.Name = "startDatePicker";
			this.startDatePicker.Size = new System.Drawing.Size(157, 26);
			this.startDatePicker.TabIndex = 4;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(203, 22);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(81, 20);
			this.label5.TabIndex = 1;
			this.label5.Text = "End Date:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(22, 22);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(87, 20);
			this.label4.TabIndex = 0;
			this.label4.Text = "Start Date:";
			// 
			// periodGroup
			// 
			this.periodGroup.Controls.Add(this.recurStartDatePicker);
			this.periodGroup.Controls.Add(this.label1);
			this.periodGroup.Controls.Add(this.periodCombo);
			this.periodGroup.Location = new System.Drawing.Point(12, 78);
			this.periodGroup.Name = "periodGroup";
			this.periodGroup.Size = new System.Drawing.Size(402, 89);
			this.periodGroup.TabIndex = 1;
			this.periodGroup.TabStop = false;
			this.periodGroup.Text = "Recurring Period";
			// 
			// recurStartDatePicker
			// 
			this.recurStartDatePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.recurStartDatePicker.Location = new System.Drawing.Point(213, 45);
			this.recurStartDatePicker.Name = "recurStartDatePicker";
			this.recurStartDatePicker.Size = new System.Drawing.Size(157, 26);
			this.recurStartDatePicker.TabIndex = 3;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(209, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(87, 20);
			this.label1.TabIndex = 13;
			this.label1.Text = "Start Date:";
			// 
			// periodCombo
			// 
			this.periodCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.periodCombo.FormattingEnabled = true;
			this.periodCombo.Items.AddRange(new object[] {
            "Monthly",
            "Quarterly",
            "Yearly"});
			this.periodCombo.Location = new System.Drawing.Point(22, 36);
			this.periodCombo.Name = "periodCombo";
			this.periodCombo.Size = new System.Drawing.Size(142, 28);
			this.periodCombo.TabIndex = 2;
			// 
			// EditBudget
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(426, 431);
			this.Controls.Add(this.typeGroup);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.budgetGroup);
			this.Controls.Add(this.datesGroup);
			this.Controls.Add(this.periodGroup);
			this.Cursor = System.Windows.Forms.Cursors.Default;
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "EditBudget";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Edit Budget";
			this.budgetGroup.ResumeLayout(false);
			this.budgetGroup.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.amountUD)).EndInit();
			this.typeGroup.ResumeLayout(false);
			this.typeGroup.PerformLayout();
			this.datesGroup.ResumeLayout(false);
			this.datesGroup.PerformLayout();
			this.periodGroup.ResumeLayout(false);
			this.periodGroup.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox budgetGroup;
		private System.Windows.Forms.NumericUpDown amountUD;
		private System.Windows.Forms.ComboBox walletCombo;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button newWalletButton;
		private System.Windows.Forms.RadioButton recurringRadio;
		private System.Windows.Forms.RadioButton customRadio;
		private System.Windows.Forms.GroupBox typeGroup;
		private System.Windows.Forms.GroupBox datesGroup;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.DateTimePicker endDatePicker;
		private System.Windows.Forms.DateTimePicker startDatePicker;
		private System.Windows.Forms.GroupBox periodGroup;
		private System.Windows.Forms.ComboBox periodCombo;
		private System.Windows.Forms.DateTimePicker recurStartDatePicker;
		private System.Windows.Forms.Label label1;
	}
}