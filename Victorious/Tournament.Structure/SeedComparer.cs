using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	/// <summary>
	/// Compares too [nullable int] seeds.
	/// "Sorts" null seeds to the back.
	/// If both seeds are equal (or null), sorts them randomly.
	/// </summary>
	public class SeedComparer : IComparer<int?>
	{
		public int Compare(int? first, int? second)
		{
			// NULL values sort to the back:
			if (!first.HasValue)
			{
				if (!second.HasValue)
				{
					// Dual null values randomize:
					return RandCompare();
				}

				return 1;
			}
			if (!second.HasValue)
			{
				return -1;
			}
		
			// Default: Both ints have value:
			int compare = (first.Value).CompareTo(second.Value);
			// Duplicate values randomize:
			return (0 == compare)
				? RandCompare() : compare;
		}

		private int RandCompare()
		{
			//Random rng = new Random();
			int rand = new Random().Next(2);
			return (rand > 0)
				? 1 : -1;
#if false
			int compare = rng.Next().CompareTo(rng.Next());
			return (0 == compare)
				? RandCompare() : compare;
#endif
		}
	}
}
