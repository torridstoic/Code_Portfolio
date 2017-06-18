using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System;
using MoneyManager.Data;

namespace MoneyManager.Forms.v2
{
    public class BarGraph : Graph
    {
        public BarGraph(SortBy sort) : base(sort)
        {

        }

        public override void Draw(Graphics g, int x, int y, int width, int height)
        {
            //Minor calculation changes.
            x += 50;
            width -= 50;
            height -= 50;

            int totalHeight = y + height;
            int totalWidth = x + width;
            Pen blackPen = new Pen(Color.Black);

            //Draw vertical Line
            g.DrawLine(blackPen, x, y, x, y + height);
            //Draw Horozonal Line
            g.DrawLine(blackPen, x, totalHeight, totalWidth, totalHeight);

            for (int i = 100; i > 0; i -= 20)
            {
                float location = y + (height * ( 1 - (i / 100.0f)));

                //Draw the percentages
                g.DrawString(i.ToString() + "%", new Font(FontFamily.GenericSansSerif, 12), Brushes.Black, x - 50, location);
                g.DrawLine(blackPen, x - 10, location, x + width, location);
            }

            //Calculate the space between each item
            float barSize = 25;
            float distance = width / data.Count;
            
            //Draw the percentages
            for (int i = 0; i < data.Count; i++)
            {
                float perc = data[i].Amount / totalAmount;
                float rectHeight = totalHeight * perc;
                float yPosition = totalHeight - rectHeight;
                float xLocation = (x + 10) + (i * distance);
                float barCenter = xLocation - (barSize / 2.0f);

                Wallet wallet = Global.db.GetWallet(Global.user, data[i].WalletId);
                Store store = Global.db.GetStore(data[i].StoreId);
                Color color = this.RandomColor();

                switch (sort)
                {
                    case SortBy.Store:
                        if (Color.FromName("Control") != Color.FromArgb(store.ColorArgb)) color = Color.FromArgb(store.ColorArgb);
                        break;
                    case SortBy.Wallet:
                        if (Color.FromName("Control") != Color.FromArgb(wallet.ColorArgb)) color = Color.FromArgb(wallet.ColorArgb);
                        break;
                }

                RectangleF rect = new RectangleF(xLocation, yPosition, barSize, rectHeight);
                g.FillRectangle(new SolidBrush(color), rect);
                
                g.DrawString(data[i].WalletName, new Font(FontFamily.GenericSansSerif, 12), Brushes.Black, barCenter, totalHeight + 5);
                if (sort == SortBy.Store)
                {
                    g.DrawString(data[i].StoreName, new Font(FontFamily.GenericSansSerif, 12), Brushes.Black, barCenter, totalHeight + 20);
                }
            }
        }
    }
}
