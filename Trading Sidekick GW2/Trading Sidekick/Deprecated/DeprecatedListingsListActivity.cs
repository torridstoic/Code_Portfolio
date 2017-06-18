#if false
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
	[Activity(Label = "ListingsListActivity")]
	public class ListingsListActivity : Activity
	{
		int itemId;
		ItemListing listing;

		ListView listView;
		Toolbar toolbar;

		protected async override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			string itemName = Intent.GetStringExtra("item_name") ?? "unknown item?";
			itemId = Intent.GetIntExtra("item_id", 0);
			SetContentView(Resource.Layout.ItemList);

			listing =  await JsonListingParser.GetListingAsync(itemId);


			// Setup the ActionBar & Tabs
			toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
			SetActionBar(toolbar);
			//ActionBar.Title = itemName;

			// Setup the ListView
			listView = FindViewById<ListView>(Resource.Id.listingList_listView);
			//listView.FastScrollEnabled = true;
#if false
			ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
			ActionBar.Tab buyTab = ActionBar.NewTab();
			buyTab.SetText("Buy Orders");
			buyTab.SetIcon(Resource.Drawable.ic_list_black_24dp);
			ActionBar.Tab sellTab = ActionBar.NewTab();
			sellTab.SetText("Sell Prices");
			sellTab.SetIcon(Resource.Drawable.ic_list_black_24dp);

			buyTab.TabSelected += tab_OnSelect;
			buyTab.TabReselected += tab_OnReselect;
			sellTab.TabSelected += tab_OnSelect;
			sellTab.TabReselected += tab_OnReselect;
			ActionBar.AddTab(buyTab);
			ActionBar.AddTab(sellTab);
#endif
		}

		private void tab_OnSelect(object sender, EventArgs e)
		{
			if("Buy Orders" == (sender as ActionBar.Tab).Text)
			{
				listView.Adapter = new ListingAdapter
					(this, listing.Buys.ToList());
			}
			else if("Sell Prices" == (sender as ActionBar.Tab).Text)
			{
				listView.Adapter = new ListingAdapter
					(this, listing.Sells.ToList());
			}
		}
		private async void tab_OnReselect(object sender, EventArgs e)
		{
			listing = await JsonListingParser.GetListingAsync(itemId);

			if ("Buy Orders" == (sender as ActionBar.Tab).Text)
			{
				listView.Adapter = new ListingAdapter
					(this, listing.Buys.ToList());
			}
			else if ("Sell Prices" == (sender as ActionBar.Tab).Text)
			{
				listView.Adapter = new ListingAdapter
					(this, listing.Sells.ToList());
			}
		}

#if false
		protected async override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			string itemName = Intent.GetStringExtra("item_name") ?? "unknown item?";
			int itemId = Intent.GetIntExtra("item_id", 0);
			string listingType = Intent.GetStringExtra("listing_type") ?? "error";
			Task<ItemListing> getListing = JsonListingParser.GetListingAsync(itemId);

			// Setup the View
			SetContentView(Resource.Layout.ItemList);
			toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
			SetActionBar(toolbar);
			listView = FindViewById<ListView>(Resource.Id.itemList_listView);
			//listView.FastScrollEnabled = true;
			
			if ("buys" == listingType)
			{
				ActionBar.Title = String.Format("{0}: Buy Orders", itemName);
				listView.Adapter = new ListingAdapter
					(this, (await getListing).Buys.ToList());
			}
			else // "sells" == listingType
			{
				ActionBar.Title = String.Format("{0}: Sale Prices", itemName);
				listView.Adapter = new ListingAdapter
					(this, (await getListing).Sells.ToList());
			}
		}
#endif

		#region ActionBar Overrides
#if true
		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.top_listings, menu);
			return base.OnCreateOptionsMenu(menu);
		}
		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			if (item.ItemId == Resource.Id.menu_buys)
			{

			}
			else if(item.ItemId == Resource.Id.menu_sales)
			{

			}

			return base.OnOptionsItemSelected(item);
		}
#endif
		#endregion

	}
}
#endif