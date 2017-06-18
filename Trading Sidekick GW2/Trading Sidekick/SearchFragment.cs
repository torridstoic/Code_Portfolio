using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Trading_Sidekick
{
	/// <summary>
	/// This Fragment (Android popup) provides a method for the user to search for items.
	/// Note: Searching is not actually done here.
	/// This fragment handles input and passes any search string to the next Activity (with "handler"),
	/// where the search itself happens.
	/// </summary>
	public class SearchFragment : DialogFragment
	{
		string searchText;
		Action<string> handler = delegate { };

		EditText searchField;

		public static SearchFragment NewInstance(string searchInput, Action<string> func)
		{
			SearchFragment frag = new SearchFragment();

			frag.searchText = searchInput;
			if (null == frag.searchText)
			{
				frag.searchText = String.Empty;
			}
			frag.handler = func;

			return frag;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View currView = inflater.Inflate(Resource.Layout.SearchFragment, container, false);

			searchField = currView.FindViewById<EditText>(Resource.Id.searchFrag_searchField);
			searchField.Text = searchText;

			// If the WatchList button is clicked, we pass along a special string.
			// That string is specifically handled in the next Activity (ItemListActivity).
			currView.FindViewById<Button>(Resource.Id.searchFrag_watchButton)
				.Click += delegate
			{
				handler("watchlist");
				Dismiss();
			};
			// Clear button: clears the search box.
			currView.FindViewById<Button>(Resource.Id.searchFrag_clearButton)
				.Click += delegate
			{
				searchField.Text = String.Empty;
			};
			// Search button: passes along the current search string,
			// and loads the next Activity.
			currView.FindViewById<Button>(Resource.Id.searchFrag_searchButton)
				.Click += delegate
			{
				//searchText = searchField.Text;
				//if (String.Empty == searchText)
				//{
				//	Toast.MakeText(Activity, "Don't search for nothing.", ToastLength.Short).Show();
				//	return;
				//}
				handler(searchField.Text);
				Dismiss();
			};

			return currView;
		}
	}
}