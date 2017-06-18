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
	public partial class RecurringTransactions : UserControl
	{
		List<RecurringTransaction> rtrans;
		List<Wallet> wallets;
        TransactionType.Types transactionType;

		int gridRow;

		///////////////////
		// Form Init
		public RecurringTransactions(TransactionType.Types type)
		{
			InitializeComponent();
            transactionType = type;

            // Replace the titles with the correct name
            titleLabel.Text = titleLabel.Text.Replace("[Title]", transactionType.ToString());
            nonrecurringButton.Text = nonrecurringButton.Text.Replace("[Title]", transactionType.ToString());
            recurringGroup.Text = recurringGroup.Text.Replace("[Title]", transactionType.ToString());
            addButton.Text = addButton.Text.Replace("[Title]", transactionType.ToString());

			// Set Initial Values
			ResetGrid();
		}

		///////////////////
		// Payments View Button
		private void paymentsButton_Click(object sender, EventArgs e)
		{
			this.Controls.Clear();
			this.Controls.Add(new Transactions(transactionType));
        }

		///////////////////
		// Grid Selection Changed
		private void gridClicked(object sender, EventArgs e)
		{
			if (gridRow < 0 || recurringGrid.CurrentRow == null)
				gridRow = 0;
			else
				gridRow = recurringGrid.CurrentRow.Index;
		}

		///////////////////
		// Edit Payment Button
		private void editButton_Click(object sender, EventArgs e)
		{
			// Validation Check
			if (gridRow < 0 || gridRow >= rtrans.Count)
			{
				MessageBox.Show("You must select a Payment.", "Warning", MessageBoxButtons.OK);
				return;
			}

			// Display EditTransaction dialog
			EditTransaction dlg = new EditTransaction(transactionType, rtrans[gridRow]);
			if (DialogResult.OK == dlg.ShowDialog())
				ResetGrid();
		}
		///////////////////
		// Delete Payment Button
		private void deleteButton_Click(object sender, EventArgs e)
		{
			// Validation Check
			if (gridRow < 0 || gridRow >= rtrans.Count)
			{
				MessageBox.Show("You must select a Payment.", "Warning", MessageBoxButtons.OK);
				return;
			}

			// Delete selected Transaction
			if (Global.db.Delete(rtrans[gridRow]))
				ResetGrid();
		}
		///////////////////
		// Add Payment Button
		private void addButton_Click(object sender, EventArgs e)
		{
			EditTransaction dlg = new EditTransaction(transactionType);
			if (DialogResult.OK == dlg.ShowDialog())
				ResetGrid();
		}

		///////////////////
		// Helper Functions
		private void ResetGrid()
		{
			// Clear the grid
			recurringGrid.Columns.Clear();
			recurringGrid.Rows.Clear();

			// Set the Header Text
			recurringGrid.Columns.Add("Period", "Payment Period");  // col 0
			recurringGrid.Columns.Add("Date", "Next Payment");      // col 1
			recurringGrid.Columns.Add("Subject", "Subject");        // col 2
			recurringGrid.Columns.Add("Amount", "Amount");          // col 3
			recurringGrid.Columns.Add("Wallet", "Wallet");          // col 4
            for (int i = 0; i < recurringGrid.ColumnCount; ++i)
            {
                recurringGrid.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

			// Populate the lists
			wallets = Global.db.GetWallets(Global.user);
			rtrans = new List<RecurringTransaction>();
			foreach (Wallet w in wallets)
			{
				List<RecurringTransaction> tmp = Global.db.GetRecurringTransactions(Global.user, w, transactionType);
                foreach (RecurringTransaction rt in tmp)
                {
                    rtrans.Add(rt);
                }
			}

			// Populate the Grid
			for (int i = 0; i < rtrans.Count; ++i)
			{
				recurringGrid.Rows.Add();

				// col 0: Budget Period
				switch (rtrans[i].ProcessPeriod)
				{
					case 0: // daily
						recurringGrid.Rows[i].Cells[0].Value = "Daily";
						break;
					case 1: // weekly
						recurringGrid.Rows[i].Cells[0].Value = "Weekly";
						break;
					case 2: // monthly
						recurringGrid.Rows[i].Cells[0].Value = "Monthly";
						break;
					case 3: // quarterly
						recurringGrid.Rows[i].Cells[0].Value = "Quarterly";
						break;
					case 4: // yearly
						recurringGrid.Rows[i].Cells[0].Value = "Yearly";
						break;
				}
				// col 1: Next Payment Date
				recurringGrid.Rows[i].Cells[1].Value = Global.ConvertTimeStampToDateTime(rtrans[i].ProcessDate).ToString("d");
				// col 2: Subject/Store Name
				recurringGrid.Rows[i].Cells[2].Value = Global.db.GetStore(rtrans[i].StoreId).Name;
				// col 3: Amount
				recurringGrid.Rows[i].Cells[3].Value = rtrans[i].Amount.ToString("c2");
				// col 4: Wallet Name
				foreach (Wallet w in wallets)
					if (w.Id == rtrans[i].WalletId)
					{
						recurringGrid.Rows[i].Cells[4].Value = w.Name;
						break;
					}
			}

			// Reset the index
			if (gridRow < recurringGrid.Rows.Count)
				recurringGrid.Rows[gridRow].Selected = true;
		}
	}
}
