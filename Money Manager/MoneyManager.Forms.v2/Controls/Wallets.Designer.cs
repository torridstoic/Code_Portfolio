namespace MoneyManager.Forms.v2
{
	partial class Wallets
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
			this.walletsGroup = new System.Windows.Forms.GroupBox();
			this.walletDataGrid = new System.Windows.Forms.DataGridView();
			this.newButton = new System.Windows.Forms.Button();
			this.deleteButton = new System.Windows.Forms.Button();
			this.editButton = new System.Windows.Forms.Button();
			this.walletsGroup.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.walletDataGrid)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(207, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(151, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "Wallet Management";
			// 
			// walletsGroup
			// 
			this.walletsGroup.Controls.Add(this.walletDataGrid);
			this.walletsGroup.Controls.Add(this.newButton);
			this.walletsGroup.Controls.Add(this.deleteButton);
			this.walletsGroup.Controls.Add(this.editButton);
			this.walletsGroup.Location = new System.Drawing.Point(40, 42);
			this.walletsGroup.Name = "walletsGroup";
			this.walletsGroup.Size = new System.Drawing.Size(489, 497);
			this.walletsGroup.TabIndex = 1;
			this.walletsGroup.TabStop = false;
			this.walletsGroup.Text = "Your Wallets";
			// 
			// walletDataGrid
			// 
			this.walletDataGrid.AllowUserToAddRows = false;
			this.walletDataGrid.AllowUserToDeleteRows = false;
			this.walletDataGrid.AllowUserToResizeColumns = false;
			this.walletDataGrid.AllowUserToResizeRows = false;
			this.walletDataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.walletDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.walletDataGrid.Location = new System.Drawing.Point(6, 25);
			this.walletDataGrid.MultiSelect = false;
			this.walletDataGrid.Name = "walletDataGrid";
			this.walletDataGrid.ReadOnly = true;
			this.walletDataGrid.Size = new System.Drawing.Size(477, 382);
			this.walletDataGrid.TabIndex = 4;
			this.walletDataGrid.SelectionChanged += new System.EventHandler(this.gridClicked);
			// 
			// newButton
			// 
			this.newButton.Location = new System.Drawing.Point(171, 450);
			this.newButton.Name = "newButton";
			this.newButton.Size = new System.Drawing.Size(147, 30);
			this.newButton.TabIndex = 3;
			this.newButton.Text = "Add New Wallet";
			this.newButton.UseVisualStyleBackColor = true;
			this.newButton.Click += new System.EventHandler(this.newButton_Click);
			// 
			// deleteButton
			// 
			this.deleteButton.Location = new System.Drawing.Point(256, 413);
			this.deleteButton.Name = "deleteButton";
			this.deleteButton.Size = new System.Drawing.Size(133, 31);
			this.deleteButton.TabIndex = 2;
			this.deleteButton.Text = "Delete Wallet";
			this.deleteButton.UseVisualStyleBackColor = true;
			this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
			// 
			// editButton
			// 
			this.editButton.Location = new System.Drawing.Point(97, 413);
			this.editButton.Name = "editButton";
			this.editButton.Size = new System.Drawing.Size(133, 31);
			this.editButton.TabIndex = 1;
			this.editButton.Text = "Edit Wallet";
			this.editButton.UseVisualStyleBackColor = true;
			this.editButton.Click += new System.EventHandler(this.editButton_Click);
			// 
			// Wallets
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.Controls.Add(this.walletsGroup);
			this.Controls.Add(this.label1);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.Name = "Wallets";
			this.Size = new System.Drawing.Size(584, 561);
			this.walletsGroup.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.walletDataGrid)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox walletsGroup;
		private System.Windows.Forms.Button newButton;
		private System.Windows.Forms.Button deleteButton;
		private System.Windows.Forms.Button editButton;
        private System.Windows.Forms.DataGridView walletDataGrid;
    }
}
