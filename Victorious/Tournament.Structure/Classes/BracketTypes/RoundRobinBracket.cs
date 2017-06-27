using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatabaseLib;

namespace Tournament.Structure
{
	public class RoundRobinBracket : Bracket
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
		//protected Dictionary<int, Match> Matches
		//public int NumberOfRounds
		//protected Dictionary<int, Match> LowerMatches = empty
		//public int NumberOfLowerRounds = 0
		//protected Match grandFinal = null
		//public IMatch GrandFinal = null
		//public int NumberOfMatches
		//protected int MatchWinValue
		//protected int MatchTieValue
		#endregion

		#region Ctors
		public RoundRobinBracket(List<IPlayer> _players, int _maxGamesPerMatch = 1, int _numberOfRounds = 0)
		{
			if (null == _players)
			{
				throw new ArgumentNullException("_players");
			}

			Players = _players;
			Id = 0;
			BracketType = BracketType.ROUNDROBIN;
			MaxRounds = _numberOfRounds;
			MatchWinValue = 2;
			MatchTieValue = 1;

			CreateBracket(_maxGamesPerMatch);
		}
		public RoundRobinBracket()
			: this(new List<IPlayer>())
		{ }
		public RoundRobinBracket(BracketModel _model)
		{
			// Call a helper method to copy the bracket status fields,
			// and to load the playerlist:
			SetDataFromModel(_model);

			foreach (IPlayer player in Players)
			{
				// Create an initial ranking for every Player:
				Rankings.Add(new PlayerScore(player.Id, player.Name));
			}

			foreach (MatchModel mm in _model.Matches)
			{
				// Create Matches from all MatchModels:
				Matches.Add(mm.MatchNumber, new Match(mm));
			}
			this.NumberOfMatches = Matches.Count;
			this.NumberOfRounds = 0;

			this.IsFinished = false;
			if (NumberOfMatches > 0)
			{
				this.NumberOfRounds = Matches.Values
					.Max(m => m.RoundIndex);
				this.IsFinished = Matches.Values.All(m => m.IsFinished);
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
		/// Uses the playerlist to generate the bracket structure & matchups.
		/// This creates & populates all the Match objects.
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
			foreach (IPlayer player in Players)
			{
				// Create an initial ranking for every Player:
				Rankings.Add(new PlayerScore(player.Id, player.Name));
			}

			// Calculate the number of round to create:
			int totalRounds = (0 == Players.Count % 2)
				? Players.Count - 1 : Players.Count;

			// Randomly choose which rounds to "remove"
			// (only applies if MaxRounds is capped).
			List<int> roundsToRemove = new List<int>();
			if (MaxRounds > 0 && MaxRounds < totalRounds)
			{
				int roundsDiff = totalRounds - MaxRounds;
				Random rng = new Random();
				while (roundsToRemove.Count < roundsDiff)
				{
					// Randomly choose a round number.
					int randomRound = rng.Next(totalRounds);
					if (!roundsToRemove.Contains(randomRound))
					{
						// Add the round number to a list:
						roundsToRemove.Add(randomRound);
					}
				}
			}

			int matchesPerRound = (int)(Players.Count * 0.5);
			// (truncated because matchups=4 for 8 players or 9)

			// Create all the matchups:
			for (int r = 0; r < totalRounds; ++r)
			{
				if (roundsToRemove.Contains(r))
				{
					// If this round was chosen to skip, do so:
					continue;
				}
				++NumberOfRounds;

				// Create the Matches for this round:
				for (int m = 0; m < matchesPerRound; ++m, ++NumberOfMatches)
				{
					Match match = new Match();
					match.SetMatchNumber(NumberOfMatches + 1);
					match.SetRoundIndex(NumberOfRounds);
					match.SetMatchIndex(m + 1);
					match.SetMaxGames(_gamesPerMatch);
					match.AddPlayer(Players[(m + r) % Players.Count]);
					match.AddPlayer(Players[(Players.Count - 1 - m + r) % Players.Count]);

					Matches.Add(match.MatchNumber, match);
				}
			}
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
			// Calling the base method (Bracket) handles:
			// resetting all Matches.
			base.ResetMatches();

			// Reset all the Rankings to initial states:
			foreach (IPlayerScore ps in Rankings)
			{
				ps.Rank = 1;
				ps.ResetScore();
			}

			/*
			 * Now, re-calculate the number of standard rounds for a round robin.
			 * (if the MaxRounds variable is used, that's all we need)
			 * This is how many rounds we initially created for the bracket.
			 * If we now have more, that means tiebreaker Matches were added.
			 * We need to find and delete those Matches.
			*/
			int standardRounds = MaxRounds;
			if (0 == standardRounds)
			{
				standardRounds = (0 == Players.Count % 2)
					? Players.Count - 1 : Players.Count;
			}

			// If no tiebreakers were added, we're done.
			// Otherwise:
			if (NumberOfRounds > standardRounds)
			{
				// Get all the tiebreaker Matches:
				List<Match> matchesToDelete = Matches.Values
					.Where(m => m.RoundIndex > standardRounds)
					.ToList();
				List<MatchModel> modelsToDelete = new List<MatchModel>();
				foreach (Match match in matchesToDelete)
				{
					// Delete each tiebreaker.
					// (but first, save a Model of it for firing an event)
					modelsToDelete.Add(GetMatchModel(match));
					Matches.Remove(match.MatchNumber);
				}

				// Update necessary Bracket variables:
				NumberOfMatches = Matches.Count;
				NumberOfRounds = Matches.Values
					.Max(m => m.RoundIndex);

				// Fire an event to notify the database to delete Matches:
				OnRoundDeleted(modelsToDelete);
			}
		}

		/// <summary>
		/// Determines if any players are tied.
		/// Criteria is W/L/T record only: scores are not taken into account here.
		/// If the bracket is not finished, an exception is thrown.
		/// </summary>
		/// <returns>true if any ties are found, false if none</returns>
		public override bool CheckForTies()
		{
			if (!IsFinished)
			{
				throw new BracketException
					("Bracket isn't finished yet!");
			}

			// Using Rankings, calculate W/L scores for all players:
			int[] scores = new int[Rankings.Count];
			for (int i = 0; i < Rankings.Count; ++i)
			{
				scores[i] = Rankings[i].CalculateScore(MatchWinValue, MatchTieValue, 0);

				if (i > 0 && scores[i] == scores[i - 1])
				{
					// Found a tie. We're finished:
					return true;
				}
			}
			// No ties found.
			return false;
		}

		/// <summary>
		/// Finds pairs/groups of tied players,
		/// and creates any necessary tiebreaker Matches for them.
		/// This may result in one new Match or in multiple rounds.
		/// All new Matches will be put in new rounds on the "back" of the Bracket.
		/// If the Bracket is not finished, an exception is thrown.
		/// If the Bracket has no ties, no Matches are created and false is returned.
		/// </summary>
		/// <returns>true if tiebreakers added, false if no ties</returns>
		public override bool GenerateTiebreakers()
		{
			if (!IsFinished)
			{
				throw new BracketException
					("Bracket isn't finished yet!");
			}

			// Calculate W/L scores for all players:
			int[] scores = new int[Rankings.Count];
			for (int i = 0; i < Rankings.Count; ++i)
			{
				scores[i] = Rankings[i].CalculateScore(MatchWinValue, MatchTieValue, 0);
			}

			// Create "groups" for any tied players:
			List<List<int>> tiedGroups = new List<List<int>>();
			for (int i = 0; i < Rankings.Count - 1;)
			{
				int j = i + 1;
				for (; j < Rankings.Count; ++j)
				{
					if (scores[i] != scores[j])
					{
						// Different scores; look for a new tie value.
						break;
					}

					if (i + 1 == j)
					{
						// NEW tied group:
						tiedGroups.Add(new List<int>());
						tiedGroups[tiedGroups.Count - 1].Add(Rankings[i].Id);
					}
					// If we're here, scores[j-1] == scores[j]:
					tiedGroups[tiedGroups.Count - 1].Add(Rankings[j].Id);
				}

				// Increment the loop:
				i = j;
			}
			if (0 == tiedGroups.Count)
			{
				// No ties: bracket is finished, so just leave:
				return false;
			}
			// else:

			this.IsFinished = false;
			List<MatchModel> newMatchModels = new List<MatchModel>();

			// Create a temporary bracket for each group:
			List<IBracket> tiebreakerBrackets = new List<IBracket>();
			foreach (List<int> group in tiedGroups)
			{
				List<IPlayer> pList = new List<IPlayer>();
				foreach (int id in group)
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
					// No more rounds to add; break out:
					break;
				}

				// Add a new round and copy applicable matches:
				this.NumberOfRounds++;
				for (int m = 0; m < currRound.Count; ++m)
				{
					Match match = new Match();
					match.SetMatchNumber(++NumberOfMatches);
					match.SetRoundIndex(NumberOfRounds);
					match.SetMatchIndex(m + 1);
					match.AddPlayer(currRound[m].Players[0]);
					match.AddPlayer(currRound[m].Players[1]);

					Matches.Add(match.MatchNumber, match);
					// Also add a model, for firing an event:
					newMatchModels.Add(GetMatchModel(match));
				}
			}

			// Fire event to notify that new Matches were created, then we're done:
			OnRoundAdded(new BracketEventArgs(newMatchModels));
			return true;
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Processes the effects of adding a "game win" to a Match.
		/// In round robins, simply updates whether the Bracket is finished.
		/// </summary>
		/// <param name="_matchNumber">Number of affected Match</param>
		/// <param name="_slot">Slot of game winner: Defender or Challenger</param>
		/// <returns>empty list</returns>
		protected override List<MatchModel> ApplyWinEffects(int _matchNumber, PlayerSlot _slot)
		{
			this.IsFinished = Matches.Values.All(m => m.IsFinished);
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
			IMatch match = GetMatch(_matchNumber);

			if (null == _games)
			{
				// Case 1: Match winner is being manually set.
				// Add the match outcome to Rankings (but no games!):
				int winnerIndex = Rankings.FindIndex(r => r.Id == match.Players[(int)(match.WinnerSlot)].Id);
				PlayerSlot loserSlot = (PlayerSlot.Defender == match.WinnerSlot)
					? PlayerSlot.Challenger : PlayerSlot.Defender;
				int loserIndex = Rankings.FindIndex(r => r.Id == match.Players[(int)loserSlot].Id);

				Rankings[winnerIndex].AddMatchOutcome(Outcome.Win, true);
				Rankings[loserIndex].AddMatchOutcome(Outcome.Loss, true);
			}
			else if (_oldMatch.IsManualWin && !(match.IsManualWin))
			{
				// Case 2: Match had a manual winner that is being removed.
				// Remove the match outcome from Rankings,
				// but ignore individual game scores:
				int loserId = (_oldMatch.WinnerID == _oldMatch.DefenderID)
					? _oldMatch.ChallengerID : _oldMatch.DefenderID;

				Rankings.Find(r => r.Id == _oldMatch.WinnerID)
					.AddMatchOutcome(Outcome.Win, false);
				Rankings.Find(r => r.Id == loserId)
					.AddMatchOutcome(Outcome.Loss, false);
			}
			else
			{
				// Standard case: Update score for new game(s).

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
							// Defender win:
							++defenderGameScore;
							--challengerGameScore;
						}
						else if (model.WinnerID == model.ChallengerID)
						{
							// Challenger win:
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
			// and to sort the Rankings list appropriately:
			UpdateRankings();
		}

		/// <summary>
		/// Clears the Rankings, and recalculates them from the Matches list.
		/// Sorts the Rankings.
		/// If no Matches have begun, the Rankings will be set to initial values.
		/// </summary>
		protected override void RecalculateRankings()
		{
			// Set initial values:
			foreach (IPlayerScore ps in Rankings)
			{
				ps.Rank = 1;
				ps.ResetScore();
			}

			/*
			 * Since this method is used in Swiss brackets
			 * and Swiss has some Matches with no Players,
			 * we throw those out when updating the Rankings.
			*/
			foreach (IMatch match in Matches.Values.Where(m => !(m.Players.Contains(null))))
			{
				// For every Match...

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

			// Sort the list, and apply Ranks:
			Rankings.Sort(SortRankingScores);
			for (int i = 0; i < Rankings.Count; ++i)
			{
				// FUTURE NOTE:
				// Here, we may want to have tied rank values for players with equal W/L scores.
				// (the list would still be sorted by the metrics we calculated)
				Rankings[i].Rank = i + 1;
			}
		}
		#endregion
	}
}
