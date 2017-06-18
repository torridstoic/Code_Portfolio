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

namespace Trading_Sidekick
{
	/// <summary>
	/// While ItemListing contains "deep" data regarding the item's presence on the trading post,
	/// ItemPrice is more of a "surface" view. Here, Buys and Sells show only the top-level Listing available:
	/// the prices the item is currently trading at.
	/// </summary>
	public class ItemPrice
	{
		public int Id { get; set; }
		public Listing Buys { get; set; }
		public Listing Sells { get; set; }

		public class Listing
		{
			public int Unit_Price { get; set; }
			public long Quantity { get; set; }
		}
	}
}