using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using MoneyManager.Data;

namespace MoneyManager.Android
{
    [Activity(Label = "MoneyManager.Android", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

			// DEMO DATA INITIALIZATION
			Global.user = new User("username", "password");

			// Top Toolbar Creation
			var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
			SetActionBar(toolbar);
			ActionBar.Title = "Main Menu";

			// Buttons Creation
			FindViewById<Button>(Resource.Id.main_wallets_button).Click += (sender, e) => {
				Intent walletsIntent = new Intent(this, typeof(WalletsActivity));
				StartActivity(walletsIntent);
			};
			FindViewById<Button>(Resource.Id.main_transactions_button).Click += (sender, e) => {
				Intent transactionsIntent = new Intent(this, typeof(TransactionsActivity));
				StartActivity(transactionsIntent);
			};
			FindViewById<Button>(Resource.Id.main_rectransactions_button).Click += (sender, e) => {
				Intent recTransactionsIntent = new Intent(this, typeof(RecurringTransactionsActivity));
				StartActivity(recTransactionsIntent);
			};
			// MORE BUTTONS WILL GO HERE
			FindViewById<Button>(Resource.Id.main_manage_acct_button).Click += (sender, e) => Toast.MakeText(this, "Manage Account", ToastLength.Short).Show();
			FindViewById<Button>(Resource.Id.main_logout_button).Click += (sender, e) => {
				Toast.MakeText(this, "Logout", ToastLength.Short).Show();
				//Global.user = null;
				//Intent logoutIntent = new Intent(this, typeof(LoginActivity));
				//StartActivity(logoutIntent);
			};
        }

		// Top Toolbar Methods
		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.top_menus, menu);
			menu.FindItem(Resource.Id.menu_filters).SetVisible(false);
			menu.FindItem(Resource.Id.menu_filters).SetEnabled(false);

			return base.OnCreateOptionsMenu(menu);
		}
		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			Toast.MakeText(this, "Action selected: " + item.TitleFormatted,
				ToastLength.Short).Show();
			return base.OnOptionsItemSelected(item);
		}
	}
}

