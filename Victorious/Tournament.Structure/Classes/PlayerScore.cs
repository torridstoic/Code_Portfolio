using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	/// <summary>
	/// PlayerScores represent each Player in bracket Rankings lists.
	/// Each of these objects contains information about a Player's performance:
	/// W/L record and various point values.
	/// </summary>
	public class PlayerScore : IPlayerScore
	{
		#region Variables & Properties
		public int Id
		{ get; private set; }
		public string Name
		{ get; private set; }

		private PlayerRecord MatchRecord
		{ get; set; }
		public int Wins
		{ get { return MatchRecord.Wins; } }
		public int W
		{ get { return MatchRecord.Wins; } }
		public int Ties
		{ get { return MatchRecord.Ties; } }
		public int T
		{ get { return MatchRecord.Ties; } }
		public int Losses
		{ get { return MatchRecord.Losses; } }
		public int L
		{ get { return MatchRecord.Losses; } }

		public int MatchScore
		{ get { return CalculateScore(2, 1, 0); } }
		public int OpponentsScore
		{ get; private set; }
		public int GameScore
		{ get; private set; }
		public int PointsScore
		{ get; private set; }
		public int Rank
		{ get; set; }
		#endregion

		#region Ctors
		/// <summary>
		/// Constructor for ELIMINATION-type brackets.
		/// Sets scores = -1 (they're not necessary).
		/// </summary>
		/// <param name="_id">Player ID</param>
		/// <param name="_name">Player name</param>
		/// <param name="_rank">Player's rank</param>
		public PlayerScore(int _id, string _name, int _rank)
		{
			this.Id = _id;
			this.Name = _name;
			this.Rank = _rank;

			// MatchRecord isn't used, but a NULL value breaks the WebApp,
			// so initialize it here:
			MatchRecord = new PlayerRecord();
			OpponentsScore = GameScore = PointsScore = -1;
		}

		/// <summary>
		/// Constructor for SCORE-based brackets.
		/// Default sets Rank = 1 and scores = 0.
		/// </summary>
		/// <param name="_id">Player ID</param>
		/// <param name="_name">Player name</param>
		public PlayerScore(int _id, string _name)
		{
			this.Id = _id;
			this.Name = _name;
			Rank = 1;

			MatchRecord = new PlayerRecord();
			OpponentsScore = GameScore = PointsScore = 0;
		}

		private PlayerScore()
			: this(0, "")
		{ }
		#endregion

		#region Public Methods
		/// <summary>
		/// Creates a int[3] containing this player's W/L/T record.
		/// Array is ordered/referenced by the Records enum.
		/// </summary>
		/// <returns>Int array showing player's record</returns>
		public int[] GetRecord()
		{
			int[] record = new int[Enum.GetNames(typeof(Record)).Length];
			record[(int)Record.Wins] = Wins;
			record[(int)Record.Losses] = Losses;
			record[(int)Record.Ties] = Ties;
			return record;
		}

		/// <summary>
		/// Replaces the player's information: ID and name.
		/// This can be used to modify the Rankings when a Player is replaced in a Bracket.
		/// </summary>
		/// <param name="_id">ID of player</param>
		/// <param name="_name">Name of player</param>
		public void ReplacePlayerData(int _id, string _name)
		{
			this.Id = _id;
			this.Name = _name;
		}

		/// <summary>
		/// Adds or subtracts a Match Outcome.
		/// </summary>
		/// <param name="_outcome">Win, Loss, or Tie</param>
		/// <param name="_isAddition">Add or subtract</param>
		public void AddMatchOutcome(Outcome _outcome, bool _isAddition)
		{
			MatchRecord.AddOutcome(_outcome, _isAddition);
		}

		/// <summary>
		/// Adds or subtracts to the score values.
		/// </summary>
		/// <param name="_gamesChange">Game wins/losses</param>
		/// <param name="_pointsChange">Points scored (within Games)</param>
		/// <param name="_isAddition">Add or subtract</param>
		public void UpdateScores(int _gamesChange, int _pointsChange, bool _isAddition)
		{
			int add = (_isAddition) ? 1 : -1;
			GameScore += (_gamesChange * add);
			PointsScore += (_pointsChange * add);
		}

		/// <summary>
		/// Adds to the OpponentsScore value.
		/// </summary>
		/// <param name="_scoreChange">Amount to add</param>
		public void AddToOpponentsScore(int _scoreChange)
		{
			OpponentsScore += _scoreChange;
		}

		/// <summary>
		/// Calculates a value representative of this W/L record.
		/// This value can be used to compare players' records numerically.
		/// </summary>
		/// <param name="_matchWinValue">Value for each Win</param>
		/// <param name="_matchTieValue">Value for each Tie</param>
		/// <param name="_matchLossValue">Value for each Loss</param>
		/// <returns></returns>
		public int CalculateScore(int _matchWinValue, int _matchTieValue, int _matchLossValue)
		{
			int score = (MatchRecord.Wins * _matchWinValue);
			score += (MatchRecord.Ties * _matchTieValue);
			score += (MatchRecord.Losses * _matchLossValue);
			return score;
		}

		/// <summary>
		/// Resets OpponentsScore to 0.
		/// </summary>
		public void ResetOpponentsScore()
		{
			OpponentsScore = 0;
		}

		/// <summary>
		/// Resets W/L record and all score values to 0.
		/// </summary>
		public void ResetScore()
		{
			MatchRecord.Reset();
			GameScore = PointsScore = 0;
			ResetOpponentsScore();
		}

		#region Obsolete Methods
		public void AddMatchOutcome(Outcome _outcome, int _gameScore, int _pointsScore, bool _isAddition)
		{
			int add = (_isAddition) ? 1 : -1;
			MatchRecord.AddOutcome(_outcome, _isAddition);
			GameScore += (_gameScore * add);
			PointsScore += (_pointsScore * add);
		}
		public void AddToScore(int _matchScore, int _gameScore, int _pointsScore, bool _isAddition)
		{
			switch (_matchScore)
			{
				case 2:
					AddMatchOutcome(Outcome.Win, _gameScore, _pointsScore, _isAddition);
					break;
				case 1:
					AddMatchOutcome(Outcome.Tie, _gameScore, _pointsScore, _isAddition);
					break;
				case 0:
					AddMatchOutcome(Outcome.Loss, _gameScore, _pointsScore, _isAddition);
					break;
			}
		}
		#endregion
		#endregion
	}
}
