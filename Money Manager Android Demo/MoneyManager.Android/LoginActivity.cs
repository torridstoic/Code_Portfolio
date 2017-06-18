using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using MoneyManager.Data;

namespace MoneyManager.Android
{
	[Activity(Label = "LoginActivity"/*, MainLauncher = true, Icon = "@drawable/icon")*/)]
	public class LoginActivity : Activity
	{
		Toolbar toolbar;
		Button loginButton;
		EditText usernameText;
		EditText passwordText;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Login);
            Global.db = new SQLiteDatabase();

			// Top Toolbar Creation
			toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
			SetActionBar(toolbar);
			ActionBar.Title = "Login";
			// Login Button Creation
			loginButton = FindViewById<Button>(Resource.Id.login_button);
			loginButton.Click += (object sender, EventArgs e) =>
			{
				Console.WriteLine("This worked");
				UserLogin(sender, e);
			};
			// Fetch EditText Boxes
			usernameText = FindViewById<EditText>(Resource.Id.login_user_text);
			passwordText = FindViewById<EditText>(Resource.Id.login_pass_text);
		}

		// Top Toolbar Methods
		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.login_menus, menu);
			return base.OnCreateOptionsMenu(menu);
		}
		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			// This SHOULD reset the DB
			//if (item.TitleFormatted.ToString().Equals("Reset DB"))
			if (item.ItemId.Equals(Resource.Id.reset_db))
			{
				Global.db.ResetDatabase();
				Toast.MakeText(this, "Database Reset!", ToastLength.Short).Show();
			}
			return base.OnOptionsItemSelected(item);
		}

		// Login Method
		private void UserLogin(object sender, EventArgs e)
		{
			// Validation
			if (usernameText.Text == String.Empty || passwordText.Text == String.Empty)
			{
				Toast.MakeText(this, "Enter a username and password!", ToastLength.Short).Show();
				return;
			}

			// Create a User object and query the DB for a match
			User currUser = new User(usernameText.Text, passwordText.Text);
			String userQuery = "SELECT * FROM Users WHERE Username = '" + currUser.Username + "'";
			ResultData userQueryData = Global.db.Query(userQuery);

			// If Username exists:
			if (userQueryData.HasRows())
			{
				userQueryData.Read();
				if (userQueryData.Value("Password").ToString() == currUser.Password)
				{
					// Fetch full User data and update LastLogin
					currUser = new User(userQueryData);
					Global.db.Query("UPDATE Users SET LastLogin = '" + Global.SecondsFromUTC() + "' WHERE Id = '" + currUser.Id + "'");
					currUser.LastLogin = Global.SecondsFromUTC();
					Global.user = currUser;

					// Intent: Main Menu Activity
					Intent menuIntent = new Intent(this, typeof(MainActivity));
					StartActivity(menuIntent);
				}
				else
					Toast.MakeText(this, "Incorrect password!", ToastLength.Short).Show();
			}
			else // User does not exist:
			{
				// Ask the User to register
				AlertDialog.Builder alert = new AlertDialog.Builder(this);
				alert.SetMessage("This user does not exist. Register?");
				alert.SetPositiveButton("Yes", (senderAlert, args) =>
				{
					// Create + Upload a new User account
					currUser.FirstName = String.Empty;
					currUser.LastName = String.Empty;
					currUser.LastLogin = Global.SecondsFromUTC();
					if (Global.db.CreateRecord(currUser))
					{
						// Re-fetch accurate User data
						userQueryData = Global.db.Query(userQuery);
						userQueryData.Read();
						currUser = new User(userQueryData);
						Global.user = currUser;

						// Intent: Main Menu Activity
						Intent menuIntent = new Intent(this, typeof(MainActivity));
						StartActivity(menuIntent);
					}
					else
						Toast.MakeText(this, "Error creating account.", ToastLength.Short).Show();
				});
				alert.SetNegativeButton("No", (senderAlert, args) =>
				{
					Toast.MakeText(this, "Please Login.", ToastLength.Short).Show();
				});
				alert.Show();
			}
		}
	}
}