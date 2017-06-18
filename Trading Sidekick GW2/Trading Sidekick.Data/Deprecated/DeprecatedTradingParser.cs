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

using System.Net;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Trading_Sidekick.Data
{
	public class TradingParser
	{
		private string apiKey;
		private const string itemsUrl = "https://api.guildwars2.com/v2/items";
		private const string pricesUrl = "https://api.guildwars2.com/v2/commerce/prices";
		private const string listingsUrl = "https://api.guildwars2.com/v2/commerce/listings";
		//private const string localFile = "item_catalog.json";

		private WebClient webClient;
		//private HttpClient httpClient;
		private List<Item> itemCatalog;


		public TradingParser(string json, string newApiKey = "0CDCF3EC-DB54-F84B-A637-C06BF44290238B11301D-75EA-4244-948B-7270C203A472")
		{
			SetApiKey(newApiKey);
			webClient = new WebClient();
			//httpClient = new HttpClient();
			UpdateCatalog(json);
		}

		public void SetApiKey(string newKey) { apiKey = newKey; }

		#region Items
		public Item GetItem(int id)
		{
			foreach (Item item in itemCatalog)
			{
				if (id == item.Id)
				{
					return item;
				}
			}
			return new Item();
		}
		public List<Item> SearchItems(string searchTerm)
		{
			List<Item> searchResults = itemCatalog
									   .Where(i => i.Name.ToLower().Contains(searchTerm.ToLower()))
									   .OrderBy(i => i.Name)
									   .Select(i => i)
									   .ToList();
			return searchResults;
		}
		#endregion
		#region Prices
		public ItemPrice GetPrice(int id)
		{
			ItemPrice result = null;
			try
			{
				string json = webClient.DownloadString(String.Format("{0}/{1}?access_token={2}", pricesUrl, id, apiKey));
				result = JsonConvert.DeserializeObject<ItemPrice>(json);
			}
			catch (WebException e)
			{
			}
			//return JsonConvert.DeserializeObject<ItemPrice>(webClient.DownloadString(String.Format("{0}/{1}?access_token={2}", pricesUrl, id, apiKey)));

			return result;
		}
		public List<ItemPrice> GetPrices(List<int> idList)
		{
			List<ItemPrice> itemPriceList = new List<ItemPrice>();

			// Make a string of id's to pass as a param to api
			string idParamsStr = String.Empty;
			for (int i = 0; i < idList.Count;)
			{
				idParamsStr += idList[i].ToString() + ",";
				++i;

				// 50 ID's at a time
				if (0 == i % 50 || i >= idList.Count)
				{
					idParamsStr = idParamsStr.Remove(idParamsStr.Length - 1);
					itemPriceList.AddRange(JsonConvert.DeserializeObject<List<ItemPrice>>
						(webClient.DownloadString
						(String.Format("{0}?ids={1}&access_token={2}", pricesUrl, idParamsStr, apiKey))));

					idParamsStr = String.Empty;
				}
			}

			return itemPriceList;
		}
		#endregion
		#region Listings
		public ItemListing GetListing(int id)
		{
			ItemListing result = null;
			try
			{
				string json = webClient.DownloadString(String.Format("{0}/{1}?access_token={2}", listingsUrl, id, apiKey));
				result = JsonConvert.DeserializeObject<ItemListing>(json);
			}
			catch (WebException e)
			{
			}
			//return JsonConvert.DeserializeObject<ItemListing>(webClient.DownloadString(String.Format("{0}/{1}?access_token={2}", listingsUrl, id, apiKey)));

			return result;
		}
		public List<ItemListing> GetListings(List<int> idList)
		{
			List<ItemListing> itemListingList = new List<ItemListing>();

			// Make a string of id's to pass as a param to api
			string idParamsStr = String.Empty;
			for (int i = 0; i < idList.Count;)
			{
				idParamsStr += idList[i].ToString() + ",";
				++i;

				// 50 ID's at a time
				if (0 == i % 50 || i >= idList.Count)
				{
					idParamsStr = idParamsStr.Remove(idParamsStr.Length - 1);
					itemListingList.AddRange(JsonConvert.DeserializeObject<List<ItemListing>>
						(webClient.DownloadString
						(String.Format("{0}?ids={1}&access_token={2}", listingsUrl, idParamsStr, apiKey))));

					idParamsStr = String.Empty;
				}
			}

			return itemListingList;
		}
		#endregion
		#region Helper Functions
		private void UpdateCatalog(string jsonData = "")
		{
			// If a json string was passed in, parse it:
			if (!jsonData.Equals(""))
			{
				itemCatalog = JsonConvert.DeserializeObject<List<Item>>
					(jsonData);
			}
			// If NOT, go through API queries to get new data
			else
			{
				// First, make a list of item ID's
				itemCatalog = new List<Item>();
				List<int> itemIdList = JsonConvert.DeserializeObject<List<int>>
					(webClient.DownloadString(String.Format("{0}?access_token={1}", itemsUrl, apiKey)));

				// Now, Get data for each id's item and add to the list
				string idParamsStr = String.Empty;
				for (int i = 0; i < itemIdList.Count;)
				{
					idParamsStr += itemIdList[i].ToString() + ",";
					++i;

					// 100 ID's at a time:
					if (0 == i % 100 || i >= itemIdList.Count)
					{
						idParamsStr = idParamsStr.Remove(idParamsStr.Length - 1);
						itemCatalog.AddRange(JsonConvert.DeserializeObject<List<Item>>
							(webClient.DownloadString
							(String.Format("{0}?ids={1}&access_token={2}", itemsUrl, idParamsStr, apiKey))));
						idParamsStr = String.Empty;
					}
				}

				//WriteCatalogToAssets();
			}
		}
#if false
		private void WriteCatalogToAssets()
		{
			using (StreamWriter file = File.CreateText(localFile))
			{
				(new JsonSerializer()).Serialize(file, itemCatalog);
			}
		}
#endif
		#endregion
	}
}
#endif