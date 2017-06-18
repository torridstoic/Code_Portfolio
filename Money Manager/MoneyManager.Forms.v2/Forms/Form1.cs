using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MoneyManager.Data;

namespace MoneyManager.Forms.v2
{
	public partial class Form1 : Form
	{
		//public event EventHandler<MenuEvents> events;
		//EventType type;

		List<Button> buttonList;
		UserControl activeControl;
		ToolTip tt;

		///////////////////
		// Form Init
		public Form1()
		{
			InitializeComponent();
			Global.db = new SQLiteDatabase();
			//type = EventType.None;

			// Set Variables
			buttonList = new List<Button>();
			buttonList.Add(walletsButton);
			buttonList.Add(budgetsButton);
			buttonList.Add(paymentButton);
			buttonList.Add(creditsButton);
			//buttonList.Add(billPaymentsButton);
			buttonList.Add(savingsButton);
			buttonList.Add(reportsButton);
			buttonList.Add(graphButton);
			//buttonList.Add(preferencesButton);
			buttonList.Add(manageAcctButton);
			buttonList.Add(logoutButton);
			buttonList.Add(startScreenButton);

			// Set Initial Values
			tt = new ToolTip();
			tt.SetToolTip(walletsButton, "Create categories for your spending.");
			tt.SetToolTip(budgetsButton, "Manage your spending limits.");
			tt.SetToolTip(paymentButton, "Enter and track your payments.");
			tt.SetToolTip(creditsButton, "Enter and track your income.");
			//tt.SetToolTip(billPaymentsButton, "Coming soon.");
			tt.SetToolTip(savingsButton, "Coming soon.");
			tt.SetToolTip(reportsButton, "View your account reports.");
			//tt.SetToolTip(preferencesButton, "Coming soon.");
			tt.SetToolTip(manageAcctButton, "Change your account information.");

			//Disable the buttons
			foreach (Button button in buttonList)
			{
				button.Enabled = false;
			}

			welcomeLabel.Text = "Welcome,\nGuest";
			activeControl = new Login();
			((Login)activeControl).UserLogin += OnUserLogin;
			((Login)activeControl).SetFocus();
			activeControl.Dock = DockStyle.Fill;
			splitContainer1.Panel2.Controls.Add(activeControl);
		}

		private void OnUserLogin(Object sender, MenuEvents e)
		{
			if (e.getEventType() == EventType.Login || e.getEventType() == EventType.NewUser)
			{
				welcomeLabel.Text = "Welcome back,\n" + Global.user.Username + ".";

				// Check Recurring data for need to create new data (Budgets/Transactions/etc.)
				List<Wallet> wallets = Global.db.GetWallets(Global.user);

				List<RecurringTransaction> rt = new List<RecurringTransaction>();
				foreach (Wallet w in wallets)
					rt.AddRange(Global.db.GetRecurringTransactions(Global.user, w, TransactionType.Types.Unknown));
				foreach (RecurringTransaction r in rt)
				{
					// Check RecurringTransactions for need to create new Transactions
					DateTime process = Global.ConvertTimeStampToDateTime(r.ProcessDate);
					while (process < DateTime.Now)
					{
						// Create a new Transaction and upload it.
						Transaction t = new Transaction(r.WalletId);
						t.StoreId = r.StoreId;
						t.Amount = r.Amount;
						t.Created = r.ProcessDate;
                        t.TransactionTypeId = r.TransactionTypeId;
						if (!Global.db.CreateRecord(t))
							MessageBox.Show("Error creating a transaction from your recurring template.", "Error", MessageBoxButtons.OK);
						else
						{
							// Update the Recurring Transaction's process date
							switch (r.ProcessPeriod)
							{
								case 0: // Daily
									process = process.AddDays(1);
									break;
								case 1: // Weekly
									process = process.AddDays(7);
									break;
								case 2: // Monthly
									process = process.AddMonths(1);
									break;
								case 3: // Quarterly
									process = process.AddMonths(3);
									break;
								case 4: // Yearly
									process = process.AddYears(1);
									break;
							}
						}
					}

					// Update the Recurring Transaction in the DB
					if (process > Global.ConvertTimeStampToDateTime(r.ProcessDate))
					{
						r.ProcessDate = Global.ConvertToUnixTimeStamp(process);
						if (!Global.db.UpdateRecord(r))
							MessageBox.Show("Error updating your Recurring Transaction data.", "Error", MessageBoxButtons.OK);
					}
				}

				List<RecurringBudget> rb = new List<RecurringBudget>();
				foreach (Wallet w in wallets)
					rb.AddRange(Global.db.GetRecurringBudgets(w.Id));
				foreach (RecurringBudget r in rb)
				{
					// Check RecurringBudgets for need to create new Budgets
					DateTime currEnd = Global.ConvertTimeStampToDateTime(r.CurrentEndDate);
					while (currEnd < DateTime.Now)
					{
						DateTime currStart = (currEnd.Date).AddDays(1);

						switch (r.Period)
						{
							case 0: // Monthly Budget
								currEnd = (currStart.AddMonths(1)).Subtract(new TimeSpan(0, 0, 1));
								break;
							case 1: // Quarterly Budget
								currEnd = (currStart.AddMonths(3)).Subtract(new TimeSpan(0, 0, 1));
								break;
							case 2: // Yearly Budget
								currEnd = (currStart.AddYears(1)).Subtract(new TimeSpan(0, 0, 1));
								break;
						}

						// Update both budgets, and add new Budget to DB
						Budget b = new Budget(r.WalletId, r.Amount);
						b.StartDate = r.CurrentStartDate = Global.ConvertToUnixTimeStamp(currStart);
						b.EndDate = r.CurrentEndDate = Global.ConvertToUnixTimeStamp(currEnd);
						if (!Global.db.CreateRecord(b))
							MessageBox.Show("Error creating a budget from your recurring template (time elapsed).", "Error", MessageBoxButtons.OK);
						if (!Global.db.UpdateRecord(r))
							MessageBox.Show("Error updating your Recurring Budget data.", "Error", MessageBoxButtons.OK);
					}
				}

				OnBalanceChanged(this, null);

				//Enable the buttons
				foreach (Button button in buttonList)
				{
					button.Enabled = true;
				}

				switch (e.getEventType())
				{
					case EventType.Login:
						startScreenButton.PerformClick();
						break;
					case EventType.NewUser:
						manageAcctButton.PerformClick();
						break;
				}
			}
			else if (e.getEventType() == EventType.Exit)
			{
				this.Close();
			}
		}

		///////////////////
		// Formulate the remaining balance
		public void OnBalanceChanged(Object sender, EventArgs e)
		{
			List<Transaction> transactions = Global.db.GetTransactions(Global.user, null);
			float remainingBalance = 0.00f;

			foreach (Transaction trans in transactions)
			{
                switch (trans.TransactionTypeId)
                {
                    case (int)TransactionType.Types.Credit:
                        remainingBalance += trans.Amount;
                        break;
                    case (int)TransactionType.Types.Payment:
                        remainingBalance -= trans.Amount;
                        break;
                }
			}
			budgetLabel.Text = "Total Balance: " + remainingBalance.ToString("c2");
		}

		///////////////////
		// Menu Buttons
		private void MenuButtonClick(object sender, EventArgs e)
		{
            // Clear any current active buttons
            foreach (Button b in buttonList)
            {
                b.BackColor = SystemColors.Control;
                b.ForeColor = Color.Black;
            }

            // Highlight the active button
            if (sender is Button)
            {
                (sender as Button).BackColor = SystemColors.ControlDarkDark;
                (sender as Button).ForeColor = Color.White;
            }


			splitContainer1.Panel2.Controls.Clear();

			// Get/Save the new Control
			switch (((Button)sender).Name)
			{
				case "walletsButton":
					activeControl = new Wallets();
					break;
				case "budgetsButton":
					activeControl = new Budgets();
					break;
                case "paymentButton":
                    activeControl = new Transactions(TransactionType.Types.Payment);
                    ((Transactions)activeControl).BalanceChanged += OnBalanceChanged;
                    break;
                case "creditsButton":
					activeControl = new Transactions(TransactionType.Types.Credit);
					((Transactions)activeControl).BalanceChanged += OnBalanceChanged;
					break;
                case "savingsButton":
                    activeControl = new Transactions(TransactionType.Types.Savings);
                    break;
                case "billPaymentsButton":
					activeControl = new BillPayments();
					break;
				case "reportsButton":
					activeControl = new Reports();
					break;
				case "preferencesButton":
					activeControl = new Preferences();
					break;
				case "manageAcctButton":
					activeControl = new ManageAccount();
					break;
				case "logoutButton":
					Logout();
					activeControl = new Login();
					((Login)activeControl).UserLogin += OnUserLogin;
					logoutButton.BackColor = SystemColors.Control;
					break;
				case "graphButton":
					activeControl = new Graphs();
					break;
				case "startScreenButton":
					activeControl = new StartScreen();
					break;
			}

			// Display the new Control
			if (activeControl != null)
			{
				activeControl.Dock = DockStyle.Fill;
				splitContainer1.Panel2.Controls.Add(activeControl);
			}
		}

		private void Logout()
		{
			budgetLabel.Text = "";
			welcomeLabel.Text = "Successful Logout";
			Global.user = null;

			//Disable the buttons
			foreach (Button button in buttonList)
			{
				button.Enabled = false;
			}
		}
	}
}
