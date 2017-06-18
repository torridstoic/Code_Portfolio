namespace MoneyManager.Forms.v2
{
	partial class ManageAccount
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
			this.userGroup = new System.Windows.Forms.GroupBox();
			this.usernameText = new System.Windows.Forms.TextBox();
			this.nameGroup = new System.Windows.Forms.GroupBox();
			this.lastNameText = new System.Windows.Forms.TextBox();
			this.firstNameText = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.saveButton = new System.Windows.Forms.Button();
			this.emailGroup = new System.Windows.Forms.GroupBox();
			this.emailText = new System.Windows.Forms.TextBox();
			this.passwordGroup = new System.Windows.Forms.GroupBox();
			this.repeatPassText = new System.Windows.Forms.TextBox();
			this.newPassText = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.passText = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.userGroup.SuspendLayout();
			this.nameGroup.SuspendLayout();
			this.emailGroup.SuspendLayout();
			this.passwordGroup.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(219, 17);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(166, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "Account Management";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// userGroup
			// 
			this.userGroup.Controls.Add(this.usernameText);
			this.userGroup.Location = new System.Drawing.Point(86, 53);
			this.userGroup.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.userGroup.Name = "userGroup";
			this.userGroup.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.userGroup.Size = new System.Drawing.Size(413, 72);
			this.userGroup.TabIndex = 0;
			this.userGroup.TabStop = false;
			this.userGroup.Text = "Username";
			// 
			// usernameText
			// 
			this.usernameText.Location = new System.Drawing.Point(26, 27);
			this.usernameText.Name = "usernameText";
			this.usernameText.Size = new System.Drawing.Size(354, 26);
			this.usernameText.TabIndex = 0;
			// 
			// nameGroup
			// 
			this.nameGroup.Controls.Add(this.lastNameText);
			this.nameGroup.Controls.Add(this.firstNameText);
			this.nameGroup.Controls.Add(this.label3);
			this.nameGroup.Controls.Add(this.label2);
			this.nameGroup.Location = new System.Drawing.Point(86, 262);
			this.nameGroup.Name = "nameGroup";
			this.nameGroup.Size = new System.Drawing.Size(413, 101);
			this.nameGroup.TabIndex = 2;
			this.nameGroup.TabStop = false;
			this.nameGroup.Text = "Name";
			// 
			// lastNameText
			// 
			this.lastNameText.Location = new System.Drawing.Point(72, 59);
			this.lastNameText.Name = "lastNameText";
			this.lastNameText.Size = new System.Drawing.Size(308, 26);
			this.lastNameText.TabIndex = 4;
			// 
			// firstNameText
			// 
			this.firstNameText.Location = new System.Drawing.Point(72, 26);
			this.firstNameText.Name = "firstNameText";
			this.firstNameText.Size = new System.Drawing.Size(308, 26);
			this.firstNameText.TabIndex = 3;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(22, 62);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(44, 20);
			this.label3.TabIndex = 1;
			this.label3.Text = "Last:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(22, 29);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(44, 20);
			this.label2.TabIndex = 0;
			this.label2.Text = "First:";
			// 
			// saveButton
			// 
			this.saveButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.saveButton.Location = new System.Drawing.Point(400, 25);
			this.saveButton.Name = "saveButton";
			this.saveButton.Size = new System.Drawing.Size(75, 36);
			this.saveButton.TabIndex = 7;
			this.saveButton.Text = "Save";
			this.saveButton.UseVisualStyleBackColor = true;
			this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
			// 
			// emailGroup
			// 
			this.emailGroup.Controls.Add(this.emailText);
			this.emailGroup.Location = new System.Drawing.Point(86, 369);
			this.emailGroup.Name = "emailGroup";
			this.emailGroup.Size = new System.Drawing.Size(413, 67);
			this.emailGroup.TabIndex = 3;
			this.emailGroup.TabStop = false;
			this.emailGroup.Text = "Email";
			// 
			// emailText
			// 
			this.emailText.Location = new System.Drawing.Point(26, 25);
			this.emailText.Name = "emailText";
			this.emailText.Size = new System.Drawing.Size(354, 26);
			this.emailText.TabIndex = 5;
			// 
			// passwordGroup
			// 
			this.passwordGroup.Controls.Add(this.repeatPassText);
			this.passwordGroup.Controls.Add(this.newPassText);
			this.passwordGroup.Controls.Add(this.label6);
			this.passwordGroup.Controls.Add(this.label5);
			this.passwordGroup.Location = new System.Drawing.Point(86, 134);
			this.passwordGroup.Name = "passwordGroup";
			this.passwordGroup.Size = new System.Drawing.Size(413, 105);
			this.passwordGroup.TabIndex = 1;
			this.passwordGroup.TabStop = false;
			this.passwordGroup.Text = "Change Password";
			// 
			// repeatPassText
			// 
			this.repeatPassText.Location = new System.Drawing.Point(94, 61);
			this.repeatPassText.Name = "repeatPassText";
			this.repeatPassText.Size = new System.Drawing.Size(286, 26);
			this.repeatPassText.TabIndex = 2;
			// 
			// newPassText
			// 
			this.newPassText.Location = new System.Drawing.Point(94, 25);
			this.newPassText.Name = "newPassText";
			this.newPassText.Size = new System.Drawing.Size(286, 26);
			this.newPassText.TabIndex = 1;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(22, 64);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(66, 20);
			this.label6.TabIndex = 2;
			this.label6.Text = "Repeat:";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(44, 28);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(44, 20);
			this.label5.TabIndex = 1;
			this.label5.Text = "New:";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.passText);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.saveButton);
			this.groupBox1.Location = new System.Drawing.Point(39, 458);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(505, 81);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			// 
			// passText
			// 
			this.passText.Location = new System.Drawing.Point(119, 30);
			this.passText.Name = "passText";
			this.passText.Size = new System.Drawing.Size(263, 26);
			this.passText.TabIndex = 6;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(31, 33);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(82, 20);
			this.label4.TabIndex = 5;
			this.label4.Text = "Password:";
			// 
			// ManageAccount
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.passwordGroup);
			this.Controls.Add(this.emailGroup);
			this.Controls.Add(this.nameGroup);
			this.Controls.Add(this.userGroup);
			this.Controls.Add(this.label1);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.Name = "ManageAccount";
			this.Size = new System.Drawing.Size(584, 561);
			this.userGroup.ResumeLayout(false);
			this.userGroup.PerformLayout();
			this.nameGroup.ResumeLayout(false);
			this.nameGroup.PerformLayout();
			this.emailGroup.ResumeLayout(false);
			this.emailGroup.PerformLayout();
			this.passwordGroup.ResumeLayout(false);
			this.passwordGroup.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox userGroup;
		private System.Windows.Forms.TextBox usernameText;
		private System.Windows.Forms.GroupBox nameGroup;
		private System.Windows.Forms.TextBox lastNameText;
		private System.Windows.Forms.TextBox firstNameText;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button saveButton;
		private System.Windows.Forms.GroupBox emailGroup;
		private System.Windows.Forms.TextBox emailText;
		private System.Windows.Forms.GroupBox passwordGroup;
		private System.Windows.Forms.TextBox repeatPassText;
		private System.Windows.Forms.TextBox newPassText;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox passText;
		private System.Windows.Forms.Label label4;
	}
}
