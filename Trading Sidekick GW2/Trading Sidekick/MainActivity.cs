using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;


namespace Trading_Sidekick
{
	/// <summary>
	/// This Activity is the main page.
	/// Not much happens here besides a page background and a Search popup (fragment).
	/// </summary>
	[Activity(Label = "Trading_Sidekick")]
	public class MainActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Main);

			// When done loading, show a Search fragment:
			SearchFragment searchFrag = SearchFragment.NewInstance
				(String.Empty, (s) => { Search(s); });
			searchFrag.Show(FragmentManager, "Search");
		}

		#region Helper Functions
#if false
		private async Task<int> InitializeDataAsync()
		{
			// Read the catalog file and initialize parsers
			string jsonContent = String.Empty;
			using (System.IO.StreamReader sr = new System.IO.StreamReader(Assets.Open("item_catalog.json")))
			{
				jsonContent = await sr.ReadToEndAsync();
			}

			Global.itemsParser = new ItemsParser(jsonContent);
			Global.listingParser = new TradingParser();

			return 1;
		}
#endif

		private void Search(string searchString)
		{
			Intent itemListIntent = new Intent(this, typeof(ItemListActivity));
			itemListIntent.PutExtra("search_string", searchString);
			StartActivity(itemListIntent);
			OverridePendingTransition(
				Android.Resource.Animation.SlideInLeft,
				Android.Resource.Animation.SlideOutRight);
		}
#endregion
	}
}

