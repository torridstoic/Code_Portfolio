using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace GameOfLife
{
	public class OptionEventArgs : EventArgs
	{
		// Private Member Variables
		int timerInt;
		int width;
		int height;
		Color gridColor;
		Color bgColor;
		Color cellColor;
		Boundary bound;

		// Ctor
		public OptionEventArgs(int _timerInt, int _width, int _height, Color _gridColor, Color _bgColor, Color _cellColor, Boundary _bound)
		{
			TimerInt = _timerInt;
			GridWidth = _width;
			GridHeight = _height;

			GridColor = _gridColor;
			BGColor = _bgColor;
			CellColor = _cellColor;

			Bound = _bound;
		}

		// Properties
		public int TimerInt
		{
			get { return timerInt; }
			set { timerInt = value; }
		}
		public int GridWidth
		{
			get { return width; }
			set { width = value; }
		}
		public int GridHeight
		{
			get { return height; }
			set { height = value; }
		}
		public Color GridColor
		{
			get { return gridColor; }
			set { gridColor = value; }
		}
		public Color BGColor
		{
			get { return bgColor; }
			set { bgColor = value; }
		}
		public Color CellColor
		{
			get { return cellColor; }
			set { cellColor = value; }
		}
		public Boundary Bound
		{
			get { return bound; }
			set { bound = value; }
		}
	}
}
