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
	public class EditTransactionFragment : DialogFragment
	{
		Transaction currTransaction;
		RecurringTransaction currRecur;
		bool isRecurring;
		bool isCredit;
		DateTime date;
		Action Refresh = delegate { };

		Button dateButton;
		EditText subjectText;
		EditText amountText;
		Spinner freqSpinner;
		Spinner walletSpinner;

		public static EditTransactionFragment NewInstance(int tId, bool isRecur, bool isPos, Action response)
		{
			#region Load Parameters
			// Load Parameters
			EditTransactionFragment frag = new EditTransactionFragment();
			frag.currTransaction = null;
			frag.currRecur = null;
			frag.isRecurring = isRecur;
			frag.isCredit = isPos;
			frag.Refresh = response;
			frag.date = DateTime.Today;

			if (tId >= 0)
			{
				if (!isRecur)
				{
					foreach (Transaction t in Global.gTransactions)
						if (t.Id == tId)
						{   // Fetch current/active Transaction
							frag.currTransaction = t;
							frag.date = Global.ConvertTimeStampToDateTime(t.Created);
							break;
						}
				}
				else // isRecurring
				{
					foreach (RecurringTransaction rt in Global.gRecurTransactions)
						if (rt.Id == tId)
						{   // Fetch current/active RTransaction
							frag.currRecur = rt;
							frag.date = Global.ConvertTimeStampToDateTime(rt.ProcessDate);
							break;
						}
				}
			}
			#endregion

			return frag;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			#region Load the Custom View
			// Load the Custom View
			View currView = inflater.Inflate(Resource.Layout.EditTransaction, container, false);
			// Set up the Date Button, Amount Text, and Spinners
			dateButton = currView.FindViewById<Button>(Resource.Id.edit_transaction_date);
			dateButton.Text = date.ToString("d");
			subjectText = currView.FindViewById<EditText>(Resource.Id.edit_transaction_subject);
			amountText = currView.FindViewById<EditText>(Resource.Id.edit_transaction_amount);
			freqSpinner = currView.FindViewById<Spinner>(Resource.Id.edit_transaction_freq);
			ArrayAdapter<string> freqAdapter = new ArrayAdapter<string>(Activity, global::Android.Resource.Layout.SimpleSpinnerItem, new string[] { "Never", "Daily", "Weekly", "Monthly", "Quarterly", "Yearly" });
			freqAdapter.SetDropDownViewResource(global::Android.Resource.Layout.SimpleSpinnerDropDownItem);
			freqSpinner.Adapter = freqAdapter;
			walletSpinner = currView.FindViewById<Spinner>(Resource.Id.edit_transaction_spinner);
			ArrayAdapter<Wallet> walletAdapter = new ArrayAdapter<Wallet>(Activity, global::Android.Resource.Layout.SimpleSpinnerItem, Global.gWallets);
			walletAdapter.SetDropDownViewResource(global::Android.Resource.Layout.SimpleSpinnerDropDownItem);
			walletSpinner.Adapter = walletAdapter;
			#endregion

			#region Load + Display Preset Data
			// Load + Display Preset Data
			if (null == currTransaction && null == currRecur) // New Transaction
			{
				currView.FindViewById<TextView>(Resource.Id.edit_transaction_title).Text = "New Transaction";

				// TEMPORARY : DISABLE RECURRING TRANSACTIONS
				//freqSpinner.SetSelection(0);
				//freqSpinner.Enabled = false;
			}
			else
			{
				currView.FindViewById<TextView>(Resource.Id.edit_transaction_title).Text = "Edit Transaction";

				if (isRecurring == false) // Single Transaction
				{
					// Lock the Frequency Spinner and Set the Date display
					freqSpinner.SetSelection(0);
					freqSpinner.Enabled = false;
					foreach (Store s in Global.gStores)
						if (s.Id == currTransaction.StoreId)
						{   // Set the Subject/Store field
							subjectText.Text = s.Name;
							break;
						}
					// Set Amount display
					amountText.Text = currTransaction.Amount.ToString();
					for (int i = 0; i < Global.gWallets.Count; ++i)
						if (Global.gWallets[i].Id == currTransaction.WalletId)
						{   // Set the Wallet spinner
							walletSpinner.SetSelection(i);
							break;
						}
				}
				else // Recurring Transaction
				{
					// Set the Frequency Spinner and Date display
					freqSpinner.SetSelection(currRecur.ProcessPeriod + 1);
					foreach (Store s in Global.gStores)
						if (s.Id == currRecur.StoreId)
						{   // Set the Subject/Store field
							subjectText.Text = s.Name;
							break;
						}
					// Set Amount display
					amountText.Text = currRecur.Amount.ToString();
					for (int i = 0; i < Global.gWallets.Count; ++i)
						if (Global.gWallets[i].Id == currRecur.WalletId)
						{   // Set the Wallet spinner
							walletSpinner.SetSelection(i);
							break;
						}
				}
			}
			#endregion

			#region Button Functions
			// Button Functions
			dateButton.Click += delegate {
				DatePickerFragment dpf = DatePickerFragment.NewInstance(date, delegate (DateTime time) {
					dateButton.Text = time.ToString("d");
					date = time;
				});
				dpf.Show(FragmentManager, DatePickerFragment.TAG);
			};

			currView.FindViewById<Button>(Resource.Id.edit_transaction_cancel).Click += delegate {
				Dismiss();
				Toast.MakeText(Activity, "Cancelled", ToastLength.Short).Show();
			};
			currView.FindViewById<Button>(Resource.Id.edit_transaction_ok).Click += delegate {
				// Validation Check
				if (subjectText.Text == String.Empty || amountText.Text == String.Empty || walletSpinner.Adapter.Count == 0)
				{
					Toast.MakeText(Activity, "Please fill all required fields.", ToastLength.Short).Show();
					return;
				}
				if (Convert.ToDouble(amountText.Text) <= 0)
				{
					Toast.MakeText(Activity, "Amount must be > $0.00", ToastLength.Short).Show();
					return;
				}

				// Fetch the chosen Subject, or create a new one
				Store store = null;
				foreach (Store s in Global.gStores)
					if (s.Name == subjectText.Text)
					{
						store = s;
						break;
					}
				if (null == store)
				{
					store = new Store(subjectText.Text, Global.GetUniqueId());
					Global.gStores.Add(store);
				}

				bool newTransaction = true;

				if (freqSpinner.SelectedItemPosition == 0) // Single Transaction
				{
					if (null == currTransaction)
						currTransaction = new Transaction(Global.gWallets[walletSpinner.SelectedItemPosition].Id, Global.GetUniqueId());
					else
					{
						newTransaction = false;
						currTransaction.WalletId = Global.gWallets[walletSpinner.SelectedItemPosition].Id;
					}
					currTransaction.Created = Global.ConvertToUnixTimeStamp(date);
					currTransaction.StoreId = store.Id;
					currTransaction.Amount = (float)Convert.ToDouble(amountText.Text);
					currTransaction.IsPositive = isCredit;

					// Upload the Transaction, and exit
					if (newTransaction)
					{
						Global.gTransactions.Add(currTransaction);
						Refresh();
						Dismiss();
						Toast.MakeText(Activity, "Transaction Added!", ToastLength.Short).Show();
					}
					else // editing
					{
						for (int i = 0; i < Global.gTransactions.Count; ++i)
							if (Global.gTransactions[i].Id == currTransaction.Id)
							{
								Global.gTransactions[i] = currTransaction;
								Refresh();
								Dismiss();
								Toast.MakeText(Activity, "Updated!", ToastLength.Short).Show();
								break;
							}
					}
				}
				else // Recurring Transaction
				{
					// Save RTransaction data
					if (null == currRecur)
						currRecur = new RecurringTransaction(Global.gWallets[walletSpinner.SelectedItemPosition].Id, Global.GetUniqueId());
					else
					{
						newTransaction = false;
						currRecur.WalletId = Global.gWallets[walletSpinner.SelectedItemPosition].Id;
					}
					currRecur.ProcessPeriod = freqSpinner.SelectedItemPosition - 1;
					currRecur.StoreId = store.Id;
					currRecur.Amount = (float)Convert.ToDouble(amountText.Text);
					currRecur.IsPositive = isCredit;

					// If prior date selected, create prior Transaction(s)
					DateTime paydate = date;
					while (paydate < DateTime.Now)
					{
						Transaction t = new Transaction(currRecur.WalletId, Global.GetUniqueId());
						t.Created = Global.ConvertToUnixTimeStamp(paydate);
						t.StoreId = currRecur.StoreId;
						t.Amount = currRecur.Amount;
						t.IsPositive = currRecur.IsPositive;
						t.WalletId = currRecur.WalletId;

						// Increment the paydate
						switch (currRecur.ProcessPeriod)
						{
							case 0: // daily
								paydate = paydate.AddDays(1);
								break;
							case 1: // weekly
								paydate = paydate.AddDays(7);
								break;
							case 2: // monthly
								paydate = paydate.AddMonths(1);
								break;
							case 3: // quarterly
								paydate = paydate.AddMonths(3);
								break;
							case 4: // yearly
								paydate = paydate.AddYears(1);
								break;
						}

						// Upload the Transaction
						Global.gTransactions.Add(t);
					}

					// Upload the Recurring Transaction, and exit
					currRecur.ProcessDate = Global.ConvertToUnixTimeStamp(paydate);
					if (newTransaction)
					{
						Global.gRecurTransactions.Add(currRecur);
						Refresh();
						Dismiss();
						Toast.MakeText(Activity, "Recurring Transaction Added!", ToastLength.Short).Show();
					}
					else // editing
					{
						for (int i = 0; i < Global.gRecurTransactions.Count; ++i)
							if (Global.gRecurTransactions[i].Id == currRecur.Id)
							{
								Global.gRecurTransactions[i] = currRecur;
								Refresh();
								Dismiss();
								Toast.MakeText(Activity, "Updated!", ToastLength.Short).Show();
								break;
							}
					}
				}
			};
			#endregion

			return currView;
		}
	}
}