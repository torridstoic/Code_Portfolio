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
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;

namespace Trading_Sidekick.Data
{
	/// <summary>
	/// This wrapper class sends search queries to the API in order to retrieve items.
	/// </summary>
	public static class JsonItemParser
	{
		private const string apiUrl = "https://api.guildwars2.com/v2/items";

		/// <summary>
		/// Search the entire item database for all related items.
		/// </summary>
		/// <param name="searchTerm">Any item containing this term will be found</param>
		/// <param name="jsonStream">A local database reference file, kept and updated to improve this process</param>
		/// <returns>List of Items matching the query (or empty List)</returns>
		public static async Task<List<Item>> SearchItemsAsync(string searchTerm, Stream jsonStream)
		{
			List<Item> results = null;
			List<int> referenceList = null;

			using (StreamReader sr = new StreamReader(jsonStream))
			{
				Task<string> getJson = sr.ReadToEndAsync();
				referenceList = JsonConvert
					.DeserializeObject<List<ItemReference>>(await getJson)
					.Where(i => i.Name.ToLower().Contains(searchTerm.ToLower()))
					.OrderBy(i => i.Name)
					.Select(i => i.Id)
					.ToList();
			}
#if false
			using (StreamReader sr = new StreamReader(jsonStream))
			{
				Task<string> json = sr.ReadToEndAsync();
				results = JsonConvert.DeserializeObject<List<Item>>(await json)
						  .Where(i => i.Name.ToLower().Contains(searchTerm.ToLower()))
						  .OrderBy(i => i.Name)
						  .Select(i => i)
						  .ToList();
			}
#endif
			results = await GetItemsAsync(referenceList);

			return results;
		}

		/// <summary>
		/// Takes an item ID and returns the corresponding Item object.
		/// Since some of the API endpoints only reference items by their ID,
		/// this method is necessary if more is wanted. (name, rarity, icon, etc.)
		/// </summary>
		/// <param name="id">ID of specific item</param>
		/// <returns>Item-type object, containing the item's data (null on failure)</returns>
		public static async Task<Item> GetItemAsync(int id)
		{
			Item result = null;

			using (HttpClient httpClient = new HttpClient())
			{
				try
				{
					Task<string> jsonString = httpClient.GetStringAsync
						(String.Format("{0}/{1}?{2}", apiUrl, id, GlobalData.GetApiToken()));
					result = JsonConvert.DeserializeObject<Item>(await jsonString);
				}
				catch (HttpRequestException)
				{
					result = null;
				}
			}

			return result;
		}

		/// <summary>
		/// Similar to the previous method, but takes a List of ID's
		/// and returns a List of Items.
		/// </summary>
		/// <param name="idList">List of ID's for Items to fetch.</param>
		/// <returns>List of Items, one for each ID (empty List on failure)</returns>
		public static async Task<List<Item>> GetItemsAsync(List<int> idList)
		{
			List<Item> results = new List<Item>();
			List<Task<List<Item>>> getItems = new List<Task<List<Item>>>();
			string idParamsStr = String.Empty;

			for (int i = 0; i < idList.Count;)
			{
				idParamsStr += String.Format("{0},", idList[i].ToString());
				++i;

				// 150 ID's at a time
				// (the endpoint can't handle too many requests at once)
				if (0 == i % 150 || i >= idList.Count)
				{
					idParamsStr.Remove(idParamsStr.LastIndexOf(','));

					// Start each query as a "task" so they can run simultaneously:
					getItems.Add(InternalAPIQueryAsync(idParamsStr));

					idParamsStr = String.Empty;
				}
			}

			foreach (Task<List<Item>> task in getItems)
			{
				// Once all the tasks are begun, we can await their results
				// and add each to our List:
				List<Item> items = await task;
				if (items != null)
				{
					results.AddRange(items);
				}
			}

			return results;
		}

		/// <summary>
		/// Helper method, used to fetch multiple Items from the API.
		/// The string parameter has already been translated from an int list into a query string.
		/// </summary>
		/// <param name="idParams">List of Item ID's reformatted into a query string</param>
		/// <returns>List of Items fetched from API</returns>
		private static async Task<List<Item>> InternalAPIQueryAsync(string idParams)
		{
			List<Item> results = null;

			using (HttpClient hc = new HttpClient())
			{
				try
				{
					Task<string> getJson = hc.GetStringAsync
						(String.Format("{0}?ids={1}&{2}", apiUrl, idParams, GlobalData.GetApiToken()));
					results = JsonConvert.DeserializeObject<List<Item>>(await getJson);
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