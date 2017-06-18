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
using System.Threading.Tasks;
using System.IO;

//using Trading_Sidekick.Data;


namespace Trading_Sidekick
{
	/// <summary>
	/// This class contains certain ANDROID-SPECIFIC static and constant fields and methods.
	/// </summary>
	public static class Global
	{
		/// <summary>
		/// This is a reference Map for replacing specific characters with resource icons.
		/// (coin pictures, for adding to price strings for display)
		/// </summary>
		private static readonly Dictionary<string, int> coinsDict = new Dictionary<string, int>
		{
			{"(g)", Resource.Drawable.goldcoin },
			{"(s)", Resource.Drawable.silvercoin },
			{"(c)", Resource.Drawable.coppercoin }
		};

		/// <summary>
		/// The WatchList is an ability to locally store a list of items that the user is interested in.
		/// This list is persistently saved on the Android device,
		/// and can be quickly loaded to view current price data.
		/// </summary>
		private const string watchListFile = "watch_list.csv";
		private static List<int> watchList = new List<int>();
		public static List<int> WatchList() { return watchList; }

		/// <summary>
		/// Takes an item price (int), and translates it into a SpannableString:
		/// a string formatted for Android display, with icons (coin pictures) included.
		/// </summary>
		/// <param name="ctx">Current Android context</param>
		/// <param name="val">Price/Value</param>
		/// <returns>Display-ready string</returns>
		public static SpannableString GetPriceWithCoins(Activity ctx, int val)
		{
			string pString = String.Empty;

			// Create a "price" string with placeholders for coins
			if (val >= 100)
			{
				if (val >= 10000)
				{
					pString += String.Format("{0}(g)", val / 10000);
					val %= 10000;
				}
				pString += String.Format("{0}(s)", val / 100);
				val %= 100;
			}
			pString += String.Format("{0}(c)", val);

			// Now, modify the string into a SpannableString
			// This allows us to add image resources
			SpannableString pSpan = new SpannableString(pString);
			foreach (KeyValuePair<string, int> coinType in coinsDict)
			{
				// Find each coin placeholder, and replace with the relevant icon:
				int i = pString.IndexOf(coinType.Key);
				if (i >= 0)
				{
					pSpan.SetSpan(new ImageSpan(ctx, coinType.Value), i, i + coinType.Key.Length, 0);
				}
			}

			return pSpan;
		}

		/// <summary>
		/// Load & read the local WatchList file from Resources,
		/// and save it into a List of ints (item ID's).
		/// </summary>
		/// <returns>Task: true on completion, false on failure</returns>
		public static async Task<bool> ReadWatchListAsync()
		{
			// Get file path
			string filePath = Path.Combine(
				System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),
				watchListFile);

			// Read file contents
			string fileContents = null;
			try
			{
				using (StreamReader sr = File.OpenText(filePath))
				{
					fileContents = await sr.ReadToEndAsync();
					if (fileContents[fileContents.Length - 1].Equals(','))
					{
						fileContents = fileContents.Remove(fileContents.Length - 1, 1);
					}
				}
			}
			catch
			{
				return false;
			}

			// Parse file contents into global watchList (List<int>)
			List<string> stringList = fileContents
				.Split(',')
				.ToList();
			watchList = new List<int>();
			foreach (string s in stringList)
			{
				watchList.Add(int.Parse(s));
			}

			return true;
		}
		/// <summary>
		/// Saves the WatchList (currently a List of ints)
		/// into the local Resources file, as a formatted string.
		/// </summary>
		/// <returns>Task: true on completion, false on failure</returns>
		public static async Task<bool> WriteWatchListAsync()
		{
			// Get file path
			string filePath = Path.Combine(
				System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),
				watchListFile);

			// Parse id list into one string
			string output = String.Empty;
			foreach (int i in watchList)
			{
				output += String.Format("{0},", i.ToString());
			}
			if (watchList.Count > 0)
			{
				// Remove trailing comma
				output = output.Remove(output.Length - 1, 1);
			}

			// Open & Write File
			try
			{
				using (StreamWriter sw = File.CreateText(filePath))
				{
					await sw.WriteAsync(output);
				}
			}
			catch
			{
				return false;
			}

			return true;
		}
		/// <summary>
		/// Adds a new item ID to the WatchList (and sorts the list),
		/// or removes an existing item ID.
		/// </summary>
		/// <param name="id">ID of particular item</param>
		/// <returns>Task: true on completion</returns>
		public static async Task<bool> UpdateWatchListAsync(int id)
		{
			if (watchList.Contains(id))
			{
				watchList.Remove(id);
			}
			else
			{
				watchList.Add(id);
				watchList.Sort();
			}

			return (await WriteWatchListAsync());
		}
	}
}