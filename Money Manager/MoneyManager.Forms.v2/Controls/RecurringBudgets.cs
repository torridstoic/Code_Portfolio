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
	public partial class RecurringBudgets : UserControl
	{
		List<RecurringBudget> rbudgets;
		List<Wallet> wallets;

		int gridRow;

		///////////////////
		// Form Init
		public RecurringBudgets()
		{
			InitializeComponent();

			// Set Initial Values
			ResetGrid();
		}

		///////////////////
		// Budget View Button
		private void budgetsButton_Click(object sender, EventArgs e)
		{
			this.Controls.Clear();
			this.Controls.Add(new Budgets());
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
		// Edit Budget Button
		private void editButton_Click(object sender, EventArgs e)
		{
			// Validation Check
			if (gridRow < 0 || gridRow >= rbudgets.Count)
			{
				MessageBox.Show("You must select a Budget.", "Warning", MessageBoxButtons.OK);
				return;
			}

			// Display EditBudget dialog
			EditBudget dlg = new EditBudget(rbudgets[gridRow]);
			if (DialogResult.OK == dlg.ShowDialog())
				ResetGrid();
		}
		///////////////////
		// Delete Budget Button
		private void deleteButton_Click(object sender, EventArgs e)
		{
			// Validation Check
			if (gridRow < 0 || gridRow >= rbudgets.Count)
			{
				MessageBox.Show("You must select a Budget.", "Warning", MessageBoxButtons.OK);
				return;
			}

			// Delete selected Budget
			if (Global.db.Delete(rbudgets[gridRow]))
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
			recurringGrid.Columns.Clear();
			recurringGrid.Rows.Clear();

			// Set the Header Text
			recurringGrid.Columns.Add("Wallet", "Wallet");		 // col 0
			recurringGrid.Columns.Add("Period", "Period");		 // col 1
			recurringGrid.Columns.Add("Amount", "Total Budget"); // col 2
			for (int i = 0; i < recurringGrid.ColumnCount; ++i)
				recurringGrid.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

			// Populate the lists
			wallets = Global.db.GetWallets(Global.user);
			rbudgets = new List<RecurringBudget>();
			foreach (Wallet w in wallets)
				rbudgets.AddRange(Global.db.GetRecurringBudgets(w.Id));

			// Populate the Grid
			for (int i = 0; i < rbudgets.Count; ++i)
			{
				recurringGrid.Rows.Add();
				foreach (Wallet w in wallets)
					if (w.Id == rbudgets[i].WalletId)
					{
						// col 0: Wallet Name
						recurringGrid.Rows[i].Cells[0].Value = w.Name;
						break;
					}
				// col 1: Budget Period
				switch (rbudgets[i].Period)
				{
					case 0: // monthly
						recurringGrid.Rows[i].Cells[1].Value = "Monthly";
						break;
					case 1: // quarterly
						recurringGrid.Rows[i].Cells[1].Value = "Quarterly";
						break;
					case 2: // yearly
						recurringGrid.Rows[i].Cells[1].Value = "Yearly";
						break;
					default: // error
						recurringGrid.Rows[i].Cells[1].Value = rbudgets[i].Period;
						break;
				}
				// col 2: Total Budget Amount
				recurringGrid.Rows[i].Cells[2].Value = "$" + rbudgets[i].Amount.ToString("0.00");
			}

			// Reset the index
			if (gridRow < recurringGrid.Rows.Count)
				recurringGrid.Rows[gridRow].Selected = true;
		}
	}
}
