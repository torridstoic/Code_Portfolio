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
	[Activity(Label = "WalletsActivity")]
	public class WalletsActivity : Activity
	{
		Toolbar toolbar;
		Toolbar bottomToolbar;
		ListView listView;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			#region View/Layout Creation
			SetContentView(Resource.Layout.DataDisplay);

			// Top Toolbar Creation
			toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
			SetActionBar(toolbar);
			ActionBar.Title = "Wallets";

			// Create + Populate the ListView
			listView = FindViewById<ListView>(Resource.Id.list_view);
			listView.ChoiceMode = ChoiceMode.Single;
			listView.Adapter = new WalletAdapter(this);

			// Bottom Toolbar Creation
			bottomToolbar = FindViewById<Toolbar>(Resource.Id.bottom_toolbar);
			bottomToolbar.Title = "Options";
			bottomToolbar.InflateMenu(Resource.Menu.data_bottom_menus);
			#endregion

			#region Editing Methods
			// Bottom Toolbar Methods
			FindViewById(Resource.Id.menu_add).Click += delegate {
				EditWalletFragment frag = EditWalletFragment.NewInstance(-1, delegate { RefreshDisplay(); });
				frag.Show(FragmentManager, "New_Wallet");
				RefreshDisplay();
			};
			FindViewById(Resource.Id.menu_edit).Click += delegate {
				if (listView.CheckedItemCount > 0)
				{
					EditWalletFragment frag = EditWalletFragment.NewInstance(Global.gWallets[listView.CheckedItemPosition].Id, delegate { RefreshDisplay(); });
					frag.Show(FragmentManager, "Edit_Wallet");
					RefreshDisplay();
				}
				else
					Toast.MakeText(this, "No wallet selected", ToastLength.Short).Show();
			};
			FindViewById(Resource.Id.menu_delete).Click += delegate {
				if (listView.CheckedItemCount > 0)
				{
					AlertDialog.Builder alert = new AlertDialog.Builder(this);
					alert.SetTitle("Confirm Delete");
					alert.SetMessage("Delete wallet: " + Global.gWallets[listView.CheckedItemPosition].Name + "?");
					alert.SetPositiveButton("Delete", (senderAlert, args) => {
						// Delete the Wallet (and all matching Transactions)
						int i = 0;
						while (i < Global.gTransactions.Count)
						{
							if (Global.gWallets[listView.CheckedItemPosition].Id == Global.gTransactions[i].WalletId)
								Global.gTransactions.RemoveAt(i);
							else
								++i;
						}
						//foreach (Transaction t in Global.gTransactions)
						//	if (t.WalletId == Global.gWallets[listView.CheckedItemPosition].Id)
						//		Global.gTransactions.Remove(t);
						Global.gWallets.RemoveAt(listView.CheckedItemPosition);
						Toast.MakeText(this, "Deleted", ToastLength.Short).Show();
						RefreshDisplay();
					});
					alert.SetNegativeButton("Cancel", (senderAlert, args) => {
						Toast.MakeText(this, "Cancelled", ToastLength.Short).Show();
					});

					Dialog dlg = alert.Create();
					dlg.Show();
				}
				else
					Toast.MakeText(this, "No wallet selected", ToastLength.Short).Show();
			};
			#endregion
		}

		#region Top Toolbar Methods
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
			Toast.MakeText(this, "Action selected: " + item.TitleFormatted, ToastLength.Short).Show();
			return base.OnOptionsItemSelected(item);
		}
		#endregion

		private void RefreshDisplay()
		{
			// TODO : is there a better way to refresh the listView?
			listView.Adapter = new WalletAdapter(this);
		}
	}
}