using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatabaseLib;

namespace Tournament.Structure
{
	public class StepladderBracket : KnockoutBracket
	{
		#region Variables & Properties
		//public int Id
		//public BracketType BracketType
		//public bool IsFinalized
		//public bool IsFinished
		//public List<IPlayer> Players
		//public List<IPlayerScore> Rankings
		//public int AdvancingPlayers
		//public int MaxRounds = 0
		//protected Dictionary<int, Match> Matches
		//public int NumberOfRounds
		//protected Dictionary<int, Match> LowerMatches = empty
		//public int NumberOfLowerRounds = 0
		//protected Match grandFinal = null
		//public IMatch GrandFinal = null
		//public int NumberOfMatches
		//protected int MatchWinValue = 0
		//protected int MatchTieValue = 0
		#endregion

		#region Ctors
		public StepladderBracket(List<IPlayer> _players, int _maxGamesPerMatch = 1)
		{
			if (null == _players)
			{
				throw new ArgumentNullException("_players");
			}

			Players = _players;
			Id = 0;
			BracketType = BracketType.STEP;

			CreateBracket(_maxGamesPerMatch);
		}
		public StepladderBracket()
			: this(new List<IPlayer>())
		{ }
		public StepladderBracket(BracketModel _model)
		{
			// Call a helper method to copy the bracket status fields,
			// and to load the playerlist:
			SetDataFromModel(_model);

			if (_model.Matches.Count > 0)
			{
				foreach (MatchModel mm in _model.Matches)
				{
					// Create Matches from all MatchModels:
					Matches.Add(mm.MatchNumber, new Match(mm));
				}
				this.NumberOfMatches = Matches.Count;
				this.NumberOfRounds = Matches.Count;
			}

			RecalculateRankings();

			if (this.IsFinalized && false == Validate())
			{
				throw new BracketValidationException
					("Bracket is Finalized but not Valid!");
			}
		}
		#endregion

		#region Public Methods

		/// <summary>
		/// Uses the playerlist to generate the bracket structure & Matches.
		/// This creates & populates all the Match objects, and ties them together.
		/// If any Matches already exist, they will be deleted first.
		/// If there are <2 players, nothing will be made.
		/// </summary>
		/// <param name="_gamesPerMatch">Max games for every Match</param>
		public override void CreateBracket(int _gamesPerMatch = 1)
		{
			// First, clear any existing Matches and results:
			ResetBracketData();
			if (Players.Count < 2)
			{
				return;
			}

			/*
			 * A stepladder/gauntlet bracket has just one match per round.
			 * The bottom two seeds face off in the first round,
			 * and the winner of each match moves on to face the next lowest seed.
			 * The top seed is placed directly in the final round.
			*/

			// Create Matches and assign Players:
			int playerIndex = Players.Count - 2;
			for (int i = 1; playerIndex >= 0; ++i, --playerIndex)
			{
				Match match = new Match();
				match.SetMatchNumber(i);
				match.SetRoundIndex(i);
				match.SetMatchIndex(1);
				match.SetMaxGames(_gamesPerMatch);
				// Every Match has one Player directly placed:
				match.AddPlayer(Players[playerIndex], PlayerSlot.Defender);

				Matches.Add(match.MatchNumber, match);
			}
			// Now, manually add the lowest seed to the first Match:
			Matches[1].AddPlayer(Players.Last(), PlayerSlot.Challenger);

			// Set Bracket fields:
			NumberOfMatches = Matches.Count;
			NumberOfRounds = Matches.Count;

			// Tie Matches together:
			// (easy in Stepladder: each MatchNumber leads to MatchNumber+1)
			for (int m = 1; m <= NumberOfMatches; ++m)
			{
				if (m > 1)
				{
					Matches[m].AddPreviousMatchNumber(m - 1, PlayerSlot.Challenger);
				}
				if (m < NumberOfMatches)
				{
					Matches[m].SetNextMatchNumber(m + 1);
				}
			}
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Finds an eliminated player's rank for a stepladder bracket.
		/// </summary>
		/// <param name="_matchNumber">Number of finished Match</param>
		/// <returns>Player's rank</returns>
		protected override int CalculateRank(int _matchNumber)
		{
			return (2 + NumberOfMatches - _matchNumber);
		}
		#endregion
	}
}
