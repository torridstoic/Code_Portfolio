using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
	// This is used for each Cell in the universe array
	public class Cell
	{
		// Private member vars
		bool mBool;

		// Ctor
		public Cell()
		{
			mBool = false;
		}

		// Functions
		public bool IsAlive()
		{
			return mBool;
		}
		public void On()
		{
			mBool = true;
		}
		public void Off()
		{
			mBool = false;
		}
		public bool Flip()
		{
			if (false == mBool)
				mBool = true;
			else
				mBool = false;

			return mBool;
		}
	}
}
