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

using Android.Text;
using Android.Text.Style;
using Android.Graphics;
using System.Threading.Tasks;

using Trading_Sidekick.Data;


namespace Trading_Sidekick
{
	/// <summary>
	/// This Activity (Android display page) shows the details for one Item.
	/// </summary>
	[Activity(Label = "ItemDetailsActivity")]
	public class ItemDetailsActivity : Activity
	{
		int itemId;

		Toolbar toolbar;
		ListView listView;
		CheckBox watchListCheck;

		protected async override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			// Get Item+Price Data
			itemId = Intent.GetIntExtra("item_id", 0);
			Item item;
			Task<Item> getItem = JsonItemParser.GetItemAsync(itemId);
			ItemPrice itemPrice;
			Task<ItemPrice> getPrice = JsonPriceParser.GetPriceAsync(itemId);

			// Setup the View
			SetContentView(Resource.Layout.ItemDisplay);
			toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
			SetActionBar(toolbar);
			TextView rarityText = FindViewById<TextView>
				(Resource.Id.item_itemRarity);
			TextView sellsText = FindViewById<TextView>
				(Resource.Id.item_sellsText);
			TextView buysText = FindViewById<TextView>
				(Resource.Id.item_buysText);
			RadioButton sellsRadio = FindViewById<RadioButton>
				(Resource.Id.item_radioSells);
			RadioButton buysRadio = FindViewById<RadioButton>
				(Resource.Id.item_radioBuys);
			listView = FindViewById<ListView>(Resource.Id.item_listingList);
			watchListCheck = FindViewById<CheckBox>(Resource.Id.item_watchCheck);

			// Display Item Data
			item = await getItem;
			Task<Bitmap> getBmp = ImageParser.GetBitmapAsync(item.Icon);
			ActionBar.Title = item.Name ?? "[]";
			FindViewById<TextView>(Resource.Id.item_itemType)
				.Text = String.Format("Type: {0}", item.Type);
			FindViewById<TextView>(Resource.Id.item_itemDesc)
				.Text = GlobalData.GetDisplayDesc(item.Description);
			rarityText.Text = item.Rarity;
			rarityText.SetTextColor(GlobalData.GetRarityColor(item.Rarity));
			if (Global.WatchList().Contains(item.Id))
			{
				watchListCheck.Checked = true;
			}

			Bitmap bmp = await getBmp;
			if (bmp != null)
			{
				FindViewById<ImageView>(Resource.Id.item_itemIcon)
					.SetImageBitmap(bmp);
			}

			// Display Price Data
			itemPrice = await getPrice;
			if (itemPrice != null)
			{
				sellsText.TextFormatted =
					Global.GetPriceWithCoins(this, itemPrice.Sells.Unit_Price);
				buysText.TextFormatted =
					Global.GetPriceWithCoins(this, itemPrice.Buys.Unit_Price);
				sellsRadio.Enabled = true;
				buysRadio.Enabled = true;
				//sellsRadio.Checked = true;
			}
			else // Item is not publicly tradeable
			{
				sellsText.Text = "Not available.";
				buysText.Text = "Not available.";
				sellsRadio.Enabled = false;
				buysRadio.Enabled = false;
			}

			// Hook up Radio Buttons & CheckBox
			sellsRadio.Click += radioButton_OnClick;
			buysRadio.Click += radioButton_OnClick;
			watchListCheck.Click += async delegate
			{
				bool updateSuccess = await Global.UpdateWatchListAsync(item.Id);
				if (updateSuccess)
				{
					if (watchListCheck.Checked)
					{
						Toast.MakeText(this, "Added to Watch List.", ToastLength.Short)
						.Show();
					}
					else
					{
						Toast.MakeText(this, "Removed From Watch List.", ToastLength.Short)
						.Show();
					}
				}
				else
				{
					Toast.MakeText(this, "Error: failed to update.", ToastLength.Short)
					.Show();
				}
			};
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
			// This pulls up a Search page:
			if (item.ItemId == Resource.Id.menu_search)
			{
				Intent searchIntent = new Intent(this, typeof(MainActivity));
				//itemListIntent.PutExtra("search_string", String.Empty);
				StartActivity(searchIntent);
				OverridePendingTransition(
					Android.Resource.Animation.SlideInLeft,
					Android.Resource.Animation.SlideOutRight);
			}
			// WatchList button.
			// This loads the Item List page and displays WatchList items:
			else if (item.ItemId == Resource.Id.menu_watchList)
			{
				Intent watchListIntent = new Intent(this, typeof(ItemListActivity));
				watchListIntent.PutExtra("search_string", "watchlist");
				StartActivity(watchListIntent);
				OverridePendingTransition(
					Android.Resource.Animation.SlideInLeft,
					Android.Resource.Animation.SlideOutRight);
			}

			return base.OnOptionsItemSelected(item);
		}
		#endregion

		#region Private Helpers
		/// <summary>
		/// Handle any clicks on the Buys/Sells buttons.
		/// This changes whether Buy or Sell listings are displayed.
		/// </summary>
		/// <param name="sender">Clicked button</param>
		/// <param name="e">EventArgs (not used here)</param>
		private async void radioButton_OnClick(object sender, EventArgs e)
		{
			Task<ItemListing> getListing = JsonListingParser.GetListingAsync(itemId);

			if (Resource.Id.item_radioSells == (sender as RadioButton).Id)
			{
				listView.Adapter = new ListingAdapter
					(this, (await getListing).Sells.ToList());
			}
			else if (Resource.Id.item_radioBuys == (sender as RadioButton).Id)
			{
				listView.Adapter = new ListingAdapter
					(this, (await getListing).Buys.ToList());
			}
		}

#if false
		private void listingsButton_OnClick(object sender, EventArgs e)
		{
			Intent listingsIntent = new Intent(this, typeof(ListingsListActivity));
			listingsIntent.PutExtra("item_name", item.Name);
			listingsIntent.PutExtra("item_id", item.Id);
			if ((sender as ImageButton).Id == Resource.Id.item_sellsButton)
			{
				listingsIntent.PutExtra("listing_type", "sells");
			}
			else if ((sender as ImageButton).Id == Resource.Id.item_buysButton)
			{
				listingsIntent.PutExtra("listing_type", "buys");
			}

			StartActivity(listingsIntent);
			OverridePendingTransition(
				Android.Resource.Animation.SlideInLeft,
				Android.Resource.Animation.SlideOutRight);
		}

		private SpannableString DisplayPrice(ItemPrice.Listing listing)
		{
			int val = listing.Unit_Price;
			string display = String.Empty;

			if (val >= 10000)
			{
				display += String.Format("{0}(g), ", val / 10000);
				val %= 10000;

				display += String.Format("{0}(s), ", val / 100);
				val %= 100;

				display += String.Format("{0}(c)", val);
			}
			else if (val >= 100)
			{
				display += String.Format("{0}(s), ", val / 100);
				val %= 100;

				display += String.Format("{0}(c)", val);
			}
			else
			{
				display += String.Format("{0}(c)", val);
			}

			// Now, modify the string into a SpannableString
			// This allows us to add image resources
			SpannableString sDisplay = new SpannableString(display);
			foreach (KeyValuePair<string, int> coinType in coinsDict)
			{
				int i = display.IndexOf(coinType.Key);
				if (i >= 0)
				{
					sDisplay.SetSpan(new ImageSpan(this, coinType.Value), i, i + coinType.Key.Length, 0);
				}
			}

			return sDisplay;
		}
#endif
		#endregion
	}
}