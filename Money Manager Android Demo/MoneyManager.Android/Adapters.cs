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

using MoneyManager.Data;

namespace MoneyManager.Android
{
	public class TransactionAdapter : BaseAdapter<Transaction>
	{
		// Fields
		Activity context;
		List<Transaction> items;

		// Ctor
		public TransactionAdapter(Activity c, List<Transaction> t) : base()
		{
			context = c;
			items = t;
		}

		// Adapter Overrides
		public override long GetItemId(int position)
		{
			return position;
		}
		public override Transaction this[int position]
		{
			get { return items[position]; }
		}
		public override int Count
		{
			get { return items.Count; }
		}
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			Transaction currItem = items[position];
			string display = Global.ConvertTimeStampToDateTime(currItem.Created).ToString("d");
			foreach (Store s in Global.gStores)
				if (s.Id == currItem.StoreId)
				{
					display += ", " + s.Name;
					break;
				}
			display += ": " + currItem.Amount.ToString("c2");
			// display = "MM/DD/YY, Store: $0.00"

			// Now set the View and return it
			View currView = convertView;
			if (null == currView)
				currView = context.LayoutInflater.Inflate(global::Android.Resource.Layout.SimpleListItemSingleChoice, null);
			currView.FindViewById<TextView>(global::Android.Resource.Id.Text1).Text = display;

			return currView;
		}
	}

	public class RecurringTransactionAdapter : BaseAdapter<RecurringTransaction>
	{
		// Fields
		Activity context;
		List<RecurringTransaction> items;

		// Ctor
		public RecurringTransactionAdapter(Activity c, List<RecurringTransaction> t) : base()
		{
			context = c;
			items = t;
		}

		// Adapter Overrides
		public override long GetItemId(int position)
		{
			return position;
		}
		public override RecurringTransaction this[int position]
		{
			get { return items[position]; }
		}
		public override int Count
		{
			get { return items.Count; }
		}
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			RecurringTransaction currItem = items[position];
			string display = "";
			switch (currItem.ProcessPeriod)
			{
				case 0:
					display += "Daily, ";
					break;
				case 1:
					display += "Weekly, ";
					break;
				case 2:
					display += "Monthly, ";
					break;
				case 3:
					display += "Quarterly, ";
					break;
				case 4:
					display += "Yearly, ";
					break;
			}
			foreach (Store s in Global.gStores)
				if (s.Id == currItem.StoreId)
				{
					display += s.Name;
					break;
				}
			display += ": " + currItem.Amount.ToString("c2");
			// display = "[Period], Store: $0.00"

			// Now set the View and return it
			View currView = convertView;
			if (null == currView)
				currView = context.LayoutInflater.Inflate(global::Android.Resource.Layout.SimpleListItemSingleChoice, null);
			currView.FindViewById<TextView>(global::Android.Resource.Id.Text1).Text = display;

			return currView;
		}
	}

	public class WalletAdapter : BaseAdapter<Wallet>
	{
		// Fields
		Activity context;
		//List<Wallet> items;

		// Ctor
		public WalletAdapter(Activity c) : base()
		{
			context = c;
			//items = w;
		}

		// Adapter Overrides
		public override long GetItemId(int position)
		{
			return position;
		}
		public override Wallet this[int position]
		{
			get { return Global.gWallets[position]; }
		}
		public override int Count
		{
			get { return Global.gWallets.Count; }
		}
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			Wallet currItem = Global.gWallets[position];
			string display = currItem.Name;

			// Now set the View and return it
			View currView = convertView;
			if (null == currView)
				currView = context.LayoutInflater.Inflate(global::Android.Resource.Layout.SimpleListItemSingleChoice, null);
			currView.FindViewById<TextView>(global::Android.Resource.Id.Text1).Text = display;

			return currView;
		}
	}

	//public class BudgetAdapter : BaseAdapter<Budget>
	//{
	//	// Fields
	//	Activity context;
	//	List<Budget> items;
	//	List<Wallet> walletList;

	//	// Ctor
	//	public BudgetAdapter(Activity c, List<Budget> b, List<Wallet> w) : base()
	//	{
	//		context = c;
	//		items = b;
	//		walletList = w;
	//	}

	//	// Adapter Overrides
	//	public override long GetItemId(int position)
	//	{
	//		return position;
	//	}
	//	public override Budget this[int position]
	//	{
	//		get { return items[position]; }
	//	}
	//	public override int Count
	//	{
	//		get { return items.Count; }
	//	}
	//	public override View GetView(int position, View convertView, ViewGroup parent)
	//	{
	//		Budget currItem = items[position];
	//		string display = String.Empty;
	//		foreach (Wallet w in walletList)
	//			if (w.Id == currItem.WalletId)
	//			{
	//				display = w.Name;
	//				break;
	//			}
	//		display += ": " + Global.ConvertTimeStampToDateTime(currItem.StartDate).ToString("d");
	//		display += "-" + Global.ConvertTimeStampToDateTime(currItem.EndDate).ToString("d");
	//		float amRemain = currItem.Amount;
	//		List<Transaction> trans = Global.db.GetTransactions(currItem.WalletId);
	//		foreach (Transaction t in trans)
	//		{
	//			if (true == t.IsPositive)
	//				amRemain += t.Amount;
	//			else
	//				amRemain -= t.Amount;
	//		}
	//		display += ", " + amRemain.ToString("c2");
	//		display += "/" + currItem.Amount.ToString("0.00");
	//		// display = "Wallet: MM/DD/YY-MM/DD/YY, $AmountRemaining/TotalAmount"

	//		// Now set the View and return it
	//		View currView = convertView;
	//		if (null == currView)
	//			currView = context.LayoutInflater.Inflate(global::Android.Resource.Layout.SimpleListItemSingleChoice, null);
	//		currView.FindViewById<TextView>(global::Android.Resource.Id.Text1).Text = display;

	//		return currView;
	//	}
	//}

	public class MultiWalletAdapter : BaseAdapter<Wallet>
	{
		// Fields
		Activity context;
		//List<Wallet> items;

		// Ctor
		public MultiWalletAdapter(Activity c) : base()
		{
			context = c;
			//items = w;
		}

		// Adapter Overrides
		public override long GetItemId(int position)
		{
			return position;
		}
		public override Wallet this[int position]
		{
			get { return Global.gWallets[position]; }
		}
		public override int Count
		{
			get { return Global.gWallets.Count; }
		}
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			Wallet currItem = Global.gWallets[position];
			string display = currItem.Name;

			// Now set the View and return it
			View currView = convertView;
			if (null == currView)
				currView = context.LayoutInflater.Inflate(global::Android.Resource.Layout.SimpleListItemChecked, null);
			currView.FindViewById<TextView>(global::Android.Resource.Id.Text1).Text = display;

			return currView;
		}
	}

	public class MultiStoreAdapter : BaseAdapter<Store>
	{
		// Fields
		Activity context;

		// Ctor
		public MultiStoreAdapter(Activity c) : base()
		{
			context = c;
		}

		// Adapter Overrides
		public override long GetItemId(int position)
		{
			return position;
		}
		public override Store this[int position]
		{
			get { return Global.gStores[position]; }
		}
		public override int Count
		{
			get { return Global.gStores.Count; }
		}
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			Store currItem = Global.gStores[position];
			string display = currItem.Name;

			// Now set the View and return it
			View currView = convertView;
			if (null == currView)
				currView = context.LayoutInflater.Inflate(global::Android.Resource.Layout.SimpleListItemChecked, null);
			currView.FindViewById<TextView>(global::Android.Resource.Id.Text1).Text = display;

			return currView;
		}
	}
}