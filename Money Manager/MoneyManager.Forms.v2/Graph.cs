using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using MoneyManager.Data;

namespace MoneyManager.Forms.v2
{
    abstract public class Graph
    {
        public enum SortBy
        {
            None,
            Wallet,
            Store
        };

        private Random rand;
        protected SortBy sort;
        protected List<Transaction> data;
        protected float totalAmount;
        protected float maxAmount;
        protected float minAmount;

        protected Graph(SortBy sort)
        {
            data = new List<Transaction>();
            rand = new Random();
            this.sort = sort;
        }

        public void AddValues(List<Transaction> transactions)
        {
            data = transactions;
            
            foreach (Transaction trans in transactions)
            {
                totalAmount += trans.Amount;
            }

            maxAmount = transactions.Max(i => i.Amount);
            minAmount = transactions.Min(i => i.Amount);
        }

        protected Color RandomColor()
        {
            
            return Color.FromArgb(rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255));
        }



        abstract public void Draw(Graphics g, int x, int y, int width, int height);
    }
}
