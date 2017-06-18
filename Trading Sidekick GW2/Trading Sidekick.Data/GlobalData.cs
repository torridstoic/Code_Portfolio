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

using Android.Graphics;


namespace Trading_Sidekick.Data
{
	/// <summary>
	/// This class is used to contain certain NON-ANDROID static and constant fields and methods.
	/// Keeping it in a separate place here allows easy access from various points in the app.
	/// </summary>
	public static class GlobalData
	{
		/// <summary>
		/// This is my private API key, used to access the endpoints I needed during development.
		/// If this app was continued and taken to release, this key would have been replaced
		/// by a method of acquiring each user's personal key.
		/// </summary>
		private const string apiKey = "0CDCF3EC-DB54-F84B-A637-C06BF44290238B11301D-75EA-4244-948B-7270C203A472";
		public static string GetApiToken()
		{
			return String.Format("access_token={0}", apiKey);
		}

		/// <summary>
		/// This dictionary associates item rarities (strings) with their ingame colors.
		/// This is called on to display item names & icons in easy-to-discern ways for players.
		/// </summary>
		private static readonly Dictionary<string, Color> colorDict = new Dictionary<string, Color>
		{
			{"Junk", Color.Rgb(170, 170, 170) },
			{"Basic", Color.White },
			{"Fine", Color.Rgb(98, 164, 218) },
			{"Masterwork", Color.Rgb(26, 147, 6) },
			{"Rare", Color.Rgb(252, 208, 11) },
			{"Exotic", Color.Rgb(255, 164, 5) },
			{"Ascended", Color.Rgb(251, 62, 141) },
			{"Legendary", Color.Rgb(76, 19, 157) }
		};
		public static Color GetRarityColor(string rarity)
		{
			return colorDict[rarity];
		}

		/// <summary>
		/// The GW2 API returns some item descriptions with unfortunate characters.
		/// This method returns a string formatted for display.
		/// </summary>
		/// <param name="description">Item desc. as returned by the API (string)</param>
		/// <returns>Readable description (string)</returns>
		public static string GetDisplayDesc(string description)
		{
			string result = description ?? String.Empty;
			while (result.Contains("<"))
			{
				int startIndex = result.IndexOf('<');
				int endIndex = result.IndexOf('>');
				result = result.Remove(startIndex, endIndex - startIndex + 1);
			}
			return result;
		}
	}
}