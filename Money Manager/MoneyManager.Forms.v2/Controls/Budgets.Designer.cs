namespace MoneyManager.Forms.v2
{
	partial class Budgets
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
			this.budgetsGroup = new System.Windows.Forms.GroupBox();
			this.budgetGrid = new System.Windows.Forms.DataGridView();
			this.newButton = new System.Windows.Forms.Button();
			this.deleteButton = new System.Windows.Forms.Button();
			this.editButton = new System.Windows.Forms.Button();
			this.periodGroup = new System.Windows.Forms.GroupBox();
			this.futureRadio = new System.Windows.Forms.RadioButton();
			this.currentRadio = new System.Windows.Forms.RadioButton();
			this.previousRadio = new System.Windows.Forms.RadioButton();
			this.recurringButton = new System.Windows.Forms.Button();
			this.budgetsGroup.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.budgetGrid)).BeginInit();
			this.periodGroup.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(207, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(159, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "Budget Management";
			// 
			// budgetsGroup
			// 
			this.budgetsGroup.Controls.Add(this.budgetGrid);
			this.budgetsGroup.Controls.Add(this.newButton);
			this.budgetsGroup.Controls.Add(this.deleteButton);
			this.budgetsGroup.Controls.Add(this.editButton);
			this.budgetsGroup.Location = new System.Drawing.Point(20, 124);
			this.budgetsGroup.Name = "budgetsGroup";
			this.budgetsGroup.Size = new System.Drawing.Size(549, 407);
			this.budgetsGroup.TabIndex = 1;
			this.budgetsGroup.TabStop = false;
			this.budgetsGroup.Text = "Your Budgets";
			// 
			// budgetGrid
			// 
			this.budgetGrid.AllowUserToAddRows = false;
			this.budgetGrid.AllowUserToDeleteRows = false;
			this.budgetGrid.AllowUserToResizeColumns = false;
			this.budgetGrid.AllowUserToResizeRows = false;
			this.budgetGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.budgetGrid.BackgroundColor = System.Drawing.SystemColors.Control;
			this.budgetGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.budgetGrid.Dock = System.Windows.Forms.DockStyle.Top;
			this.budgetGrid.Location = new System.Drawing.Point(3, 22);
			this.budgetGrid.MultiSelect = false;
			this.budgetGrid.Name = "budgetGrid";
			this.budgetGrid.ReadOnly = true;
			this.budgetGrid.Size = new System.Drawing.Size(543, 298);
			this.budgetGrid.TabIndex = 3;
			this.budgetGrid.SelectionChanged += new System.EventHandler(this.gridClicked);
			// 
			// newButton
			// 
			this.newButton.Location = new System.Drawing.Point(191, 361);
			this.newButton.Name = "newButton";
			this.newButton.Size = new System.Drawing.Size(155, 30);
			this.newButton.TabIndex = 2;
			this.newButton.Text = "Add New Budget";
			this.newButton.UseVisualStyleBackColor = true;
			this.newButton.Click += new System.EventHandler(this.newButton_Click);
			// 
			// deleteButton
			// 
			this.deleteButton.Location = new System.Drawing.Point(293, 326);
			this.deleteButton.Name = "deleteButton";
			this.deleteButton.Size = new System.Drawing.Size(134, 29);
			this.deleteButton.TabIndex = 1;
			this.deleteButton.Text = "Delete Budget";
			this.deleteButton.UseVisualStyleBackColor = true;
			this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
			// 
			// editButton
			// 
			this.editButton.Location = new System.Drawing.Point(128, 326);
			this.editButton.Name = "editButton";
			this.editButton.Size = new System.Drawing.Size(134, 29);
			this.editButton.TabIndex = 0;
			this.editButton.Text = "Edit Budget";
			this.editButton.UseVisualStyleBackColor = true;
			this.editButton.Click += new System.EventHandler(this.editButton_Click);
			// 
			// periodGroup
			// 
			this.periodGroup.Controls.Add(this.futureRadio);
			this.periodGroup.Controls.Add(this.currentRadio);
			this.periodGroup.Controls.Add(this.previousRadio);
			this.periodGroup.Location = new System.Drawing.Point(26, 54);
			this.periodGroup.Name = "periodGroup";
			this.periodGroup.Size = new System.Drawing.Size(303, 64);
			this.periodGroup.TabIndex = 2;
			this.periodGroup.TabStop = false;
			this.periodGroup.Text = "Viewing Period";
			// 
			// futureRadio
			// 
			this.futureRadio.AutoSize = true;
			this.futureRadio.Location = new System.Drawing.Point(214, 25);
			this.futureRadio.Name = "futureRadio";
			this.futureRadio.Size = new System.Drawing.Size(74, 24);
			this.futureRadio.TabIndex = 2;
			this.futureRadio.Text = "Future";
			this.futureRadio.UseVisualStyleBackColor = true;
			this.futureRadio.CheckedChanged += new System.EventHandler(this.radioChange);
			// 
			// currentRadio
			// 
			this.currentRadio.AutoSize = true;
			this.currentRadio.Checked = true;
			this.currentRadio.Location = new System.Drawing.Point(117, 25);
			this.currentRadio.Name = "currentRadio";
			this.currentRadio.Size = new System.Drawing.Size(80, 24);
			this.currentRadio.TabIndex = 1;
			this.currentRadio.TabStop = true;
			this.currentRadio.Text = "Current";
			this.currentRadio.UseVisualStyleBackColor = true;
			this.currentRadio.CheckedChanged += new System.EventHandler(this.radioChange);
			// 
			// previousRadio
			// 
			this.previousRadio.AutoSize = true;
			this.previousRadio.Location = new System.Drawing.Point(17, 25);
			this.previousRadio.Name = "previousRadio";
			this.previousRadio.Size = new System.Drawing.Size(87, 24);
			this.previousRadio.TabIndex = 0;
			this.previousRadio.Text = "Previous";
			this.previousRadio.UseVisualStyleBackColor = true;
			this.previousRadio.CheckedChanged += new System.EventHandler(this.radioChange);
			// 
			// recurringButton
			// 
			this.recurringButton.Location = new System.Drawing.Point(389, 67);
			this.recurringButton.Name = "recurringButton";
			this.recurringButton.Size = new System.Drawing.Size(177, 49);
			this.recurringButton.TabIndex = 3;
			this.recurringButton.Text = "Your Recurring Budget Templates >>";
			this.recurringButton.UseVisualStyleBackColor = true;
			this.recurringButton.Click += new System.EventHandler(this.recurringButton_Click);
			// 
			// Budgets
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.recurringButton);
			this.Controls.Add(this.periodGroup);
			this.Controls.Add(this.budgetsGroup);
			this.Controls.Add(this.label1);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.Name = "Budgets";
			this.Size = new System.Drawing.Size(584, 661);
			this.budgetsGroup.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.budgetGrid)).EndInit();
			this.periodGroup.ResumeLayout(false);
			this.periodGroup.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox budgetsGroup;
		private System.Windows.Forms.Button newButton;
		private System.Windows.Forms.Button deleteButton;
		private System.Windows.Forms.Button editButton;
		private System.Windows.Forms.DataGridView budgetGrid;
		private System.Windows.Forms.GroupBox periodGroup;
		private System.Windows.Forms.RadioButton futureRadio;
		private System.Windows.Forms.RadioButton currentRadio;
		private System.Windows.Forms.RadioButton previousRadio;
		private System.Windows.Forms.Button recurringButton;
	}
}
