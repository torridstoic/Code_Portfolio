using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace MoneyManager.Forms.v2
{
    public class LineGraph : Graph
    {

        public LineGraph(SortBy sort) : base(sort)
        {

        }

        public override void Draw(Graphics g, int x, int y, int width, int height)
        {
            int totalHeight = y + height;
            int totalWidth = x + width;
            Pen blackPen = new Pen(Color.Black);

            //Draw vertical Line
            g.DrawLine(blackPen, x, y, x, y + height);
            //Draw Horozonal Line
            g.DrawLine(blackPen, x, totalHeight, totalWidth, totalHeight);

            for (int i = 100; i > 0; i -= 20)
            {
                float location = y + (height * (1 - (i / 100.0f)));

                //Draw the percentages
                g.DrawString(i.ToString() + "%", new Font(FontFamily.GenericSansSerif, 12), Brushes.Black, x - 50, location);
                g.DrawLine(blackPen, x - 10, location, x + width, location);
            }

            //Calculate the space between each item
            DateTime minDate = Global.ConvertTimeStampToDateTime(data.Min(i => i.Created));
            DateTime maxDate = Global.ConvertTimeStampToDateTime(data.Max(i => i.Created));
            int dateDifference = (maxDate - minDate).Days;
            

            float distance = width / dateDifference;
            List<Point> points = new List<Point>();

            for (int i = 0; i < data.Count; i++)
            {
                float totalPerWallet = data.Where(w => w.WalletId == data[i].WalletId).Sum(w => w.Amount);

                float perc = data[i].Amount / totalAmount;
                float rectHeight = totalHeight * perc;
                float yPosition = totalHeight - rectHeight;
                float xLocation = (x + 10) + (i * distance);
                
                DateTime dataDate = Global.ConvertTimeStampToDateTime(data[i].Created);

            }
        }
    }
}
