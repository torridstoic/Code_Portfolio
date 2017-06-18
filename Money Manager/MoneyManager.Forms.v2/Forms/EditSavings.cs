using MoneyManager.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MoneyManager.Forms.v2.Forms
{
    public partial class EditSavings : Form
    {
        Wallet wallet;

        public EditSavings(Wallet data)
        {
            InitializeComponent();
            if (data == null)
            {
                data = new Wallet(Global.user);
            }

            this.wallet = data;
        }
        
        private void SaveSavings(object sender, EventArgs e)
        {
            wallet.Name = savingTextbox.Text;
            wallet.WalletTypeId = (int)WalletType.Types.Savings;

            if (wallet.Id > 0)
            {
                if (!Global.db.UpdateRecord(wallet))
                {
                    MessageBox.Show("Unable to update savings.", "Error updating", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    this.DialogResult = DialogResult.OK;
                }
            }
            else
            {
                if (!Global.db.CreateRecord(wallet))
                {
                    MessageBox.Show("Unable to create savings.", "Error creatign", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    this.DialogResult = DialogResult.OK;
                }
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void SelectColorClick(object sender, EventArgs e)
        {
            ColorDialog colors = new ColorDialog();
            if (colors.ShowDialog() == DialogResult.OK)
            {
                colorSelectorButton.BackColor = colors.Color;
                wallet.ColorArgb = colors.Color.ToArgb();
            }
        }
    }
}
