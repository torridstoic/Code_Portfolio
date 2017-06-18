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
using Newtonsoft.Json;
using System.Net.Http;


namespace Trading_Sidekick.Data
{
	/// <summary>
	/// This wrapper class sends search queries to the API in order to retrieve price objects.
	/// </summary>
	public static class JsonPriceParser
	{
		private const string apiUrl = "https://api.guildwars2.com/v2/commerce/prices";

		/// <summary>
		/// Takes an item ID and returns the corresponding ItemPrice.
		/// (the ItemPrice contains *current* trading information, such as price/quantity)
		/// </summary>
		/// <param name="id">ID of the specified item</param>
		/// <returns>The relevant ItemPrice (null on failure)</returns>
		public static async Task<ItemPrice> GetPriceAsync(int id)
		{
			ItemPrice result = null;

			using (HttpClient httpClient = new HttpClient())
			{
				try
				{
					Task<string> json = httpClient.GetStringAsync
						(String.Format("{0}/{1}?{2}", apiUrl, id, GlobalData.GetApiToken()));
					result = JsonConvert.DeserializeObject<ItemPrice>(await json);
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
		/// and returns a List of ItemPrices.
		/// </summary>
		/// <param name="idList">List of ID's for Prices to fetch</param>
		/// <returns>Corresponding List of ItemPrices (empty List on failure)</returns>
		public static async Task<List<ItemPrice>> GetPricesAsync(List<int> idList)
		{
			List<ItemPrice> results = new List<ItemPrice>();

			if (1 == idList.Count)
			{
				results.Add(await GetPriceAsync(idList[0]));
			}
			else
			{
				List<Task<List<ItemPrice>>> apiRequests = new List<Task<List<ItemPrice>>>();
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

						// Start each query as a "task" so they can run simultanously:
						apiRequests.Add(InternalPricesFetchAsync(idParamsStr));

						idParamsStr = String.Empty;
					}
				}

				foreach (Task<List<ItemPrice>> t in apiRequests)
				{
					// Once all the tasks are begun, we can await their results
					// and add each to our List:
					List<ItemPrice> items = await t;
					if (items != null)
					{
						results.AddRange(items);
					}
				}
			}

			return results;
		}
		
		/// <summary>
		/// Helper method, used to fetch multiple ItemPrices from the API.
		/// The string parameter has already been translated from an int list into a query string.
		/// </summary>
		/// <param name="param">List of item ID's reformatted into a query string</param>
		/// <returns>List of ItemPrices fetched from API (empty List on failure)</returns>
		private static async Task<List<ItemPrice>> InternalPricesFetchAsync(string param)
		{
			List<ItemPrice> results = null;

			using (HttpClient httpClient = new HttpClient())
			{
				try
				{
					Task<string> json = httpClient.GetStringAsync
						(String.Format("{0}?ids={1}&{2}", apiUrl, param, GlobalData.GetApiToken()));
					results = JsonConvert.DeserializeObject<List<ItemPrice>>(await json);
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