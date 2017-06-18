namespace GameOfLife
{
	partial class SeedDialog
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
			this.seedLabel = new System.Windows.Forms.Label();
			this.seedUD = new System.Windows.Forms.NumericUpDown();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.seedUD)).BeginInit();
			this.SuspendLayout();
			// 
			// seedLabel
			// 
			this.seedLabel.AutoSize = true;
			this.seedLabel.Location = new System.Drawing.Point(34, 26);
			this.seedLabel.Name = "seedLabel";
			this.seedLabel.Size = new System.Drawing.Size(32, 13);
			this.seedLabel.TabIndex = 0;
			this.seedLabel.Text = "Seed";
			// 
			// seedUD
			// 
			this.seedUD.Location = new System.Drawing.Point(72, 24);
			this.seedUD.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
			this.seedUD.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
			this.seedUD.Name = "seedUD";
			this.seedUD.Size = new System.Drawing.Size(120, 20);
			this.seedUD.TabIndex = 1;
			// 
			// okButton
			// 
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(36, 65);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 2;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(117, 65);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 3;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// SeedDialog
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(226, 100);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.seedUD);
			this.Controls.Add(this.seedLabel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SeedDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Seed Dialog";
			((System.ComponentModel.ISupportInitialize)(this.seedUD)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label seedLabel;
		private System.Windows.Forms.NumericUpDown seedUD;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
	}
}