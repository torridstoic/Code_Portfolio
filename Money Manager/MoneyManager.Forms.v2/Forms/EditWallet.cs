using System;
using System.Windows.Forms;

using MoneyManager.Data;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace MoneyManager.Forms.v2
{
	public partial class EditWallet : Form
	{
		private Wallet currWallet;
        private List<WalletType> types;

		///////////////////
		// Form Init
		public EditWallet(Wallet w = null)
		{
			InitializeComponent();

            //Load the types
            walletTypeCombobox.DataSource = Enum.GetNames(typeof(WalletType.Types));
            types = Global.db.GetWalletTypes();

            // Set Initial Values
            if (w == null)
            {
                currWallet = new Wallet(Global.user);
                walletGroup.Text = this.Text = "Add Wallet";
				walletColorButton.BackColor = Color.FromName("Desktop");
            }
            else
            {
                walletGroup.Text = this.Text = "Edit Wallet";
                nameText.Text = w.Name;
                walletColorButton.BackColor = Color.FromArgb(w.ColorArgb);
                walletTypeCombobox.SelectedIndex = walletTypeCombobox.Items.IndexOf(Enum.GetName(typeof(WalletType.Types), w.WalletTypeId));

                currWallet = w;
            }

		}

		///////////////////
		// Save Button
		private void okButton_Click(object sender, EventArgs e)
		{
            //Gather all the wallets
            List<Wallet> wallets = Global.db.GetWallets(Global.user);
            

			// Validation Check
			if(nameText.Text == String.Empty)
			{
				MessageBox.Show("Please enter a name for this wallet.", "Error", MessageBoxButtons.OK);
				return;
			}
            if (wallets.Where(x => x.Name == nameText.Text).ToList().Count > 0)
            {
                MessageBox.Show("You all ready have a wallet named this. Please user another name.", "Invalid wallet name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            currWallet.Name = nameText.Text;
            currWallet.WalletTypeId = types.Where(x => x.Type == walletTypeCombobox.SelectedValue.ToString()).Select(x => x.Id).ToArray()[0];

            // Update DB, and Exit
            if (currWallet.Id > 0)
			{
				if (Global.db.UpdateRecord(currWallet))
					this.DialogResult = DialogResult.OK;
				else
					MessageBox.Show("Error saving your Wallet information.", "Error", MessageBoxButtons.OK);
			}
			else
			{
				if (Global.db.CreateRecord(currWallet))
					this.DialogResult = DialogResult.OK;
				else
					MessageBox.Show("Error saving your Wallet information.", "Error", MessageBoxButtons.OK);
			}
		}

        private void ColorEdit(object sender, EventArgs e)
        {
            ColorDialog colors = new ColorDialog();
            if (colors.ShowDialog() == DialogResult.OK)
            {
                walletColorButton.BackColor = colors.Color;
                currWallet.ColorArgb = colors.Color.ToArgb();
            }
        }
    }
}
