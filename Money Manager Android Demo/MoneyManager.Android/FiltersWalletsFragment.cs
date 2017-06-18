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
	public class FiltersWalletsFragment : DialogFragment
	{
		bool filterWallets;
		List<Wallet> filteredList;
		Action<Boolean, List<Wallet>> handler = delegate { };

		ListView listView;

		public static FiltersWalletsFragment NewInstance(bool filter, List<Wallet> fWallets, Action<Boolean, List<Wallet>> func)
		{
			FiltersWalletsFragment frag = new FiltersWalletsFragment();

			// Load Parameters
			frag.filterWallets = filter;
			frag.filteredList = fWallets;
			frag.handler = func;

			return frag;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			#region Load the Custom View
			// Load the Custom View and Preset Data
			View currView = inflater.Inflate(Resource.Layout.FiltersListView, container, false);
			currView.FindViewById<TextView>(Resource.Id.filter_listview_title).Text = "Filter Wallets";
			listView = currView.FindViewById<ListView>(Resource.Id.filter_listview_listview);
			listView.ChoiceMode = ChoiceMode.Multiple;
			listView.Adapter = new MultiWalletAdapter(Activity);
			foreach (Wallet w in filteredList)
				for (int i = 0; i < Global.gWallets.Count; ++i)
					if (w.Id == Global.gWallets[i].Id)
					{
						listView.SetItemChecked(i, true);
						break;
					}
			#endregion

			#region Button Functions
			// Button Functions
			currView.FindViewById<Button>(Resource.Id.filter_listview_clear_button).Click += delegate {
				filterWallets = false;
				filteredList = new List<Wallet>();
				handler(filterWallets, filteredList);
				Dismiss();
				Toast.MakeText(Activity, "Wallets Filter Reset", ToastLength.Short).Show();
			};

			currView.FindViewById<Button>(Resource.Id.filter_listview_cancel).Click += delegate {
				Dismiss();
				Toast.MakeText(Activity, "Changes Cancelled", ToastLength.Short).Show();
			};
			currView.FindViewById<Button>(Resource.Id.filter_listview_ok).Click += delegate {
				filterWallets = true;
				filteredList = new List<Wallet>();
				for (int i = 0; i < listView.Count; ++i)
					if (listView.IsItemChecked(i))
						filteredList.Add(Global.gWallets[i]);

				handler(filterWallets, filteredList);
				Dismiss();
				Toast.MakeText(Activity, "Wallets Filter Added", ToastLength.Short).Show();
			};
			#endregion

			return currView;
		}
	}
}