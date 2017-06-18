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

namespace Trading_Sidekick.Data
{
	/// <summary>
	/// Each object of this class represents an item's "presence" on the public market, as returned by the API.
	/// Id references the item tables, which must unfortunately be fetched separately.
	/// Buys and Sells are arrays of Listings, which contains things like price & quantity.
	/// </summary>
	public class ItemListing
	{
		public int Id { get; set; }
		public Listing[] Buys { get; set; }
		public Listing[] Sells { get; set; }

		//public void Print()
		//{
		//	Console.WriteLine("Id: {0}", Id);
		//	Console.WriteLine("Buys:");
		//	foreach (Listing buy in Buys)
		//	{
		//		Console.Write("  ");
		//		buy.Print();
		//	}
		//	Console.WriteLine("Sells:");
		//	foreach (Listing sell in Sells)
		//	{
		//		Console.Write("  ");
		//		sell.Print();
		//	}
		//}

		/// <summary>
		/// Each Listing contains:
		/// Listings = How many postings are available (each posting can have 1 item or many) at this price
		/// Unit_Price = Price of this Listing
		/// Quantity = How many items are at this price (can be higher than "Listings" if multiple items are in each posting)
		/// </summary>
		public class Listing
		{
			public long Listings { get; set; }
			public int Unit_Price { get; set; }
			public long Quantity { get; set; }

			//public void Print()
			//{
			//	Console.WriteLine("Listings: {0}, Price: {1}, Quantity: {2}", Listings, Unit_Price, Quantity);
			//}
		}
	}
}