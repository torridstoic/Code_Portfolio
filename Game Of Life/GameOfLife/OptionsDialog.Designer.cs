namespace GameOfLife
{
	partial class OptionsDialog
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
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.generalTab = new System.Windows.Forms.TabPage();
			this.heightUD = new System.Windows.Forms.NumericUpDown();
			this.widthUD = new System.Windows.Forms.NumericUpDown();
			this.timerUD = new System.Windows.Forms.NumericUpDown();
			this.heightLabel = new System.Windows.Forms.Label();
			this.widthLabel = new System.Windows.Forms.Label();
			this.timerLabel = new System.Windows.Forms.Label();
			this.viewTab = new System.Windows.Forms.TabPage();
			this.cellColorLabel = new System.Windows.Forms.Label();
			this.bgColorLabel = new System.Windows.Forms.Label();
			this.gridColorLabel = new System.Windows.Forms.Label();
			this.resetButton = new System.Windows.Forms.Button();
			this.cellColorButton = new System.Windows.Forms.Button();
			this.bgColorButton = new System.Windows.Forms.Button();
			this.gridColorButton = new System.Windows.Forms.Button();
			this.advancedTab = new System.Windows.Forms.TabPage();
			this.boundaryGroupBox = new System.Windows.Forms.GroupBox();
			this.infiniteRadio = new System.Windows.Forms.RadioButton();
			this.toroidalRadio = new System.Windows.Forms.RadioButton();
			this.finiteRadio = new System.Windows.Forms.RadioButton();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.resetDefaultsButton = new System.Windows.Forms.Button();
			this.applyButton = new System.Windows.Forms.Button();
			this.tabControl1.SuspendLayout();
			this.generalTab.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.heightUD)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.widthUD)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.timerUD)).BeginInit();
			this.viewTab.SuspendLayout();
			this.advancedTab.SuspendLayout();
			this.boundaryGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.generalTab);
			this.tabControl1.Controls.Add(this.viewTab);
			this.tabControl1.Controls.Add(this.advancedTab);
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(497, 274);
			this.tabControl1.TabIndex = 0;
			// 
			// generalTab
			// 
			this.generalTab.Controls.Add(this.heightUD);
			this.generalTab.Controls.Add(this.widthUD);
			this.generalTab.Controls.Add(this.timerUD);
			this.generalTab.Controls.Add(this.heightLabel);
			this.generalTab.Controls.Add(this.widthLabel);
			this.generalTab.Controls.Add(this.timerLabel);
			this.generalTab.Location = new System.Drawing.Point(4, 22);
			this.generalTab.Name = "generalTab";
			this.generalTab.Padding = new System.Windows.Forms.Padding(3);
			this.generalTab.Size = new System.Drawing.Size(489, 248);
			this.generalTab.TabIndex = 0;
			this.generalTab.Text = "General";
			this.generalTab.UseVisualStyleBackColor = true;
			// 
			// heightUD
			// 
			this.heightUD.Location = new System.Drawing.Point(164, 75);
			this.heightUD.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
			this.heightUD.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.heightUD.Name = "heightUD";
			this.heightUD.Size = new System.Drawing.Size(90, 20);
			this.heightUD.TabIndex = 5;
			this.heightUD.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
			// 
			// widthUD
			// 
			this.widthUD.Location = new System.Drawing.Point(164, 49);
			this.widthUD.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
			this.widthUD.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.widthUD.Name = "widthUD";
			this.widthUD.Size = new System.Drawing.Size(90, 20);
			this.widthUD.TabIndex = 4;
			this.widthUD.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
			// 
			// timerUD
			// 
			this.timerUD.Location = new System.Drawing.Point(164, 23);
			this.timerUD.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
			this.timerUD.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.timerUD.Name = "timerUD";
			this.timerUD.Size = new System.Drawing.Size(90, 20);
			this.timerUD.TabIndex = 3;
			this.timerUD.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// heightLabel
			// 
			this.heightLabel.AutoSize = true;
			this.heightLabel.Location = new System.Drawing.Point(16, 77);
			this.heightLabel.Name = "heightLabel";
			this.heightLabel.Size = new System.Drawing.Size(131, 13);
			this.heightLabel.TabIndex = 2;
			this.heightLabel.Text = "Height of Universe in Cells";
			// 
			// widthLabel
			// 
			this.widthLabel.AutoSize = true;
			this.widthLabel.Location = new System.Drawing.Point(16, 51);
			this.widthLabel.Name = "widthLabel";
			this.widthLabel.Size = new System.Drawing.Size(128, 13);
			this.widthLabel.TabIndex = 1;
			this.widthLabel.Text = "Width of Universe in Cells";
			// 
			// timerLabel
			// 
			this.timerLabel.AutoSize = true;
			this.timerLabel.Location = new System.Drawing.Point(16, 25);
			this.timerLabel.Name = "timerLabel";
			this.timerLabel.Size = new System.Drawing.Size(142, 13);
			this.timerLabel.TabIndex = 0;
			this.timerLabel.Text = "Timer Interval in Milliseconds";
			// 
			// viewTab
			// 
			this.viewTab.Controls.Add(this.cellColorLabel);
			this.viewTab.Controls.Add(this.bgColorLabel);
			this.viewTab.Controls.Add(this.gridColorLabel);
			this.viewTab.Controls.Add(this.resetButton);
			this.viewTab.Controls.Add(this.cellColorButton);
			this.viewTab.Controls.Add(this.bgColorButton);
			this.viewTab.Controls.Add(this.gridColorButton);
			this.viewTab.Location = new System.Drawing.Point(4, 22);
			this.viewTab.Name = "viewTab";
			this.viewTab.Padding = new System.Windows.Forms.Padding(3);
			this.viewTab.Size = new System.Drawing.Size(489, 248);
			this.viewTab.TabIndex = 1;
			this.viewTab.Text = "View";
			this.viewTab.UseVisualStyleBackColor = true;
			// 
			// cellColorLabel
			// 
			this.cellColorLabel.AutoSize = true;
			this.cellColorLabel.Location = new System.Drawing.Point(110, 108);
			this.cellColorLabel.Name = "cellColorLabel";
			this.cellColorLabel.Size = new System.Drawing.Size(74, 13);
			this.cellColorLabel.TabIndex = 6;
			this.cellColorLabel.Text = "Live Cell Color";
			// 
			// bgColorLabel
			// 
			this.bgColorLabel.AutoSize = true;
			this.bgColorLabel.Location = new System.Drawing.Point(110, 79);
			this.bgColorLabel.Name = "bgColorLabel";
			this.bgColorLabel.Size = new System.Drawing.Size(92, 13);
			this.bgColorLabel.TabIndex = 5;
			this.bgColorLabel.Text = "Background Color";
			// 
			// gridColorLabel
			// 
			this.gridColorLabel.AutoSize = true;
			this.gridColorLabel.Location = new System.Drawing.Point(110, 50);
			this.gridColorLabel.Name = "gridColorLabel";
			this.gridColorLabel.Size = new System.Drawing.Size(53, 13);
			this.gridColorLabel.TabIndex = 4;
			this.gridColorLabel.Text = "Grid Color";
			// 
			// resetButton
			// 
			this.resetButton.Location = new System.Drawing.Point(59, 147);
			this.resetButton.Name = "resetButton";
			this.resetButton.Size = new System.Drawing.Size(104, 23);
			this.resetButton.TabIndex = 3;
			this.resetButton.Text = "Reset Colors";
			this.resetButton.UseVisualStyleBackColor = true;
			this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
			// 
			// cellColorButton
			// 
			this.cellColorButton.BackColor = System.Drawing.Color.DarkGray;
			this.cellColorButton.Location = new System.Drawing.Point(59, 103);
			this.cellColorButton.Name = "cellColorButton";
			this.cellColorButton.Size = new System.Drawing.Size(36, 23);
			this.cellColorButton.TabIndex = 2;
			this.cellColorButton.UseVisualStyleBackColor = false;
			this.cellColorButton.Click += new System.EventHandler(this.Color_Button);
			// 
			// bgColorButton
			// 
			this.bgColorButton.BackColor = System.Drawing.Color.White;
			this.bgColorButton.Location = new System.Drawing.Point(59, 74);
			this.bgColorButton.Name = "bgColorButton";
			this.bgColorButton.Size = new System.Drawing.Size(36, 23);
			this.bgColorButton.TabIndex = 1;
			this.bgColorButton.UseVisualStyleBackColor = false;
			this.bgColorButton.Click += new System.EventHandler(this.Color_Button);
			// 
			// gridColorButton
			// 
			this.gridColorButton.BackColor = System.Drawing.Color.Black;
			this.gridColorButton.Location = new System.Drawing.Point(59, 45);
			this.gridColorButton.Name = "gridColorButton";
			this.gridColorButton.Size = new System.Drawing.Size(36, 23);
			this.gridColorButton.TabIndex = 0;
			this.gridColorButton.UseVisualStyleBackColor = false;
			this.gridColorButton.Click += new System.EventHandler(this.Color_Button);
			// 
			// advancedTab
			// 
			this.advancedTab.Controls.Add(this.boundaryGroupBox);
			this.advancedTab.Location = new System.Drawing.Point(4, 22);
			this.advancedTab.Name = "advancedTab";
			this.advancedTab.Size = new System.Drawing.Size(489, 248);
			this.advancedTab.TabIndex = 2;
			this.advancedTab.Text = "Advanced";
			this.advancedTab.UseVisualStyleBackColor = true;
			// 
			// boundaryGroupBox
			// 
			this.boundaryGroupBox.Controls.Add(this.infiniteRadio);
			this.boundaryGroupBox.Controls.Add(this.toroidalRadio);
			this.boundaryGroupBox.Controls.Add(this.finiteRadio);
			this.boundaryGroupBox.Location = new System.Drawing.Point(8, 14);
			this.boundaryGroupBox.Name = "boundaryGroupBox";
			this.boundaryGroupBox.Size = new System.Drawing.Size(111, 114);
			this.boundaryGroupBox.TabIndex = 0;
			this.boundaryGroupBox.TabStop = false;
			this.boundaryGroupBox.Text = "Boundary Type";
			// 
			// infiniteRadio
			// 
			this.infiniteRadio.AutoSize = true;
			this.infiniteRadio.Enabled = false;
			this.infiniteRadio.Location = new System.Drawing.Point(6, 65);
			this.infiniteRadio.Name = "infiniteRadio";
			this.infiniteRadio.Size = new System.Drawing.Size(56, 17);
			this.infiniteRadio.TabIndex = 2;
			this.infiniteRadio.Text = "Infinite";
			this.infiniteRadio.UseVisualStyleBackColor = true;
			// 
			// toroidalRadio
			// 
			this.toroidalRadio.AutoSize = true;
			this.toroidalRadio.Location = new System.Drawing.Point(6, 42);
			this.toroidalRadio.Name = "toroidalRadio";
			this.toroidalRadio.Size = new System.Drawing.Size(63, 17);
			this.toroidalRadio.TabIndex = 1;
			this.toroidalRadio.Text = "Toroidal";
			this.toroidalRadio.UseVisualStyleBackColor = true;
			// 
			// finiteRadio
			// 
			this.finiteRadio.AutoSize = true;
			this.finiteRadio.Location = new System.Drawing.Point(6, 19);
			this.finiteRadio.Name = "finiteRadio";
			this.finiteRadio.Size = new System.Drawing.Size(50, 17);
			this.finiteRadio.TabIndex = 0;
			this.finiteRadio.Text = "Finite";
			this.finiteRadio.UseVisualStyleBackColor = true;
			// 
			// okButton
			// 
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(12, 280);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 1;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(102, 280);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 2;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// resetDefaultsButton
			// 
			this.resetDefaultsButton.Location = new System.Drawing.Point(355, 280);
			this.resetDefaultsButton.Name = "resetDefaultsButton";
			this.resetDefaultsButton.Size = new System.Drawing.Size(129, 23);
			this.resetDefaultsButton.TabIndex = 3;
			this.resetDefaultsButton.Text = "Reset All Defaults";
			this.resetDefaultsButton.UseVisualStyleBackColor = true;
			this.resetDefaultsButton.Click += new System.EventHandler(this.resetDefaultsButton_Click);
			// 
			// applyButton
			// 
			this.applyButton.Location = new System.Drawing.Point(194, 280);
			this.applyButton.Name = "applyButton";
			this.applyButton.Size = new System.Drawing.Size(75, 23);
			this.applyButton.TabIndex = 4;
			this.applyButton.Text = "Apply";
			this.applyButton.UseVisualStyleBackColor = true;
			this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
			// 
			// OptionsDialog
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(496, 315);
			this.Controls.Add(this.applyButton);
			this.Controls.Add(this.resetDefaultsButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.tabControl1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "OptionsDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Options Dialog";
			this.tabControl1.ResumeLayout(false);
			this.generalTab.ResumeLayout(false);
			this.generalTab.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.heightUD)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.widthUD)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.timerUD)).EndInit();
			this.viewTab.ResumeLayout(false);
			this.viewTab.PerformLayout();
			this.advancedTab.ResumeLayout(false);
			this.boundaryGroupBox.ResumeLayout(false);
			this.boundaryGroupBox.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage generalTab;
		private System.Windows.Forms.NumericUpDown heightUD;
		private System.Windows.Forms.NumericUpDown widthUD;
		private System.Windows.Forms.NumericUpDown timerUD;
		private System.Windows.Forms.Label heightLabel;
		private System.Windows.Forms.Label widthLabel;
		private System.Windows.Forms.Label timerLabel;
		private System.Windows.Forms.TabPage viewTab;
		private System.Windows.Forms.TabPage advancedTab;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Label cellColorLabel;
		private System.Windows.Forms.Label bgColorLabel;
		private System.Windows.Forms.Label gridColorLabel;
		private System.Windows.Forms.Button resetButton;
		private System.Windows.Forms.Button cellColorButton;
		private System.Windows.Forms.Button bgColorButton;
		private System.Windows.Forms.Button gridColorButton;
		private System.Windows.Forms.GroupBox boundaryGroupBox;
		private System.Windows.Forms.RadioButton infiniteRadio;
		private System.Windows.Forms.RadioButton toroidalRadio;
		private System.Windows.Forms.RadioButton finiteRadio;
		private System.Windows.Forms.Button resetDefaultsButton;
		private System.Windows.Forms.Button applyButton;
	}
}