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
	[Activity(Label = "RecurringTransactionsActivity")]
	public class RecurringTransactionsActivity : Activity
	{
		bool filterWallets, filterStores, filterAmounts;
		List<Wallet> filteredWallets;
		List<Store> filteredStores;
		float minAmount, maxAmount;

		Toolbar toolbar;
		Toolbar bottomToolbar;
		ListView listView;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			ResetFilters();

			#region View/Layout Creation
			SetContentView(Resource.Layout.DataDisplay);

			// Top Toolbar Creation
			toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
			SetActionBar(toolbar);
			ActionBar.Title = "Recurring Transactions";

			// Create + Populate the ListView
			listView = FindViewById<ListView>(Resource.Id.list_view);
			listView.ChoiceMode = ChoiceMode.Single;
			ApplyFilters();

			// Bottom Toolbar Creation
			bottomToolbar = FindViewById<Toolbar>(Resource.Id.bottom_toolbar);
			bottomToolbar.Title = "Options";
			bottomToolbar.InflateMenu(Resource.Menu.data_bottom_menus);
			#endregion

			#region Editing Methods
			// Bottom Toolbar Methods
			FindViewById(Resource.Id.menu_add).Click += delegate {
				EditTransactionFragment frag = EditTransactionFragment.NewInstance(-1, true, false, delegate { ApplyFilters(); });
				frag.Show(FragmentManager, "New_Rec_Transaction");
			};
			FindViewById(Resource.Id.menu_edit).Click += delegate {
				if (listView.CheckedItemCount > 0)
				{
					EditTransactionFragment frag = EditTransactionFragment.NewInstance(Global.gRecurTransactions[listView.CheckedItemPosition].Id, true, false, delegate { ApplyFilters(); });
					frag.Show(FragmentManager, "Edit_Rec_Transaction");
				}
				else
					Toast.MakeText(this, "No transaction selected", ToastLength.Short).Show();
			};
			FindViewById(Resource.Id.menu_delete).Click += delegate {
				if (listView.CheckedItemCount > 0)
				{
					RecurringTransaction rt = Global.gRecurTransactions[listView.CheckedItemPosition];
					string rtDisplay = "";
					switch (rt.ProcessPeriod)
					{
						case 0:
							rtDisplay += "Daily, ";
							break;
						case 1:
							rtDisplay += "Weekly, ";
							break;
						case 2:
							rtDisplay += "Monthly, ";
							break;
						case 3:
							rtDisplay += "Quarterly, ";
							break;
						case 4:
							rtDisplay += "Yearly, ";
							break;
					}
					foreach (Store s in Global.gStores)
						if (s.Id == rt.StoreId)
						{
							rtDisplay += ", " + s.Name;
							break;
						}
					rtDisplay += ": " + rt.Amount.ToString("c2");

					AlertDialog.Builder alert = new AlertDialog.Builder(this);
					alert.SetTitle("Confirm Delete");
					alert.SetMessage("Delete recurring transaction: " + rtDisplay + "?");
					alert.SetPositiveButton("Delete", (senderAlert, args) => {
						Global.gRecurTransactions.RemoveAt(listView.CheckedItemPosition);
						Toast.MakeText(this, "Deleted", ToastLength.Short).Show();
						ApplyFilters();
					});
					alert.SetNegativeButton("Cancel", (senderAlert, args) => {
						Toast.MakeText(this, "Cancelled", ToastLength.Short).Show();
					});

					Dialog dlg = alert.Create();
					dlg.Show();
				}
				else
					Toast.MakeText(this, "No transaction selected", ToastLength.Short).Show();
			};
			#endregion
		}

		#region Top Toolbar Methods
		// Top Toolbar Methods
		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.top_menus, menu);
			return base.OnCreateOptionsMenu(menu);
		}
		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			if (item.ItemId == Resource.Id.menu_filters)
			{
				PopupMenu filtersPopup = new PopupMenu(this, FindViewById(Resource.Id.menu_filters));
				filtersPopup.Inflate(Resource.Menu.filter_transactions_popup);

				filtersPopup.MenuItemClick += (s, arg) =>
				{
					if (arg.Item.ItemId == Resource.Id.filter_trans_reset)
					{
						AlertDialog.Builder alert = new AlertDialog.Builder(this);
						alert.SetTitle("Confirm Reset");
						alert.SetMessage("Reset All Filters: Are you sure?");
						alert.SetPositiveButton("Reset", (senderAlert, args) => {
							ResetFilters();
							Toast.MakeText(this, "Filters Reset", ToastLength.Short).Show();
							ApplyFilters();
						});
						alert.SetNegativeButton("Cancel", (senderAlert, args) => {
							Toast.MakeText(this, "Cancelled", ToastLength.Short).Show();
						});

						Dialog dlg = alert.Create();
						dlg.Show();
					}
					else if (arg.Item.ItemId == Resource.Id.filter_trans_wallets)
					{
						FiltersWalletsFragment fwf = FiltersWalletsFragment.NewInstance(filterWallets, filteredWallets, (fOn, wList) => {
							filterWallets = fOn; filteredWallets = wList; ApplyFilters();
						});
						fwf.Show(FragmentManager, "filter_wallets");
					}
					else if (arg.Item.ItemId == Resource.Id.filter_trans_stores)
					{
						FiltersStoresFragment fsf = FiltersStoresFragment.NewInstance(filterStores, filteredStores, (fOn, sList) => {
							filterStores = fOn; filteredStores = sList; ApplyFilters();
						});
						fsf.Show(FragmentManager, "filter_stores");
					}
					else if (arg.Item.ItemId == Resource.Id.filter_trans_dates)
					{
						Toast.MakeText(this, "Not yet available...", ToastLength.Short).Show();
					}
					else if (arg.Item.ItemId == Resource.Id.filter_trans_amounts)
					{
						FiltersAmountsFragment faf = FiltersAmountsFragment.NewInstance(filterAmounts, minAmount, maxAmount, (fOn, min, max) => {
							filterAmounts = fOn; minAmount = min; maxAmount = max; ApplyFilters();
						});
						faf.Show(FragmentManager, "filter_amounts");
					}
				};
				filtersPopup.Show();
			}
			else
				Toast.MakeText(this, "Action selected: " + item.TitleFormatted, ToastLength.Short).Show();

			return base.OnOptionsItemSelected(item);
		}
		#endregion

		#region Private Helper Functions
		private void ResetFilters()
		{
			filterWallets = filterStores = filterAmounts = false;

			filteredWallets = new List<Wallet>();
			filteredStores = new List<Store>();
			minAmount = 0.0f;
			maxAmount = float.MaxValue;
		}

		private void ApplyFilters()
		{
			List<RecurringTransaction> filtered = Global.gRecurTransactions.Select(t => t).ToList();

			if (filterWallets)
			{
				List<RecurringTransaction> tmp = new List<RecurringTransaction>();
				foreach (Wallet w in filteredWallets)
				{
					int i = 0;
					while (i < filtered.Count)
					{
						if (filtered[i].WalletId == w.Id)
						{
							tmp.Add(filtered[i]);
							filtered.RemoveAt(i);
						}
						else
							++i;
					}
				}
				filtered = tmp;
			}

			if (filterStores)
			{
				List<RecurringTransaction> tmp = new List<RecurringTransaction>();
				foreach (Store s in filteredStores)
				{
					int i = 0;
					while (i < filtered.Count)
					{
						if (filtered[i].StoreId == s.Id)
						{
							tmp.Add(filtered[i]);
							filtered.RemoveAt(i);
						}
						else
							++i;
					}
				}
				filtered = tmp;
			}

			if (filterAmounts)
			{
				filtered = filtered
					.Where(t => t.Amount > minAmount && t.Amount < maxAmount)
					.Select(t => t)
					.ToList();
			}

			listView.Adapter = new RecurringTransactionAdapter(this, filtered);
		}
		#endregion

	}
}