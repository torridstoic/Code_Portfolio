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
	public partial class SeedDialog : Form
	{
		public SeedDialog(int _seed)
		{
			InitializeComponent();

			// Set maximum values
			seedUD.Minimum = Decimal.MinValue;
			seedUD.Maximum = Decimal.MaxValue;

			// Set initial value
			Seed = _seed;
		}

		// Properties
		public int Seed
		{
			get { return (int)(seedUD.Value); }
			set { seedUD.Value = value; }
		}
	}
}
