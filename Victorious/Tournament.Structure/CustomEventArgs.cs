using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatabaseLib;

namespace Tournament.Structure
{
	/// <summary>
	/// Custom EventArgs used for Bracket events.
	/// This contains data for adding/removing/modifying Matches,
	/// and/or deleting Games from the database.
	/// </summary>
	public class BracketEventArgs : EventArgs
	{
		/// <summary>
		/// New Models of all changed Matches.
		/// </summary>
		public List<MatchModel> UpdatedMatches
		{ get; private set; }

		/// <summary>
		/// ID's of any Games to delete from database.
		/// </summary>
		public List<int> DeletedGameIDs
		{ get; private set; }

		public BracketEventArgs(List<MatchModel> _matches, List<int> _gameIDs)
		{
			UpdatedMatches = _matches;
			DeletedGameIDs = _gameIDs;
		}
		public BracketEventArgs(List<MatchModel> _matches)
			: this(_matches, new List<int>())
		{ }
		public BracketEventArgs(List<int> _gameIDs)
			: this(new List<MatchModel>(), _gameIDs)
		{ }
	}
}
