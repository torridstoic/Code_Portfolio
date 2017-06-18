using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;


// Enum for universe boundary selection
public enum Boundary
{
	Finite = 0,
	Toroidal,
	Infinite // not implemented
}

namespace GameOfLife
{
	// Delegate
	public delegate void OptionEventHandler(object sender, OptionEventArgs e);

	public partial class Form1 : Form
	{
		// member ints
		int numCols;
		int numRows;
		int generations;
		int destination;
		int alive;
		int randSeed;
		Boundary mBound;
		Random rng;

		// colors for the graphicPanel
		Color bgColor;
		Color gridColor;
		Color cellColor;

		// 2d Cell array, the displayed universe
		Cell[,] universe;


		public Form1()
		{
			InitializeComponent();
			this.Text = "Game of Life";

			// Initialize the game
			randSeed = Properties.Settings.Default.Seed; // default 0
			rng = new Random(randSeed);
			SetNewGrid(Properties.Settings.Default.GHeight, Properties.Settings.Default.GWidth); // default (50,50)
			mBound = (Boundary)Properties.Settings.Default.Boundary; // default Finite

			// Get/Set the initial colors
			bgColor = graphicPanel1.BackColor = Properties.Settings.Default.BGColor; // default White
			gridColor = Properties.Settings.Default.GridColor; // default Black
			cellColor = Properties.Settings.Default.CellColor; // default Dark Gray

			// Setup the timer
			timer1.Interval = Properties.Settings.Default.TimerInt; // default 200 ms
			timer1.Enabled = false;
			//timer1.Tick += timer1_Tick;
			intervalStrip.Text = "Interval: " + timer1.Interval.ToString();
		}

		// Set up a new, empty grid
		private void SetNewGrid(int _rows, int _cols)
		{
			numRows = _rows;
			numCols = _cols;

			// Create a new, empty universe
			universe = new Cell[numCols, numRows];
			for (int x = 0; x < numCols; ++x)
				for (int y = 0; y < numRows; ++y)
					universe[x, y] = new Cell();

			// Reset the status strip counters
			generations = 0;
			destination = -1;
			alive = 0;

			// Reset the status strip labels
			generationsStrip.Text = "Generation: " + generations.ToString();
			aliveStrip.Text = "Alive: " + alive.ToString();
			seedStrip.Text = "Seed: " + randSeed.ToString();

			graphicPanel1.Invalidate();
		}

		// Return "# of neighbors" for the [_x,_y] cell
		private int CountNeighbors(int _x, int _y)
		{
			int count = 0;

			switch (mBound)
			{
				case (Boundary.Finite):
					{
						// Finite Universe calculations
						if (_x > 0 && _y > 0 && universe[_x - 1, _y - 1].IsAlive())
							count++;
						if (_y > 0 && universe[_x, _y - 1].IsAlive())
							count++;
						if (_x < (numCols - 1) && _y > 0 && universe[_x + 1, _y - 1].IsAlive())
							count++;
						if (_x > 0 && universe[_x - 1, _y].IsAlive())
							count++;
						if (_x < (numCols - 1) && universe[_x + 1, _y].IsAlive())
							count++;
						if (_x > 0 && _y < (numRows - 1) && universe[_x - 1, _y + 1].IsAlive())
							count++;
						if (_y < (numRows - 1) && universe[_x, _y + 1].IsAlive())
							count++;
						if (_x < (numCols - 1) && _y < (numRows - 1) && universe[_x + 1, _y + 1].IsAlive())
							count++;

						break;
					}
				case (Boundary.Toroidal):
					{
						// Toroidal Universe calculations
						int xUnder = _x - 1;
						if (0 > xUnder)
							xUnder = numCols - 1;
						int xOver = _x + 1;
						if (numCols == xOver)
							xOver = 0;

						int yUnder = _y - 1;
						if (0 > yUnder)
							yUnder = numRows - 1;
						int yOver = _y + 1;
						if (numRows == yOver)
							yOver = 0;

						if (universe[xUnder, yUnder].IsAlive())
							count++;
						if (universe[_x, yUnder].IsAlive())
							count++;
						if (universe[xOver, yUnder].IsAlive())
							count++;
						if (universe[xUnder, _y].IsAlive())
							count++;
						if (universe[xOver, _y].IsAlive())
							count++;
						if (universe[xUnder, yOver].IsAlive())
							count++;
						if (universe[_x, yOver].IsAlive())
							count++;
						if (universe[xOver, yOver].IsAlive())
							count++;

						break;
					}
				case (Boundary.Infinite):
					{
						// Not implemented
						break;
					}
			}

			return count;
		}

		// Next Generation
		private void NextGeneration()
		{
			// Create a next generation array
			Cell[,] nextGen = new Cell[numCols, numRows];

			// Set the next generation, one cell at a time
			alive = 0;
			for (int x = 0; x < numCols; ++x)
			{
				for (int y = 0; y < numRows; ++y)
				{
					// Initialize each cell
					nextGen[x, y] = new Cell();

					// Check the neighbors
					int neighbors = CountNeighbors(x, y);

					// Living cells with 2 live neighbors live on
					if (universe[x, y].IsAlive() && 2 == neighbors)
					{
						nextGen[x, y].On();
						alive++;
					}
					// All (living/dead) cells with exactly 3 live neighbors live
					else if (3 == neighbors)
					{
						nextGen[x, y].On();
						alive++;
					}
					// All other cells die
					else
						nextGen[x, y].Off();
				}
			}

			// "Move" to next generation
			universe = nextGen;
			generations++;
			generationsStrip.Text = "Generation: " + generations.ToString();
			aliveStrip.Text = "Alive: " + alive.ToString();

			graphicPanel1.Invalidate();

			// Stop the timer if "destination" is reached
			if (generations == destination)
			{
				timer1.Stop();
				// Reset destination
				destination = -1;
			}
		}

		// RANDOMIZE
		private void Randomize(object sender, EventArgs e)
		{
			if(sender.Equals(fromTimeToolStripMenuItem))
				// Seed the Random object based on current Time
				randSeed = (int)DateTime.Now.Ticks;
			rng = new Random(randSeed);

			// Update the status strip "Seed" text
			seedStrip.Text = "Seed: " + randSeed.ToString();

			// Iterate through the universe array, and randomize each cell
			for (int y = 0; y < numRows; ++y)
			{
				for (int x = 0; x < numCols; ++x)
				{
					// 1/3 On, 2/3 Off
					int rand = rng.Next(3);

					if (0 == rand)
						universe[x, y].On();
					else
						universe[x, y].Off();
				}
			}
			
			graphicPanel1.Invalidate();
		}

		// OPEN FUNC (open/import)
		private void Open_File(object sender, EventArgs e)
		{
			// Set up OpenFileDialog
			OpenFileDialog open = new OpenFileDialog();
			open.Filter = "All Files|*.*|Cells|*.cells";
			open.FilterIndex = 2;

			if (DialogResult.OK == open.ShowDialog())
			{
				StreamReader reader = new StreamReader(open.FileName);

				// Variables for the new template size
				int width = 0;
				int height = 0;

				// Find the new size
				while (!reader.EndOfStream)
				{
					string currRow = reader.ReadLine();

					// Ignore commented rows (begin with '!')
					if (!currRow[0].Equals('!'))
					{
						// Increment new height
						height++;

						// Set the new width
						// (this should hopefully only trigger once)
						if (currRow.Length > width)
							width = currRow.Length;
					}
				}

				// Return to the beginning of the stream
				reader.BaseStream.Seek(0, SeekOrigin.Begin);
				int xInit = 0;
				int y = 0;

				if (sender.Equals(openToolStripMenuItem) || sender.Equals(openToolStripButton)) // OPEN
				{
					// Resize/Reset the current universe
					// (this also sets all cells to default "dead")
					SetNewGrid(height, width);
				}
				else if (sender.Equals(importToolStripMenuItem)) // IMPORT
				{
					// BREAK if the template is larger than the universe
					if (height > numRows || width > numCols)
					{
						reader.Close();
						return;
					}

					// Center the template in the current universe
					// Find the top left corner
					xInit = (numCols / 2) - (width / 2);
					y = (numRows / 2) - (height / 2);
				}

				// Update the new universe
				while (!reader.EndOfStream)
				{
					string currRow = reader.ReadLine();

					// Ignore commented rows
					if (!currRow[0].Equals('!'))
					{
						for (int x = 0; x < currRow.Length; ++x)
						{
							// Find and set live cells
							if (currRow[x].Equals('O'))
							{
								if (sender.Equals(openToolStripMenuItem) || sender.Equals(openToolStripButton)) // OPEN
									universe[x, y].On();
								else if (sender.Equals(importToolStripMenuItem)) // IMPORT
									universe[(x + xInit), y].On();
							}
						}

						// Increment y after each "real" row is read
						y++;
					}
				}

				// Close the reader
				reader.Close();
				graphicPanel1.Invalidate();
			}
		}

		// PAINT
		private void graphicPanel1_Paint(object sender, PaintEventArgs e)
		{
			// Draw Vertical Lines
			float xInterval = (float)graphicPanel1.ClientSize.Width / (float)numCols;
			for (int i = 0; i < numCols; ++i)
				e.Graphics.DrawLine(new Pen(gridColor), new Point((int)(xInterval * i), 0), new Point((int)(xInterval * i), graphicPanel1.ClientSize.Height));
			
			// Draw Horizontal Lines
			float yInterval = (float)graphicPanel1.ClientSize.Height / (float)numRows;
			for (int i = 0; i < numRows; ++i)
				e.Graphics.DrawLine(new Pen(gridColor), new Point(0, (int)(yInterval * i)), new Point(graphicPanel1.ClientSize.Width, (int)(yInterval * i)));

			// Fill Living Cells
			for (int y = 0; y < numRows; ++y)
				for (int x = 0; x < numCols; ++x)
					if (universe[x, y].IsAlive())
						e.Graphics.FillRectangle(new SolidBrush(cellColor), xInterval * x, yInterval * y, xInterval - 1, yInterval - 1);
		}

		// Mouse Click on GPanel
		private void graphicPanel1_MouseClick(object sender, MouseEventArgs e)
		{
			// Find the selected cell
			int row = (int)(e.Location.Y / ((float)graphicPanel1.ClientSize.Height / (float)numRows));
			int col = (int)(e.Location.X / ((float)graphicPanel1.ClientSize.Width / (float)numCols));
			// Turn the cell on/off
			bool isActive = universe[col, row].Flip();

			// Update the status bar
			if (isActive)
				alive++;
			else
				alive--;
			aliveStrip.Text = "Alive: " + alive.ToString();

			graphicPanel1.Invalidate();
		}

		// NEW / CLEAR
		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			timer1.Stop();
			SetNewGrid(numRows, numCols);
		}

		// SAVE
		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// Set up SaveFileDialog
			SaveFileDialog save = new SaveFileDialog();
			save.Filter = "All Files|*.*|Cells|*.cells";
			save.FilterIndex = 2;
			save.DefaultExt = "cells";

			if (DialogResult.OK == save.ShowDialog())
			{
				// Initialize the StreamWriter
				StreamWriter writer = new StreamWriter(save.FileName);

				// Beginning comments
				writer.WriteLine("!" + save.FileName);
				writer.WriteLine("!");

				// Copy the universe, row by row
				for (int y = 0; y < numRows; ++y)
				{
					string currRow = String.Empty;

					for (int x = 0; x < numCols; ++x)
					{
						if (universe[x, y].IsAlive())
							currRow += 'O';
						else
							currRow += '.';
					}

					writer.WriteLine(currRow);
				}

				// Close the writer
				writer.Close();
			}
		}
		// EXIT
		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Close();
			//Application.Exit();
		}
		// TICK (NEXT also calls this)
		private void timer1_Tick(object sender, EventArgs e)
		{
			NextGeneration();
		}
		// START
		private void startToolStripMenuItem_Click(object sender, EventArgs e)
		{
			timer1.Start();
		}
		// STOP
		private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
		{
			timer1.Stop();
		}

		// TO
		private void toToolStripMenuItem_Click(object sender, EventArgs e)
		{
			RunToDialog rtdBox = new RunToDialog(generations + 1);

			if (DialogResult.OK == rtdBox.ShowDialog())
			{
				// Set a new destination and start the timer
				destination = rtdBox.Generation;
				timer1.Start();
			}
		}

		// OPTIONS
		private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// Create a new Modal Dialog instance
			OptionsDialog optBox = new OptionsDialog(timer1.Interval, numCols, numRows, gridColor, bgColor, cellColor, mBound);

			optBox.Apply += Opt_Apply;

			if (DialogResult.OK == optBox.ShowDialog())
			{
				Opt_Apply(this, new OptionEventArgs(optBox.TimerInterval, optBox.GridWidth, optBox.GridHeight, optBox.GridColor, optBox.BGColor, optBox.CellColor, optBox.Bound));

				//// Update program values
				//timer1.Interval = optBox.TimerInterval;
				//intervalStrip.Text = "Interval: " + timer1.Interval.ToString();
				//mBound = optBox.Bound;

				//// If the universe size was changed, make a new one
				//if (numCols != optBox.GridWidth || numRows != optBox.GridHeight)
				//	SetNewGrid(optBox.GridHeight, optBox.GridWidth);

				//// If colors were changed, update them
				//if(gridColor != optBox.GridColor || bgColor != optBox.BGColor || cellColor != optBox.CellColor)
				//{
				//	gridColor = optBox.GridColor;
				//	graphicPanel1.BackColor = bgColor = optBox.BGColor;
				//	cellColor = optBox.CellColor;
					
				//	graphicPanel1.Invalidate();
				//}
			}
		}
		// Options APPLY
		private void Opt_Apply(object sender, OptionEventArgs e)
		{
			// Update program values
			timer1.Interval = e.TimerInt;
			intervalStrip.Text = "Interval: " + timer1.Interval.ToString();
			mBound = e.Bound;

			// If the universe size was changed, make a new one
			if (numCols != e.GridWidth || numRows != e.GridHeight)
				SetNewGrid(e.GridHeight, e.GridWidth);

			// If colors were changed, update them
			if (gridColor != e.GridColor || bgColor != e.BGColor || cellColor != e.CellColor)
			{
				gridColor = e.GridColor;
				graphicPanel1.BackColor = bgColor = e.BGColor;
				cellColor = e.CellColor;

				graphicPanel1.Invalidate();
			}
		}

		// Randomize FROM NEW SEED
		private void fromSeedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// Call up a modal dialog to prompt for a new Random seed
			SeedDialog seedBox = new SeedDialog(randSeed);

			if(DialogResult.OK == seedBox.ShowDialog())
			{
				// Retrieve the chosen seed and call Randomize
				randSeed = seedBox.Seed;
				Randomize(this, e);
			}
		}

		// FORM CLOSED
		private void Form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			// Save color settings
			Properties.Settings.Default.BGColor = bgColor;
			Properties.Settings.Default.GridColor = gridColor;
			Properties.Settings.Default.CellColor = cellColor;

			// Save universe settings
			Properties.Settings.Default.GWidth = numCols;
			Properties.Settings.Default.GHeight = numRows;
			Properties.Settings.Default.TimerInt = timer1.Interval;
			Properties.Settings.Default.Seed = randSeed;
			Properties.Settings.Default.Boundary = (int)mBound;

			Properties.Settings.Default.Save();
		}
	}
}
