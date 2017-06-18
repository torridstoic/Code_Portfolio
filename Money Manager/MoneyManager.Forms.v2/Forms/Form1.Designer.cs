namespace MoneyManager.Forms.v2
{
	partial class Form1
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.budgetLabel = new System.Windows.Forms.Label();
            this.welcomeLabel = new System.Windows.Forms.Label();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.startScreenButton = new System.Windows.Forms.Button();
            this.graphButton = new System.Windows.Forms.Button();
            this.walletsButton = new System.Windows.Forms.Button();
            this.reportsButton = new System.Windows.Forms.Button();
            this.paymentButton = new System.Windows.Forms.Button();
            this.budgetsButton = new System.Windows.Forms.Button();
            this.logoutButton = new System.Windows.Forms.Button();
            this.manageAcctButton = new System.Windows.Forms.Button();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.savingsButton = new System.Windows.Forms.Button();
            this.creditsButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel1.Controls.Add(this.splitter1);
            this.splitContainer1.Size = new System.Drawing.Size(784, 661);
            this.splitContainer1.SplitterDistance = 200;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.IsSplitterFixed = true;
            this.splitContainer2.Location = new System.Drawing.Point(3, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.budgetLabel);
            this.splitContainer2.Panel1.Controls.Add(this.welcomeLabel);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer2.Size = new System.Drawing.Size(197, 661);
            this.splitContainer2.SplitterDistance = 86;
            this.splitContainer2.TabIndex = 2;
            // 
            // budgetLabel
            // 
            this.budgetLabel.AutoSize = true;
            this.budgetLabel.Location = new System.Drawing.Point(8, 53);
            this.budgetLabel.Name = "budgetLabel";
            this.budgetLabel.Size = new System.Drawing.Size(104, 20);
            this.budgetLabel.TabIndex = 1;
            this.budgetLabel.Text = "Total Budget:";
            // 
            // welcomeLabel
            // 
            this.welcomeLabel.AutoSize = true;
            this.welcomeLabel.Location = new System.Drawing.Point(8, 8);
            this.welcomeLabel.Name = "welcomeLabel";
            this.welcomeLabel.Size = new System.Drawing.Size(113, 20);
            this.welcomeLabel.TabIndex = 0;
            this.welcomeLabel.Text = "Welcome back";
            // 
            // splitContainer3
            // 
            this.splitContainer3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.IsSplitterFixed = true;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.startScreenButton);
            this.splitContainer3.Panel1.Controls.Add(this.graphButton);
            this.splitContainer3.Panel1.Controls.Add(this.walletsButton);
            this.splitContainer3.Panel1.Controls.Add(this.reportsButton);
            this.splitContainer3.Panel1.Controls.Add(this.creditsButton);
            this.splitContainer3.Panel1.Controls.Add(this.paymentButton);
            this.splitContainer3.Panel1.Controls.Add(this.savingsButton);
            this.splitContainer3.Panel1.Controls.Add(this.budgetsButton);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.logoutButton);
            this.splitContainer3.Panel2.Controls.Add(this.manageAcctButton);
            this.splitContainer3.Size = new System.Drawing.Size(197, 571);
            this.splitContainer3.SplitterDistance = 413;
            this.splitContainer3.TabIndex = 0;
            // 
            // startScreenButton
            // 
            this.startScreenButton.FlatAppearance.BorderSize = 0;
            this.startScreenButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.startScreenButton.Location = new System.Drawing.Point(0, 3);
            this.startScreenButton.Name = "startScreenButton";
            this.startScreenButton.Size = new System.Drawing.Size(194, 32);
            this.startScreenButton.TabIndex = 0;
            this.startScreenButton.Text = "Main Menu";
            this.startScreenButton.UseVisualStyleBackColor = true;
            this.startScreenButton.Click += new System.EventHandler(this.MenuButtonClick);
            // 
            // graphButton
            // 
            this.graphButton.FlatAppearance.BorderSize = 0;
            this.graphButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.graphButton.Location = new System.Drawing.Point(0, 376);
            this.graphButton.Name = "graphButton";
            this.graphButton.Size = new System.Drawing.Size(194, 32);
            this.graphButton.TabIndex = 8;
            this.graphButton.Text = "Graphs";
            this.graphButton.UseVisualStyleBackColor = true;
            this.graphButton.Click += new System.EventHandler(this.MenuButtonClick);
            // 
            // walletsButton
            // 
            this.walletsButton.FlatAppearance.BorderSize = 0;
            this.walletsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.walletsButton.Location = new System.Drawing.Point(0, 41);
            this.walletsButton.Name = "walletsButton";
            this.walletsButton.Size = new System.Drawing.Size(194, 32);
            this.walletsButton.TabIndex = 1;
            this.walletsButton.Text = "Wallets";
            this.walletsButton.UseVisualStyleBackColor = true;
            this.walletsButton.Click += new System.EventHandler(this.MenuButtonClick);
            // 
            // reportsButton
            // 
            this.reportsButton.FlatAppearance.BorderSize = 0;
            this.reportsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.reportsButton.Location = new System.Drawing.Point(0, 333);
            this.reportsButton.Name = "reportsButton";
            this.reportsButton.Size = new System.Drawing.Size(194, 32);
            this.reportsButton.TabIndex = 7;
            this.reportsButton.Text = "Reports";
            this.reportsButton.UseVisualStyleBackColor = true;
            this.reportsButton.Click += new System.EventHandler(this.MenuButtonClick);
            // 
            // paymentButton
            // 
            this.paymentButton.FlatAppearance.BorderSize = 0;
            this.paymentButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.paymentButton.Location = new System.Drawing.Point(0, 117);
            this.paymentButton.Name = "paymentButton";
            this.paymentButton.Size = new System.Drawing.Size(194, 32);
            this.paymentButton.TabIndex = 3;
            this.paymentButton.Text = "Payments";
            this.paymentButton.UseVisualStyleBackColor = true;
            this.paymentButton.Click += new System.EventHandler(this.MenuButtonClick);
            // 
            // budgetsButton
            // 
            this.budgetsButton.FlatAppearance.BorderSize = 0;
            this.budgetsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.budgetsButton.Location = new System.Drawing.Point(0, 79);
            this.budgetsButton.Name = "budgetsButton";
            this.budgetsButton.Size = new System.Drawing.Size(194, 32);
            this.budgetsButton.TabIndex = 2;
            this.budgetsButton.Text = "Budgets";
            this.budgetsButton.UseVisualStyleBackColor = true;
            this.budgetsButton.Click += new System.EventHandler(this.MenuButtonClick);
            // 
            // logoutButton
            // 
            this.logoutButton.FlatAppearance.BorderSize = 0;
            this.logoutButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.logoutButton.Location = new System.Drawing.Point(0, 41);
            this.logoutButton.Name = "logoutButton";
            this.logoutButton.Size = new System.Drawing.Size(194, 32);
            this.logoutButton.TabIndex = 11;
            this.logoutButton.Text = "Logout";
            this.logoutButton.UseVisualStyleBackColor = true;
            this.logoutButton.Click += new System.EventHandler(this.MenuButtonClick);
            // 
            // manageAcctButton
            // 
            this.manageAcctButton.FlatAppearance.BorderSize = 0;
            this.manageAcctButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.manageAcctButton.Location = new System.Drawing.Point(0, 3);
            this.manageAcctButton.Name = "manageAcctButton";
            this.manageAcctButton.Size = new System.Drawing.Size(194, 32);
            this.manageAcctButton.TabIndex = 10;
            this.manageAcctButton.Text = "Manage Account";
            this.manageAcctButton.UseVisualStyleBackColor = true;
            this.manageAcctButton.Click += new System.EventHandler(this.MenuButtonClick);
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(0, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 661);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // savingsButton
            // 
            this.savingsButton.FlatAppearance.BorderSize = 0;
            this.savingsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.savingsButton.Location = new System.Drawing.Point(0, 191);
            this.savingsButton.Name = "savingsButton";
            this.savingsButton.Size = new System.Drawing.Size(194, 32);
            this.savingsButton.TabIndex = 6;
            this.savingsButton.Text = "Savings";
            this.savingsButton.UseVisualStyleBackColor = true;
            this.savingsButton.Click += new System.EventHandler(this.MenuButtonClick);
            // 
            // creditsButton
            // 
            this.creditsButton.FlatAppearance.BorderSize = 0;
            this.creditsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.creditsButton.Location = new System.Drawing.Point(0, 155);
            this.creditsButton.Name = "creditsButton";
            this.creditsButton.Size = new System.Drawing.Size(194, 32);
            this.creditsButton.TabIndex = 4;
            this.creditsButton.Text = "Credits";
            this.creditsButton.UseVisualStyleBackColor = true;
            this.creditsButton.Click += new System.EventHandler(this.MenuButtonClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 661);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Money Manager";
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.Button paymentButton;
		private System.Windows.Forms.Button budgetsButton;
		private System.Windows.Forms.Button walletsButton;
		private System.Windows.Forms.Button reportsButton;
		private System.Windows.Forms.Button logoutButton;
		private System.Windows.Forms.Button manageAcctButton;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.SplitContainer splitContainer3;
		private System.Windows.Forms.Label welcomeLabel;
		private System.Windows.Forms.Label budgetLabel;
        private System.Windows.Forms.Button graphButton;
        private System.Windows.Forms.Button startScreenButton;
        private System.Windows.Forms.Button creditsButton;
        private System.Windows.Forms.Button savingsButton;
    }
}

