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
	[Activity(Label = "TransactionsActivity")]
	public class TransactionsActivity : Activity
	{
		bool filterWallets, filterStores, filterDates, filterAmounts;
		List<Wallet> filteredWallets;
		List<Store> filteredStores;
		DateTime startDate, endDate;
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
			ActionBar.Title = "Transactions";

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
				EditTransactionFragment frag = EditTransactionFragment.NewInstance(-1, false, false, delegate { ApplyFilters(); });
				frag.Show(FragmentManager, "New_Transaction");
				//ApplyFilters();
			};
			FindViewById(Resource.Id.menu_edit).Click += delegate {
				if (listView.CheckedItemCount > 0)
				{
					EditTransactionFragment frag = EditTransactionFragment.NewInstance(Global.gTransactions[listView.CheckedItemPosition].Id, false, false, delegate { ApplyFilters(); });
					frag.Show(FragmentManager, "Edit_Transaction");
					//ApplyFilters();
				}
				else
					Toast.MakeText(this, "No transaction selected", ToastLength.Short).Show();
			};
			FindViewById(Resource.Id.menu_delete).Click += delegate {
				if (listView.CheckedItemCount > 0)
				{
					Transaction t = Global.gTransactions[listView.CheckedItemPosition];
					string tDisplay = Global.ConvertTimeStampToDateTime(t.Created).ToString("d");
					foreach (Store s in Global.gStores)
						if (s.Id == t.StoreId)
						{
							tDisplay += ", " + s.Name;
							break;
						}
					tDisplay += ": " + t.Amount.ToString("c2");

					AlertDialog.Builder alert = new AlertDialog.Builder(this);
					alert.SetTitle("Confirm Delete");
					alert.SetMessage("Delete transaction: " + tDisplay + "?");
					alert.SetPositiveButton("Delete", (senderAlert, args) => {
						Global.gTransactions.RemoveAt(listView.CheckedItemPosition);
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
				filtersPopup.MenuItemClick += (s, arg) => {
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
						FiltersDatesFragment fdf = FiltersDatesFragment.NewInstance(filterDates, startDate, endDate, (fon, sdate, edate) => {
							filterDates = fon; startDate = sdate; endDate = edate; ApplyFilters();
						});
						fdf.Show(FragmentManager, "filter_dates");
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
			{
				Toast.MakeText(this, "Action selected: " + item.TitleFormatted,
					ToastLength.Short).Show();
			}
			return base.OnOptionsItemSelected(item);
		}
		#endregion

		#region Private Helper Functions
		private void ResetFilters()
		{
			filterWallets = filterStores = filterDates = filterAmounts = false;

			filteredWallets = new List<Wallet>();
			filteredStores = new List<Store>();
			startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
			endDate = (startDate.AddMonths(1)).Subtract(new TimeSpan(1, 0, 0, 0));
			minAmount = 0.0f;
			maxAmount = float.MaxValue;
		}

		private void ApplyFilters()
		{
			List<Transaction> filtered = Global.gTransactions.Select(t => t).ToList();

			if (filterWallets)
			{
				List<Transaction> tmp = new List<Transaction>();
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
				List<Transaction> tmp = new List<Transaction>();
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

			if (filterDates)
			{
				double sdate = Global.ConvertToUnixTimeStamp(startDate);
				double edate = Global.ConvertToUnixTimeStamp(endDate);

				filtered = filtered
					.Where(t => t.Created > sdate && t.Created < edate)
					.Select(t => t)
					.ToList();
			}

			if (filterAmounts)
			{
				filtered = filtered
					.Where(t => t.Amount > minAmount && t.Amount < maxAmount)
					.Select(t => t)
					.ToList();
			}

			listView.Adapter = new TransactionAdapter(this, filtered);
		}
		#endregion
	}
}