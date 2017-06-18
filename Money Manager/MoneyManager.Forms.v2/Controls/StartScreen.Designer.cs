namespace MoneyManager.Forms.v2
{
    partial class StartScreen
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
            this.currentBudgetLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // currentBudgetLabel
            // 
            this.currentBudgetLabel.AutoSize = true;
            this.currentBudgetLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.currentBudgetLabel.Location = new System.Drawing.Point(53, 10);
            this.currentBudgetLabel.Name = "currentBudgetLabel";
            this.currentBudgetLabel.Size = new System.Drawing.Size(126, 20);
            this.currentBudgetLabel.TabIndex = 1;
            this.currentBudgetLabel.Text = "Current Budgets";
            // 
            // StartScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.currentBudgetLabel);
            this.Name = "StartScreen";
            this.Size = new System.Drawing.Size(584, 561);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.DrawLayout);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label currentBudgetLabel;
    }
}
