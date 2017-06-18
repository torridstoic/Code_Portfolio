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
    public partial class EditTransaction : Form
    {
        TransactionType.Types transactionType;
        Transaction currTransaction;
        RecurringTransaction currRTransaction;

        List<Wallet> wallets;
        List<Store> subjects;

        ///////////////////
        // Form Init (Overloaded)
        public EditTransaction(TransactionType.Types type) // New Transaction/RTransaction
        {
            InitializeComponent();

            // Set Max Values
            amountUD.Maximum = Decimal.MaxValue;

            // Set Variables
            transactionType = type;
            currTransaction = null;
            currRTransaction = null;
            wallets = Global.db.GetWallets(Global.user);
            subjects = Global.db.GetStores();

            // Set Initial Values
            switch (transactionType)
            {
                case TransactionType.Types.Credit:
                    this.Text = transactionGroup.Text = "Add Credit";
                    subjectLabel.Text = "Payer:";
                    break;
                case TransactionType.Types.Payment:
                    this.Text = transactionGroup.Text = "Add Payment";
                    subjectLabel.Text = "Payee:";
                    break;
                case TransactionType.Types.Savings:
                    this.Text = transactionGroup.Text = "Add Savings";
                    subjectLabel.Text = "Payee:";
                    break;
            }

            singleRadio.Checked = true;
            periodCombo.SelectedIndex = 0;
            frequencyGroup.Visible = false;
            datePicker.Value = DateTime.Today.Date;
            foreach (Store s in subjects)
                subjectCombo.Items.Add(s);
			storeColorButton.BackColor = Color.FromName("Desktop");
            foreach (Wallet w in wallets)
                walletCombo.Items.Add(w);
        }
        public EditTransaction(TransactionType.Types type, Transaction t) // Editing a Transaction
        {
            InitializeComponent();

            // Set Max Values
            amountUD.Maximum = Decimal.MaxValue;

            // Set Variables
            transactionType = type;
            currTransaction = t;
            currRTransaction = null;
            wallets = Global.db.GetWallets(Global.user);
            subjects = Global.db.GetStores();

            // Set Initial Values
            switch (transactionType)
            {
                case TransactionType.Types.Credit:
                    this.Text = transactionGroup.Text = "Edit Credit";
                    subjectLabel.Text = "Payer:";
                    break;
                case TransactionType.Types.Payment:
                    this.Text = transactionGroup.Text = "Edit Payment";
                    subjectLabel.Text = "Payee:";
                    break;
                case TransactionType.Types.Savings:
                    this.Text = transactionGroup.Text = "Edit Savings";
                    subjectLabel.Text = "Payee:";
                    break;
            }

            singleRadio.Checked = true;
            recurringRadio.Enabled = false;
            frequencyGroup.Visible = false;
            datePicker.Value = Global.ConvertTimeStampToDateTime(t.Created);
            foreach (Store s in subjects)
            {
                subjectCombo.Items.Add(s);
                if (s.Id == t.StoreId)
                {
                    subjectCombo.SelectedItem = s;
                    storeColorButton.BackColor = Color.FromArgb(s.ColorArgb);
                }
            }
            amountUD.Value = (Decimal)t.Amount;
            foreach (Wallet w in wallets)
            {
                walletCombo.Items.Add(w);
                if (w.Id == t.WalletId)
                    walletCombo.SelectedItem = w;
            }
        }
        public EditTransaction(TransactionType.Types type, RecurringTransaction rt) // Editing a Recurring Transaction
        {
            InitializeComponent();

            // Set Max Values
            amountUD.Maximum = Decimal.MaxValue;

            // Set Variables
            transactionType = type;
            currTransaction = null;
            currRTransaction = rt;
            wallets = Global.db.GetWallets(Global.user);
            subjects = Global.db.GetStores();

            // Set Initial Values
            switch (transactionType)
            {
                case TransactionType.Types.Credit:
                    this.Text = transactionGroup.Text = "Edit Credit";
                    subjectLabel.Text = "Payer:";
                    break;
                case TransactionType.Types.Payment:
                    this.Text = transactionGroup.Text = "Edit Payment";
                    subjectLabel.Text = "Payee:";
                    break;
                case TransactionType.Types.Savings:
                    this.Text = transactionGroup.Text = "Edit Savings";
                    subjectLabel.Text = "Payee:";
                    break;
            }

            recurringRadio.Checked = true;
            singleRadio.Enabled = false;
            periodCombo.SelectedIndex = rt.ProcessPeriod;
            frequencyGroup.Visible = true;
            datePicker.Value = Global.ConvertTimeStampToDateTime(rt.ProcessDate);
            foreach (Store s in subjects)
            {
                subjectCombo.Items.Add(s);
                if (s.Id == rt.StoreId)
                    subjectCombo.SelectedItem = s;
            }
            amountUD.Value = (Decimal)rt.Amount;
            foreach (Wallet w in wallets)
            {
                walletCombo.Items.Add(w);
                if (w.Id == rt.WalletId)
                    walletCombo.SelectedItem = w;
            }
        }

        ///////////////////
        // Radio Button Changed
        private void radioChange(object sender, EventArgs e)
        {
            if (singleRadio.Checked == true)
                frequencyGroup.Visible = false;
            else // recurringRadio.Checked
                frequencyGroup.Visible = true;
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
            if (subjectCombo.Text == String.Empty)
            {
                MessageBox.Show("Please select a " + subjectLabel.Text, "Warning", MessageBoxButtons.OK);
                return;
            }
            if (0 == amountUD.Value)
            {
                MessageBox.Show("Please input a transaction amount.", "Warning", MessageBoxButtons.OK);
                return;
            }
            if (walletCombo.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a Wallet.", "Warning", MessageBoxButtons.OK);
                return;
            }

            // Check if Subject/Store exists: if not, add it to DB,
            // then save the StoreId
            Store store = null;
            int subjectIndex = subjectCombo.SelectedIndex;
            //List<Store> matchingStores = subjects.Where(x => x.Name == subjectCombo.Text).ToList();
            if (subjectIndex > -1)
            {
                //Lets update the color for the store
                subjects[subjectIndex].ColorArgb = storeColorButton.BackColor.ToArgb();
                Global.db.UpdateRecord(subjects[subjectIndex]);

                store = subjects[subjectIndex];
            }
            else
            {
                store = new Store(subjectCombo.Text);
                store.ColorArgb = storeColorButton.BackColor.ToArgb();

                if (!Global.db.CreateRecord(store))
                {
                    MessageBox.Show("Error creating store/subject.", "Error", MessageBoxButtons.OK);
                    return;
                }

                store = Global.db.GetStore(Global.db.GetLastInsertedId());
            }

            bool newTransaction = true;

            // Recurring Transaction
            if (recurringRadio.Checked == true)
            {
                // One more Validation
                if (periodCombo.SelectedIndex < 0)
                {
                    MessageBox.Show("Please select a Payment Frequency.", "Warning", MessageBoxButtons.OK);
                    return;
                }

                // Save RTransaction data
                if (currRTransaction != null)
                {
                    newTransaction = false;
                    currRTransaction.WalletId = wallets[walletCombo.SelectedIndex].Id;
                }
                else // new
                    currRTransaction = new RecurringTransaction(wallets[walletCombo.SelectedIndex].Id);

                currRTransaction.ProcessPeriod = periodCombo.SelectedIndex;
                currRTransaction.StoreId = store.Id;
                currRTransaction.Amount = (float)amountUD.Value;
                currRTransaction.TransactionTypeId = (int)transactionType;

                // If prior date selected, create prior Transactions
                DateTime paydate = datePicker.Value;
                while (paydate < DateTime.Now)
                {
                    Transaction t = new Transaction(currRTransaction.WalletId);
                    t.Created = Global.ConvertToUnixTimeStamp(paydate);
                    t.StoreId = currRTransaction.StoreId;
                    t.Amount = currRTransaction.Amount;
                    t.TransactionTypeId = currRTransaction.TransactionTypeId;
                    t.WalletId = currRTransaction.WalletId;

                    // Increment the paydate
                    switch (currRTransaction.ProcessPeriod)
                    {
                        case 0: // daily
                            paydate = paydate.AddDays(1);
                            break;
                        case 1: // weekly
                            paydate = paydate.AddDays(7);
                            break;
                        case 2: // monthly
                            paydate = paydate.AddMonths(1);
                            break;
                        case 3: // quarterly
                            paydate = paydate.AddMonths(3);
                            break;
                        case 4: // yearly
                            paydate = paydate.AddYears(1);
                            break;
                    }

                    // Upload the Transaction
                    if (!Global.db.CreateRecord(t))
                    {
                        MessageBox.Show("Error saving your new Transaction.", "Error", MessageBoxButtons.OK);
                        break;
                    }
                }

                // Upload the Recurring Transaction, and exit
                currRTransaction.ProcessDate = Global.ConvertToUnixTimeStamp(paydate);
                if (true == newTransaction && Global.db.CreateRecord(currRTransaction))
                    this.DialogResult = DialogResult.OK;
                else if (false == newTransaction && Global.db.UpdateRecord(currRTransaction))
                    this.DialogResult = DialogResult.OK;
                else
                    MessageBox.Show("Error saving your Recurring Transaction.", "Error", MessageBoxButtons.OK);
            }
            else // Single Transaction
            {
                // Save Transaction Data
                if (currTransaction != null)
                {
                    newTransaction = false;
                    currTransaction.WalletId = wallets[walletCombo.SelectedIndex].Id;
                }
                else
                    currTransaction = new Transaction(wallets[walletCombo.SelectedIndex].Id);

                currTransaction.Created = Global.ConvertToUnixTimeStamp(datePicker.Value);
                currTransaction.StoreId = store.Id;
                currTransaction.Amount = (float)amountUD.Value;
                currTransaction.TransactionTypeId = (int)transactionType;

                // Upload the Transaction, and exit
                if (true == newTransaction && Global.db.CreateRecord(currTransaction))
                    this.DialogResult = DialogResult.OK;
                else if (false == newTransaction && Global.db.UpdateRecord(currTransaction))
                    this.DialogResult = DialogResult.OK;
                else
                    MessageBox.Show("Error saving your Transaction.", "Error", MessageBoxButtons.OK);
            }
        }

        ///////////////////
        // Properties

        private void ColorEdit(object sender, EventArgs e)
        {
            ColorDialog colors = new ColorDialog();
            if (colors.ShowDialog() == DialogResult.OK)
            {
                storeColorButton.BackColor = colors.Color;
            }
        }

        private void PayeeChanged(object sender, EventArgs e)
        {
            if (subjectCombo.SelectedIndex > -1)
            {
                storeColorButton.BackColor = Color.FromArgb(subjects[subjectCombo.SelectedIndex].ColorArgb);
            }
            else
            {
                storeColorButton.BackColor = Color.FromName("Control");
            }
        }
    }
}
