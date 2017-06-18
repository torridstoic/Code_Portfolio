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

using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace Trading_Sidekick.Data
{
	/// <summary>
	/// 
	/// </summary>
	public static class JsonListingParser
	{
		private const string apiUrl = "https://api.guildwars2.com/v2/commerce/listings";

		/// <summary>
		/// Takes an item ID and returns the corresponding ItemListing.
		/// (the ItemListing contains trading information, such as price and quantity)
		/// </summary>
		/// <param name="id">ID of the specified item</param>
		/// <returns>The relevant ItemListing (null on failure)</returns>
		public static async Task<ItemListing> GetListingAsync(int id)
		{
			ItemListing result = null;

			using (HttpClient httpClient = new HttpClient())
			{
				try
				{
					Task<string> json = httpClient.GetStringAsync
						(String.Format("{0}/{1}?{2}", apiUrl, id, GlobalData.GetApiToken()));
					result = JsonConvert.DeserializeObject<ItemListing>(await json);
				}
				catch (HttpRequestException)
				{
					result = null;
				}
			}

			return result;
		}

		/// <summary>
		/// Similar, but takes a List of ID's
		/// and returns a List of ItemListings.
		/// </summary>
		/// <param name="idList">List of ID's for Listings to fetch</param>
		/// <returns>Corresponding List of ItemListings (empty List on failure)</returns>
		public static async Task<List<ItemListing>> GetListingsAsync(List<int> idList)
		{
			List<ItemListing> results = new List<ItemListing>();

			if (1 == idList.Count)
			{
				results.Add(await GetListingAsync(idList[0]));
			}
			else
			{
				List<Task<List<ItemListing>>> apiRequests = new List<Task<List<ItemListing>>>();
				string idParamsStr = String.Empty;
				for (int i = 0; i < idList.Count;)
				{
					idParamsStr += idList[i].ToString() + ",";
					++i;

					// 50 ID's at a time
					// (the endpoint can't handle too many requests at once)
					if (0 == i % 50 || i >= idList.Count)
					{
						idParamsStr.Remove(idParamsStr.LastIndexOf(','));

						// Start each query as a "task" so they can run simultaneously:
						apiRequests.Add(InternalListingsFetchAsync(idParamsStr));

						idParamsStr = String.Empty;
					}
				}

				foreach (Task<List<ItemListing>> t in apiRequests)
				{
					// Once all the tasks are begun, we can await their results
					// and add each to our List:
					List<ItemListing> items = await t;
					if (items != null)
					{
						results.AddRange(items);
					}
				}
			}

			return results;
		}

		/// <summary>
		/// Helper method, used to fetch multiple ItemListings from the API.
		/// The string parameter has already been translated from an int list into a query string.
		/// </summary>
		/// <param name="param">List of item ID's reformatted into a query string</param>
		/// <returns>List of ItemListings fetched from API (empty List on failure)</returns>
		private static async Task<List<ItemListing>> InternalListingsFetchAsync(string param)
		{
			List<ItemListing> results = null;

			using (HttpClient httpClient = new HttpClient())
			{
				try
				{
					Task<string> json = httpClient.GetStringAsync
						(String.Format("{0}?ids={1}&{2}", apiUrl, param, GlobalData.GetApiToken()));
					results = JsonConvert.DeserializeObject<List<ItemListing>>(await json);
				}
				catch (HttpRequestException)
				{
					results = null;
				}
			}

			return results;
		}
	}
}