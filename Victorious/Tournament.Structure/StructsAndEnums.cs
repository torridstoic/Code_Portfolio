using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	/// <summary>
	/// References one of the two playerslots within Matches.
	/// This is used for Players, score, previous matches, etc.
	/// </summary>
	public enum PlayerSlot
	{
		unspecified = -1,
		Defender = 0,
		Challenger = 1
	};

	/// <summary>
	/// References W/L/T index in an array denoting a player's match record.
	/// </summary>
	public enum Record
	{
		Wins = 0,
		Losses = 1,
		Ties = 2
	};

	/// <summary>
	/// References a Player's result/outcome in a particular Match.
	/// </summary>
	public enum Outcome
	{
		Loss,
		Tie,
		Win
	};

	/// <summary>
	/// Denotes the method of matching Players together,
	/// specifically in Swiss-style brackets.
	/// </summary>
	/// <remarks>
	/// Examples:
	/// Fold = 1-8, 2-7, 3-6, 4-5
	/// Slide = 1-5, 2-6, 3-7, 4-8
	/// Adjacent = 1-2, 3-4, 5-6, 7-8
	/// </remarks>
	public enum PairingMethod
	{
		Fold,
		Slide,
		Adjacent
	};

	/// <summary>
	/// This class is used to track a player's match record.
	/// It holds the W/L/T ints and the methods to modify/update them.
	/// </summary>
	public class PlayerRecord
	{
		public int Wins
		{ get; private set; }
		public int Ties
		{ get; private set; }
		public int Losses
		{ get; private set; }

		public PlayerRecord()
		{
			Reset();
		}

		/// <summary>
		/// Adds or subtracts the given Outcome from this Record.
		/// Does NOT error-check for resulting negative records.
		/// </summary>
		/// <param name="_outcome">Match result: Win, Loss, or Tie</param>
		/// <param name="_isAddition">Add or subtract</param>
		public void AddOutcome(Outcome _outcome, bool _isAddition)
		{
			int add = (_isAddition) ? 1 : -1;
			switch (_outcome)
			{
				case Outcome.Win:
					this.Wins += add;
					break;
				case Outcome.Tie:
					this.Ties += add;
					break;
				case Outcome.Loss:
					this.Losses += add;
					break;
			}
		}

		/// <summary>
		/// Resets this Record to 0-0-0.
		/// </summary>
		public void Reset()
		{
			Wins = Ties = Losses = 0;
		}
	}
}
