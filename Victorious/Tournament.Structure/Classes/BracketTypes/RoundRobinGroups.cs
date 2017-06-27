using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatabaseLib;

namespace Tournament.Structure
{
	public class RoundRobinGroups : GroupStage
	{
		#region Variables & Properties
		//public int Id
		//public BracketType BracketType
		//public bool IsFinalized
		//public bool IsFinished
		//public List<IPlayer> Players
		//public List<IPlayerScore> Rankings
		//public int AdvancingPlayers
		//public int MaxRounds
		//protected Dictionary<int, Match> Matches = empty
		//public int NumberOfRounds
		//protected Dictionary<int, Match> LowerMatches = empty
		//public int NumberOfLowerRounds = 0
		//protected Match grandFinal = null
		//public IMatch GrandFinal = null
		//public int NumberOfMatches
		//protected int MatchWinValue
		//protected int MatchTieValue
		//public int NumberOfGroups
		//protected List<List<IPlayerScore>> GroupRankings
		#endregion

		#region Ctors
		public RoundRobinGroups(List<IPlayer> _players, int _numberOfGroups, int _maxGamesPerMatch = 1, int _numberOfRounds = 0)
		{
			if (null == _players)
			{
				throw new ArgumentNullException("_players");
			}

			Players = _players;
			Id = 0;
			BracketType = BracketType.RRGROUP;
			NumberOfGroups = _numberOfGroups;
			MaxRounds = _numberOfRounds;
			MatchWinValue = 2;
			MatchTieValue = 1;

			CreateBracket(_maxGamesPerMatch);
		}
		public RoundRobinGroups()
			: this(new List<IPlayer>(), 0, 0)
		{ }
		public RoundRobinGroups(BracketModel _model)
		{
			SetDataFromModel(_model);
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Uses the playerlist to generate the bracket structure & matchups.
		/// This divides the players into the given number of groups,
		/// then creates & populates all the Match objects.
		/// If any Matches already exist, they will be deleted first.
		/// If there are too few players, nothing will be made.
		/// </summary>
		/// <param name="_gamesPerMatch">Max games for every Match</param>
		public override void CreateBracket(int _gamesPerMatch = 1)
		{
			// First, clear any existing Matches and results:
			ResetBracketData();
			if (NumberOfGroups < 2 ||
				NumberOfGroups * 2 > Players.Count)
			{
				return;
			}

			// DividePlayersIntoGroups() uses our chosen method to separate the playerlist:
			List<List<IPlayer>> playerGroups = DividePlayersIntoGroups();
			GroupRankings.Capacity = NumberOfGroups;

			List<IBracket> groups = new List<IBracket>();
			for (int g = 0; g < playerGroups.Count; ++g)
			{
				// For each group, generate a full round robin bracket:
				groups.Add(new RoundRobinBracket(playerGroups[g], _gamesPerMatch, MaxRounds));
			}

			/*
			 * So now we've generated all of our Matches.
			 * We want to copy them into our main Bracket, but there's a problem:
			 * each new round robin numbers their Matches from 1.
			 * We need unique MatchNumbers, so we go through and create
			 * new "copy" Matches, but with those unique MatchNumbers we need.
			*/

			// For every group...
			for (int g = 0; g < groups.Count; ++g)
			{
				// For every Match within that group...
				for (int m = 1; m <= groups[g].NumberOfMatches; ++m)
				{
					++NumberOfMatches;
					IMatch currMatch = groups[g].GetMatch(m);

					// Copy the Match into our main Bracket:
					if (0 == g)
					{
						Matches.Add(currMatch.MatchNumber, (currMatch as Match));
					}
					else
					{
						// If we're in a group after the first, we need to:
						// create a new Match, and give it a higher MatchNumber.
						Match match = new Match();
						match.SetMaxGames(currMatch.MaxGames);
						match.SetRoundIndex(currMatch.RoundIndex);
						match.SetMatchIndex(currMatch.MatchIndex);
						match.SetMatchNumber(NumberOfMatches);
						match.AddPlayer(currMatch.Players[(int)PlayerSlot.Defender]);
						match.AddPlayer(currMatch.Players[(int)PlayerSlot.Challenger]);

						Matches.Add(match.MatchNumber, match);
					}
					// Apply a GroupNumber to the Match:
					Matches[NumberOfMatches].SetGroupNumber(g + 1);
				}

				// Each round robin generated initial Rankings lists.
				// Copy them into Rankings and GroupRankings.
				// Since we COPY instead of recreating, modifying one PlayerScore
				// will modify it in both of these locations. Handy!
				Rankings.AddRange(groups[g].Rankings);
				GroupRankings.Add(groups[g].Rankings);
			}

			NumberOfRounds = Matches.Values
				.Max(m => m.RoundIndex);
			Rankings.Sort(SortRankingRanks);
		}

		/// <summary>
		/// Resets the state of all Matches and Rankings.
		/// Deletes all Games and sets scores to 0-0.
		/// If tiebreakers had been added, those Matches will be deleted.
		/// May fire MatchesModified and GamesDeleted events, if updates occur.
		/// May fire a RoundDeleted event, if tiebreakers are deleted.
		/// </summary>
		public override void ResetMatches()
		{
			base.ResetMatches();

			foreach (List<IPlayerScore> group in GroupRankings)
			{
				foreach (IPlayerScore ps in group)
				{
					// Reset each ranking in GroupRankings.
					// (this also resets the main Rankings objects)
					ps.Rank = 1;
					ps.ResetScore();
				}
				// Sort each group:
				group.Sort(SortRankingRanks);
			}

			// Sort the main Rankings too:
			Rankings.Sort(SortRankingRanks);
#if false
			Rankings.Clear();
			foreach (List<IPlayerScore> group in GroupRankings)
			{
				foreach (IPlayerScore playerScore in group)
				{
					playerScore.Rank = 1;
					playerScore.ResetScore();
				}

				Rankings.AddRange(group);
			}
#endif
		}

		/// <summary>
		/// Determines if any players, within each group, are tied.
		/// Criteria is W/L/T record only: scores are not taken into account here.
		/// If no groups are finished, returns false.
		/// </summary>
		/// <returns>true if any ties are found, false if none</returns>
		public override bool CheckForTies()
		{
			// For each group...
			for (int g = 0; g < NumberOfGroups; ++g)
			{
				// Only check if the group is finished:
				if (Matches.Values
					.Where(m => m.GroupNumber == 1 + g)
					.All(m => m.IsFinished))
				{
					// Calculate W/L/T scores for each player:
					int[] scores = new int[GroupRankings[g].Count];
					for (int i = 0; i < GroupRankings[g].Count; ++i)
					{
						scores[i] = GroupRankings[g][i]
							.CalculateScore(MatchWinValue, MatchTieValue, 0);

						if (i > 0 && scores[i] == scores[i - 1])
						{
							// Found a tie. We're finished:
							return true;
						}
					}
				}
			}
			
			// No ties in finished groups!
			return false;
		}

		/// <summary>
		/// Finds pairs/groups of tied players within each group,
		/// and creates any necessary tiebreaker Matches for them.
		/// This may result in one new Match or in multiple rounds.
		/// All new Matches will be put in new rounds on the "back" of the Bracket.
		/// If the Bracket has no ties, no Matches are created and false is returned.
		/// </summary>
		/// <returns>true if tiebreakers added, false if no ties</returns>
		public override bool GenerateTiebreakers()
		{
			List<MatchModel> createdMatches = new List<MatchModel>();

			// Do this for every group!
			for (int grp = 0; grp < NumberOfGroups; ++grp)
			{
				// Get the group's matches, and make sure they're all finished:
				List<Match> group = Matches.Values
					.Where(m => m.GroupNumber == 1 + grp)
					.ToList();
				if (group.Any(m => !m.IsFinished))
				{
					continue;
				}
				
				// Find the last round.
				// (any new Matches will be placed after this)
				int finalGroupRound = group
					.Max(m => m.RoundIndex);

				// Calculate W/L scores for all players:
				int[] scores = new int[GroupRankings[grp].Count];
				for (int i = 0; i < GroupRankings[grp].Count; ++i)
				{
					scores[i] = GroupRankings[grp][i]
						.CalculateScore(MatchWinValue, MatchTieValue, 0);
				}

				// Create lists for any tied players:
				List<List<int>> tiedBlocks = new List<List<int>>();
				for (int i = 0; i < GroupRankings[grp].Count - 1;)
				{
					int j = i + 1;
					for (; j < GroupRankings[grp].Count; ++j)
					{
						if (scores[i] != scores[j])
						{
							// Different scores; look for a new tie value.
							break;
						}

						if (i + 1 == j)
						{
							// NEW tied block:
							tiedBlocks.Add(new List<int>());
							tiedBlocks[tiedBlocks.Count - 1].Add(GroupRankings[grp][i].Id);
						}
						// If we're here, scores[j-1] == scores[j]:
						tiedBlocks[tiedBlocks.Count - 1].Add(GroupRankings[grp][j].Id);
					}

					// Increment the loop:
					i = j;
				}
				if (0 == tiedBlocks.Count)
				{
					// No ties in this group; continue to the next:
					continue;
				}
				// else:

				this.IsFinished = false;
				List<MatchModel> newMatchModels = new List<MatchModel>();

				// Create a temporary bracket for each tied block:
				List<IBracket> tiebreakerBrackets = new List<IBracket>();
				tiebreakerBrackets.Capacity = tiedBlocks.Count;
				foreach (List<int> block in tiedBlocks)
				{
					List<IPlayer> pList = new List<IPlayer>();
					foreach (int id in block)
					{
						pList.Add(Players.Find(p => p.Id == id));
					}
					tiebreakerBrackets.Add(new RoundRobinBracket(pList));
				}

				// Copy matches from new temp brackets onto the end of this bracket:
				for (int r = 1; ; ++r) // Run through once for each new round
				{
					// Round-by-round, make a list of matches to add:
					List<IMatch> currRound = new List<IMatch>();
					foreach (IBracket bracket in tiebreakerBrackets
						.Where(b => b.NumberOfRounds >= r))
					{
						currRound.AddRange(bracket.GetRound(r));
					}
					if (0 == currRound.Count)
					{
						// No more rounds to add; break out.
						break;
					}

					// Add the new round and copy applicable matches:
					++finalGroupRound;
					for (int m = 0; m < currRound.Count; ++m)
					{
						Match match = new Match();
						match.SetMatchNumber(++NumberOfMatches);
						match.SetRoundIndex(finalGroupRound);
						match.SetMatchIndex(m + 1);
						match.AddPlayer(currRound[m].Players[0]);
						match.AddPlayer(currRound[m].Players[1]);

						Matches.Add(match.MatchNumber, match);
						// Also add a model, for firing an event:
						createdMatches.Add(GetMatchModel(match));
					}
				}
				// Update Bracket.NumberOfRounds, if necessary.
				this.NumberOfRounds = Math.Max(NumberOfRounds, finalGroupRound);
			}

			if (createdMatches.Count > 0)
			{
				// Fire event to notify that new Matches were created, then we're done:
				OnRoundAdded(new BracketEventArgs(createdMatches));
				return true;
			}

			// No tiebreakers generated. False!
			return false;
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Sets this Bracket's main data from a related BracketModel.
		/// Data affected includes most fields, as well as the playerlist.
		/// </summary>
		/// <param name="_model">Related BracketModel</param>
		protected override void SetDataFromModel(BracketModel _model)
		{
			// Call the base (Bracket) method to set common data and playerlist:
			base.SetDataFromModel(_model);
			this.NumberOfGroups = _model.NumberOfGroups;

			if (_model.Matches.Count > 0)
			{
				foreach (MatchModel matchModel in _model.Matches)
				{
					// Convert each MatchModel to a Match, and add:
					Matches.Add(matchModel.MatchNumber, new Match(matchModel));
				}

				this.NumberOfMatches = Matches.Count;
				this.NumberOfRounds = Matches.Values
					.Max(m => m.RoundIndex);
				this.IsFinished = Matches.Values
					.All(m => m.IsFinished);
			}

			// "Recreate" the groups:
			List<List<IPlayer>> playerGroups = DividePlayersIntoGroups();
			for (int g = 0; g < playerGroups.Count; ++g)
			{
				// Use these groups to remake the GroupRankings:
				GroupRankings.Add(new List<IPlayerScore>());
				foreach (IPlayer player in playerGroups[g])
				{
					// Make a new ranking object for each Player,
					// and add it to Rankings and GroupRankings:
					IPlayerScore pScore = new PlayerScore(player.Id, player.Name);
					Rankings.Add(pScore);
					GroupRankings[g].Add(pScore);
				}
			}
			
			// Go through the Matches to recalculate the current Rankings:
			RecalculateRankings();

			if (this.IsFinalized && false == Validate())
			{
				throw new BracketValidationException
					("Bracket is Finalized but not Valid!");
			}
		}

		/// <summary>
		/// Processes the effects of adding a "game win" to a Match.
		/// In round robins, simply updates whether the Bracket is finished.
		/// </summary>
		/// <param name="_matchNumber">Number of affected Match</param>
		/// <param name="_slot">Slot of game winner: Defender or Challenger</param>
		/// <returns>empty list</returns>
		protected override List<MatchModel> ApplyWinEffects(int _matchNumber, PlayerSlot _slot)
		{
			this.IsFinished = Matches.Values
				.All(m => m.IsFinished);
			return (new List<MatchModel>());
		}

		/// <summary>
		/// Processes the effects of removing a game(s) from a Match.
		/// In round robins, simply updates whether the Bracket is finished.
		/// </summary>
		/// <param name="_matchNumber">Number of affected Match</param>
		/// <param name="_games">List of Games removed (if any)</param>
		/// <param name="_formerMatchWinnerSlot">Slot of Match winner prior to removal</param>
		/// <returns>empty list</returns>
		protected override List<MatchModel> ApplyGameRemovalEffects(int _matchNumber, List<GameModel> _games, PlayerSlot _formerMatchWinnerSlot)
		{
			this.IsFinished = (IsFinished && GetMatch(_matchNumber).IsFinished);
			return (new List<MatchModel>());
		}

		/// <summary>
		/// Applies effects to Rankings any time a Match is modified.
		/// This updates match outcomes and score values for any affected players.
		/// </summary>
		/// <param name="_matchNumber">Number of affected Match</param>
		/// <param name="_games">List of Games added/removed (if any)</param>
		/// <param name="_isAddition">Is the result being added or removed?</param>
		/// <param name="_oldMatch">Model of the Match before the modification</param>
		protected override void UpdateScore(int _matchNumber, List<GameModel> _games, bool _isAddition, MatchModel _oldMatch)
		{
			/*
			 * This method explicitly updates the Rankings list.
			 * Keep in mind, ranking objects are shared between Rankings and GroupRankings.
			 * These explicity updates also update the GroupRankings lists.
			*/

			IMatch match = GetMatch(_matchNumber);

			if (null == _games)
			{
				// Case 1: Match winner was manually set.
				// Apply outcome to rankings (but no games!):
				int winnerIndex = Rankings.FindIndex(r => r.Id == match.Players[(int)(match.WinnerSlot)].Id);
				PlayerSlot loserSlot = (PlayerSlot.Defender == match.WinnerSlot)
					? PlayerSlot.Challenger : PlayerSlot.Defender;
				int loserIndex = Rankings.FindIndex(r => r.Id == match.Players[(int)loserSlot].Id);

				Rankings[winnerIndex].AddMatchOutcome(Outcome.Win, true);
				Rankings[loserIndex].AddMatchOutcome(Outcome.Loss, true);
			}
			else if (_oldMatch.IsManualWin && !(match.IsManualWin))
			{
				// Case 2: Match had a manual winner: being removed.
				// Update rankings accordingly, ignoring individual game scores:
				int loserId = (_oldMatch.WinnerID == _oldMatch.DefenderID)
					? _oldMatch.ChallengerID : _oldMatch.DefenderID;

				Rankings.Find(r => r.Id == _oldMatch.WinnerID)
					.AddMatchOutcome(Outcome.Win, false);
				Rankings.Find(r => r.Id == loserId)
					.AddMatchOutcome(Outcome.Loss, false);
			}
			else
			{
				// Standard case: Update score for new game(s):

				// Find both Players' indexes in Rankings:
				int defIndex = Rankings.FindIndex(r => r.Id == match.Players[(int)PlayerSlot.Defender].Id);
				int chalIndex = Rankings.FindIndex(r => r.Id == match.Players[(int)PlayerSlot.Challenger].Id);

				// First, find out if the Match's Outcome is changing:
				bool oldMatchFinished = _oldMatch.IsManualWin;
				if (!oldMatchFinished)
				{
					if (_oldMatch.WinnerID.HasValue && _oldMatch.WinnerID > -1)
					{
						oldMatchFinished = true;
					}
					else if (_oldMatch.DefenderScore + _oldMatch.ChallengerScore >= match.MaxGames)
					{
						oldMatchFinished = true;
					}
				}
				if (match.IsFinished != oldMatchFinished)
				{
					// Match Outcome has changed.
					PlayerSlot oldWinnerSlot = (_oldMatch.WinnerID == _oldMatch.DefenderID)
						? PlayerSlot.Defender : PlayerSlot.unspecified;
					oldWinnerSlot = (_oldMatch.WinnerID == _oldMatch.ChallengerID)
						? PlayerSlot.Challenger : oldWinnerSlot;

					// Find the Outcome type to update:
					Outcome defenderOutcome = Outcome.Tie;
					Outcome challengerOutcome = Outcome.Tie;
					if (PlayerSlot.Defender == match.WinnerSlot ||
						PlayerSlot.Defender == oldWinnerSlot)
					{
						// Defender is or was the winner:
						defenderOutcome = Outcome.Win;
						challengerOutcome = Outcome.Loss;
					}
					else if (PlayerSlot.Challenger == match.WinnerSlot ||
						PlayerSlot.Challenger == oldWinnerSlot)
					{
						// Challenger is or was the winner:
						defenderOutcome = Outcome.Loss;
						challengerOutcome = Outcome.Win;
					}

					// Add/subtract the Match Outcome:
					Rankings[defIndex].AddMatchOutcome(defenderOutcome, _isAddition);
					Rankings[chalIndex].AddMatchOutcome(challengerOutcome, _isAddition);
				}

				// Now, calculate the Score updates:
				if (_games.Count > 0 && !(match.IsManualWin))
				{
					int defenderGameScore = 0, defenderPointScore = 0;
					int challengerGameScore = 0, challengerPointScore = 0;

					foreach (GameModel model in _games)
					{
						// GameScore +1 for wins, -1 for losses
						if (model.WinnerID == model.DefenderID)
						{
							++defenderGameScore;
							--challengerGameScore;
						}
						else if (model.WinnerID == model.ChallengerID)
						{
							--defenderGameScore;
							++challengerGameScore;
						}
						// PointScore adjusts for "points" in each Game
						defenderPointScore += model.DefenderScore;
						challengerPointScore += model.ChallengerScore;
					}

					// Apply the Score updates to Rankings:
					Rankings[defIndex].UpdateScores
						(defenderGameScore, defenderPointScore, _isAddition);
					Rankings[chalIndex].UpdateScores
						(challengerGameScore, challengerPointScore, _isAddition);
				}
			}

			// Call UpdateRankings() to calculate advanced scoring values,
			// and to sort all of the Rankings lists:
			UpdateRankings();
		}

		/// <summary>
		/// Clears the Rankings, and recalculates them from the Matches list.
		/// Sorts the Rankings.
		/// If no Matches have begun, the Rankings will be set to initial values.
		/// </summary>
		protected override void RecalculateRankings()
		{
			if (0 == (Players?.Count ?? 0))
			{
				return;
			}

			// Set initial values for all ranking objects:
			foreach (IPlayerScore playerScore in Rankings)
			{
				playerScore.Rank = 1;
				playerScore.ResetScore();
			}

			// For every Match...
			foreach (IMatch match in Matches.Values.Where(m => !(m.Players.Contains(null))))
			{
				// Find indexes for both players in the Rankings list:
				int defIndex = Rankings.FindIndex(r => r.Id == match.Players[(int)PlayerSlot.Defender].Id);
				int chalIndex = Rankings.FindIndex(r => r.Id == match.Players[(int)PlayerSlot.Challenger].Id);

				if (match.IsFinished)
				{
					// Determine the match outcome:
					Outcome defOutcome = Outcome.Tie;
					Outcome chalOutcome = Outcome.Tie;
					switch (match.WinnerSlot)
					{
						case PlayerSlot.Defender:
							defOutcome = Outcome.Win;
							chalOutcome = Outcome.Loss;
							break;
						case PlayerSlot.Challenger:
							defOutcome = Outcome.Loss;
							chalOutcome = Outcome.Win;
							break;
					}

					// Add the Outcome to relevant player rankings:
					Rankings[defIndex].AddMatchOutcome(defOutcome, true);
					Rankings[chalIndex].AddMatchOutcome(chalOutcome, true);
				}

				// If this Match has a winner OR any Games played:
				if (match.Games.Count > 0 && !(match.IsManualWin))
				{
					// Update the player scores for each Game:
					int defGameScore = 0, chalGameScore = 0;
					int defPointScore = 0, chalPointScore = 0;
					foreach (IGame game in match.Games)
					{
						// Update each player's GameScore:
						switch (game.WinnerSlot)
						{
							case PlayerSlot.Defender:
								++defGameScore;
								--chalGameScore;
								break;
							case PlayerSlot.Challenger:
								--defGameScore;
								++chalGameScore;
								break;
						}
						// Update each player's PointScore:
						defPointScore += game.Score[(int)PlayerSlot.Defender];
						chalPointScore += game.Score[(int)PlayerSlot.Challenger];
					}

					// Apply the score updates to Rankings:
					Rankings[defIndex].UpdateScores
						(defGameScore, defPointScore, true);
					Rankings[chalIndex].UpdateScores
						(chalGameScore, chalPointScore, true);
				}
			}

			// Call UpdateRankings() to calculate advanced scoring values,
			// and to sort the Rankings list appropriately:
			UpdateRankings();
		}

		/// <summary>
		/// Sorts the Rankings list, and assigns ranks.
		/// This also calculates each player's OpponentsScore,
		/// a strength-of-schedule metric.
		/// </summary>
		protected override void UpdateRankings()
		{
			// Calculate MatchScore values for each player:
			int[] playerScores = new int[Rankings.Count];
			for (int p = 0; p < Rankings.Count; ++p)
			{
				playerScores[p] = Rankings[p].CalculateScore(MatchWinValue, MatchTieValue, 0);

				// Also reset their OpponentsScore (we'll update it below).
				Rankings[p].ResetOpponentsScore();
			}

			// Calculate & Assign OpponentsPoints value for each player:
			/*
			 * Here, we find every opponent a player has faced,
			 * and total up those opponents' MatchScores.
			 * This provides a "strength-of-schedule" value
			 * for the Player in question: high numbers mean tough competition.
			*/
			foreach (IMatch match in Matches.Values.Where(m => m.IsFinished).ToList())
			{
				Rankings
					.Find(p => p.Id == match.Players[(int)PlayerSlot.Challenger].Id)
					.AddToOpponentsScore(playerScores[Rankings
						.FindIndex(p => p.Id == match.Players[(int)PlayerSlot.Defender].Id)]);
				Rankings
					.Find(p => p.Id == match.Players[(int)PlayerSlot.Defender].Id)
					.AddToOpponentsScore(playerScores[Rankings
						.FindIndex(p => p.Id == match.Players[(int)PlayerSlot.Challenger].Id)]);
			}

			// Sort each group in GroupRankings, and apply Ranks:
			foreach (List<IPlayerScore> rankGroup in GroupRankings)
			{
				rankGroup.Sort(SortRankingScores);
				for (int i = 0; i < rankGroup.Count; ++i)
				{
					// FUTURE NOTE:
					// Here, we may want to have tied rank values for players with equal W/L scores.
					// (the list would still be sorted by the metrics we calculated)
					rankGroup[i].Rank = i + 1;
				}
			}

			// Sort the main list, too:
			Rankings.Sort(SortRankingRanks);
		}

		/// <summary>
		/// Resets the Bracket.
		/// Affects Matches, Rankings, and bracket status.
		/// </summary>
		protected override void ResetBracketData()
		{
			base.ResetBracketData();

			if (null == GroupRankings)
			{
				GroupRankings = new List<List<IPlayerScore>>();
			}
			GroupRankings.Clear();
		}
		#endregion
	}
}
