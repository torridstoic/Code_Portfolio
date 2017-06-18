namespace MoneyManager.Forms.v2
{
	partial class EditWallet
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
            this.walletGroup = new System.Windows.Forms.GroupBox();
            this.walletColorButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.nameText = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.walletTypeCombobox = new System.Windows.Forms.ComboBox();
            this.walletGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // walletGroup
            // 
            this.walletGroup.Controls.Add(this.walletTypeCombobox);
            this.walletGroup.Controls.Add(this.label3);
            this.walletGroup.Controls.Add(this.walletColorButton);
            this.walletGroup.Controls.Add(this.label2);
            this.walletGroup.Controls.Add(this.nameText);
            this.walletGroup.Controls.Add(this.label1);
            this.walletGroup.Location = new System.Drawing.Point(12, 12);
            this.walletGroup.Name = "walletGroup";
            this.walletGroup.Size = new System.Drawing.Size(371, 236);
            this.walletGroup.TabIndex = 0;
            this.walletGroup.TabStop = false;
            this.walletGroup.Text = "[title]";
            // 
            // walletColorButton
            // 
            this.walletColorButton.Location = new System.Drawing.Point(113, 114);
            this.walletColorButton.Name = "walletColorButton";
            this.walletColorButton.Size = new System.Drawing.Size(101, 23);
            this.walletColorButton.TabIndex = 3;
            this.walletColorButton.UseVisualStyleBackColor = true;
            this.walletColorButton.Click += new System.EventHandler(this.ColorEdit);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 115);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Wallet Color:";
            // 
            // nameText
            // 
            this.nameText.Location = new System.Drawing.Point(113, 48);
            this.nameText.Name = "nameText";
            this.nameText.Size = new System.Drawing.Size(211, 26);
            this.nameText.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(52, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name:";
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(87, 278);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(94, 27);
            this.okButton.TabIndex = 4;
            this.okButton.Text = "Save";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(215, 278);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(94, 27);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(60, 83);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 20);
            this.label3.TabIndex = 3;
            this.label3.Text = "Type:";
            // 
            // walletTypeCombobox
            // 
            this.walletTypeCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.walletTypeCombobox.FormattingEnabled = true;
            this.walletTypeCombobox.Location = new System.Drawing.Point(113, 80);
            this.walletTypeCombobox.Name = "walletTypeCombobox";
            this.walletTypeCombobox.Size = new System.Drawing.Size(121, 28);
            this.walletTypeCombobox.TabIndex = 1;
            // 
            // EditWallet
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(395, 363);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.walletGroup);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditWallet";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Wallet";
            this.walletGroup.ResumeLayout(false);
            this.walletGroup.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox walletGroup;
		private System.Windows.Forms.TextBox nameText;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button walletColorButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox walletTypeCombobox;
        private System.Windows.Forms.Label label3;
    }
}