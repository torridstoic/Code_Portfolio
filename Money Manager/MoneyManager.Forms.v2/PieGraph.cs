using MoneyManager.Data;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace MoneyManager.Forms.v2
{
    public class PieGraph : Graph
    {
        public PieGraph(SortBy sort) : base(sort)
        {

        }

        public override void Draw(Graphics g, int x, int y, int width, int height)
        {
            int pieX = x;
            int pieY = y;
            int legendSize = 75;

            int pieWidth = width - legendSize;
            int pieHeight = height - legendSize;
            int pieMinSize = Math.Min(pieWidth, pieHeight);
            
            int legendX = pieMinSize;
            int legendY = y;
            Size legendColor = new Size(20, 15);

            //Find area of the circle;
            float radius = x + (pieMinSize / 2);

            g.DrawEllipse(new Pen(Color.Black), pieX, pieY, pieMinSize, pieMinSize);

            //g.FillPie(Brushes.Green, graphRect, 270.0f, 359.99f); //Rotation goes 360 degrees
            float startDegree = 270.0f;
            Rectangle graphRect = new Rectangle(pieX, pieY, pieMinSize, pieMinSize);

            for (int i = 0; i < data.Count; i++)
            {
                float perc = data[i].Amount / totalAmount;
                float degreesOfAmount = 360.0f * perc;

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

                Brush brush = new SolidBrush(color);
                g.FillPie(brush, graphRect, startDegree, degreesOfAmount); //Rotation goes 360 degrees
                g.FillPie(Brushes.Black, graphRect, startDegree, 2);

                startDegree += degreesOfAmount;

                //Now draw the key on the right side of the pie
                g.FillRectangle(brush, legendX + 5, legendY + (i*50), legendColor.Width, legendColor.Height);
                g.DrawString(data[i].WalletName, new Font(FontFamily.GenericSansSerif, 12), Brushes.Black, legendX + legendColor.Width + 5, legendY + (i*50));
                if (sort == SortBy.Store)
                    g.DrawString(data[i].StoreName, new Font(FontFamily.GenericSansSerif, 12), Brushes.Black, legendX + legendColor.Width + 5, legendY + (i * 50) + legendColor.Width);
            }

            //g.FillEllipse(new SolidBrush(Color.FromName("Control")), radius - cxw / 2, radius - cyw / 2, cxw, cyw); //Center dot
            
        }
    }
}
