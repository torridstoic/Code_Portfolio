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
	public partial class EditBudget : Form
	{
		private Budget currBudget;
		private RecurringBudget currRBudget;

		List<Wallet> wallets;
		DateTime sdate;
		DateTime edate;

		///////////////////
		// Form Init (Overloaded)
		public EditBudget() // New Budget/RBudget
		{
			InitializeComponent();

			// Set Variables
			currBudget = null;
			currRBudget = null;
			wallets = Global.db.GetWallets(Global.user);
			sdate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
			edate = (sdate.AddMonths(1)).Subtract(new TimeSpan(0, 0, 1));

			// Set Max Values
			amountUD.Maximum = Decimal.MaxValue;

			// Set Initial Values
			periodGroup.Visible = false;
			datesGroup.Visible = true;
			recurStartDatePicker.Value = DateTime.Today.Date;
			startDatePicker.Value = sdate.Date;
			endDatePicker.Value = edate.Date;

			foreach (Wallet w in wallets)
				walletCombo.Items.Add(w);
		}
		public EditBudget(Budget b) // Editing a Budget
		{
			InitializeComponent();

			// Set Variables
			currBudget = b;
			currRBudget = null;
			wallets = Global.db.GetWallets(Global.user);
			sdate = Global.ConvertTimeStampToDateTime(b.StartDate);
			edate = Global.ConvertTimeStampToDateTime(b.EndDate);

			// Set Max Values
			amountUD.Maximum = Decimal.MaxValue;

			// Set Initial Values
			customRadio.Checked = true;
			recurringRadio.Enabled = false;

			periodGroup.Visible = false;
			datesGroup.Visible = true;
			startDatePicker.Value = sdate.Date;
			endDatePicker.Value = edate.Date;

			foreach (Wallet w in wallets)
				walletCombo.Items.Add(w);
			foreach (Wallet w in walletCombo.Items)
				if (w.Id == b.WalletId)
				{
					walletCombo.SelectedItem = w;
					break;
				}
			amountUD.Value = (Decimal)b.Amount;
		}
		public EditBudget(RecurringBudget rb) // Editing a Recurring Budget
		{
			InitializeComponent();

			// Set Variables
			currBudget = null;
			currRBudget = rb;
			wallets = Global.db.GetWallets(Global.user);

			// Set Max Values
			amountUD.Maximum = Decimal.MaxValue;

			// Set Initial Values
			recurringRadio.Checked = true;
			customRadio.Enabled = false;

			datesGroup.Visible = false;
			periodGroup.Visible = true;
			periodCombo.SelectedIndex = rb.Period;
			recurStartDatePicker.Value = DateTime.Today.Date;

			foreach (Wallet w in wallets)
				walletCombo.Items.Add(w);
			foreach (Wallet w in walletCombo.Items)
				if (w.Id == rb.WalletId)
				{
					walletCombo.SelectedItem = w;
					break;
				}
			amountUD.Value = (Decimal)rb.Amount;
		}

		///////////////////
		// Radio Button Changed
		private void radioChange(object sender, EventArgs e)
		{
			if (recurringRadio.Checked == true)
			{
				datesGroup.Visible = false;
				periodGroup.Visible = true;
			}
			else // customRadio.Checked
			{
				periodGroup.Visible = false;
				datesGroup.Visible = true;
			}
		}

		///////////////////
		// New Wallet Button
		private void newWalletButton_Click(object sender, EventArgs e)
		{
			// Show an empty EditWallet dialog
			EditWallet dlg = new EditWallet();
			if (DialogResult.OK == dlg.ShowDialog())
			{
				// Refresh the wallets List and ListBox
				walletCombo.Items.Clear();
				wallets = Global.db.GetWallets(Global.user);
				foreach (Wallet w in wallets)
					walletCombo.Items.Add(w);

				// Select the new Wallet
				walletCombo.SelectedIndex = wallets.Count - 1;
			}
		}

		///////////////////
		// Save Button
		private void okButton_Click(object sender, EventArgs e)
		{
			// Validation Checks
			if (walletCombo.SelectedIndex < 0)
			{
				MessageBox.Show("Please select a Wallet.", "Warning", MessageBoxButtons.OK);
				return;
			}
			if (amountUD.Value == 0)
			{
				MessageBox.Show("Please input a valid Amount.", "Warning", MessageBoxButtons.OK);
				return;
			}

			bool newBudget = true;

			// Recurring Budget
			if (recurringRadio.Checked == true)
			{
				// One more Validation
				if (periodCombo.SelectedIndex < 0)
				{
					MessageBox.Show("Please select a Recurring Period", "Warning", MessageBoxButtons.OK);
					return;
				}

				// Save RBudget data
				if (currRBudget != null)
				{
					newBudget = false;
					currRBudget.WalletId = wallets[walletCombo.SelectedIndex].Id;
					currRBudget.Amount = (float)amountUD.Value;
				}
				else // new
					currRBudget = new RecurringBudget(wallets[walletCombo.SelectedIndex].Id, (float)amountUD.Value);

				currRBudget.Period = periodCombo.SelectedIndex;
				// Set End Date before Start Date to allow budget creation on rollover for "future" budgets
				// For current/prev budgets, this will be reset
				sdate = recurStartDatePicker.Value;
				edate = sdate.Subtract(new TimeSpan(0, 0, 1));
				while (edate < DateTime.Now)
				{
					switch (currRBudget.Period)
					{
						case 0: // monthly
							edate = (sdate.AddMonths(1)).Subtract(new TimeSpan(0, 0, 1));
							break;
						case 1: // quarterly
							edate = (sdate.AddMonths(3)).Subtract(new TimeSpan(0, 0, 1));
							break;
						case 2: // yearly
							edate = (sdate.AddYears(1)).Subtract(new TimeSpan(0, 0, 1));
							break;
					}
					if (edate < DateTime.Now)
						sdate = edate.Date.AddDays(1);

					// Create and upload a Budget corresponding to the new recurring template
					Budget b = new Budget(currRBudget.WalletId, currRBudget.Amount);
					b.StartDate = Global.ConvertToUnixTimeStamp(sdate);
					b.EndDate = Global.ConvertToUnixTimeStamp(edate);
					if (!Global.db.CreateRecord(b))
					{
						MessageBox.Show("Error saving your new Budget.", "Error", MessageBoxButtons.OK);
						break;
					}
				}

				currRBudget.CurrentStartDate = Global.ConvertToUnixTimeStamp(sdate);
				currRBudget.CurrentEndDate = Global.ConvertToUnixTimeStamp(edate);

				// Upload the Recurring Budget, and exit
				if (true == newBudget && Global.db.CreateRecord(currRBudget))
					this.DialogResult = DialogResult.OK;
				else if (false == newBudget && Global.db.UpdateRecord(currRBudget))
					this.DialogResult = DialogResult.OK;
				else
					MessageBox.Show("Error saving your Recurring Budget.", "Error", MessageBoxButtons.OK);
			}
			else // Single Budget
			{
				sdate = startDatePicker.Value;
				edate = endDatePicker.Value + new TimeSpan(23, 59, 59);
				// One more Validation
				if (sdate > edate)
				{
					MessageBox.Show("Budget must begin before it ends!", "Warning", MessageBoxButtons.OK);
					return;
				}

				// Save Budget data
				if (currBudget != null)
				{
					newBudget = false;
					currBudget.WalletId = wallets[walletCombo.SelectedIndex].Id;
					currBudget.Amount = (float)amountUD.Value;
				}
				else // new
					currBudget = new Budget(wallets[walletCombo.SelectedIndex].Id, (float)amountUD.Value);

				currBudget.StartDate = Global.ConvertToUnixTimeStamp(sdate);
				currBudget.EndDate = Global.ConvertToUnixTimeStamp(edate);

				// Upload the Budget, and exit
				if (true == newBudget && Global.db.CreateRecord(currBudget))
					this.DialogResult = DialogResult.OK;
				else if (false == newBudget && Global.db.UpdateRecord(currBudget))
					this.DialogResult = DialogResult.OK;
				else
					MessageBox.Show("Error saving your Budget.", "Error", MessageBoxButtons.OK);
			}
		}
	}
}
