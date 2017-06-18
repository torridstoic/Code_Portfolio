using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MoneyManager.Data;

namespace MoneyManager.Forms.v2
{
    public partial class Login : UserControl
    {
        public event EventHandler<MenuEvents> UserLogin;

        public Login()
        {
            InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                okButton.PerformClick();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        public void SetFocus()
        {
            usernameText.Focus();
        }

        ///////////////////
        // Reset Database MenuButton
        private void resetDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Global.db.ResetDatabase();
        }

        ///////////////////
        // Login Button
        private void LoginUser(object sender, EventArgs e)
        {
            // Validation Check
            if (usernameText.Text == String.Empty)
            {
                MessageBox.Show("You must enter a username.", "Warning", MessageBoxButtons.OK);
                return;
            }
            if (passwordText.Text == String.Empty)
            {
                MessageBox.Show("You must enter a password.", "Warning", MessageBoxButtons.OK);
                return;
            }

            User user = new User(usernameText.Text, passwordText.Text);

            String userQuery = "SELECT * FROM Users WHERE Username = '" + user.Username + "'";
            ResultData userQueryData = Global.db.Query(userQuery);

            if (userQueryData.HasRows())
            {
                userQueryData.Read();
                if (userQueryData.Value("Password").ToString() == user.Password)
                {
                    user = new User(userQueryData);

                    //We need to update the LastLogged in date
                    Global.db.Query("UPDATE Users SET LastLogin = '" + Global.SecondsFromUTC() + "' WHERE Id = '" + user.Id + "'");
                    user.LastLogin = Global.SecondsFromUTC();

                    Global.user = user;
                    UserLogin(this, new MenuEvents(EventType.Login));
                }
                else
                {
                    MessageBox.Show("Username or password is incorect.", "Error", MessageBoxButtons.OK);
                }
            }
            else // This determines that the user / email doesn't exist
            {
                if (DialogResult.Yes == MessageBox.Show("This user does not exist. Would you like to register this username and password?", "Register", MessageBoxButtons.YesNo))
                {
                    user.FirstName = "New";
                    user.LastName = "User";
                    user.LastLogin = Global.SecondsFromUTC();

                    if (Global.db.CreateRecord(user))
                    {
                        //Requery the database to grab all data correctly.
                        userQueryData = Global.db.Query(userQuery);
                        userQueryData.Read();

                        user = new User(userQueryData);

                        Global.user = user;
                        UserLogin(this, new MenuEvents(EventType.NewUser));
                    }
                    else
                    {
                        MessageBox.Show("There was an error in creation of your account.", "Error", MessageBoxButtons.OK);
                    }
                }
            }
        }

        private void ExitApplication(object sender, EventArgs e)
        {
            UserLogin(this, new MenuEvents(EventType.Exit));
        }
    }
}
