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
	public partial class Wallets : UserControl
	{
        private int gridRow;
		private List<Wallet> wallets;

		///////////////////
		// Form Init
		public Wallets()
		{
			InitializeComponent();
			Reset();
            gridRow = -1;
        }

        ///////////////////
        // Edit Wallet Button
        private void editButton_Click(object sender, EventArgs e)
        {
            // Validation Check
            if (gridRow == -1)
            {
                MessageBox.Show("You must have a Wallet selected.", "Warning", MessageBoxButtons.OK);
                return;
            }

            // Load an EditWallet dialog
            EditWallet dlg = new EditWallet(wallets[gridRow]);

            //if (DialogResult.OK == dlg.ShowDialog())
            switch (dlg.ShowDialog()) {
                case DialogResult.OK:
                case DialogResult.Cancel:
                    Reset();
                    break;
            }
		}
		///////////////////
		// Delete Wallet Button
		private void deleteButton_Click(object sender, EventArgs e)
		{
			// Validation Check
			if (gridRow == -1)
			{
				MessageBox.Show("You must have a Wallet selected.", "Warning", MessageBoxButtons.OK);
				return;
			}

			// Delete Selected Wallet
			if (Global.db.Delete(wallets[gridRow]))
				Reset();
		}
		///////////////////
		// New Wallet Button
		private void newButton_Click(object sender, EventArgs e)
		{
			// Load an Empty EditWallet dialog
			EditWallet dlg = new EditWallet();
			if (DialogResult.OK == dlg.ShowDialog())
				Reset();
		}

		///////////////////
		// Helper Functions
		private void Reset()
		{
            //Load all the savings
            wallets = Global.db.GetWallets(Global.user);

            //Setup the Savings Data Grid

            //Clear the grid
            walletDataGrid.Columns.Clear();
            walletDataGrid.Rows.Clear();

            // Set the Header Text
            walletDataGrid.Columns.Add("Name", "Name");
            walletDataGrid.Columns.Add("Type", "Type");

            for (int i = 0; i < wallets.Count; i++)
            {
                String typeName = "";

                //Create the row
                walletDataGrid.Rows.Add();

                //Add the data
                walletDataGrid.Rows[i].Cells[0].Value = wallets[i].Name;
                switch (wallets[i].WalletTypeId)
                {
                    case (int)WalletType.Types.Default:
                        typeName = "Default";
                        break;
                    case (int)WalletType.Types.Savings:
                        typeName = "Savings";
                        break;
                }

                walletDataGrid.Rows[i].Cells[1].Value = typeName;
            }

            //walletDataGrid.Rows[gridRow].Selected = true;
		}

        private void gridClicked(object sender, EventArgs e)
        {
            if (gridRow < 0 || walletDataGrid.CurrentRow == null)
                gridRow = 0;
            else
                gridRow = walletDataGrid.CurrentRow.Index;
        }
    }
}
