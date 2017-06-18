namespace MoneyManager.Forms.v2
{
	partial class RecurringTransactions
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
            this.nonrecurringButton = new System.Windows.Forms.Button();
            this.recurringGroup = new System.Windows.Forms.GroupBox();
            this.addButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.editButton = new System.Windows.Forms.Button();
            this.recurringGrid = new System.Windows.Forms.DataGridView();
            this.recurringGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.recurringGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Location = new System.Drawing.Point(179, 13);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(197, 20);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "Recurring [Title] Templates";
            // 
            // nonrecurringButton
            // 
            this.nonrecurringButton.Location = new System.Drawing.Point(17, 45);
            this.nonrecurringButton.Name = "nonrecurringButton";
            this.nonrecurringButton.Size = new System.Drawing.Size(199, 33);
            this.nonrecurringButton.TabIndex = 1;
            this.nonrecurringButton.Text = "<< View Your [Title]";
            this.nonrecurringButton.UseVisualStyleBackColor = true;
            this.nonrecurringButton.Click += new System.EventHandler(this.paymentsButton_Click);
            // 
            // recurringGroup
            // 
            this.recurringGroup.Controls.Add(this.addButton);
            this.recurringGroup.Controls.Add(this.deleteButton);
            this.recurringGroup.Controls.Add(this.editButton);
            this.recurringGroup.Controls.Add(this.recurringGrid);
            this.recurringGroup.Location = new System.Drawing.Point(31, 97);
            this.recurringGroup.Name = "recurringGroup";
            this.recurringGroup.Size = new System.Drawing.Size(522, 440);
            this.recurringGroup.TabIndex = 2;
            this.recurringGroup.TabStop = false;
            this.recurringGroup.Text = "Your Recurring [Title]";
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(171, 396);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(161, 30);
            this.addButton.TabIndex = 3;
            this.addButton.Text = "Create [Title]";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.Location = new System.Drawing.Point(265, 360);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(137, 30);
            this.deleteButton.TabIndex = 2;
            this.deleteButton.Text = "Delete Template";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // editButton
            // 
            this.editButton.Location = new System.Drawing.Point(97, 360);
            this.editButton.Name = "editButton";
            this.editButton.Size = new System.Drawing.Size(137, 30);
            this.editButton.TabIndex = 1;
            this.editButton.Text = "Edit Template";
            this.editButton.UseVisualStyleBackColor = true;
            this.editButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // recurringGrid
            // 
            this.recurringGrid.AllowUserToAddRows = false;
            this.recurringGrid.AllowUserToDeleteRows = false;
            this.recurringGrid.AllowUserToResizeColumns = false;
            this.recurringGrid.AllowUserToResizeRows = false;
            this.recurringGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.recurringGrid.BackgroundColor = System.Drawing.SystemColors.Control;
            this.recurringGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.recurringGrid.Dock = System.Windows.Forms.DockStyle.Top;
            this.recurringGrid.Location = new System.Drawing.Point(3, 22);
            this.recurringGrid.MultiSelect = false;
            this.recurringGrid.Name = "recurringGrid";
            this.recurringGrid.ReadOnly = true;
            this.recurringGrid.Size = new System.Drawing.Size(516, 323);
            this.recurringGrid.TabIndex = 0;
            this.recurringGrid.SelectionChanged += new System.EventHandler(this.gridClicked);
            // 
            // RecurringTransactions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.recurringGroup);
            this.Controls.Add(this.nonrecurringButton);
            this.Controls.Add(this.titleLabel);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "RecurringTransactions";
            this.Size = new System.Drawing.Size(584, 661);
            this.recurringGroup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.recurringGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label titleLabel;
		private System.Windows.Forms.Button nonrecurringButton;
		private System.Windows.Forms.GroupBox recurringGroup;
		private System.Windows.Forms.DataGridView recurringGrid;
		private System.Windows.Forms.Button editButton;
		private System.Windows.Forms.Button addButton;
		private System.Windows.Forms.Button deleteButton;
	}
}
