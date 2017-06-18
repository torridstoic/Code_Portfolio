using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MoneyManager.Forms.v2
{
	public partial class ManageAccount : UserControl
	{
		///////////////////
		// Form Init
		public ManageAccount()
		{
			InitializeComponent();

			usernameText.Text = Global.user.Username;
			firstNameText.Text = Global.user.FirstName;
			lastNameText.Text = Global.user.LastName;
			emailText.Text = Global.user.Email;
		}

		///////////////////
		// Save Button
		private void saveButton_Click(object sender, EventArgs e)
		{
			// Validation Check
			if (passText.Text == String.Empty || !Global.user.PasswordValidate(passText.Text))
			{
				MessageBox.Show("Incorrect password!", "Warning", MessageBoxButtons.OK);
				return;
			}
			if (usernameText.Text == String.Empty)
			{
				MessageBox.Show("Please enter a username.", "Warning", MessageBoxButtons.OK);
				return;
			}
			if(!newPassText.Text.Equals(repeatPassText.Text))
			{
				MessageBox.Show("New password fields must match!", "Warning", MessageBoxButtons.OK);
				return;
			}
			if (firstNameText.Text == String.Empty || lastNameText.Text == String.Empty)
			{
				MessageBox.Show("Please enter your name.", "Warning", MessageBoxButtons.OK);
				return;
			}
			if (emailText.Text == String.Empty)
			{
				MessageBox.Show("Please enter your email.", "Warning", MessageBoxButtons.OK);
				return;
			}

			// Update User's info and upload to DB
			Global.user.Username = usernameText.Text;
			if (newPassText.Text != String.Empty)
				Global.user.Password = newPassText.Text;
			Global.user.FirstName = firstNameText.Text;
			Global.user.LastName = lastNameText.Text;
			Global.user.Email = emailText.Text;

			if (Global.db.UpdateRecord(Global.user))
            {
                MessageBox.Show("Information was updated.", "Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("There was an error with updating your information.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
		}
	}
}
