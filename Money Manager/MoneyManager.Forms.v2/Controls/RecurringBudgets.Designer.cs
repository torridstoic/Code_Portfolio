namespace MoneyManager.Forms.v2
{
	partial class RecurringBudgets
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
			this.recurringGroup = new System.Windows.Forms.GroupBox();
			this.newButton = new System.Windows.Forms.Button();
			this.deleteButton = new System.Windows.Forms.Button();
			this.editButton = new System.Windows.Forms.Button();
			this.recurringGrid = new System.Windows.Forms.DataGridView();
			this.budgetsButton = new System.Windows.Forms.Button();
			this.recurringGroup.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.recurringGrid)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(187, 17);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(212, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "Recurring Budget Templates";
			// 
			// recurringGroup
			// 
			this.recurringGroup.Controls.Add(this.newButton);
			this.recurringGroup.Controls.Add(this.deleteButton);
			this.recurringGroup.Controls.Add(this.editButton);
			this.recurringGroup.Controls.Add(this.recurringGrid);
			this.recurringGroup.Location = new System.Drawing.Point(28, 106);
			this.recurringGroup.Name = "recurringGroup";
			this.recurringGroup.Size = new System.Drawing.Size(527, 437);
			this.recurringGroup.TabIndex = 1;
			this.recurringGroup.TabStop = false;
			this.recurringGroup.Text = "Your Recurring Budgets";
			// 
			// newButton
			// 
			this.newButton.Location = new System.Drawing.Point(182, 394);
			this.newButton.Name = "newButton";
			this.newButton.Size = new System.Drawing.Size(166, 29);
			this.newButton.TabIndex = 3;
			this.newButton.Text = "Add New Budget";
			this.newButton.UseVisualStyleBackColor = true;
			this.newButton.Click += new System.EventHandler(this.newButton_Click);
			// 
			// deleteButton
			// 
			this.deleteButton.Location = new System.Drawing.Point(275, 359);
			this.deleteButton.Name = "deleteButton";
			this.deleteButton.Size = new System.Drawing.Size(144, 29);
			this.deleteButton.TabIndex = 2;
			this.deleteButton.Text = "Delete Template";
			this.deleteButton.UseVisualStyleBackColor = true;
			this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
			// 
			// editButton
			// 
			this.editButton.Location = new System.Drawing.Point(101, 359);
			this.editButton.Name = "editButton";
			this.editButton.Size = new System.Drawing.Size(144, 29);
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
			this.recurringGrid.Size = new System.Drawing.Size(521, 317);
			this.recurringGrid.TabIndex = 0;
			this.recurringGrid.SelectionChanged += new System.EventHandler(this.gridClicked);
			// 
			// budgetsButton
			// 
			this.budgetsButton.Location = new System.Drawing.Point(16, 59);
			this.budgetsButton.Name = "budgetsButton";
			this.budgetsButton.Size = new System.Drawing.Size(196, 30);
			this.budgetsButton.TabIndex = 2;
			this.budgetsButton.Text = "<< View Your Budgets";
			this.budgetsButton.UseVisualStyleBackColor = true;
			this.budgetsButton.Click += new System.EventHandler(this.budgetsButton_Click);
			// 
			// RecurringBudgets
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.budgetsButton);
			this.Controls.Add(this.recurringGroup);
			this.Controls.Add(this.label1);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.Name = "RecurringBudgets";
			this.Size = new System.Drawing.Size(584, 661);
			this.recurringGroup.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.recurringGrid)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox recurringGroup;
		private System.Windows.Forms.DataGridView recurringGrid;
		private System.Windows.Forms.Button newButton;
		private System.Windows.Forms.Button deleteButton;
		private System.Windows.Forms.Button editButton;
		private System.Windows.Forms.Button budgetsButton;
	}
}
