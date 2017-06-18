using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameOfLife
{
	public partial class OptionsDialog : Form
	{
		// Event Handler Instance
		public event OptionEventHandler Apply;

		// Ctor
		public OptionsDialog(int _timerInt, int _width, int _height, Color _gridColor, Color _bgColor, Color _cellColor, Boundary _bound)
		{
			InitializeComponent();

			// Set boundary values
			timerUD.Maximum = Decimal.MaxValue;
			widthUD.Maximum = Decimal.MaxValue;
			heightUD.Maximum = Decimal.MaxValue;

			// Set initial values
			TimerInterval = _timerInt;
			GridWidth = _width;
			GridHeight = _height;
			GridColor = _gridColor;
			BGColor = _bgColor;
			CellColor = _cellColor;
			Bound = _bound;
		}

		// Boundary Radio Buttons Property
		public Boundary Bound
		{
			get
			{
				Boundary curBound = 0;
				if (finiteRadio.Checked)
					curBound = Boundary.Finite;
				else if (toroidalRadio.Checked)
					curBound = Boundary.Toroidal;
				else //if (infiniteRadio.Checked)
					curBound = Boundary.Infinite;

				return curBound;
			}
			set
			{
				switch (value)
				{
					case (Boundary.Finite):
						finiteRadio.Select();
						break;
					case (Boundary.Toroidal):
						toroidalRadio.Select();
						break;
					case (Boundary.Infinite):
						infiniteRadio.Select();
						break;
				}
			}
		}

		// Simple Properties
		public int TimerInterval
		{
			get { return (int)timerUD.Value; }
			set { timerUD.Value = value; }
		}
		public int GridWidth
		{
			get { return (int)widthUD.Value; }
			set { widthUD.Value = value; }
		}
		public int GridHeight
		{
			get { return (int)heightUD.Value; }
			set { heightUD.Value = value; }
		}
		public Color GridColor
		{
			get { return gridColorButton.BackColor; }
			set { gridColorButton.BackColor = value; }
		}
		public Color BGColor
		{
			get { return bgColorButton.BackColor; }
			set { bgColorButton.BackColor = value; }
		}
		public Color CellColor
		{
			get { return cellColorButton.BackColor; }
			set { cellColorButton.BackColor = value; }
		}

		// clicking a COLOR CHOOSER button
		private void Color_Button(object sender, EventArgs e)
		{
			// Cast the sender
			Button b = sender as Button;

			// Open the dialog and set the default
			ColorDialog clrDlg = new ColorDialog();
			clrDlg.Color = b.BackColor;

			// Pass back the chosen color
			if(DialogResult.OK==clrDlg.ShowDialog())
				b.BackColor = clrDlg.Color;
		}
		// RESET COLORS
		private void resetButton_Click(object sender, EventArgs e)
		{
			// Reset to default colors
			gridColorButton.BackColor = Color.Black;
			bgColorButton.BackColor = Color.White;
			cellColorButton.BackColor = Color.DarkGray;
		}

		// RESET SETTINGS
		private void resetDefaultsButton_Click(object sender, EventArgs e)
		{
			// Reset Default Settings (except Seed)
			Properties.Settings.Default.Reset();

			// Reset the currently open Controls, too
			resetButton_Click(this, e); // this resets color buttons
			GridWidth = Properties.Settings.Default.GWidth;
			GridHeight = Properties.Settings.Default.GHeight;
			TimerInterval = Properties.Settings.Default.TimerInt;
			Bound = (Boundary)(Properties.Settings.Default.Boundary);
		}

		private void applyButton_Click(object sender, EventArgs e)
		{
			if (Apply != null)
			{
				Apply(this, new OptionEventArgs(TimerInterval, GridWidth, GridHeight, GridColor, BGColor, CellColor, Bound));
			}
		}
	}
}
