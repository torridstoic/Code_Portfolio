using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	/// <summary>
	/// These objects are used in Bracket Ranking systems.
	/// </summary>
	public interface IPlayerScore
	{
		#region Variables & Properties
		int Id { get; }
		string Name { get; }

		int Wins { get; }
		int W { get; }
		int Ties { get; }
		int T { get; }
		int Losses { get; }
		int L { get; }

		int GameScore { get; }
		int OpponentsScore { get; }
		int PointsScore { get; }

		/// <summary>
		/// In the case of a "ranged" rank, returns the minimum.
		/// Ex: Rank 5-8 returns 5.
		/// </summary>
		int Rank { get; set; }
		#endregion

		#region Methods
		/// <summary>
		/// Get an array of [W, L, T], ordered by Records enum.
		/// </summary>
		/// <returns>int[3] array showing [W, L, T]</returns>
		int[] GetRecord();

		/// <summary>
		/// Replace the player's information.
		/// </summary>
		/// <param name="_id">New player ID</param>
		/// <param name="_name">New player name</param>
		void ReplacePlayerData(int _id, string _name);

		/// <summary>
		/// Add/subtract outcome of one Match to this PlayerScore.
		/// </summary>
		/// <param name="_outcome">Win, Loss, or Tie</param>
		/// <param name="_isAddition">Add or subtract these values</param>
		void AddMatchOutcome(Outcome _outcome, bool _isAddition);

		/// <summary>
		/// Add/subtract to the Score values of this PlayerScore.
		/// </summary>
		/// <param name="_gamesChange">Game wins/losses</param>
		/// <param name="_pointsChange">Points scored</param>
		/// <param name="_isAddition">Add or subtract these values</param>
		void UpdateScores(int _gamesChange, int _pointsChange, bool _isAddition);

		/// <summary>
		/// Add to the OpponentsScore value of this PlayerScore.
		/// </summary>
		/// <param name="_scoreChange">Amount to add</param>
		void AddToOpponentsScore(int _scoreChange);

		/// <summary>
		/// Get a score representative of this player's W/L record.
		/// </summary>
		/// <param name="_matchWinValue">Value of each Win (def: 2)</param>
		/// <param name="_matchTieValue">Value of each Tie (def: 1)</param>
		/// <param name="_matchLossValue">Value of each Loss (def: 0)</param>
		/// <returns>Score</returns>
		int CalculateScore(int _matchWinValue, int _matchTieValue, int _matchLossValue);

		/// <summary>
		/// Set OpponentsScore = 0.
		/// </summary>
		void ResetOpponentsScore();

		/// <summary>
		/// Reset this object's W/L Record & score values.
		/// </summary>
		void ResetScore();
		#endregion
	}
}
