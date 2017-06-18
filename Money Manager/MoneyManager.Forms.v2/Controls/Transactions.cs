using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MoneyManager.Data;

namespace MoneyManager.Forms.v2
{
	public partial class Transactions : UserControl
	{
		List<Transaction> transactions;
		List<Wallet> wallets;
        TransactionType.Types transactionType;

		int gridRow;
		DateTime sdate;
		DateTime edate;

        //Events
        public event EventHandler BalanceChanged;

        ///////////////////
        // Form Init
        public Transactions(TransactionType.Types transactionType)
		{
			InitializeComponent();
            this.transactionType = transactionType;

			// Set Variables
			sdate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1); //sdate = first of month
			edate = (sdate.AddMonths(1)).Subtract(new TimeSpan(0, 0, 1)); //edate = end of month

			// Set Initial Values
			monthPicker.Value = DateTime.Now;
			startDatePicker.Value = sdate;
			endDatePicker.Value = edate;
			datesGroup.Visible = false;
			monthGroup.Visible = true;
			ResetGrid();

            //Display the correct titles on the form.
            titleLabel.Text = titleLabel.Text.Replace("[Title]", transactionType.ToString());
            editButton.Text = editButton.Text.Replace("[Title]", transactionType.ToString());
            deleteButton.Text = deleteButton.Text.Replace("[Title]", transactionType.ToString());
            addButton.Text = addButton.Text.Replace("[Title]", transactionType.ToString());
            recurringButton.Text = recurringButton.Text.Replace("[Title]", transactionType.ToString());
            transactionGroup.Text = transactionGroup.Text.Replace("[Title]", transactionType.ToString());
        }

        private void ResetGrid()
		{
			// Clear the grid
			dataGridData.Columns.Clear();
			dataGridData.Rows.Clear();

			// Set the Header Text
			dataGridData.Columns.Add("Date", "Date");        // col 0
			dataGridData.Columns.Add("Subject", "Subject");  // col 1
			dataGridData.Columns.Add("Amount", "Amount");    // col 2
			dataGridData.Columns.Add("Wallet", "Wallet");    // col 3
			for (int i = 0; i < dataGridData.ColumnCount; ++i)
				dataGridData.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

			// Populate the lists
			wallets = Global.db.GetWallets(Global.user);
			transactions = new List<Transaction>();
			foreach (Wallet w in wallets)
			{
				List<Transaction> tmp = Global.db.GetTransactions(Global.user, w, transactionType);
                foreach (Transaction t in tmp)
                {
                    if (Global.ConvertTimeStampToDateTime(t.Created) >= sdate && Global.ConvertTimeStampToDateTime(t.Created) <= edate)
                    {
                        transactions.Add(t);
                    }
                }
			}

			// Populate the Grid
			for (int i = 0; i < transactions.Count; ++i)
			{
				dataGridData.Rows.Add();
				dataGridData.Rows[i].Cells[0].Value = Global.ConvertTimeStampToDateTime(transactions[i].Created).ToString("d");
				dataGridData.Rows[i].Cells[1].Value = Global.db.GetStore(transactions[i].StoreId).Name;
				dataGridData.Rows[i].Cells[2].Value = transactions[i].Amount.ToString("c2");
				foreach (Wallet w in wallets)
					if (w.Id == transactions[i].WalletId)
					{
						dataGridData.Rows[i].Cells[3].Value = w.Name;
						break;
					}
			}

            // Reset the index
            if (gridRow < dataGridData.Rows.Count)
            {
                dataGridData.Rows[gridRow].Selected = true;
            }

            //When this is called we should just reupdate the balance.
            BalanceChanged?.Invoke(this, null);
        }

		///////////////////
		// Recurring Payments View Button
		private void recurringButton_Click(object sender, EventArgs e)
		{
			this.Controls.Clear();
            this.Controls.Add(new RecurringTransactions(transactionType));
		}

		///////////////////
		// Radio Button Changed
		private void radioChange(object sender, EventArgs e)
		{
			if (monthlyRadio.Checked == true)
			{
				datesGroup.Visible = false;
				monthGroup.Visible = true;
			}
			else // custom dates radio
			{
				monthGroup.Visible = false;
				datesGroup.Visible = true;
			}
			ResetGrid();
		}

		///////////////////
		// Viewing Dates Changed
		private void DateChanged(object sender, EventArgs e)
		{
			if (monthlyRadio.Checked == true)
			{
				sdate = new DateTime(monthPicker.Value.Year, monthPicker.Value.Month, 1);
				edate = (sdate.AddMonths(1)).Subtract(new TimeSpan(0, 0, 1));

				startDatePicker.Value = sdate;
				endDatePicker.Value = edate;
			}
			else // custom dates
			{
				sdate = startDatePicker.Value;
				edate = endDatePicker.Value + new TimeSpan(23, 59, 59);

				monthPicker.Value = sdate;
			}
			ResetGrid();
		}

		///////////////////
		// Grid Selection Changed
		private void gridClicked(object sender, EventArgs e)
		{
			if (gridRow < 0 || dataGridData.CurrentRow == null)
				gridRow = 0;
			else
				gridRow = dataGridData.CurrentRow.Index;
		}

		///////////////////
		// Edit Transaction Button
		private void editButton_Click(object sender, EventArgs e)
		{
			// Validation Check
			if (gridRow < 0 || gridRow >= transactions.Count)
			{
				MessageBox.Show("You must select a Transaction.", "Warning", MessageBoxButtons.OK);
				return;
			}

			// Display EditTransaction dialog
			EditTransaction dlg = new EditTransaction(transactionType, transactions[gridRow]);
			if (DialogResult.OK == dlg.ShowDialog())
				ResetGrid();
		}
		///////////////////
		// Delete Transaction Button
		private void deleteButton_Click(object sender, EventArgs e)
		{
			// Validation Check
			if (gridRow < 0 || gridRow >= transactions.Count)
			{
				MessageBox.Show("You must select a Transaction.", "Warning", MessageBoxButtons.OK);
				return;
			}

			// Delete selected Transaction
			if (Global.db.Delete(transactions[gridRow]))
				ResetGrid();
		}
		///////////////////
		// Add Transaction Button
		private void addButton_Click(object sender, EventArgs e)
		{
			EditTransaction dlg = new EditTransaction(transactionType);
			if (DialogResult.OK == dlg.ShowDialog())
				ResetGrid();
		}
    }
}
