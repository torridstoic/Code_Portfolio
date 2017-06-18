namespace MoneyManager.Forms.v2
{
    partial class Graphs
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
			this.controlsGroupbox = new System.Windows.Forms.GroupBox();
			this.storeSortRadio = new System.Windows.Forms.RadioButton();
			this.walletSortRadio = new System.Windows.Forms.RadioButton();
			this.barGraphCheckbox = new System.Windows.Forms.CheckBox();
			this.pieGraphCheckbox = new System.Windows.Forms.CheckBox();
			this.controlsGroupbox.SuspendLayout();
			this.SuspendLayout();
			// 
			// controlsGroupbox
			// 
			this.controlsGroupbox.Controls.Add(this.storeSortRadio);
			this.controlsGroupbox.Controls.Add(this.walletSortRadio);
			this.controlsGroupbox.Controls.Add(this.barGraphCheckbox);
			this.controlsGroupbox.Controls.Add(this.pieGraphCheckbox);
			this.controlsGroupbox.Dock = System.Windows.Forms.DockStyle.Top;
			this.controlsGroupbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.controlsGroupbox.Location = new System.Drawing.Point(0, 0);
			this.controlsGroupbox.Name = "controlsGroupbox";
			this.controlsGroupbox.Size = new System.Drawing.Size(584, 79);
			this.controlsGroupbox.TabIndex = 4;
			this.controlsGroupbox.TabStop = false;
			this.controlsGroupbox.Text = "Graphing Controls";
			// 
			// storeSortRadio
			// 
			this.storeSortRadio.Location = new System.Drawing.Point(114, 49);
			this.storeSortRadio.Name = "storeSortRadio";
			this.storeSortRadio.Size = new System.Drawing.Size(130, 24);
			this.storeSortRadio.TabIndex = 7;
			this.storeSortRadio.TabStop = true;
			this.storeSortRadio.Text = "Sort By Stores";
			this.storeSortRadio.UseVisualStyleBackColor = true;
			this.storeSortRadio.Click += new System.EventHandler(this.UpdateGraph);
			// 
			// walletSortRadio
			// 
			this.walletSortRadio.Location = new System.Drawing.Point(114, 25);
			this.walletSortRadio.Name = "walletSortRadio";
			this.walletSortRadio.Size = new System.Drawing.Size(135, 24);
			this.walletSortRadio.TabIndex = 6;
			this.walletSortRadio.TabStop = true;
			this.walletSortRadio.Text = "Sort By Wallets";
			this.walletSortRadio.UseVisualStyleBackColor = true;
			this.walletSortRadio.Click += new System.EventHandler(this.UpdateGraph);
			// 
			// barGraphCheckbox
			// 
			this.barGraphCheckbox.Location = new System.Drawing.Point(6, 49);
			this.barGraphCheckbox.Name = "barGraphCheckbox";
			this.barGraphCheckbox.Size = new System.Drawing.Size(102, 24);
			this.barGraphCheckbox.TabIndex = 5;
			this.barGraphCheckbox.Text = "Bar Graph";
			this.barGraphCheckbox.UseVisualStyleBackColor = true;
			this.barGraphCheckbox.Click += new System.EventHandler(this.UpdateGraph);
			// 
			// pieGraphCheckbox
			// 
			this.pieGraphCheckbox.Location = new System.Drawing.Point(6, 25);
			this.pieGraphCheckbox.Name = "pieGraphCheckbox";
			this.pieGraphCheckbox.Size = new System.Drawing.Size(99, 24);
			this.pieGraphCheckbox.TabIndex = 4;
			this.pieGraphCheckbox.Text = "Pie Graph";
			this.pieGraphCheckbox.UseVisualStyleBackColor = true;
			this.pieGraphCheckbox.Click += new System.EventHandler(this.UpdateGraph);
			// 
			// Graphs
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.Controls.Add(this.controlsGroupbox);
			this.Name = "Graphs";
			this.Size = new System.Drawing.Size(584, 561);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.DrawGraph);
			this.controlsGroupbox.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox controlsGroupbox;
        private System.Windows.Forms.RadioButton storeSortRadio;
        private System.Windows.Forms.RadioButton walletSortRadio;
        private System.Windows.Forms.CheckBox barGraphCheckbox;
        private System.Windows.Forms.CheckBox pieGraphCheckbox;
    }
}
