#define CHOOSE_PAIRING

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatabaseLib;

namespace Tournament.Structure
{
	/// <summary>
	/// A compact representation of a Match, holding data:
	/// round number, and defender & challenger indexes.
	/// These indexes reference the bracket's Players list.
	/// </summary>
	internal struct Matchup
	{
		/// <summary>
		/// Defender's index in the Bracket's Players list.
		/// </summary>
		public int DefenderIndex { get; private set; }

		/// <summary>
		/// Challenger's index in the Bracket's Players list.
		/// </summary>
		public int ChallengerIndex { get; private set; }

		/// <summary>
		/// Round of the represented Match.
		/// </summary>
		public int RoundNumber { get; private set; }

		public Matchup(int _defIndex, int _chalIndex, int _roundNumber)
		{
			DefenderIndex = _defIndex;
			ChallengerIndex = _chalIndex;
			RoundNumber = _roundNumber;
		}

		/// <summary>
		/// Determines if DefenderIndex or ChallengerIndex equals the given int.
		/// </summary>
		/// <param name="_int">Index to test against</param>
		/// <returns>true if found, false otherwise</returns>
		public bool ContainsInt(int _int)
		{
			if (DefenderIndex == _int || ChallengerIndex == _int)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Determines if the given Matchup has the same
		/// player indexes as this Matchup.
		/// </summary>
		/// <param name="_m">Matchup to test players against</param>
		/// <returns>true if same players, false otherwise</returns>
		public bool HasMatchingPlayers(Matchup _m)
		{
			return (this.ContainsInt(_m.DefenderIndex) &&
				this.ContainsInt(_m.ChallengerIndex));
		}
	}

	public class SwissBracket : RoundRobinBracket
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

		/// <summary>
		/// A list of all player matchups that have been generated.
		/// This is updated every time a new round is generated,
		/// and whenever rounds are unmade.
		/// </summary>
		private List<Matchup> Matchups
		{ get; set; }

		/// <summary>
		/// A list of every bye.
		/// Each bye is represented by the ID of a Player.
		/// </summary>
		private List<int> PlayerByes
		{ get; set; }

		/// <summary>
		/// Method of creating pairings for this Bracket:
		/// Slide (default), Fold, or Adjancent.
		/// </summary>
		private PairingMethod PairingMethod
		{ get; set; }

		/// <summary>
		/// The round currently being played.
		/// (1-indexed)
		/// </summary>
		private int ActiveRound
		{ get; set; }
		#endregion

		#region Ctors
		public SwissBracket(List<IPlayer> _players, PairingMethod _pairing = PairingMethod.Slide, int _maxGamesPerMatch = 1, int _numberOfRounds = 0)
		{
			if (null == _players)
			{
				throw new ArgumentNullException("_players");
			}

			Players = _players;
			Id = 0;
			BracketType = BracketType.SWISS;
			PairingMethod = _pairing;

			// Limit the number of rounds to make,
			// according to user input and the playercount.
			MaxRounds = _numberOfRounds;
			if (Players.Count > 8 && MaxRounds > (int)(Players.Count * 0.5))
			{
				MaxRounds = Players.Count / 2;
			}
			else if (Players.Count <= 8 && MaxRounds >= Players.Count)
			{
				MaxRounds = Players.Count - 1;
			}

			CreateBracket(_maxGamesPerMatch);
		}
		public SwissBracket()
			: this(new List<IPlayer>())
		{ }
		public SwissBracket(BracketModel _model)
			: base(_model)
		{
			/*
			 * This constructor extends the parent (round robin) version.
			 * By the time we're "here," we already have our main data restored:
			 * Players, Matches, and inherited Bracket fields.
			 * Now, we need to recreate the Swiss-specific fields:
			 * Matchups, PlayerByes, and PairingMethod.
			*/

			for (int r = 1; r <= NumberOfRounds; ++r)
			{
				List<IMatch> round = GetRound(r);
				if (round.Any(m => m.Players.Contains(null)))
				{
					// This round isn't populated yet. Break out:
					break;
				}

				// playersInRound is a list of all players (by index)
				// in matchups this round. We check for the missing player
				// to know who got a bye.
				List<int> playersInRound = new List<int>();

				foreach (IMatch match in round)
				{
					// For each populated Match, add a Matchup to the private list:
					int defIndex = Players.FindIndex(p => p.Id == match.Players[(int)PlayerSlot.Defender].Id);
					int chalIndex = Players.FindIndex(p => p.Id == match.Players[(int)PlayerSlot.Challenger].Id);

					playersInRound.Add(defIndex);
					playersInRound.Add(chalIndex);
					Matchups.Add(new Matchup(defIndex, chalIndex, r));
				}

				if (playersInRound.Count < Players.Count)
				{
					// Find the player with a bye this round:
					int byePlayerIndex = Enumerable
						.Range(0, Players.Count).ToList()
						.Except(playersInRound).First();

					// Add him to Byes list and award a "Win" Outcome:
					PlayerByes.Add(Players[byePlayerIndex].Id);
					Rankings.Find(p => p.Id == Players[byePlayerIndex].Id)
						.AddMatchOutcome(Outcome.Win, true);
				}

				// Set ActiveRound (this finds the last populated round):
				ActiveRound = r;
			}

			// Determine the Pairing Method from examining round 1:
			// (default is Slide)
			this.PairingMethod = PairingMethod.Slide;
			if (NumberOfMatches > 0 && ActiveRound > 0)
			{
#if CHOOSE_PAIRING
				// Find the top seed's first Match:
				int firstPlayerIndex = (0 == PlayerByes.Count)
					? 0 : 1;
				IMatch firstPlayerMatch = GetRound(1)
					.Where(m => m.Players.Select(p => p.Id).Contains(this.Players[firstPlayerIndex].Id))
					.First();
				// Find the ID of the Player the top seed is matched against:
				int secondPlayerId = firstPlayerMatch.Players
					.Select(p => p.Id)
					.Where(i => i != this.Players[firstPlayerIndex].Id)
					.First();
				
				if (Players[1 + firstPlayerIndex].Id == secondPlayerId)
				{
					// The second Player is also the second seed.
					// This must be Adjancent pairings.
					PairingMethod = PairingMethod.Adjacent;
				}
				else if (Players.Last().Id == secondPlayerId)
				{
					// The second Player is the last seed.
					// This must be Fold pairings.
					PairingMethod = PairingMethod.Fold;
				}
#endif

				if (PlayerByes.Count > 0)
				{
					// If we added points for byes, we need to update rankings.
					// Call the parent (round robin) method:
					UpdateRankings();
				}
			}
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Uses the playerlist to generate the bracket structure & matchups.
		/// This creates all the necessary Match objects,
		/// and populates the first round.
		/// If any Matches already exist, they will be deleted first.
		/// If there are <2 players, nothing will be made.
		/// </summary>
		/// <param name="_gamesPerMatch">Max games for every Match</param>
		public override void CreateBracket(int _gamesPerMatch = 1)
		{
			// First, clear any existing Matches and results:
			// (this also clears Matchups and PlayerByes)
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

			// Calculate round count.
			// If this isn't specified by MaxRounds, the default value
			// is [floor(log2(playercount)) = rounds].
			// Example: 8 players = 3 rounds. 67 players = 6 rounds.
			NumberOfRounds = MaxRounds;
			if (0 == NumberOfRounds)
			{
				while (Math.Pow(2, NumberOfRounds) < Players.Count)
				{
					++NumberOfRounds;
				}
			}

			// Create empty Matches for all of the rounds:
			int matchesPerRound = (int)(Players.Count * 0.5);
			for (int r = 1; r <= NumberOfRounds; ++r)
			{
				for (int m = 1; m <= matchesPerRound; ++m)
				{
					Match match = new Match();
					match.SetMatchNumber(++NumberOfMatches);
					match.SetRoundIndex(r);
					match.SetMatchIndex(m);
					match.SetMaxGames(_gamesPerMatch);

					Matches.Add(match.MatchNumber, match);
				}
			}

			// Populate the next round (1) by calling our algorithm:
			AddSwissRound(_gamesPerMatch);
#if false
			int divisionPoint = Players.Count / 2;
			NumberOfRounds = 1;
			for (int m = 0; m < divisionPoint; ++m, ++NumberOfMatches)
			{
				Match match = new Match();
				match.SetMatchNumber(NumberOfMatches + 1);
				match.SetRoundIndex(NumberOfRounds);
				match.SetMatchIndex(m + 1);
				match.SetMaxGames(_gamesPerMatch);
				match.AddPlayer(Players[m]);
				match.AddPlayer(Players[m + divisionPoint]);

				Matches.Add(match.MatchNumber, match);
			}
#endif
		}

		/// <summary>
		/// Resets the state of all Matches and Rankings.
		/// Deletes all Games and sets scores to 0-0.
		/// Removes Players from all rounds after the first.
		/// May fire MatchesModified and GamesDeleted events, if updates occur.
		/// </summary>
		public override void ResetMatches()
		{
			// RemoveFutureRounds() is a recursive method.
			// It will remove the Players from all Matches
			// after the given round number (also resetting the Matches).
			// It will always leave the Players in round 1, however.
			List<MatchModel> clearedMatches = RemoveFutureRounds(0);
			ActiveRound = 1;
			IsFinalized = false;

			// Reset the Rankings:
			RecalculateRankings();
			
			// Fire a notification event that we altered Matches:
			OnMatchesModified(clearedMatches);
		}

		/// <summary>
		/// Always returns false.
		/// (Does not apply to Swiss-style brackets)
		/// </summary>
		/// <returns>false</returns>
		public override bool CheckForTies()
		{
			return false;
		}

		/// <summary>
		/// Always throws a NotImplementedException.
		/// (Does not apply to Swiss-style brackets)
		/// </summary>
		public override bool GenerateTiebreakers()
		{
			throw new NotImplementedException
				("Can't create tiebreakers for a Swiss bracket!");
		}

		/// <summary>
		/// Removes a Player from the playerlist,
		/// and replaces him with a given Player.
		/// The new Player inherits the old's seed value.
		/// The removed Player is replaced in all Matches, Games, & Rankings
		/// by the new Player.
		/// This extension method also replaces the Player in the list of byes.
		/// May fire MatchesModified event, if updates happen.
		/// If the Player-to-replace's index is invalid, an exception is thrown.
		/// </summary>
		/// <param name="_player">Player to add</param>
		/// <param name="_index">Index (in playerlist) of Player to remove</param>
		public override void ReplacePlayer(IPlayer _player, int _index)
		{
			int oldId = Players[_index].Id;
			base.ReplacePlayer(_player, _index);

			// Replace the old player's ID in the Byes list:
			for (int i = 0; i < PlayerByes.Count; ++i)
			{
				if (PlayerByes[i] == oldId)
				{
					PlayerByes[i] = _player.Id;
				}
			}
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Resets the Bracket.
		/// Affects Matches, Rankings, and bracket status.
		/// Also resets the private Matchups and PlayerByes lists.
		/// </summary>
		protected override void ResetBracketData()
		{
			base.ResetBracketData();

			ActiveRound = 0;
			if (null == Matchups)
			{
				Matchups = new List<Matchup>();
			}
			if (null == PlayerByes)
			{
				PlayerByes = new List<int>();
			}
			Matchups.Clear();
			PlayerByes.Clear();
		}

		/// <summary>
		/// Processes the effects of adding a "game win" to a Match.
		/// If the active round is not finished, this does nothing.
		/// If the round is done, this calls methods to (try to)
		/// generate Swiss matchups for the next round.
		/// On success, the next round's Matches are populated.
		/// On failure, any future empty rounds are deleted.
		/// May fire MatchesModified or RoundDeleted events.
		/// </summary>
		/// <param name="_matchNumber">Number of Match initially affected</param>
		/// <param name="_slot">Slot of game winner: Defender or Challenger</param>
		/// <returns>List of altered Matches; empty if none change</returns>
		protected override List<MatchModel> ApplyWinEffects(int _matchNumber, PlayerSlot _slot)
		{
			// Check if the active round is finished:
			bool makeNewRound = GetRound(ActiveRound).All(m => m.IsFinished);
			List<MatchModel> modelList = new List<MatchModel>();

			if (makeNewRound)
			{
				// If active round is finished, try to generate a new round.
				if (AddSwissRound(GetMatch(_matchNumber).MaxGames))
				{
					// Success! Next round was populated and ActiveRound was incremented.
					List<IMatch> addedRound = GetRound(ActiveRound);
					foreach (IMatch match in addedRound)
					{
						// Add the new MatchModels to a list:
						modelList.Add(GetMatchModel(match));
					}
				}
				else
				{
					// Failed to generate new round.
					// Clean up all the excess stuff.
					List<MatchModel> deletedMatches = new List<MatchModel>();

					// Get all unpopulated Matches:
					List<Match> extraMatches = Matches.Values
						.Where(m => m.RoundIndex > ActiveRound)
						.ToList();
					foreach (Match match in extraMatches)
					{
						// Save a Model for each of these, then delete the Match:
						deletedMatches.Add(GetMatchModel(match));
						Matches.Remove(match.MatchNumber);
					}

					// Update Bracket variables:
					NumberOfRounds = ActiveRound;
					NumberOfMatches = Matches.Count;
					IsFinished = true;

					// Fire a notification event:
					OnRoundDeleted(deletedMatches);
				}
			}

			// Return any MatchModels that we populated (on success):
			return modelList;
		}

		/// <summary>
		/// Processes the effects of removing a game(s) from a Match.
		/// If this is done to a Match before the active round,
		/// all rounds after that Match are reverted (Players are removed).
		/// May fire MatchesModified, GamesDeleted events, if updates occur.
		/// </summary>
		/// <param name="_matchNumber">Number of Match initially affected</param>
		/// <param name="_games">List of Games removed (if any)</param>
		/// <param name="_formerMatchWinnerSlot">Slot of Match winner prior to removal</param>
		/// <returns>List of altered Matches</returns>
		protected override List<MatchModel> ApplyGameRemovalEffects(int _matchNumber, List<GameModel> _games, PlayerSlot _formerMatchWinnerSlot)
		{
			IMatch match = GetMatch(_matchNumber);
			List<MatchModel> alteredMatches = new List<MatchModel>();

			// If the Match WAS finished, but no longer...
			if (!(match.IsFinished) &&
				((PlayerSlot.unspecified != _formerMatchWinnerSlot) ||
				(_games.Count + match.Score[0] + match.Score[1] >= match.MaxGames)))
			{
				// This removal may invalidate future matches.
				// RemoveFutureRounds() will revert future rounds, if necessary:
				alteredMatches = RemoveFutureRounds(match.RoundIndex);
				this.IsFinished = false;
			}
			else
			{
				// Otherwise, use parent (round robin) method to update Bracket finish status:
				base.ApplyGameRemovalEffects(_matchNumber, _games, _formerMatchWinnerSlot);
			}

			return alteredMatches;
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
			if (!(match.IsFinished))
			{
				// Figure out if the Match was finished, prior to this alteration.
				// This is true if the WinnerID > 0,
				// or if there were enough Games to result in a tied Match.
				bool oldMatchFinished = _oldMatch.IsManualWin;
				if (!oldMatchFinished)
				{
					if (_oldMatch.WinnerID.GetValueOrDefault(-1) > -1)
					{
						oldMatchFinished = true;
					}
					else if (_oldMatch.DefenderScore + _oldMatch.ChallengerScore >= match.MaxGames)
					{
						oldMatchFinished = true;
					}
				}

				if (oldMatchFinished)
				{
					// Match WAS finished, but no longer!
					// We just invalidated future match results.
					// Instead of regular updating, we need to reset/recalculate:
					RecalculateRankings();
				}
			}
			else // match IS finished
			{
				// If the current Match is finished, we can use round robin's
				// method implementation to apply Rankings updates:
				base.UpdateScore(_matchNumber, _games, _isAddition, _oldMatch);
			}
		}

		/// <summary>
		/// Clears the Rankings, and recalculates them from the Matches list.
		/// Sorts the Rankings.
		/// If no Matches have begun, the Rankings will be set to initial values.
		/// </summary>
		protected override void RecalculateRankings()
		{
			// Parent (round robin)'s method handles the legwork.
			// This method call resets the Rankings and
			// rebuilds them from the Matches list.
			base.RecalculateRankings();

			if (PlayerByes.Count > 0)
			{
				foreach (int playerId in PlayerByes)
				{
					// Add a "match win" for each player with a bye:
					Rankings.Find(r => r.Id == playerId)
						.AddMatchOutcome(Outcome.Win, true);
				}

				// We just added outcomes, so we need to re-update & sort.
				// Again, round robin handles this for us:
				UpdateRankings();
			}
		}

		#region Swiss Algorithm Methods
		/// <summary>
		/// Attempts to create a new round of (legal) Swiss matchups.
		/// This takes all prior results into account for the following:
		/// - Match strong players together, and weak players together.
		/// - No rematches.
		/// - No repeat byes.
		/// If a new legal round is created, the next Bracket round is populated.
		/// </summary>
		/// <param name="_gamesPerMatch">Max Games for each Match</param>
		/// <returns>true on success, false on failure</returns>
		private bool AddSwissRound(int _gamesPerMatch)
		{
			// Don't try to populate nonexistent rounds:
			if (ActiveRound >= NumberOfRounds)
			{
				return false;
			}

			#region Swiss Algorithm
			// CreateGroups() divides players into "groups"
			// based on their W/L record.
			List<List<int>> scoreBrackets = CreateGroups();

			// GetHeuristicEdges() generates a graph representing
			// matchup preferences between every player.
			// Players with the same record and no prior matchups
			// will have a very low heuristic value here.
			List<int[]> possibleMatches = GetHeuristicEdges(scoreBrackets);

			// Access the Python weight-matching script:
			var engine = IronPython.Hosting.Python.CreateEngine();
			var scope = engine.CreateScope();
			engine.ExecuteFile("mwmatching.py", scope);
			dynamic maxWeightMatching = scope.GetVariable("maxWeightMatching");

			// Calling maxWeightMatching returns a Swiss matchup solution (if possible).
			// The resulting list is a list of legal matchups:
			IronPython.Runtime.List pySolution = maxWeightMatching(possibleMatches, true);
			#endregion

			#region Legal Checks
			// Double-check that all the new matchups are Swiss-legal.
			// Create a list of all the new Matchups:
			List<Matchup> newRoundMatchups = new List<Matchup>();
			for (int i = 0; i < pySolution.Count; ++i)
			{
				// 'i' = 'Defender index'
				// 'pySolution[i]' = 'Challenger index'
				int challengerIndex = Convert.ToInt32(pySolution[i]);
				if (i == challengerIndex)
				{
					// Player is matched against himself!
					return false;
				}
				else if (i > challengerIndex)
				{
					continue;
				}

				Matchup newMatchup = new Matchup(i, challengerIndex, (1 + ActiveRound));
				foreach (Matchup m in Matchups)
				{
					if (m.HasMatchingPlayers(newMatchup))
					{
						// This is a rematch from a previous round!
						return false;
					}
				}
				foreach (Matchup m in newRoundMatchups)
				{
					if (m.ContainsInt(newMatchup.DefenderIndex) ||
						m.ContainsInt(newMatchup.ChallengerIndex))
					{
						// A player has multiple matchups this round!
						return false;
					}
				}
				newRoundMatchups.Add(newMatchup);
			}
			if (newRoundMatchups.Count < (int)(Players.Count * 0.5))
			{
				// Not enough Matchups were created!
				// (probably means: unable to create enough legal matches)
				return false;
			}
			#endregion

			#region New Round Populating
			// CONFIRMED LEGAL!
			// Populate the next round of Matches with these Players:
			for (int i = 0; i < newRoundMatchups.Count; ++i)
			{
				int matchNum = (i + 1 + (newRoundMatchups.Count * ActiveRound));

				Matches[matchNum].AddPlayer(Players[newRoundMatchups[i].DefenderIndex]);
				Matches[matchNum].AddPlayer(Players[newRoundMatchups[i].ChallengerIndex]);
			}
			++ActiveRound;

			// Add the new Matchups to our matchups list:
			Matchups.AddRange(newRoundMatchups);

			// Now that we have a new legal round...
			// Award points to the player with a bye, if there is one:
			if (PlayerByes.Count > 0)
			{
				int rIndex = Rankings.FindIndex(r => r.Id == PlayerByes[PlayerByes.Count - 1]);
				Rankings[rIndex].AddMatchOutcome(Outcome.Win, true);
				UpdateRankings();
			}
			#endregion

			return true;
		}

		/// <summary>
		/// Places players with matching W/L scores into groups.
		/// These groups are used to create & weigh potential matchups.
		/// </summary>
		/// <returns>List of groups (each is a list of Player indexes)</returns>
		private List<List<int>> CreateGroups()
		{
			// If playercount is odd, find the top-ranked player to give a Bye
			// (no player should have >1 bye)
			int currentByeId = -1;
			if (Players.Count % 2 > 0)
			{
				foreach (int id in Rankings.Select(r => r.Id))
				{
					if (!PlayerByes.Contains(id))
					{
						PlayerByes.Add(id);
						currentByeId = id;
						break;
					}
				}
			}

			// Create score-brackets (groups) of players, separated by their MatchScore:
			List<List<int>> groups = new List<List<int>>();
			for (int i = 0; i < Rankings.Count; ++i)
			{
				if (PlayerByes.Count > 0 &&
					currentByeId == Rankings[i].Id)
				{
					// This player has a bye this round. Do not add him to groups!
					continue;
				}

				int prevIndex = i - 1;
				if (PlayerByes.Count > 0 &&
					prevIndex >= 0 &&
					currentByeId == Rankings[prevIndex].Id)
				{
					// Prev player has a bye this round. Decrement the index:
					--prevIndex;
				}
				if (prevIndex < 0 ||
					Rankings[i].CalculateScore(MatchWinValue, MatchTieValue, 0) < Rankings[prevIndex].CalculateScore(MatchWinValue, MatchTieValue, 0))
				{
					// New MatchPoints value, so we add a new group:
					groups.Add(new List<int>());
				}

				// Add player's index in the Players array:
				// this value represents the player in these heuristic methods.
				groups[groups.Count - 1].Add
					(Players.FindIndex(p => p.Id == Rankings[i].Id));
			}

			// Sort the players within each group according to their accumulated opponents' scores
#if false
			foreach (List<int> group in groups)
			{
				group.Sort(
					(first, second) =>
					Rankings.FindIndex(r => r.Id == first)
					.CompareTo(Rankings.FindIndex(r => r.Id == second)));
			}
#endif

			// Make sure each group has an even playercount.
			// This is done for matchmaking purposes.
			for (int i = 0; i < groups.Count; ++i)
			{
				if (groups[i].Count % 2 > 0)
				{
					// If group.count is odd, take top player out of next group,
					// and shift him up to current group:
					int playerIndex = groups[i + 1][0];
					groups[i].Add(playerIndex);
					groups[i + 1].RemoveAt(0);
				}
			}

			return groups;
		}

		/// <summary>
		/// Makes a list of all possible matchups,
		/// and assigns a heuristic weight to each.
		/// The heuristic weight is determined by Swiss rules;
		/// a higher value is preffered.
		/// Each "edge" is an int array: [playerIndex, playerIndex, weight].
		/// </summary>
		/// <param name="_groups">List of groups from CreateGroups()</param>
		/// <returns>List of "edge" arrays</returns>
		private List<int[]> GetHeuristicEdges(List<List<int>> _groups)
		{
			// numCompetitors is the amount of players participating this round.
			// If there's a bye, this = Players.Count - 1.
			int numCompetitors = NumberOfPlayers();
			numCompetitors = (0 == numCompetitors % 2)
				? numCompetitors : (numCompetitors - 1);

			// competitors array is used to reference player-indexes when there's a bye.
			// So, competitors[0] => Players[1], if Players[0] has a bye.
			int[] competitors = new int[numCompetitors];
			for (int i = 0, pOffset = 0; i < numCompetitors; ++i)
			{
				if (PlayerByes.Count > 0 &&
					PlayerByes[PlayerByes.Count - 1] == Players[i].Id)
				{
					++pOffset;
				}
				competitors[i] = i + pOffset;
			}

			// Create a grid to store h-values for all possible matchups.
			// The x- and y-axes are indexes, referring:
			// IN-ORDER from the GROUPS list.
			// The inner values are heuristic weights for the matchup.
			int[,] heuristicGrid = new int[numCompetitors, numCompetitors];
			for (int y = 0; y < numCompetitors; ++y)
			{
				// Find PlayerY in the Groups list:
				int playerYindex = -1, groupNumberY = -1;
				for (int g = 0; g < _groups.Count; ++g)
				{
					playerYindex = _groups[g].FindIndex(num => num == competitors[y]);

					if (playerYindex > -1)
					{
						groupNumberY = g;
						break;
					}
				}

				// Make a list of "preferred" Matchups for the players in this player's group:
#if CHOOSE_PAIRING
				List<Matchup> groupYmatchups = CreatePairingsList(_groups[groupNumberY].Count);
#else
				List<Matchup> groupYmatchups = new List<Matchup>();
				int divisionPoint = (int)(_groups[groupNumberY].Count * 0.5);
				for (int i = 0; i < divisionPoint; ++i)
				{
					// Make fake "preferred" matchups for the players in this group, for use later:
					// SLIDE pairing:
					groupYmatchups.Add(new Matchup(i, i + divisionPoint, -1));
				}
#endif

				// matchupYindex is the index of the new matchup that PlayerY is in:
				int matchupYindex = groupYmatchups.FindIndex(m => m.ContainsInt(playerYindex));

				// Assign heuristic weights for PlayerY's matchups against every other player:
				for (int x = 0; x < numCompetitors; ++x)
				{
					if (x == y)
					{
						// Can't play against self! Add heuristic=100M:
						heuristicGrid[y, x] = 100000000;
						continue;
					}

					// Find PlayerX in the Groups list:
					int playerXindex = -1, groupNumberX = -1;
					for (int g = 0; g < _groups.Count; ++g)
					{
						playerXindex = _groups[g].FindIndex(num => num == competitors[x]);

						if (playerXindex > -1)
						{
							groupNumberX = g;
							break;
						}
					}

					// Add heuristic=20 for each group-line crossed for this matchup:
					heuristicGrid[y, x] = Math.Abs(groupNumberX - groupNumberY) * 20;

					// Add heuristic=1 for each slot *within group* away from preferred matchup.
					// This is denoted by "split."
					int split = 0;
					if (groupNumberY == groupNumberX)
					{
						// PlayerX and PlayerY are in the same group.
						// Use the preffered-matchup-list we made earlier for this:
						int idealMatchup = (groupYmatchups[matchupYindex].DefenderIndex == playerYindex)
							? groupYmatchups[matchupYindex].ChallengerIndex
							: groupYmatchups[matchupYindex].DefenderIndex;
						split = Math.Abs(idealMatchup - playerXindex);
					}
					else
					{
						// PlayerX and PlayerY are in different groups.
						// Create a preferred-matchup-list for PlayerX's group,
						// and "add on" PlayerY to the front or back.
#if CHOOSE_PAIRING
						List<Matchup> groupXmatchups = CreatePairingsList(_groups[groupNumberX].Count);
#else
						List<Matchup> groupXmatchups = new List<Matchup>();
						divisionPoint = (int)(_groups[groupNumberX].Count * 0.5);
						for (int i = 0; i < divisionPoint; ++i)
						{
							// Make fake "preferred" matchups for the players in this group:
							// SLIDE pairing:
							groupXmatchups.Add(new Matchup(i, i + divisionPoint, -1));
						}
#endif
						int matchupXindex, idealMatchup;
						if (groupNumberY > groupNumberX)
						{
							// First player is adding on to BACK of groupX
							matchupXindex = groupXmatchups
								.FindIndex(m => m.ContainsInt(_groups[groupNumberX].Count - 1));
							idealMatchup =
								(groupXmatchups[matchupXindex].DefenderIndex == _groups[groupNumberX].Count - 1)
								? groupXmatchups[matchupXindex].ChallengerIndex
								: groupXmatchups[matchupXindex].DefenderIndex;
						}
						else // if (groupNumberY < groupNumberX)
						{
							// PlayerY is adding on to FRONT of groupX
							matchupXindex = groupXmatchups.FindIndex(m => m.ContainsInt(0));
							idealMatchup =
								(groupXmatchups[matchupXindex].DefenderIndex == 0)
								? groupXmatchups[matchupXindex].ChallengerIndex
								: groupXmatchups[matchupXindex].DefenderIndex;
						}
						split = Math.Abs(idealMatchup - playerXindex);
					}
					heuristicGrid[y, x] += split;

					// Check for Rematch:
					foreach (Matchup matchup in Matchups)
					{
						if (matchup.ContainsInt(competitors[x]) &&
							matchup.ContainsInt(competitors[y]))
						{
							// Rematch found. Add heuristic=100K:
							heuristicGrid[y, x] += 100000;
							break;
						}
					}
				}
			}

			// Translate the big grid into a list of heuristic "edges":
			// Each node is a player, each edge is a possible matchup.
			List<int[]> heuristicGraph = new List<int[]>();

			for (int y = 0; y < (numCompetitors - 1); ++y)
			{
				for (int x = 1; x < numCompetitors; ++x)
				{
					if (y >= x)
					{
						// Skip edges we've already added, and self-edges:
						continue;
					}

					// Edge: [Defender index, Challenger index, negative heuristic value].
					// The sign is flipped on the h-value for use with the weight-function later.
					int[] edge = new int[3]
					{
						competitors[y],
						competitors[x],
						(-1 * (heuristicGrid[y, x] + heuristicGrid[x, y]))
					};
					heuristicGraph.Add(edge);
				}
			}

			// Return the graph (list of edges):
			return heuristicGraph;
		}

		/// <summary>
		/// Given a playercount (and Bracket's pairing method),
		/// creates a list of Matchups.
		/// Ex: 4 players = [0 3], [1 2].
		/// </summary>
		/// <param name="_numPlayers">Amount of players to pair up</param>
		/// <returns>List of Matchups</returns>
		private List<Matchup> CreatePairingsList(int _numPlayers)
		{
			List<Matchup> matchups = new List<Matchup>();

			// Truncate the division point; if numplayers is odd, one gets a "bye."
			int divisionPoint = (int)(_numPlayers * 0.5);

			switch (PairingMethod)
			{
				case PairingMethod.Adjacent:
					for (int i = 0; i < _numPlayers; i += 2)
					{
						matchups.Add(new Matchup(i, i + 1, -1));
					}
					break;
				case PairingMethod.Fold:
					for (int i = 0; i < divisionPoint; ++i)
					{
						matchups.Add(new Matchup(i, _numPlayers - i, -1));
					}
					break;
				case PairingMethod.Slide:
					for (int i = 0; i < divisionPoint; ++i)
					{
						matchups.Add(new Matchup(i, divisionPoint + i, -1));
					}
					break;
			}

			return matchups;
		}

		/// <summary>
		/// Resets every round after the given round index.
		/// This removes the Players from every affected Match,
		/// in addition to resetting their Games and scores.
		/// Will NOT remove the Players from round 1, even if passed a 0.
		/// Sets ActiveRound to the given round index.
		/// May fire MatchesModified and GamesDeleted events, if updates occur.
		/// If _currentRoundIndex is out of range, nothing is modified.
		/// </summary>
		/// <param name="_currentRoundIndex">Reset all rounds after this index</param>
		/// <returns>List of Models of altered Matches</returns>
		private List<MatchModel> RemoveFutureRounds(int _currentRoundIndex)
		{
			List<MatchModel> clearedMatches = new List<MatchModel>();
			List<int> deletedGameIDs = new List<int>();

			int nextRoundIndex = 1 + _currentRoundIndex;
			if (nextRoundIndex > NumberOfRounds)
			{
				// Recursive exit: we've reached the final round.
				return clearedMatches;
			}

			// Recursive call on all rounds after this one:
			clearedMatches.AddRange(RemoveFutureRounds(nextRoundIndex));

			if (_currentRoundIndex >= 0)
			{
				// Reset all Matches in this round:
				List<IMatch> nextRound = GetRound(nextRoundIndex);
				foreach (Match match in nextRound)
				{
					if (!(match.Players.Contains(null)) ||
						match.Games.Count > 0 ||
						match.IsManualWin)
					{
						deletedGameIDs.AddRange(match.Games.Select(g => g.Id));
						if (nextRoundIndex > 1)
						{
							// Remove the Players (and reset score & Games):
							match.ResetPlayers();
						}
						else
						{
							// Keep Players if Round = 1 (still reset score):
							match.ResetScore();
						}
						
						// Save a Model of the updated Match:
						clearedMatches.Add(GetMatchModel(match));
					}
				}
				if (nextRoundIndex > 1)
				{
					// For all rounds after 1,
					// delete associated Matchups and Bye:
					Matchups.RemoveAll(m => m.RoundNumber == nextRoundIndex);
					if (PlayerByes.Count == nextRoundIndex)
					{
						// Remove the last Bye in the list:
						PlayerByes.RemoveAt(PlayerByes.Count - 1);
					}

					// Update bracket Properties:
					ActiveRound = _currentRoundIndex;
				}

				// Fire notification event: Games were deleted.
				OnGamesDeleted(deletedGameIDs);
			}

			// Return a list of modified MatchModels:
			return clearedMatches;
		}
		#endregion
		#endregion
	}
}
