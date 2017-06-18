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
	public partial class Budgets : UserControl
	{
		List<Budget> budgets;
		List<Wallet> wallets;

		int gridRow;

		///////////////////
		// Form Init
		public Budgets()
		{
			InitializeComponent();

			// Set Initial Values
			ResetGrid();
		}

		///////////////////
		// Radio Button Changed
		private void radioChange(object sender, EventArgs e)
		{
			ResetGrid();
		}

		///////////////////
		// Recurring Budget View Button
		private void recurringButton_Click(object sender, EventArgs e)
		{
			this.Controls.Clear();
			this.Controls.Add(new RecurringBudgets());
		}

		///////////////////
		// Grid Selection Changed
		private void gridClicked(object sender, EventArgs e)
		{
			if (gridRow < 0 || budgetGrid.CurrentRow == null)
				gridRow = 0;
			else
				gridRow = budgetGrid.CurrentRow.Index;
		}

		///////////////////
		// Edit Budget Button
		private void editButton_Click(object sender, EventArgs e)
		{
			// Validation Check
			if (gridRow < 0 || gridRow >= budgets.Count)
			{
				MessageBox.Show("You must select a Budget.", "Warning", MessageBoxButtons.OK);
				return;
			}

			// Display EditBudget dialog
			EditBudget dlg = new EditBudget(budgets[gridRow]);
			if (DialogResult.OK == dlg.ShowDialog())
				ResetGrid();
		}
		///////////////////
		// Delete Budget Button
		private void deleteButton_Click(object sender, EventArgs e)
		{
			// Validation Check
			if (gridRow < 0 || gridRow >= budgets.Count)
			{
				MessageBox.Show("You must select a Budget.", "Warning", MessageBoxButtons.OK);
				return;
			}

			// Delete selected Budget
			if (Global.db.Delete(budgets[gridRow]))
				ResetGrid();
		}
		///////////////////
		// Add Budget Button
		private void newButton_Click(object sender, EventArgs e)
		{
			EditBudget dlg = new EditBudget();
			if (DialogResult.OK == dlg.ShowDialog())
				ResetGrid();
		}

		///////////////////
		// Helper Functions
		private void ResetGrid()
		{
			// Clear the grid
			budgetGrid.Columns.Clear();
			budgetGrid.Rows.Clear();

			// Set the Header Text
			budgetGrid.Columns.Add("Wallet", "Wallet");             // col 0
			budgetGrid.Columns.Add("StartDate", "Start Date");      // col 1
			budgetGrid.Columns.Add("EndDate", "End Date");          // col 2
			budgetGrid.Columns.Add("AmountRemaining", "Remaining"); // col 3
			budgetGrid.Columns.Add("PercentUsed", "Percent Used");  // col 4
			for (int i = 0; i < budgetGrid.ColumnCount; ++i)
				budgetGrid.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

			// Populate the lists
			wallets = Global.db.GetWallets(Global.user);
			budgets = new List<Budget>();
			foreach (Wallet w in wallets)
			{
				// Save any Budgets that apply to current Radio selection
				List<Budget> tmp = Global.db.GetBudgets(Global.user, w);
				foreach (Budget b in tmp)
				{
					if (previousRadio.Checked) // Previous Budgets
					{
						if (Global.ConvertTimeStampToDateTime(b.EndDate) < DateTime.Now)
							budgets.Add(b);
					}
					else if (futureRadio.Checked) // Future Budgets
					{
						if (Global.ConvertTimeStampToDateTime(b.StartDate) > DateTime.Now)
							budgets.Add(b);
					}
					else // Current Budgets
					{
						if (Global.ConvertTimeStampToDateTime(b.StartDate) < DateTime.Now && Global.ConvertTimeStampToDateTime(b.EndDate) > DateTime.Now)
							budgets.Add(b);
					}
				}
			}

			// Populate the Grid
			for (int i = 0; i < budgets.Count; ++i)
			{
				budgetGrid.Rows.Add();
				foreach (Wallet w in wallets)
					if (w.Id == budgets[i].WalletId)
					{
						// col 0: Wallet Name
						budgetGrid.Rows[i].Cells[0].Value = w.Name;
						break;
					}
				// col 1&2: Start Date & End Date
				budgetGrid.Rows[i].Cells[1].Value = Global.ConvertTimeStampToDateTime(budgets[i].StartDate).ToString("d");
				budgetGrid.Rows[i].Cells[2].Value = Global.ConvertTimeStampToDateTime(budgets[i].EndDate).ToString("d");

				// Calculate Remaining Budget
				float amRemain = budgets[i].Amount;
				List<Transaction> trans = Global.db.GetTransactions(Global.user, Global.db.GetWallet(Global.user, budgets[i].WalletId));
				foreach (Transaction t in trans)
				{
					if (t.Created >= budgets[i].StartDate && t.Created <= budgets[i].EndDate)
					{
                        switch(t.TransactionTypeId)
                        {
                            case (int)TransactionType.Types.Credit:
                                amRemain += t.Amount;
                                break;
                            case (int)TransactionType.Types.Payment:
                                amRemain -= t.Amount;
                                break;
                        }
					}
				}

				// col 3&4: Remaining Budget and Percent Used
				budgetGrid.Rows[i].Cells[3].Value = "$" + amRemain.ToString("0.00");
				budgetGrid.Rows[i].Cells[4].Value = (100.0f - (100.0f * amRemain / budgets[i].Amount)).ToString("0.00") + "%";
			}

			// Reset the index
			if (gridRow < budgetGrid.Rows.Count)
				budgetGrid.Rows[gridRow].Selected = true;
		}
	}
}
