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

using Trading_Sidekick.Data;
using Android.Graphics;

namespace Trading_Sidekick
{
	// Adapters are used in Xamarin Android to populate ListViews.
	// ListViews are a very common layout, making the page into a scrollable list.
	// By extending the BaseAdapter class, I can easily populate my ListViews with customized content.

	/// <summary>
	/// This Adapter is designed to show a list of Items.
	/// Each entry shows the most relevant info for a specific Item.
	/// </summary>
	public class ItemAdapter : BaseAdapter<Item>
	{
		Activity context;
		List<Item> items;

		public ItemAdapter(Activity c, List<Item> i) : base()
		{
			context = c;
			items = i;
		}

		public override long GetItemId(int position)
		{
			return position;
		}
		public override Item this[int position]
		{
			get { return items[position]; }
		}
		public override int Count
		{
			get { return items.Count; }
		}
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View currView = convertView;
			if (null == currView)
			{
				// CustomListItem is the layout designed for each entry in this list.
				// It contains space for the item's name and icon.
				currView = context.LayoutInflater.Inflate
					(Resource.Layout.CustomListItem, null);
			}
			// Get the View's sections:
			TextView nameText = currView.FindViewById<TextView>(Resource.Id.listItem_name);
			ImageView iconView = currView.FindViewById<ImageView>(Resource.Id.listItem_icon);

			// Get our current Item to display by accessing the "position" param:
			Item currItem = items[position];
			Bitmap bmp = ImageParser.GetBitmap(currItem.Icon);

			// Pair up the Item's data with the View's sections:
			nameText.Text = currItem.Name;
			if (bmp != null)
			{
				iconView.SetImageBitmap(bmp);
				iconView.SetBackgroundColor
					(GlobalData.GetRarityColor(currItem.Rarity));
			}

			return currView;
		}
	}

	/// <summary>
	/// This Adapter is designed to show a list of ItemListings.
	/// </summary>
	public class ListingAdapter : BaseAdapter<ItemListing.Listing>
	{
		Activity context;
		List<ItemListing.Listing> items;

		public ListingAdapter(Activity c, List<ItemListing.Listing> l) : base()
		{
			context = c;
			items = l;
		}

		public override long GetItemId(int position)
		{
			return position;
		}
		public override ItemListing.Listing this[int position]
		{
			get { return items[position]; }
		}
		public override int Count
		{
			get { return items.Count; }
		}
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View currView = convertView;
			if (null == currView)
			{
				// CustomListListing is the layout designed for each entry in this list.
				// It contains space for the listing's price, quantity, and number of listings.
				currView = context.LayoutInflater.Inflate
					(Resource.Layout.CustomListListing, null);
			}

			// Get our current Listing by using the "position" param:
			ItemListing.Listing currItem = items[position];
			
			// Pair up the Listing's fields with the Layout's sections:
			currView.FindViewById<TextView>(Resource.Id.listingItem_price)
				.TextFormatted = Global.GetPriceWithCoins(context, currItem.Unit_Price);
			currView.FindViewById<TextView>(Resource.Id.listingItem_quantity)
				.Text = String.Format("{0} items", currItem.Quantity);
			currView.FindViewById<TextView>(Resource.Id.listingItem_listings)
				.Text = String.Format("{0} listings", currItem.Listings);

			return currView;
		}
	}
}