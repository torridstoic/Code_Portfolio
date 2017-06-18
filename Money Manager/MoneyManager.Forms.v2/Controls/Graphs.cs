using System.Windows.Forms;
using System;
using System.Collections.Generic;
using MoneyManager.Data;

namespace MoneyManager.Forms.v2
{
    public partial class Graphs : UserControl
    {
        Boolean selectionChanged;
        List<Graph> graphs;

        public Graphs()
        {
            InitializeComponent();
            selectionChanged = false;
            walletSortRadio.Checked = true;
            graphs = new List<Graph>();
        }

        private void DrawGraph(object sender, PaintEventArgs e)
        {
            if (!selectionChanged) return;
            else selectionChanged = false;

            //Draw the graphs. Need to space them out and plan them
            if (graphs.Count > 0)
            {
                Console.WriteLine("Redrawing the graph");
                int graphPanelHeight = (this.Height - controlsGroupbox.Height - 50) / graphs.Count;
                int graphPanelWidth = this.Width;
                int xLocation = controlsGroupbox.Location.X;
                int yLocation = controlsGroupbox.Height + 15;

                for (int i = 0; i < graphs.Count; i++)
                {
                    graphs[i].Draw(e.Graphics, xLocation, yLocation, graphPanelWidth, graphPanelHeight);
                    yLocation += graphPanelHeight;
                }
            }
        }

        private void UpdateGraph(object sender, EventArgs e)
        {
            //Remove all the graphs
            graphs.Clear();
            List<Transaction> transactions = new List<Transaction>();
            Graph.SortBy sorted = Graph.SortBy.None;

            //Check the radio buttons
            if (storeSortRadio.Checked)
            {
                sorted = Graph.SortBy.Store;
                ResultData transStoreQuery = Global.db.Query("SELECT SUM(t.Amount) as Amount, t.WalletId, t.StoreId, s.Name as StoreName, (SELECT Name FROM Wallets WHERE t.WalletId = Id) as WalletName, Created FROM Transactions t INNER JOIN Stores s ON t.StoreId = s.Id WHERE t.WalletId IN (SELECT Id FROM Wallets WHERE UserId = " + Global.user.Id + ") GROUP BY t.WalletId, t.StoreId ORDER BY WalletId, Created");
                while (transStoreQuery.Read())
                {
                    transactions.Add(new Transaction(transStoreQuery));
                }
            }
            else if (walletSortRadio.Checked)
            {
                sorted = Graph.SortBy.Wallet;
                ResultData transWalletQuery = Global.db.Query("SELECT SUM(t.Amount) as Amount, t.WalletId, w.Name as WalletName, Created FROM Transactions t INNER JOIN Wallets w ON t.WalletId = w.Id WHERE w.UserId = " + Global.user.Id + " GROUP BY w.Id ORDER BY WalletId, Created ASC");
                while (transWalletQuery.Read())
                {
                    transactions.Add(new Transaction(transWalletQuery));
                }
            }

            //Safe guard
            if (transactions.Count > 0)
            {

                if (barGraphCheckbox.Checked)
                {
                    BarGraph bars = new BarGraph(sorted);

                    bars.AddValues(transactions);
                    graphs.Add(bars);
                }

                if (pieGraphCheckbox.Checked)
                {
                    PieGraph pie = new PieGraph(sorted);

                    pie.AddValues(transactions);
                    graphs.Add(pie);
                }

                this.selectionChanged = true;
                this.Invalidate();
            }
            else
            {
                MessageBox.Show("You need to create a transaction to use this feature!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
