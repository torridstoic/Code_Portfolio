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
    public partial class StartScreen : UserControl
    {
        public StartScreen()
        {
            InitializeComponent();
        }

        private void InitializeBudgetData(Graphics g)
        {
            List<Budget> budgets = Global.db.GetBudgets(Global.user);

            for (int i = 0; i < budgets.Count; i++)
            {
                Rectangle bar;
                Rectangle amountUsed;

                Wallet wallet = Global.db.GetWallet(Global.user, budgets[i].WalletId);
                List<Transaction> transactions = Global.db.GetTransactions(Global.user, wallet);
                int budgetDaysLeft = Global.TimeLeft(budgets[i].EndDate);
                float totalTransactions = transactions.Where(x => x.TransactionTypeId == (int)TransactionType.Types.Payment).Sum(x => x.Amount);
                float totalCredits = transactions.Where(x => x.TransactionTypeId == (int)TransactionType.Types.Credit).Sum(x => x.Amount);
                float totalAmountUsed = totalTransactions - totalCredits;
                float percBudgetUsed = totalAmountUsed / budgets[i].Amount;
                percBudgetUsed = percBudgetUsed > 1 ? 1 : percBudgetUsed;
                

                float startX = 5;
                float startY = currentBudgetLabel.Location.Y + 20 + i * 75;
                bar = new Rectangle((int)startX, (int)startY + 40, 250, 20);
                amountUsed = new Rectangle((int)startX, (int)startY + 40, (int)(250 * percBudgetUsed), 20);


                g.DrawString(wallet.Name + ": "+budgetDaysLeft+" Days Remaining", new Font(FontFamily.GenericSansSerif, 12), Brushes.Black, startX, startY);
                g.DrawString("Balance Remaining " + (budgets[i].Amount - totalAmountUsed).ToString("c2"), new Font(FontFamily.GenericSansSerif, 12), Brushes.Black, startX, startY + 20);
                g.DrawRectangle(Pens.Black, bar);
                g.FillRectangle(new SolidBrush(Color.FromArgb(wallet.ColorArgb)), amountUsed);

            }
        }

        private void DrawLayout(object sender, PaintEventArgs e)
        {
            InitializeBudgetData(e.Graphics);
        }
    }
}
