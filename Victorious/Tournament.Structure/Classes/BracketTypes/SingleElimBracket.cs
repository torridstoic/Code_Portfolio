using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatabaseLib;

namespace Tournament.Structure
{
	public class SingleElimBracket : KnockoutBracket
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
		public SingleElimBracket(List<IPlayer> _players, int _maxGamesPerMatch = 1)
		{
			if (null == _players)
			{
				throw new ArgumentNullException("_players");
			}

			Players = _players;
			Id = 0;
			BracketType = BracketType.SINGLE;

			CreateBracket(_maxGamesPerMatch);
		}
		public SingleElimBracket()
			: this(new List<IPlayer>())
		{ }
		public SingleElimBracket(BracketModel _model)
		{
			// Call a helper method to copy the bracket status fields,
			// and to load the playerlist:
			SetDataFromModel(_model);

			/*
			 * Since this method is extended in child classes,
			 * we may be loading a lower bracket.
			 * We need to add a couple checks here to make sure
			 * we don't accidentally load lower bracket Matches
			 * into our upper bracket.
			 */
			int totalUBMatches = Players.Count - 1;
			if (_model.Matches.Count > 0)
			{
				foreach (MatchModel mm in _model.Matches.OrderBy(m => m.MatchNumber))
				{
					if (mm.MatchNumber <= totalUBMatches)
					{
						Matches.Add(mm.MatchNumber, new Match(mm));
					}
					else
					{
						// Match doesn't belong in upper bracket, so we're done:
						break;
					}
				}
				this.NumberOfMatches = Matches.Count;
				this.NumberOfRounds = Matches.Values
					.Max(m => m.RoundIndex);
			}

			if (BracketType.SINGLE == BracketType)
			{
				RecalculateRankings();

				if (this.IsFinalized && false == Validate())
				{
					throw new BracketValidationException
						("Bracket is Finalized but not Valid!");
				}
			}
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Uses the playerlist to generate the bracket structure & Matches.
		/// This creates & populates all the Match objects, and ties them together.
		/// If any Matches already exist, they will be deleted first.
		/// If there are <2 players, nothing will be made.
		/// This method is extended in child classes: Double-Elim and GSL brackets.
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

			#region Create the Bracket
			/*
			 * Process: to achieve the correct number of Matches in any Bracket
			 * (even an abnormally sized one; say, for 11 players),
			 * we start at the final match. Then we move backward,
			 * making twice as many Matches for each new round,
			 * until we hit the correct number of Matches.
			 * If the first round has a weird number of Matches,
			 * we'll know our Bracket is abnormally sized and we can deal with it.
			*/

			// totalMatches is the number of Matches we'll create:
			int totalMatches = Players.Count - 1;
			int numMatches = 0;
			
			// roundList is how we'll temporarily create & store Matches.
			// This is a list of rounds (each round is a list of Matches).
			// Rounds are ordered back-to-front: first is the final match.
			List<List<Match>> roundList = new List<List<Match>>();

			// Create the Matches, round-by-round:
			for (int r = 0; numMatches < totalMatches; ++r)
			{
				roundList.Add(new List<Match>());
				for (int i = 0;
					i < Math.Pow(2, r) && numMatches < totalMatches;
					++i, ++numMatches)
				{
					// Add new Matches per round:
					Match m = new Match();
					m.SetMaxGames(_gamesPerMatch);
					roundList[r].Add(m);
				}
			}

			// Assign Match Numbers
			int matchNum = 1;
			for (int r = roundList.Count - 1; r >= 0; --r)
			{
				foreach (Match match in roundList[r])
				{
					match.SetMatchNumber(matchNum++);
				}
			}

			// Tie Matches Together
			for (int r = 0; r + 1 < roundList.Count; ++r)
			{
				if (roundList[r + 1].Count == (roundList[r].Count * 2))
				{
					// "Normal" rounds: twice as many Matches
					// (example: 4 Matches -> 8 Matches)
					for (int m = 0; m < roundList[r].Count; ++m)
					{
						// Assign prev/next matchup numbers
						int currNum = roundList[r][m].MatchNumber;

						// [r][3] <-> [r+1][6]
						roundList[r][m].AddPreviousMatchNumber(roundList[r + 1][m * 2].MatchNumber);
						roundList[r + 1][m * 2].SetNextMatchNumber(currNum);

						// [r][3] <-> [r+1][7]
						roundList[r][m].AddPreviousMatchNumber(roundList[r + 1][m * 2 + 1].MatchNumber);
						roundList[r + 1][m * 2 + 1].SetNextMatchNumber(currNum);
					}
				}
				// Else: round is abnormal. Ignore it for now (we'll handle it later).
			}
			#endregion

			#region Assign the Players
			/*
			 * In a correctly seeded bracket, high seeds are split apart.
			 * For instance, the 1 and 2 seeds will each have their own bracket "branch."
			 * The solution to accomplishing this is to assign seeds
			 * to the FINAL round. From there, we move them backward one round.
			 * We fill the Matches in that round (with seeds 3 & 4), then repeat.
			 * If this is done all the way to the first round, our seeds are correctly split.
			*/

			// Assign top two seeds to final match
			int pIndex = 0;
			roundList[0][0].AddPlayer(Players[pIndex++]);
			roundList[0][0].AddPlayer(Players[pIndex++]);

			for (int r = 0; r + 1 < roundList.Count; ++r)
			{
				// We're shifting back one player for each Match in the prev round
				int prevRoundMatches = roundList[r + 1].Count;

				if ((roundList[r].Count * 2) > prevRoundMatches)
				{
					// Abnormal round ahead: we need to allocate prevMatchIndexes
					// to correctly distribute bye seeds
					int prevMatchNumber = 1;

					for (int m = 0; m < roundList[r].Count; ++m)
					{
						int[] playerIndexes = new int[2] { -1, -1 };
						for (int i = 0; i < Players.Count; ++i)
						{
							if (Players[i].Equals(roundList[r][m].Players[0]))
							{
								playerIndexes[0] = i;
							}
							else if (Players[i].Equals(roundList[r][m].Players[1]))
							{
								playerIndexes[1] = i;
							}
						}

						int playersToMove = 0;
						foreach (int p in playerIndexes)
						{
							if (p >= pIndex - prevRoundMatches)
							{
								++playersToMove;
							}
						}
						for (int i = 0; i < playerIndexes.Length; ++i)
						{
							if (playerIndexes[i] >= pIndex - prevRoundMatches)
							{
								roundList[r][m].AddPreviousMatchNumber(prevMatchNumber,
									(2 == playersToMove)
									? (PlayerSlot)i : PlayerSlot.Challenger);
								roundList[r + 1][prevMatchNumber - 1].SetNextMatchNumber
									(roundList[r][m].MatchNumber);
								++prevMatchNumber;
							}
						}
					}
				}

				for (int m = 0; m < roundList[r].Count; ++m)
				{
					// For each match, shift/reassign all Players to the prev bracket level
					// If prev level is abnormal, only shift 1 (or 0) teams
					foreach (int n in roundList[r][m].PreviousMatchNumbers)
					{
						if (n > 0)
						{
							// ReassignPlayers() handles the task of moving Players.
							// Any Players to be moved from this Match will be removed.
							// Then, using the match number references,
							// those Players will be added to the correct Matches.
							ReassignPlayers(roundList[r][m], roundList[r + 1]);
							break;
						}
					}
				}

				// Now that the correct Players have been shifted back,
				// we need to fill in the rest of this round's Player slots
				// from our master playerlist.
				for (int prePlayers = pIndex - 1; prePlayers >= 0; --prePlayers)
				{
					for (int m = 0; m < prevRoundMatches; ++m)
					{
						if (roundList[r + 1][m].Players.Contains(Players[prePlayers]))
						{
							// Add prev round's Players (according to seed) from the master list.
							roundList[r + 1][m].AddPlayer(Players[pIndex++]);
							break;
						}
					}
				}
			}
			#endregion

			#region Set Bracket Member Variables
			/* 
			 * Now we've generated our Bracket.
			 * Time to move the Matches from the double array
			 * into an easily accessible Dictionary for storage.
			 * While we do this, we'll update their data: Round and Match indexes.
			*/
			NumberOfRounds = roundList.Count;
			for (int r = 0; r < roundList.Count; ++r)
			{
				for (int m = 0; m < roundList[r].Count; ++m)
				{
					roundList[r][m].SetRoundIndex(roundList.Count - r);
					roundList[r][m].SetMatchIndex(m + 1);
					Matches.Add(roundList[r][m].MatchNumber, roundList[r][m]);
				}
			}
			NumberOfMatches = Matches.Count;
			#endregion
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Finds an eliminated player's rank for a single-elim bracket.
		/// </summary>
		/// <param name="_matchNumber">Number of finished Match</param>
		/// <returns>Player's rank</returns>
		protected override int CalculateRank(int _matchNumber)
		{
			int round = GetInternalMatch(_matchNumber).RoundIndex;
			return Convert.ToInt32(Math.Pow(2, NumberOfRounds - round) + 1);
		}

		/// <summary>
		/// Takes the Players from the given Match
		/// and reassigns them to the correct Matches in the previous round.
		/// This is used during bracket generation to correctly split up seeds.
		/// If the Players do not have a previous Match to move to, they remain.
		/// Otherwise, they are removed from the given Match.
		/// </summary>
		/// <param name="_currMatch">Match to move Players out of</param>
		/// <param name="_prevRound">Previous round: to move Players into</param>
		private void ReassignPlayers(Match _currMatch, List<Match> _prevRound)
		{
			if (null == _currMatch ||
				0 == (_prevRound?.Count ?? 0))
			{
				throw new NullReferenceException
					("NULL error in calling ReassignPlayers()...");
			}

			int playersToMove = 2;
			foreach (int n in _currMatch.PreviousMatchNumbers)
			{
				if (n < 0)
				{
					// If a PreviousMatchNumber is -1, that Player will stay here.
					--playersToMove;
				}
			}
			foreach (Match match in _prevRound)
			{
				// Check previous round Matches to find the correct one(s):
				for (int i = 0; i < _currMatch.PreviousMatchNumbers.Length; ++i)
				{
					if (match.MatchNumber == _currMatch.PreviousMatchNumbers[i])
					{
						// Add the Player to the found Match:
						match.AddPlayer(_currMatch.Players[i]);
						// Remove the Player from the given Match:
						_currMatch.RemovePlayer(_currMatch.Players[i].Id);

						--playersToMove;
					}
				}

				if (0 == playersToMove)
				{
					break;
				}
			}
		}
		#endregion
	}
}
