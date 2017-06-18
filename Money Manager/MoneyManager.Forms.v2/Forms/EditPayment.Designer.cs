namespace MoneyManager.Forms.v2
{
	partial class EditTransaction
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
			this.transactionGroup = new System.Windows.Forms.GroupBox();
			this.storeColorButton = new System.Windows.Forms.Button();
			this.frequencyGroup = new System.Windows.Forms.GroupBox();
			this.periodCombo = new System.Windows.Forms.ComboBox();
			this.recurringRadio = new System.Windows.Forms.RadioButton();
			this.singleRadio = new System.Windows.Forms.RadioButton();
			this.newWalletButton = new System.Windows.Forms.Button();
			this.walletCombo = new System.Windows.Forms.ComboBox();
			this.amountUD = new System.Windows.Forms.NumericUpDown();
			this.subjectCombo = new System.Windows.Forms.ComboBox();
			this.datePicker = new System.Windows.Forms.DateTimePicker();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.subjectLabel = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.transactionGroup.SuspendLayout();
			this.frequencyGroup.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.amountUD)).BeginInit();
			this.SuspendLayout();
			// 
			// transactionGroup
			// 
			this.transactionGroup.Controls.Add(this.label2);
			this.transactionGroup.Controls.Add(this.storeColorButton);
			this.transactionGroup.Controls.Add(this.frequencyGroup);
			this.transactionGroup.Controls.Add(this.recurringRadio);
			this.transactionGroup.Controls.Add(this.singleRadio);
			this.transactionGroup.Controls.Add(this.newWalletButton);
			this.transactionGroup.Controls.Add(this.walletCombo);
			this.transactionGroup.Controls.Add(this.amountUD);
			this.transactionGroup.Controls.Add(this.subjectCombo);
			this.transactionGroup.Controls.Add(this.datePicker);
			this.transactionGroup.Controls.Add(this.label4);
			this.transactionGroup.Controls.Add(this.label3);
			this.transactionGroup.Controls.Add(this.subjectLabel);
			this.transactionGroup.Controls.Add(this.label1);
			this.transactionGroup.Location = new System.Drawing.Point(12, 12);
			this.transactionGroup.Name = "transactionGroup";
			this.transactionGroup.Size = new System.Drawing.Size(371, 329);
			this.transactionGroup.TabIndex = 0;
			this.transactionGroup.TabStop = false;
			this.transactionGroup.Text = "[title]";
			// 
			// storeColorButton
			// 
			this.storeColorButton.Location = new System.Drawing.Point(183, 166);
			this.storeColorButton.Name = "storeColorButton";
			this.storeColorButton.Size = new System.Drawing.Size(58, 27);
			this.storeColorButton.TabIndex = 5;
			this.storeColorButton.UseVisualStyleBackColor = true;
			this.storeColorButton.Click += new System.EventHandler(this.ColorEdit);
			// 
			// frequencyGroup
			// 
			this.frequencyGroup.Controls.Add(this.periodCombo);
			this.frequencyGroup.Location = new System.Drawing.Point(131, 31);
			this.frequencyGroup.Name = "frequencyGroup";
			this.frequencyGroup.Size = new System.Drawing.Size(154, 63);
			this.frequencyGroup.TabIndex = 1;
			this.frequencyGroup.TabStop = false;
			this.frequencyGroup.Text = "Frequency";
			this.frequencyGroup.Visible = false;
			// 
			// periodCombo
			// 
			this.periodCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.periodCombo.FormattingEnabled = true;
			this.periodCombo.Items.AddRange(new object[] {
            "Daily",
            "Weekly",
            "Monthly",
            "Quarterly",
            "Yearly"});
			this.periodCombo.Location = new System.Drawing.Point(6, 25);
			this.periodCombo.Name = "periodCombo";
			this.periodCombo.Size = new System.Drawing.Size(141, 28);
			this.periodCombo.TabIndex = 2;
			// 
			// recurringRadio
			// 
			this.recurringRadio.AutoSize = true;
			this.recurringRadio.Location = new System.Drawing.Point(29, 61);
			this.recurringRadio.Name = "recurringRadio";
			this.recurringRadio.Size = new System.Drawing.Size(96, 24);
			this.recurringRadio.TabIndex = 1;
			this.recurringRadio.Text = "Recurring";
			this.recurringRadio.UseVisualStyleBackColor = true;
			this.recurringRadio.CheckedChanged += new System.EventHandler(this.radioChange);
			// 
			// singleRadio
			// 
			this.singleRadio.AutoSize = true;
			this.singleRadio.Checked = true;
			this.singleRadio.Location = new System.Drawing.Point(29, 31);
			this.singleRadio.Name = "singleRadio";
			this.singleRadio.Size = new System.Drawing.Size(71, 24);
			this.singleRadio.TabIndex = 0;
			this.singleRadio.TabStop = true;
			this.singleRadio.Text = "Single";
			this.singleRadio.UseVisualStyleBackColor = true;
			this.singleRadio.CheckedChanged += new System.EventHandler(this.radioChange);
			// 
			// newWalletButton
			// 
			this.newWalletButton.Location = new System.Drawing.Point(131, 282);
			this.newWalletButton.Name = "newWalletButton";
			this.newWalletButton.Size = new System.Drawing.Size(142, 29);
			this.newWalletButton.TabIndex = 8;
			this.newWalletButton.Text = "Add New Wallet";
			this.newWalletButton.UseVisualStyleBackColor = true;
			this.newWalletButton.Click += new System.EventHandler(this.newWalletButton_Click);
			// 
			// walletCombo
			// 
			this.walletCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.walletCombo.FormattingEnabled = true;
			this.walletCombo.Location = new System.Drawing.Point(131, 248);
			this.walletCombo.Name = "walletCombo";
			this.walletCombo.Size = new System.Drawing.Size(185, 28);
			this.walletCombo.TabIndex = 7;
			// 
			// amountUD
			// 
			this.amountUD.DecimalPlaces = 2;
			this.amountUD.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
			this.amountUD.Location = new System.Drawing.Point(131, 216);
			this.amountUD.Maximum = new decimal(new int[] {
            5000000,
            0,
            0,
            0});
			this.amountUD.Name = "amountUD";
			this.amountUD.Size = new System.Drawing.Size(110, 26);
			this.amountUD.TabIndex = 6;
			// 
			// subjectCombo
			// 
			this.subjectCombo.FormattingEnabled = true;
			this.subjectCombo.Location = new System.Drawing.Point(131, 132);
			this.subjectCombo.Name = "subjectCombo";
			this.subjectCombo.Size = new System.Drawing.Size(185, 28);
			this.subjectCombo.TabIndex = 4;
			this.subjectCombo.SelectedValueChanged += new System.EventHandler(this.PayeeChanged);
			// 
			// datePicker
			// 
			this.datePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.datePicker.Location = new System.Drawing.Point(131, 100);
			this.datePicker.Name = "datePicker";
			this.datePicker.Size = new System.Drawing.Size(129, 26);
			this.datePicker.TabIndex = 3;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(68, 251);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(57, 20);
			this.label4.TabIndex = 3;
			this.label4.Text = "Wallet:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(56, 218);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(69, 20);
			this.label3.TabIndex = 2;
			this.label3.Text = "Amount:";
			// 
			// subjectLabel
			// 
			this.subjectLabel.AutoSize = true;
			this.subjectLabel.Location = new System.Drawing.Point(68, 135);
			this.subjectLabel.Name = "subjectLabel";
			this.subjectLabel.Size = new System.Drawing.Size(57, 20);
			this.subjectLabel.TabIndex = 1;
			this.subjectLabel.Text = "Payee:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(77, 105);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(48, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "Date:";
			// 
			// okButton
			// 
			this.okButton.Location = new System.Drawing.Point(84, 357);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(93, 32);
			this.okButton.TabIndex = 9;
			this.okButton.Text = "Save";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(206, 357);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(93, 32);
			this.cancelButton.TabIndex = 10;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(127, 169);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(50, 20);
			this.label2.TabIndex = 9;
			this.label2.Text = "Color:";
			// 
			// EditTransaction
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(395, 401);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.transactionGroup);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "EditTransaction";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Edit Transaction";
			this.transactionGroup.ResumeLayout(false);
			this.transactionGroup.PerformLayout();
			this.frequencyGroup.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.amountUD)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox transactionGroup;
		private System.Windows.Forms.DateTimePicker datePicker;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label subjectLabel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox subjectCombo;
		private System.Windows.Forms.NumericUpDown amountUD;
		private System.Windows.Forms.ComboBox walletCombo;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button newWalletButton;
		private System.Windows.Forms.RadioButton recurringRadio;
		private System.Windows.Forms.RadioButton singleRadio;
		private System.Windows.Forms.GroupBox frequencyGroup;
		private System.Windows.Forms.ComboBox periodCombo;
        private System.Windows.Forms.Button storeColorButton;
		private System.Windows.Forms.Label label2;
	}
}