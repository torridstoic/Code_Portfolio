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

using System.Threading.Tasks;

using Trading_Sidekick.Data;


namespace Trading_Sidekick
{
	/// <summary>
	/// This Activity (Android display page) shows a ListView of Items,
	/// such as the results of a search.
	/// </summary>
	[Activity(Label = "ItemListActivity")]
	public class ItemListActivity : Activity
	{
		List<Item> itemList;

		ListView listView;
		Toolbar toolbar;

		protected async override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			string searchString = Intent.GetStringExtra("search_string") ?? String.Empty;
			//Task searchForItems = SearchAsync(searchString);

			// Setup the View
			SetContentView(Resource.Layout.ItemList);
			toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
			SetActionBar(toolbar);
			listView = FindViewById<ListView>(Resource.Id.itemList_listView);
			//listView.FastScrollEnabled = true;
			listView.ItemClick += listView_ItemClick;

			//await searchForItems;
			await SearchAsync(searchString);
		}

		#region ActionBar Overrides
		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.top_menus, menu);
			return base.OnCreateOptionsMenu(menu);
		}
		/// <summary>
		/// Handle any clicks on the toolbar "Options" menu.
		/// </summary>
		/// <param name="item">Clicked MenuItem</param>
		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			// Search button.
			// This pulls up a Search fragment (popup):
			if (item.ItemId == Resource.Id.menu_search)
			{
				string searchString = String.Empty;
				if (toolbar.Title.Contains("\""))
				{
					searchString = toolbar.Title;
					while (searchString.Contains("\""))
					{
						searchString = searchString.Remove(searchString.IndexOf('\"'), 1);
					}
				}

				// When the fragment's Search is run,
				// reload the results into this Activity:
				SearchFragment searchFrag = SearchFragment.NewInstance
					(searchString, (s) => { SearchAsync(s); });
				searchFrag.Show(FragmentManager, "Search");
			}
			// WatchList button.
			// Search for and display any WatchList items:
			else if (item.ItemId == Resource.Id.menu_watchList)
			{
				SearchAsync("watchlist");
			}

			return base.OnOptionsItemSelected(item);
		}
		#endregion

		#region Helper Functions
		/// <summary>
		/// Handle any clicks on the ListView items.
		/// If an item is clicked, load the Details page for that item.
		/// </summary>
		/// <param name="sender">This Activity</param>
		/// <param name="e">EventArgs (not used here)</param>
		private void listView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
		{
			Intent itemDetailsIntent = new Intent(this, typeof(ItemDetailsActivity));
			itemDetailsIntent.PutExtra("item_id", itemList[e.Position].Id);
			//var selectedItem = listView.GetItemAtPosition(e.Position);
			//itemDetailsIntent.PutExtra("item_id", (selectedItem as Item).Id);
			StartActivity(itemDetailsIntent);
			OverridePendingTransition(
				Android.Resource.Animation.SlideInLeft,
				Android.Resource.Animation.SlideOutRight);
		}

		/// <summary>
		/// Search the database for the entered string.
		/// Call wrapper methods in the JsonItemParser class to do this.
		/// </summary>
		/// <param name="searchString">String entered in the Search box (if any)</param>
		/// <returns>Task: the app can run this in background or wait on results</returns>
		private async Task SearchAsync(string searchString)
		{

			if (searchString.Equals(String.Empty))
			{
				ActionBar.Title = "Search Cleared";
				itemList = new List<Item>();
				listView.Adapter = new ItemAdapter(this, itemList);

				Toast.MakeText(this, "Click the Search icon", ToastLength.Short)
					 .Show();
			}
			else if (searchString.Equals("watchlist"))
			{
				Task<List<Item>> getWatchList = JsonItemParser.GetItemsAsync
					(Global.WatchList());
				ActionBar.Title = "Watch List";
				itemList = await getWatchList;
				listView.Adapter = new ItemAdapter(this, itemList);

				Toast.MakeText(this, "Displaying watch list.", ToastLength.Short)
					.Show();
			}
			else
			{
				Task<List<Item>> getItemList = JsonItemParser.SearchItemsAsync
					(searchString, Assets.Open("item_reference.json"));
				ActionBar.Title = String.Format("\"{0}\"", searchString);
				itemList = await getItemList;
				listView.Adapter = new ItemAdapter(this, itemList);

				Toast.MakeText(this, "Search complete.", ToastLength.Short)
					 .Show();
			}
		}
#if false
		private void Search(string searchString)
		{
			ActionBar.Title = String.Format("\"{0}\"", searchString);
			itemList = new List<Item>();

			// If searchString isn't empty, populate the ListView
			if (!searchString.Equals(String.Empty))
			{
				itemList = Global.tradingParser.SearchItems(searchString);
			}
			listView.Adapter = new ItemAdapter(this, itemList);

			Toast.MakeText(this, "Search complete.", ToastLength.Short).Show();
		}
#endif
		#endregion
	}
}