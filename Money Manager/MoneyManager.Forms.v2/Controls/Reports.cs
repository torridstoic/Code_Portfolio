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
	public partial class Reports : UserControl
	{
		List<Wallet> wallets;
		List<Transaction> transactions;
		DateTime sdate;
		DateTime edate;

		///////////////////
		// Form Init
		public Reports()
		{
			InitializeComponent();

			// Set Variables
			wallets = Global.db.GetWallets(Global.user);
			transactions = new List<Transaction>();
			foreach (Wallet w in wallets)
				transactions.AddRange(Global.db.GetTransactions(Global.user, w));
			sdate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1); //sdate = first of month
			edate = DateTime.Now; //edate = now

			// Set Initial Values
			monthPicker.Value = DateTime.Now;
			startDatePicker.Value = sdate;
			endDatePicker.Value = edate;
			datesGroup.Visible = false;
			monthGroup.Visible = true;
			ResetGrid();
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
		// Helper Functions
		private void ResetGrid()
		{
			// Clear the grid
			walletGrid.Columns.Clear();
			walletGrid.Rows.Clear();

			// Set Header Text
			walletGrid.Columns.Add("Name", "Wallet Name");	// col 0
			walletGrid.Columns.Add("Amount", "Balance");	// col 1
			for (int i = 0; i < walletGrid.ColumnCount; ++i)
				walletGrid.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

			// Populate the Grid
			for(int i=0; i<wallets.Count; ++i)
			{
				walletGrid.Rows.Add();
				walletGrid.Rows[i].Cells[0].Value = wallets[i].Name;

				float sum = 0.0f;
				foreach (Transaction t in transactions)
					if (t.WalletId == wallets[i].Id && Global.ConvertTimeStampToDateTime(t.Created) >= sdate && Global.ConvertTimeStampToDateTime(t.Created) <= edate)
					{
                        switch (t.TransactionTypeId)
                        {
                            case (int)TransactionType.Types.Credit:
                                sum += t.Amount;
                                break;
                            case (int)TransactionType.Types.Payment:
                                sum -= t.Amount;
                                break;
                        }
                    }
				walletGrid.Rows[i].Cells[1].Value = sum.ToString("c2");
			}
		}
	}
}
