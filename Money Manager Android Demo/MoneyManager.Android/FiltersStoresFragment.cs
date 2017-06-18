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
	public class FiltersStoresFragment : DialogFragment
	{
		bool filterStores;
		List<Store> filteredList;
		Action<Boolean, List<Store>> handler = delegate { };

		ListView listView;

		public static FiltersStoresFragment NewInstance(bool filter, List<Store> fStores, Action<Boolean, List<Store>> func)
		{
			FiltersStoresFragment frag = new FiltersStoresFragment();

			// Load Parameters
			frag.filterStores = filter;
			frag.filteredList = fStores;
			frag.handler = func;

			return frag;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			#region Load the Custom View
			// Load the Custom View and Preset Data
			View currView = inflater.Inflate(Resource.Layout.FiltersListView, container, false);
			currView.FindViewById<TextView>(Resource.Id.filter_listview_title).Text = "Filter Stores";
			listView = currView.FindViewById<ListView>(Resource.Id.filter_listview_listview);
			listView.ChoiceMode = ChoiceMode.Multiple;
			listView.Adapter = new MultiStoreAdapter(Activity);
			foreach (Store s in filteredList)
				for (int i = 0; i < Global.gStores.Count; ++i)
					if (s.Id == Global.gStores[i].Id)
					{
						listView.SetItemChecked(i, true);
						break;
					}
			#endregion

			#region Button Functions
			// Button Functions
			currView.FindViewById<Button>(Resource.Id.filter_listview_clear_button).Click += delegate {
				filterStores = false;
				filteredList = new List<Store>();
				handler(filterStores, filteredList);
				Dismiss();
				Toast.MakeText(Activity, "Stores Filter Reset", ToastLength.Short).Show();
			};

			currView.FindViewById<Button>(Resource.Id.filter_listview_cancel).Click += delegate {
				Dismiss();
				Toast.MakeText(Activity, "Changes Cancelled", ToastLength.Short).Show();
			};
			currView.FindViewById<Button>(Resource.Id.filter_listview_ok).Click += delegate {
				filterStores = true;
				filteredList = new List<Store>();
				for (int i = 0; i < listView.Count; ++i)
					if (listView.IsItemChecked(i))
						filteredList.Add(Global.gStores[i]);

				handler(filterStores, filteredList);
				Dismiss();
				Toast.MakeText(Activity, "Stores Filter Added", ToastLength.Short).Show();
			};
			#endregion

			return currView;
		}
	}
}