using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatabaseLib;

namespace Tournament.Structure
{
	public class DoubleElimBracket : SingleElimBracket
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
		//protected Dictionary<int, Match> LowerMatches
		//public int NumberOfLowerRounds
		//protected Match grandFinal
		//public IMatch GrandFinal
		//public int NumberOfMatches
		//protected int MatchWinValue = 0
		//protected int MatchTieValue = 0
		#endregion

		#region Ctors
		public DoubleElimBracket(List<IPlayer> _players, int _maxGamesPerMatch = 1)
			: base(_players, _maxGamesPerMatch)
		{
			BracketType = BracketType.DOUBLE;
		}
		public DoubleElimBracket()
			: this(new List<IPlayer>())
		{ }
		public DoubleElimBracket(BracketModel _model)
			: base(_model)
		{
			this.NumberOfLowerRounds = 0;
			if (_model.Matches.Count > 0)
			{
				if (CalculateTotalLowerBracketMatches(Players.Count) > 0)
				{
					int numOfGrandFinal = _model.Matches.Count;

					// Create Matches from MatchModels.
					// This is extending SEB's method, so all upper bracket Matches are already done.
					foreach (MatchModel mm in _model.Matches.OrderBy(m => m.MatchNumber))
					{
						if (Matches.ContainsKey(mm.MatchNumber))
						{
							// Case 1: match is upper bracket:
							continue;
						}
						if (mm.MatchNumber == numOfGrandFinal)
						{
							// Case 2: match is grand final:
							this.grandFinal = new Match(mm);
						}
						else
						{
							// Case 3: match is lower bracket:
							Match match = new Match(mm);
							LowerMatches.Add(match.MatchNumber, match);
							this.NumberOfLowerRounds = Math.Max(NumberOfLowerRounds, match.RoundIndex);
						}
					}
				}
				this.NumberOfMatches = Matches.Count + LowerMatches.Count;
				if (null != grandFinal)
				{
					++NumberOfMatches;
				}
			}

			RecalculateRankings();
			if (grandFinal?.IsFinished ?? false)
			{
				this.IsFinished = true;
			}

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
		/// If there are <4 players, nothing will be made.
		/// This method first calls the base (Single Elim) version,
		/// which creates the upper bracket.
		/// </summary>
		/// <param name="_gamesPerMatch">Max games for every Match</param>
		public override void CreateBracket(int _gamesPerMatch = 1)
		{
			if (Players.Count < 4)
			{
				return;
			}

			// The base/single-elim method creates the upper bracket:
			base.CreateBracket(_gamesPerMatch);

			/*
			 * Similar to the SEB version, we're going to create a list of rounds.
			 * Each of these is a lower-bracket round.
			 * Again, we order the rounds back-to-front.
			*/
			List<List<Match>> roundList = new List<List<Match>>();
			int totalMatches = CalculateTotalLowerBracketMatches(Players.Count);
			int numMatches = 0;
			int r = 0;

			// Create the Matches
			while (numMatches < totalMatches)
			{
				// Go through the while loop once per round.
				// Add a new round and populate it:
				roundList.Add(new List<Match>());
				for (int i = 0;
					i < Math.Pow(2, r / 2) && numMatches < totalMatches;
					++i, ++numMatches)
				{
					// Go through the for loop once per match.
					// Add a Match to the current round list:
					Match m = new Match();
					m.SetMaxGames(_gamesPerMatch);
					roundList[r].Add(m);
				}
				++r;
			}

			// Assign Match Numbers (start counting after upper bracket matches)
			int matchNum = 1 + Matches.Count;
			for (r = roundList.Count - 1; r >= 0; --r)
			{
				foreach (Match match in roundList[r])
				{
					match.SetMatchNumber(matchNum++);
				}
			}

			/*
			 * Tie Matches together.
			 * This gets a little complicated. Three main cases:
			 * 1) First round:
			 *    - Both players per match come from the UB.
			 * 2) Previous round has 2x the matches:
			 *    - Both players per match come from the prev LB round.
			 * 3) Previous round has == the matches:
			 *    - One player from UB, one from the prev LB round.
			 * In addition, we need to "re-seed" the lower bracket,
			 * so that if Jon beats Bob in UB, then loses his next match,
			 * they don't get an immediate rematch.
			*/

			// FlipSeeds is the current method of re-seeding.
			// Every round that gets players from the UB
			// will toggle this bool to flip the order those players come down.
			// This isn't perfect, but it works well for any bracket with <32 players.
			bool flipSeeds = true;
			for (r = roundList.Count - 2; r >= 0; --r)
			{
				bool rIndexIsEven = (0 == r % 2) ? true : false;
				if (rIndexIsEven && roundList[r + 1].Count == roundList[r].Count)
				{
					// Round is "normal," but one Player is coming from Upper Bracket.
					for (int m = 0; m < roundList[r].Count; ++m)
					{
						// Calculate the UB round # that's sending its loser here:
						List<IMatch> upperRound = GetRound(NumberOfRounds - (r / 2));
						int currNum = roundList[r][m].MatchNumber;

						// Assign prev/next matchup indexes:
						if (flipSeeds)
						{
							roundList[r][m].AddPreviousMatchNumber(upperRound[upperRound.Count - 1 - m].MatchNumber);
							Matches[upperRound[upperRound.Count - 1 - m].MatchNumber].SetNextLoserMatchNumber(currNum);
						}
						else
						{
							roundList[r][m].AddPreviousMatchNumber(upperRound[m].MatchNumber);
							Matches[upperRound[m].MatchNumber].SetNextLoserMatchNumber(currNum);
						}
						// ************* THIS ISN'T QUITE RIGHT (RE-SEED ORDER)

						roundList[r][m].AddPreviousMatchNumber(roundList[r + 1][m].MatchNumber);
						roundList[r + 1][m].SetNextMatchNumber(currNum);
					}

					// Flip the re-seeding bool:
					flipSeeds = !flipSeeds;
				}
				else if (!rIndexIsEven && roundList[r + 1].Count == (roundList[r].Count * 2))
				{
					// Round is "normal," and both Players come from the lower bracket.
					for (int m = 0; m < roundList[r].Count; ++m)
					{
						// Assign prev/next matchup indexes
						int currNum = roundList[r][m].MatchNumber;

						// [r][3] <-> [r+1][6]
						roundList[r][m].AddPreviousMatchNumber(roundList[r + 1][m * 2].MatchNumber);
						roundList[r + 1][m * 2].SetNextMatchNumber(currNum);

						// [r][4] <-> [r+1][7]
						roundList[r][m].AddPreviousMatchNumber(roundList[r + 1][m * 2 + 1].MatchNumber);
						roundList[r + 1][m * 2 + 1].SetNextMatchNumber(currNum);
					}
				}
				else
				{
					// Round is abnormal. Case is not possible
					// (unless we later decide to include it)
				}

				//flipSeeds = !flipSeeds;
			}

			// As stated above, the "first" LB round is its own special case.
			// Here, we manually set its previous match numbers:
			r = roundList.Count - 1;
			if (r >= 0)
			{
				// We have enough teams to have created a Lower Bracket.
				// Manually update the first Lower round,
				// and create a Grand Final match.

				for (int m = 0; m < roundList[r].Count; ++m)
				{
					// Calculate which UB round sends its losing players here:
					List<IMatch> upperRound = GetRound(NumberOfRounds - (r / 2 + 1));
					int currNum = roundList[r][m].MatchNumber;

					// Assign prev/next matchup indexes for FIRST round.
					// (both teams come from Upper Bracket)
					roundList[r][m].AddPreviousMatchNumber(upperRound[m * 2].MatchNumber);
					Matches[upperRound[m * 2].MatchNumber].SetNextLoserMatchNumber(currNum);

					roundList[r][m].AddPreviousMatchNumber(upperRound[m * 2 + 1].MatchNumber);
					Matches[upperRound[m * 2 + 1].MatchNumber].SetNextLoserMatchNumber(currNum);
				}

				// Create a Grand Final:
				grandFinal = new Match();
				grandFinal.SetMatchNumber(matchNum);
				grandFinal.SetMaxGames(_gamesPerMatch);
				grandFinal.SetRoundIndex(1);
				grandFinal.SetMatchIndex(1);
				grandFinal.AddPreviousMatchNumber(Matches.Count);
				grandFinal.AddPreviousMatchNumber(roundList[0][0].MatchNumber);

				// Connect UB final and LB final to Grand Final:
				roundList[0][0].SetNextMatchNumber(grandFinal.MatchNumber);
				Matches[Matches.Count].SetNextMatchNumber(grandFinal.MatchNumber);

				/* 
				 * Now we've generated our Bracket.
				 * Time to move the Matches from the double array
				 * into an easily accessible Dictionary for storage.
				 * While we do this, we'll update their data: Round and Match indexes.
				*/
				NumberOfLowerRounds = roundList.Count;
				for (r = 0; r < roundList.Count; ++r)
				{
					for (int m = 0; m < roundList[r].Count; ++m)
					{
						roundList[r][m].SetRoundIndex(roundList.Count - r);
						roundList[r][m].SetMatchIndex(m + 1);
						LowerMatches.Add(roundList[r][m].MatchNumber, roundList[r][m]);
					}
				}
				NumberOfMatches += (LowerMatches.Count + 1);
			}
		}

		/// <summary>
		/// Verifies this bracket's status is legal.
		/// This is called before allowing play to begin.
		/// Makes sure we have at least 4 Players.
		/// Extends the Bracket class's method.
		/// </summary>
		/// <returns>true if okay, false if errors</returns>
		public override bool Validate()
		{
			if ((Players?.Count ?? 0) < 4)
			{
				return false;
			}

			return true;
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Finds an eliminated player's rank for a double-elim bracket.
		/// </summary>
		/// <param name="_matchNumber">Number of finished Match</param>
		/// <returns>Player's rank</returns>
		protected override int CalculateRank(int _matchNumber)
		{
			int rank = 2; // 2 = GrandFinal loser

			if (LowerMatches?.ContainsKey(_matchNumber) ?? false)
			{
				// Standard case: lower bracket match.
				Match match = GetInternalMatch(_matchNumber);
				rank = NumberOfMatches - GetLowerRound(match.RoundIndex)[0].MatchNumber + 2;
			}
			else if (Matches?.ContainsKey(_matchNumber) ?? false)
			{
				// Special case: upper bracket "play-in" round.
				// These special rounds eliminate their losers
				// instead of sending them to the lower bracket.
				rank = Convert.ToInt32(Math.Pow(2, NumberOfRounds - 1) + 1);
			}

			return rank;
		}

		/// <summary>
		/// Clears the Rankings, and recalculates them from the Matches list.
		/// Finds every eliminated player, calculates his rank, and adds him to the Rankings.
		/// Sorts the Rankings.
		/// If no players have been eliminated, the Rankings will be an empty list.
		/// Extends KnockoutBracket's method.
		/// </summary>
		protected override void RecalculateRankings()
		{
			// The base method already adds all LB losers to Rankings.
			// It also handles the Grand Final.
			base.RecalculateRankings();

			// If there's no "play-in" round, we're done:
			if (0 == NumberOfMatches || Matches[1].NextLoserMatchNumber > 0)
			{
				return;
			}

			// Get the play-in round (first round in UB):
			List<IMatch> firstRound = GetRound(1)
				//.Where(m => m.IsFinished && m.NextLoserMatchNumber > 0)
				.ToList();
			foreach (IMatch match in firstRound)
			{
				// Add each losing Player to the Rankings:
				int rank = CalculateRank(match.MatchNumber);
				IPlayer losingPlayer = match.Players[
					(PlayerSlot.Defender == match.WinnerSlot)
					? (int)PlayerSlot.Challenger
					: (int)PlayerSlot.Defender];
				Rankings.Add(new PlayerScore(losingPlayer.Id, losingPlayer.Name, rank));
			}

			// This sorts the list by player rank.
			// Tied ranks are ordered by player seed.
			Rankings.Sort(SortRankingRanks);
		}

		/// <summary>
		/// Finds the amount of lower bracket Matches to create,
		/// based on the playercount.
		/// If players <= 4, returns 0.
		/// </summary>
		/// <param name="_numPlayers">Playercount</param>
		/// <returns>Amount of LB Matches to create</returns>
		private int CalculateTotalLowerBracketMatches(int _numPlayers)
		{
			if (_numPlayers < 4)
			{
				return 0;
			}

			int normalizedPlayers = 2;
			while (true)
			{
				int next = normalizedPlayers * 2;
				if (next <= _numPlayers)
				{
					normalizedPlayers = next;
				}
				else
				{
					break;
				}
			}
			return (normalizedPlayers - 2);
		}
		#endregion
	}
}
