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
using System.IO;
using Newtonsoft.Json;

namespace Trading_Sidekick.Data
{
	public class ItemsParser
	{
		private string apiKey;
		private const string apiUrl = "https://api.guildwars2.com/v2/items";
		private const string localFile = "item_catalog.json";

		private WebClient webClient;
		private List<Item> itemCatalog;

		// Ctor needs to setup the WebClient and Item Catalog
		public ItemsParser(string json, string newApiKey = "0CDCF3EC-DB54-F84B-A637-C06BF44290238B11301D-75EA-4244-948B-7270C203A472")
		{
			SetApiKey(newApiKey);
			webClient = new WebClient();
			UpdateCatalog(json);
		}

		public void SetApiKey(string newKey) { apiKey = newKey; }

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
									   .ToList<Item>();
			return searchResults;
		}

		public void UpdateCatalog(string jsonData = "")
		{
			// If a json string was passed in, parse it:
			if (!jsonData.Equals(""))
			{
				itemCatalog = JsonConvert.DeserializeObject<List<Item>>
					(jsonData);
			}
			// If not, go through API queries to get new data:
			else
			{
				// First, make a list of item ID's
				itemCatalog = new List<Item>();
				List<int> itemIdList = JsonConvert.DeserializeObject<List<int>>
					(webClient.DownloadString(String.Format("{0}?access_token={1}", apiUrl, apiKey)));

				// Now, Get data for each id's item and add to the List
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
							(String.Format("{0}?ids={1}&access_token={2}", apiUrl, idParamsStr, apiKey))));
						idParamsStr = String.Empty;
					}
				}

				//WriteCatalogToAssets();
			}
		}

		private void WriteCatalogToAssets()
		{
			using (StreamWriter file = File.CreateText(localFile))
			{
				(new JsonSerializer()).Serialize(file, itemCatalog);
			}
		}
	}
}
#endif