//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Android.App;
//using Android.Content;
//using Android.OS;
//using Android.Runtime;
//using Android.Views;
//using Android.Widget;

//using MoneyManager.Data;

//namespace MoneyManager.Android
//{
//	[Activity(Label = "BudgetsActivity")]
//	public class BudgetsActivity : Activity
//	{
//		List<Budget> budgetList;
//		List<Wallet> walletList;

//		Toolbar toolbar;
//		Toolbar bottomToolbar;
//		ListView listView;

//		protected override void OnCreate(Bundle savedInstanceState)
//		{
//			base.OnCreate(savedInstanceState);
//			SetContentView(Resource.Layout.DataDisplay);

//			// Fetch Data
//			walletList = Global.db.GetWallets(Global.user.Id);
//			// (temporary: does not filter dates)
//			budgetList = new List<Budget>();
//			foreach (Wallet w in walletList)
//				budgetList.AddRange(Global.db.GetBudgets(w.Id));

//			// Top Toolbar Creation
//			toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
//			SetActionBar(toolbar);
//			ActionBar.Title = "Budgets";

//			// Bottom Toolbar Creation + Methods
//			bottomToolbar = FindViewById<Toolbar>(Resource.Id.bottom_toolbar);
//			bottomToolbar.Title = "Options";
//			bottomToolbar.InflateMenu(Resource.Menu.data_bottom_menus);
//			bottomToolbar.MenuItemClick += (sender, e) => {
//				Toast.MakeText(this, "Bottom toolbar tapped: " + e.Item.TitleFormatted, ToastLength.Short).Show();
//			};

//			// Create + Populate the ListView
//			listView = FindViewById<ListView>(Resource.Id.list_view);
//			listView.Adapter = new BudgetAdapter(this, budgetList, walletList);
//		}

//		// Top Toolbar Methods
//		public override bool OnCreateOptionsMenu(IMenu menu)
//		{
//			MenuInflater.Inflate(Resource.Menu.top_menus, menu);
//			return base.OnCreateOptionsMenu(menu);
//		}
//		public override bool OnOptionsItemSelected(IMenuItem item)
//		{
//			Toast.MakeText(this, "Action selected: " + item.TitleFormatted, ToastLength.Short).Show();
//			return base.OnOptionsItemSelected(item);
//		}
//	}
//}