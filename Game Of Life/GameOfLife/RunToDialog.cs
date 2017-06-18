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
	public partial class RunToDialog : Form
	{
		// Ctor
		public RunToDialog(int _gen)
		{
			InitializeComponent();

			// Set maximum values
			destinationUD.Maximum = Decimal.MaxValue;

			// Set initial/min values
			destinationUD.Minimum = _gen;
		}

		// Properties
		public int Generation
		{
			get { return (int)destinationUD.Value; }
			set { destinationUD.Value = value; }
		}
	}
}
