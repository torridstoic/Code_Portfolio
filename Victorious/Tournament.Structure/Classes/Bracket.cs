using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatabaseLib;

namespace Tournament.Structure
{
	/// <summary>
	/// One or more Brackets form a Tournament.
	/// Each Bracket is a group of Matches,
	/// and is designed either for elimination or seeding (or both).
	/// The type of bracket determines the progression and algorithms.
	/// </summary>
	public abstract class Bracket : IBracket
	{
		#region Variables & Properties
		public int Id
		{ get; protected set; }
		public BracketType BracketType
		{ get; protected set; }
		public bool IsFinalized
		{ get; protected set; }
		public bool IsFinished
		{ get; protected set; }
		public List<IPlayer> Players
		{ get; protected set; }
		public List<IPlayerScore> Rankings
		{ get; protected set; }
		public int AdvancingPlayers
		{ get; protected set; }
		public int MaxRounds
		{ get; set; }
		protected Dictionary<int, Match> Matches
		{ get; set; }
		public int NumberOfRounds
		{ get; protected set; }
		protected Dictionary<int, Match> LowerMatches
		{ get; set; }
		public int NumberOfLowerRounds
		{ get; protected set; }
		protected Match grandFinal
		{ get; set; }
		public IMatch GrandFinal
		{ get { return (grandFinal as IMatch); } }
		public int NumberOfMatches
		{ get; protected set; }
		protected int MatchWinValue
		{ get; set; }
		protected int MatchTieValue
		{ get; set; }
		#endregion

		#region Events
		/// <summary>
		/// Notifies subscribers when new Matches have been created and added.
		/// Associated BracketEventArgs must contain updated MatchModels
		/// for all new additions.
		/// </summary>
		public event EventHandler<BracketEventArgs> RoundAdded;

		/// <summary>
		/// Notifies subscribers when Matches have been deleted/removed.
		/// Associated BracketEventArgs must contain Models of all removals.
		/// May also contain Game ID's, if any were deleted.
		/// </summary>
		public event EventHandler<BracketEventArgs> RoundDeleted;

		/// <summary>
		/// Notifies subscribers when Matches have been changed.
		/// This can include adding/removing a Game,
		/// or changing the Match's values (such as MatchNumber).
		/// Associated BracketEventArgs must contain all updated MatchModels.
		/// May also contain Game ID's, if any were deleted.
		/// </summary>
		public event EventHandler<BracketEventArgs> MatchesModified;

		/// <summary>
		/// Notifies subscribers when Games have been deleted.
		/// Associated BracketEventArgs must contain ID's for all deleted Games.
		/// </summary>
		public event EventHandler<BracketEventArgs> GamesDeleted;

		/* 
		 * The following protected methods are helpers.
		 * They are included to simplify the firing of Bracket events.
		 * Each helper will translate the given parameter(s) to a relevant BracketEventArgs.
		 * If the passed list is empty, no event will be fired.
		 * If the relevant event's subscriber-list is empty/null, no event will be fired.
		*/

		protected virtual void OnRoundAdded(BracketEventArgs _e)
		{
			RoundAdded?.Invoke(this, _e);
		}
		protected void OnRoundDeleted(BracketEventArgs _e)
		{
			RoundDeleted?.Invoke(this, _e);
		}
		protected void OnRoundDeleted(List<MatchModel> _modelList)
		{
			if ((_modelList?.Count ?? 0) > 0)
			{
				OnRoundDeleted(new BracketEventArgs(_modelList));
			}
		}
		protected void OnMatchesModified(BracketEventArgs _e)
		{
			MatchesModified?.Invoke(this, _e);
		}
		protected void OnMatchesModified(List<MatchModel> _modelList)
		{
			if ((_modelList?.Count ?? 0) > 0)
			{
				OnMatchesModified(new BracketEventArgs(_modelList));
			}
		}
		protected void OnGamesDeleted(BracketEventArgs _e)
		{
			GamesDeleted?.Invoke(this, _e);
		}
		protected void OnGamesDeleted(List<IGame> _games)
		{
			if ((_games?.Count ?? 0) > 0)
			{
				OnGamesDeleted(new BracketEventArgs(_games
					.Select(g => g.Id).ToList()));
			}
		}
		protected void OnGamesDeleted(List<int> _gameIDs)
		{
			if ((_gameIDs?.Count ?? 0) > 0)
			{
				OnGamesDeleted(new BracketEventArgs(_gameIDs));
			}
		}
		#endregion

		#region Abstract Methods
		/*
		 * These abstract methods are fundamental to the behavior of brackets,
		 * but their specific functionality depends on the bracket type.
		 * See the derived bracket classes for implementation details.
		*/

		public abstract void CreateBracket(int _gamesPerMatch = 1);

		public abstract bool CheckForTies();
		public abstract bool GenerateTiebreakers();

		protected abstract List<MatchModel> ApplyWinEffects(int _matchNumber, PlayerSlot _slot);
		protected abstract List<MatchModel> ApplyGameRemovalEffects(int _matchNumber, List<GameModel> _games, PlayerSlot _formerMatchWinnerSlot);
		protected abstract void UpdateScore(int _matchNumber, List<GameModel> _games, bool _isAddition, MatchModel _oldMatch);
		protected abstract void RecalculateRankings();
		protected abstract void UpdateRankings();
		#endregion

		#region Public Methods
		/// <summary>
		/// Verifies this bracket's status is legal.
		/// This is called before allowing play to begin.
		/// </summary>
		/// <remarks>
		/// This method is overriden in some derived Brackets
		/// in order to allow additional type-specific functionality.
		/// </remarks>
		/// <returns>true if okay, false if errors</returns>
		public virtual bool Validate()
		{
			// Playercount must be at least 2.
			if ((Players?.Count ?? 0) < 2)
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Resets the state of all Matches.
		/// Deletes all Games and sets scores to 0-0.
		/// Removes Players from Matches they had advanced to.
		/// May fire MatchesModified and GamesDeleted events, if updates occur.
		/// </summary>
		/// <remarks>
		/// This method is overriden in some derived Brackets
		/// in order to allow additional type-specific functionality.
		/// </remarks>
		public virtual void ResetMatches()
		{
			List<MatchModel> alteredMatches = new List<MatchModel>();
			List<int> deletedGameIDs = new List<int>();

			for (int n = 1; n <= NumberOfMatches; ++n)
			{
				Match match = GetInternalMatch(n);
				bool affected = false;

				if (match.IsManualWin || match.Games.Count > 0)
				{
					// Populate the list for GamesDeleted event:
					affected = true;
					deletedGameIDs.AddRange(match.Games.Select(g => g.Id));
				}
				for (int i = 0; i < 2; ++i)
				{
					// Remove Players (but only if they advanced into this match):
					if (match.PreviousMatchNumbers[i] > -1 &&
						null != match.Players[i])
					{
						affected = true;
						match.RemovePlayer(match.Players[i].Id);
					}
				}

				match.ResetScore();
				if (affected)
				{
					// Populate the list for MatchesModified event:
					alteredMatches.Add(GetMatchModel(match));
				}
			}
			IsFinished = false;
			IsFinalized = false;

			OnGamesDeleted(deletedGameIDs);
			OnMatchesModified(alteredMatches);
		}

		#region Player Methods
		/// <summary>
		/// Gets the playercount.
		/// </summary>
		/// <returns>Players.Count</returns>
		public int NumberOfPlayers()
		{
			if (null == Players)
			{
				Players = new List<IPlayer>();
			}
			return Players.Count;
		}

		/// <summary>
		/// Finds a Player's seed value for this bracket.
		/// If the given Player ID is not found, an exception is thrown.
		/// </summary>
		/// <param name="_playerId">ID of Player to find</param>
		/// <returns>Seed number of given Player</returns>
		public int GetPlayerSeed(int _playerId)
		{
			if (null == Players)
			{
				throw new NullReferenceException
					("Players is null. This shouldn't happen...");
			}

			for (int i = 0; i < Players.Count; ++i)
			{
				if (_playerId == Players[i].Id)
				{
					// Each Player's seed is their (list index + 1):
					return (i + 1);
				}
			}

			throw new PlayerNotFoundException
				("Player not found in this Bracket!");
		}

		/// <summary>
		/// Randomized the order of the current playerlist.
		/// Reassigns seeds for all Players.
		/// Deletes any/all Matches.
		/// May fire MatchesModified and GamesDeleted events, if updates occur.
		/// </summary>
		public void RandomizeSeeds()
		{
			if (null == Players)
			{
				Players = new List<IPlayer>();
			}
			if (Players.Count < 2)
			{
				// Not applicable for playerlists smaller than 2. Just exit:
				DeleteBracketData();
				return;
			}

			List<IPlayer> pList = new List<IPlayer>();

			// Get random rolls for each player
			// (match rolls -> player-index)
			Random rng = new Random();
			Dictionary<int, int> rolls = new Dictionary<int, int>();
			for (int i = 0; i < Players.Count; ++i)
			{
				int rand = -1;
				while (rand < 0 || rolls.ContainsKey(rand))
				{
					// Roll random values until we get a valid one.
					// Valid is >=0 and not-yet used:
					rand = rng.Next(Players.Count * 3);
				}
				// Associate each Player with a random roll.
				// Dictionary Key = random roll, Value = old playerlist index:
				rolls.Add(rand, i);
			}

			// Sort rolls in a list...
			List<int> rollsList = rolls.Keys
				.OrderByDescending(v => v)
				.ToList();
			// ...then add Players to new temp playerlist in order:
			foreach (int key in rollsList)
			{
				pList.Add(Players[rolls[key]]);
			}

			// Replace the old playerlist with the new, and delete any Matches, etc.
			// (a new playerlist means Matches will have to be regenerated)
			Players = pList;
			DeleteBracketData();
		}

		/// <summary>
		/// Deletes the old playerlist (if any)
		/// and replaces it with the given list.
		/// Deletes any/all Matches.
		/// May fire MatchesModified and GamesDeleted events, if updates occur.
		/// </summary>
		/// <param name="_players">New playerlist for the Bracket</param>
		public void SetNewPlayerlist(List<IPlayer> _players)
		{
			if (null == _players)
			{
				throw new ArgumentNullException("_players");
			}

			Players = _players;
			DeleteBracketData();
		}

		/// <summary>
		/// Deletes the old playerlist (if any)
		/// and replaces it with one generated from the given Models.
		/// Deletes any/all Matches.
		/// May fire MatchesModified and GamesDeleted events, if updates occur.
		/// </summary>
		/// <param name="_players">List of Models associating Players and seeds</param>
		public void SetNewPlayerlist(ICollection<TournamentUsersBracketModel> _players)
		{
			// Order the given Collection by their seed values:
			List<TournamentUserModel> userModels = _players
				.OrderBy(p => p.Seed, new SeedComparer())
				.Select(p => p.TournamentUser)
				.ToList();

			// Construct Players from the Models, and save the new list:
			List<IPlayer> playerList = new List<IPlayer>();
			foreach (TournamentUserModel model in userModels)
			{
				playerList.Add(new Player(model));
			}
			SetNewPlayerlist(playerList);
		}

		/// <summary>
		/// Adds a Player to (the end of) the playerlist.
		/// Deletes any/all Matches.
		/// May fire MatchesModified and GamesDeleted events, if updates occur.
		/// If the Player is a duplicate, an exception is thrown.
		/// </summary>
		/// <param name="_player">Player to add</param>
		public void AddPlayer(IPlayer _player)
		{
			if (null == _player)
			{
				throw new ArgumentNullException("_player");
			}
			if (null == Players)
			{
				throw new NullReferenceException
					("Players is null. This shouldn't happen...");
			}
			if (Players.Contains(_player))
			{
				throw new DuplicateObjectException
					("Bracket already contains this Player!");
			}

			Players.Add(_player);
			DeleteBracketData();
		}

		/// <summary>
		/// Removes a Player from the playerlist,
		/// and replaces him with a given Player.
		/// The new Player inherits the old's seed value.
		/// The removed Player is replaced in all Matches, Games, & Rankings
		/// by the new Player.
		/// May fire MatchesModified event, if updates happen.
		/// If the Player-to-replace's index is invalid, an exception is thrown.
		/// </summary>
		/// <remarks>
		/// This method is overriden in some derived Brackets
		/// in order to allow additional type-specific functionality.
		/// </remarks>
		/// <param name="_player">Player to add</param>
		/// <param name="_index">Index (in playerlist) of Player to remove</param>
		public virtual void ReplacePlayer(IPlayer _player, int _index)
		{
			if (null == _player)
			{
				throw new ArgumentNullException("_player");
			}
			if (_index < 0 || _index >= Players.Count)
			{
				throw new InvalidIndexException
					("Invalid index; outside Playerlist bounds.");
			}

			List<MatchModel> alteredMatches = new List<MatchModel>();

			if (null != Players[_index])
			{
				// Replace existing Player in any Matches
				for (int n = 1; n <= NumberOfMatches; ++n)
				{
					try
					{
						Match match = GetInternalMatch(n);
						match.ReplacePlayer(_player, Players[_index].Id);
						// If this Match was altered, save its Model for firing an event:
						alteredMatches.Add(GetMatchModel(match));
					}
					catch (PlayerNotFoundException)
					{ }
				}
				// Replace existing Player in the Rankings
				for (int i = 0; i < Rankings.Count(); ++i)
				{
					if (Rankings[i].Id == Players[_index].Id)
					{
						Rankings[i].ReplacePlayerData(_player.Id, _player.Name);
						break;
					}
				}
			}

			// Now we can replace the Player in the playerlist,
			// and fire a notification event before continuing:
			Players[_index] = _player;
			OnMatchesModified(alteredMatches);
		}

		/// <summary>
		/// Removes a Player from the playerlist.
		/// Deletes any/all Matches.
		/// May fire MatchesModified and GamesDeleted events, if updates occur.
		/// If the Player is not found, an exception is thrown.
		/// </summary>
		/// <param name="_playerId">ID of Player to remove</param>
		public void RemovePlayer(int _playerId)
		{
			if (null == Players)
			{
				throw new NullReferenceException
					("Playerlist is null. This shouldn't happen...");
			}

			for (int i = 0; i < Players.Count; ++i)
			{
				if (Players[i].Id == _playerId)
				{
					Players.RemoveAt(i);
					DeleteBracketData();
					return;
				}
			}
			throw new PlayerNotFoundException
				("Player not found in this Bracket!");
		}

		/// <summary>
		/// Swaps the seeds of two Players.
		/// (also swaps their positions in the playerlist)
		/// Deletes any/all Matches.
		/// May fire MatchesModified and GamesDeleted events, if updates occur.
		/// If either playerindex is invalid, an exception is thrown.
		/// </summary>
		/// <param name="_index1">Playerlist index of Player</param>
		/// <param name="_index2">Playerlist index of Player</param>
		public void SwapPlayers(int _index1, int _index2)
		{
			if (_index1 < 0 || _index1 >= Players.Count
				|| _index2 < 0 || _index2 >= Players.Count)
			{
				throw new InvalidIndexException
					("Invalid index; outside Playerlist bounds.");
			}

			IPlayer tmp = Players[_index1];
			Players[_index1] = Players[_index2];
			Players[_index2] = tmp;

			DeleteBracketData();
		}

		/// <summary>
		/// Applies a new seed to one Player.
		/// (also moves him in the playerlist)
		/// Other Players will be shifted up/down as needed.
		/// Deletes any/all Matches.
		/// May fire MatchesModified and GamesDeleted events, if updates occur.
		/// If either playerindex is invalid, an exception is thrown.
		/// </summary>
		/// <param name="_oldIndex">Playerlist index of Player to move</param>
		/// <param name="_newIndex">Playerlist index of destination</param>
		public void ReinsertPlayer(int _oldIndex, int _newIndex)
		{
			if (_oldIndex < 0 || _oldIndex >= Players.Count
				|| _newIndex < 0 || _newIndex >= Players.Count)
			{
				throw new InvalidIndexException
					("Invalid index; outside Playerlist bounds.");
			}
			if (_oldIndex == _newIndex)
			{
				return;
			}

			IPlayer tmp = Players[_oldIndex];
			if (_oldIndex > _newIndex)
			{
				// We're moving the Player "down" in the list.
				// All other affected Players need to be shifted up one:
				for (int i = _oldIndex; i > _newIndex; --i)
				{
					Players[i] = Players[i - 1];
				}
			}
			else // _oldIndex < _newIndex
			{
				// We're moving the Player "up" in the list.
				// All other affected Players need to be shifted down one:
				for (int i = _oldIndex; i < _newIndex; ++i)
				{
					Players[i] = Players[i + 1];
				}
			}
			
			// Now, reinsert the Player (and reset the bracket):
			Players[_newIndex] = tmp;
			DeleteBracketData();
		}

		/// <summary>
		/// Clears the playerlist.
		/// Deletes any/all Matches.
		/// May fire MatchesModified and GamesDeleted events, if updates occur.
		/// </summary>
		public void ResetPlayers()
		{
			if (null == Players)
			{
				Players = new List<IPlayer>();
			}
			Players.Clear();
			DeleteBracketData();
		}
		#endregion

		#region Match & Game Methods
		/// <summary>
		/// Adds a new Game to a Match.
		/// Takes a given gamescore to create a new Game.
		/// The Game is added to the Match's gamelist, and the Match's score & status is updated.
		/// The new game's affects on the rest of the Bracket are also applied here.
		/// Rankings are updated as necessary.
		/// May fire events: MatchesModified, RoundDeleted.
		/// If Match can't be found, an exception is thrown.
		/// If the Match is finished or not ready, an exception is thrown.
		/// If game-data input is invalid, an exception is thrown.
		/// </summary>
		/// <param name="_matchNumber">Number of Match to add to</param>
		/// <param name="_defenderScore">First player's score</param>
		/// <param name="_challengerScore">Second player's score</param>
		/// <param name="_winnerSlot">Game winner: Defender or Challenger</param>
		/// <returns>Model of added Game</returns>
		public GameModel AddGame(int _matchNumber, int _defenderScore, int _challengerScore, PlayerSlot _winnerSlot)
		{
			Match match = GetInternalMatch(_matchNumber);
			MatchModel oldModel = GetMatchModel(match);

			// Add the new Game (only affects the Match, not the bracket):
			GameModel gameModel = match
				.AddGame(_defenderScore, _challengerScore, _winnerSlot);

			// UpdateScore will update the bracket's Rankings:
			UpdateScore(_matchNumber, new List<GameModel>() { gameModel }, true, oldModel);

			// ApplyWinEffects will apply any updates to other affected Matches:
			// (also updates the Bracket's status, as necessary)
			List<MatchModel> alteredMatches = ApplyWinEffects(_matchNumber, _winnerSlot);

			// Fire Event with any Matches that changed:
			alteredMatches.Add(GetMatchModel(match));
			OnMatchesModified(alteredMatches);

			// Return a Model of the new Game:
			return gameModel;
		}

		/// <summary>
		/// Applies the given gamescore to an existing Game within a Match.
		/// The Match's score & status is updated.
		/// If the update affects the rest of the Bracket, those changes are applied.
		/// Rankings are updated as necessary.
		/// May fire events: MatchesModified, GamesDeleted, RoundDeleted.
		/// If Match or Game is not fond, an exception is thrown.
		/// If game-data input is invalid, an exception is thrown.
		/// </summary>
		/// <param name="_matchNumber">Number of Match to update</param>
		/// <param name="_gameNumber">Number of Game to update</param>
		/// <param name="_defenderScore">First Player's score</param>
		/// <param name="_challengerScore">Second Player's score</param>
		/// <param name="_winnerSlot">Game winner: Defender or Challenger</param>
		/// <returns></returns>
		public GameModel UpdateGame(int _matchNumber, int _gameNumber, int _defenderScore, int _challengerScore, PlayerSlot _winnerSlot)
		{
			Match match = GetInternalMatch(_matchNumber);
			int gameIndex = match.Games.FindIndex(g => g.GameNumber == _gameNumber);
			if (gameIndex < 0)
			{
				// Case 1: Game doesn't exist (this is bad):
				throw new GameNotFoundException
					("Game not found; Game Number may be invalid!");
			}

			MatchModel oldMatchModel = GetMatchModel(match);
			List<MatchModel> alteredMatches = new List<MatchModel>();
			List<GameModel> alteredGames = new List<GameModel>();

			if (match.Games[gameIndex].WinnerSlot == _winnerSlot)
			{
				// Case 2: Game winner won't change.
				// This won't affect other Matches, so our job is easier.

				// Subtract old scores from rankings:
				GameModel oldGame = match.Games[gameIndex].GetModel();
				UpdateScore(_matchNumber, new List<GameModel> { oldGame }, false, oldMatchModel);

				// Update the Game's score...
				match.Games[gameIndex].Score[(int)PlayerSlot.Defender] = _defenderScore;
				match.Games[gameIndex].Score[(int)PlayerSlot.Challenger] = _challengerScore;
				// and save the updated GameModel for returning:
				alteredGames.Add(match.Games[gameIndex].GetModel());
				alteredGames[0].MatchID = match.Id;

				// Add new scores to Rankings:
				UpdateScore(_matchNumber, alteredGames, true, oldMatchModel);
			}
			else
			{
				// Case 3: Game winner changes.
				// This is the hard case. Other Matches may well be affected.

				// Remove (and save) the current/old Game from the Match.
				// This will affect the Bracket as if that Game had not been entered:
				GameModel removedGame = RemoveGameNumber(_matchNumber, _gameNumber, true);
				oldMatchModel = GetMatchModel(match);

				// Update the Rankings according to this state:
				UpdateScore(_matchNumber, new List<GameModel> { removedGame }, false, oldMatchModel);

				// Update the Game's values:
				removedGame.DefenderScore = _defenderScore;
				removedGame.ChallengerScore = _challengerScore;
				removedGame.WinnerID = (PlayerSlot.Defender == _winnerSlot)
					? removedGame.DefenderID : -1;
				removedGame.WinnerID = (PlayerSlot.Challenger == _winnerSlot)
					? removedGame.ChallengerID : removedGame.WinnerID;

				// Add the updated Game back to the Match:
				alteredGames.Add(match.AddGame(removedGame));

				// Apply the "new" Game's affects to the rest of the Bracket:
				UpdateScore(_matchNumber, alteredGames, true, oldMatchModel);
				alteredMatches.AddRange(ApplyWinEffects(_matchNumber, _winnerSlot));
			}

			// Fire Event with any changed Matches:
			alteredMatches.Add(GetMatchModel(match));
			OnMatchesModified(alteredMatches);

			// Return a Model of the updated Game:
			return alteredGames[0];
		}

		/// <summary>
		/// Remove the last Game from a Match.
		/// Applies necessary affects to other, related Matches.
		/// Updates scores and Rankings accordingly.
		/// May fire events: MatchesModified, GamesDeleted.
		/// If Match is not found, an exception is thrown.
		/// If Match has no games, an exception is thrown.
		/// </summary>
		/// <param name="_matchNumber">Match to alter</param>
		/// <returns>Model of removed Game</returns>
		public GameModel RemoveLastGame(int _matchNumber)
		{
			// Find the Match and Game in question:
			IGame lastGame = GetInternalMatch(_matchNumber).Games.LastOrDefault();
			if (null == lastGame)
			{
				throw new GameNotFoundException
					("No Games to remove!");
			}

			// Call RemoveGameNumber to handle the rest of the work.
			// This method handles the Game removal,
			// the Bracket-wide updates,
			// and the score/Rankings updates.
			return (RemoveGameNumber(_matchNumber, lastGame.GameNumber));
		}

		/// <summary>
		/// Remove a specified Game from a Match.
		/// Applies necessary affects to other, related Matches.
		/// Updates scores and Rankings accordingly.
		/// _updateInstead determines whether to notify the DB to delete the game.
		/// May fire events: MatchesModified, GamesDeleted.
		/// If Match or Game is not found, an exception is thrown.
		/// </summary>
		/// <param name="_matchNumber">Match to alter</param>
		/// <param name="_gameNumber">Game to remove</param>
		/// <param name="_updateInstead">false if removing the game, true if updating</param>
		/// <returns>Model of removed Game</returns>
		public GameModel RemoveGameNumber(int _matchNumber, int _gameNumber, bool _updateInstead = false)
		{
			Match match = GetInternalMatch(_matchNumber);
			MatchModel oldMatchModel = GetMatchModel(match);
			PlayerSlot winnerSlot = match.WinnerSlot;
			List<GameModel> modelList = new List<GameModel>();

			// Remove the Game and update the Bracket & Rankings:
			modelList.Add(match.RemoveGameNumber(_gameNumber));
			List<MatchModel> alteredMatches = ApplyGameRemovalEffects(_matchNumber, modelList, winnerSlot);
			UpdateScore(_matchNumber, modelList, false, oldMatchModel);

			// Fire Event with any Matches that changed:
			alteredMatches.Add(GetMatchModel(match));
			BracketEventArgs args = null;
			if (_updateInstead)
			{
				args = new BracketEventArgs(alteredMatches);

			}
			else
			{
				args = new BracketEventArgs
					(alteredMatches, modelList.Select(g => g.GameID).ToList());
			}
			OnMatchesModified(args);

			// Return a Model of the removed Game:
			return modelList[0];
		}

		/// <summary>
		/// Manually sets a winner for a Match, regardless of Games or score.
		/// This can signify a forfeit or default-win.
		/// Applies necessary affects to other, related Matches.
		/// Sets the matchscore to [-1, 0], but this translates to correct values for Rankings.
		/// May fire events: MatchesModified, RoundDeleted, GamesDeleted.
		/// If Match is not found, an exception is thrown.
		/// If the Match is finished or not ready, an exception is thrown.
		/// If the winner slot is invalid, an exception is thrown.
		/// </summary>
		/// <param name="_matchNumber">Number of Match to alter</param>
		/// <param name="_winnerSlot">Slot of winner: Defender or Challenger</param>
		public void SetMatchWinner(int _matchNumber, PlayerSlot _winnerSlot)
		{
			Match match = GetInternalMatch(_matchNumber);
			MatchModel oldMatchModel = GetMatchModel(match);
			List<GameModel> modelList = new List<GameModel>();

			if (PlayerSlot.unspecified != match.WinnerSlot ||
				match.Games.Count > 0)
			{
				// Remove (and keep) all the Games from this Match:
				if (_winnerSlot != match.WinnerSlot)
				{
					// The current winner of the Match (if any)
					// does not match the one we're applying.
					// Reset the Match AND other affected Matches:
					modelList = ResetMatchScore(_matchNumber);
				}
				else
				{
					// The current Match winner won't change. (this is rare)
					// Reset just the one Match:
					modelList = match.ResetScore();
					UpdateScore(_matchNumber, modelList, false, oldMatchModel);
				}
			}

			oldMatchModel = GetMatchModel(match);

			// Set the match winner directly:
			match.SetWinner(_winnerSlot);

			// Then re-add any Games we removed:
			foreach (GameModel model in modelList)
			{
				match.AddGame(model);
			}

			// Update the Rankings:
			UpdateScore(_matchNumber, null, true, oldMatchModel);

			// Apply any affects to the rest of the Matches:
			List<MatchModel> alteredMatches = ApplyWinEffects(_matchNumber, _winnerSlot);

			// Fire Event with any Matches that changed:
			alteredMatches.Add(GetMatchModel(match));
			OnMatchesModified(alteredMatches);
		}

		/// <summary>
		/// Resets a Match's score and status, deleting any Games.
		/// Applies necessary affects to other, related Matches.
		/// Updates Rankings accordingly.
		/// May fire events: MatchesModified, GamesDeleted.
		/// If Match is not found, an exception is thrown.
		/// </summary>
		/// <param name="_matchNumber">Number of Match to alter</param>
		/// <returns>Models of any deleted Games</returns>
		public List<GameModel> ResetMatchScore(int _matchNumber)
		{
			Match match = GetInternalMatch(_matchNumber);
			PlayerSlot winnerSlot = match.WinnerSlot;
			MatchModel oldMatchModel = GetMatchModel(match);

			// Reset the Match's score, removing any existing Games:
			List<GameModel> modelList = match.ResetScore();

			// Apply any affects to related Matches in the bracket:
			List<MatchModel> alteredMatches = ApplyGameRemovalEffects(_matchNumber, modelList, winnerSlot);

			// Update the Rankings according to new results:
			UpdateScore(_matchNumber, modelList, false, oldMatchModel);

			// Fire Event with any Matches that changed:
			alteredMatches.Add(GetMatchModel(match));
			OnMatchesModified(new BracketEventArgs
				(alteredMatches, modelList.Select(g => g.GameID).ToList()));
			// Return Models of any removed Games:
			return modelList;
		}
		#endregion

		#region Accessors
		/// <summary>
		/// Creates a Model of this Bracket's current state.
		/// Any contained objects (Players, Matches) are also converted into Models.
		/// </summary>
		/// <remarks>
		/// This method is overriden in Group Stages.
		/// </remarks>
		/// <param name="_tournamentID">ID of containing Tournament</param>
		/// <returns>Matching BracketModel</returns>
		public virtual BracketModel GetModel(int _tournamentID)
		{
			BracketModel model = new BracketModel();
			model.TournamentID = _tournamentID;
			model.BracketID = this.Id;
			model.BracketTypeID = Convert.ToInt32(this.BracketType);
			model.Finalized = this.IsFinalized;
			model.NumberOfGroups = 0;
			model.MaxRounds = this.MaxRounds;
			model.NumberPlayersAdvance = this.AdvancingPlayers;

			// Convert the BracketType to relevant model, and add it:
			model.BracketType = new BracketTypeModel();
			model.BracketType.BracketTypeID = model.BracketTypeID;
			model.BracketType.Type = this.BracketType;
			model.BracketType.TypeName = this.BracketType.ToString("f");

			// Convert all Players to Models, and add them:
			//model.TournamentUsersBrackets = new List<TournamentUsersBracketModel>();
			foreach (IPlayer player in Players)
			{
				TournamentUsersBracketModel m = player.GetTournamentUsersBracketModel(this.Id, GetPlayerSeed(player.Id), _tournamentID);
				model.TournamentUsersBrackets.Add(m);
			}

			// Convert all Matches to Models, and add them:
			//model.Matches = new List<MatchModel>();
			//if (!(this is IGroupStage))
			//{
				for (int n = 1; n <= NumberOfMatches; ++n)
				{
					model.Matches.Add(GetMatchModel(n));
				}
			//}

			return model;
		}

		/// <summary>
		/// Gets all IMatches in the given Round.
		/// The list of IMatches is correctly ordered.
		/// If the round index is out-of-range, an exception is thrown.
		/// </summary>
		/// <param name="_round">1-indexed</param>
		/// <returns>Ordered list of IMatches</returns>
		public List<IMatch> GetRound(int _round)
		{
			if (null == Matches)
			{
				throw new NullReferenceException
					("Matches doesn't exist! Create a bracket first.");
			}
			if (_round < 1)
			{
				throw new InvalidIndexException
					("Round index cannot be less than 1!");
			}

			return Matches.Values
				.Where(m => m.RoundIndex == _round)
				.OrderBy(m => m.MatchIndex)
				.Cast<IMatch>()
				.ToList();
		}

		/// <summary>
		/// Gets all IMatches in the given Lower-Bracket Round.
		/// The list of IMatches is correctly ordered.
		/// If the round index is out-of-range, an exception is thrown.
		/// </summary>
		/// <param name="_round">1-indexed</param>
		/// <returns>Ordered list of IMatches</returns>
		public List<IMatch> GetLowerRound(int _round)
		{
			if (null == LowerMatches)
			{
				throw new NullReferenceException
					("Lower Round doesn't exist!");
			}
			if (_round < 1)
			{
				throw new InvalidIndexException
					("Round index cannot be less than 1!");
			}

			return LowerMatches.Values
				.Where(m => m.RoundIndex == _round)
				.OrderBy(m => m.MatchIndex)
				.Cast<IMatch>()
				.ToList();
		}

		/// <summary>
		/// Gets one IMatch, from its Match Number.
		/// If no matching IMatch is found, an exception is thrown.
		/// </summary>
		/// <param name="_matchNumber">Number of Match to find</param>
		/// <returns>relevant IMatch</returns>
		public virtual IMatch GetMatch(int _matchNumber)
		{
			return (GetInternalMatch(_matchNumber) as IMatch);
		}

		/// <summary>
		/// Creates a Model from the given Match Number's Match.
		/// If no matching Match is found, an exception is thrown.
		/// </summary>
		/// <param name="_matchNumber">Number of Match to find/use for Model</param>
		/// <returns>Matching MatchModel</returns>
		public MatchModel GetMatchModel(int _matchNumber)
		{
			MatchModel model = GetInternalMatch(_matchNumber).GetModel();
			model.BracketID = this.Id;
			return model;
		}
		#endregion
		#region Mutators
		/// <summary>
		/// Sets MaxGames for every Match in the given round.
		/// Can also be used to affect the GrandFinal (rounds + 1).
		/// If the given value is invalid, an exception is thrown.
		/// If the round can't be found, an exception is thrown.
		/// If any Match in the round is finished, an exception is thrown.
		/// </summary>
		/// <param name="_round">Number of round to affect</param>
		/// <param name="_maxGamesPerMatch">Max Games per Match</param>
		public virtual void SetMaxGamesForWholeRound(int _round, int _maxGamesPerMatch)
		{
			if (_maxGamesPerMatch < 1)
			{
				throw new ScoreException
					("Games per match cannot be less than 1!");
			}

			List<IMatch> round = null;
			if (null != grandFinal && _round == 1 + NumberOfRounds)
			{
				// If the given Round is (rounds+1),
				// then apply effects to the Grand Final:
				round = new List<IMatch>() { grandFinal };
			}
			else
			{
				round = GetRound(_round);
			}

			if (round.Any(m => m.IsFinished))
			{
				throw new InactiveMatchException
					("One or more matches in this round is already finished!");
			}

			foreach (Match match in round)
			{
				match.SetMaxGames(_maxGamesPerMatch);
			}
		}

		/// <summary>
		/// Sets MaxGames for every Match in the given Lower-Bracket round.
		/// If the given value is invalid, an exception is thrown.
		/// If the round can't be found, an exception is thrown.
		/// If any Match in the round is finished, an exception is thrown.
		/// </summary>
		/// <param name="_round">Number of round to affect</param>
		/// <param name="_maxGamesPerMatch">Max Games per Match</param>
		public virtual void SetMaxGamesForWholeLowerRound(int _round, int _maxGamesPerMatch)
		{
			if (_maxGamesPerMatch < 1)
			{
				throw new ScoreException
					("Games per match cannot be less than 1!");
			}

			List<IMatch> round = GetLowerRound(_round);
			if (round.Any(m => m.IsFinished))
			{
				throw new InactiveMatchException
					("One or more matches in this round is already finished!");
			}

			foreach (Match match in round)
			{
				match.SetMaxGames(_maxGamesPerMatch);
			}
		}
		#endregion
		#endregion

		#region Private Methods
		/// <summary>
		/// Sets this Bracket's main data from a related BracketModel.
		/// Data affected includes most fields, as well as the playerlist.
		/// </summary>
		/// <remarks>
		/// This is overriden in Group Stages for additional functionality.
		/// </remarks>
		/// <param name="_model">Related BracketModel</param>
		protected virtual void SetDataFromModel(BracketModel _model)
		{
			if (null == _model)
			{
				throw new ArgumentNullException("_model");
			}

			this.Id = _model.BracketID;
			this.BracketType = _model.BracketType.Type;
			this.IsFinalized = _model.Finalized;
			this.AdvancingPlayers = _model.NumberPlayersAdvance;
			this.MaxRounds = _model.MaxRounds;
			this.MatchWinValue = 2;
			this.MatchTieValue = 1;

			// Convert TournamentUserBracketModels into Players,
			// and add them to the Bracket's playerlist:
			List<TournamentUserModel> userModels = _model.TournamentUsersBrackets
				.OrderBy(u => u.Seed, new SeedComparer())
				.Select(u => u.TournamentUser)
				.ToList();
			this.Players = new List<IPlayer>();
			foreach (TournamentUserModel userModel in userModels)
			{
				Players.Add(new Player(userModel));
			}

			ResetBracketData();
		}

		/// <summary>
		/// Resets the Bracket.
		/// Affects Matches, Rankings, and bracket status.
		/// </summary>
		/// <remarks>
		/// This is overriden in Group Stages and Swiss, for added functionality.
		/// </remarks>
		protected virtual void ResetBracketData()
		{
			if (null == Matches)
			{
				Matches = new Dictionary<int, Match>();
			}
			if (null == LowerMatches)
			{
				LowerMatches = new Dictionary<int, Match>();
			}
			if (null == Rankings)
			{
				Rankings = new List<IPlayerScore>();
			}

			IsFinished = false;
			Matches.Clear();
			LowerMatches.Clear();
			grandFinal = null;
			NumberOfRounds = NumberOfLowerRounds = 0;
			NumberOfMatches = 0;
			Rankings.Clear();
		}

		/// <summary>
		/// Resets the Bracket, AND fires appropriate events:
		/// MatchesModified and GamesDeleted.
		/// </summary>
		protected void DeleteBracketData()
		{
			List<MatchModel> alteredMatches = new List<MatchModel>();
			List<int> deletedGameIDs = new List<int>();
			if (Matches?.Count > 0)
			{
				for (int n = 1; n <= NumberOfMatches; ++n)
				{
					// For each Match, save the Model...
					MatchModel match = GetMatchModel(n);
					alteredMatches.Add(match);
					// ... and save any Game ID's (these will be deleted):
					deletedGameIDs.AddRange(match.Games.Select(g => g.GameID));
				}
			}

			// ResetBracketData handles the work of resetting the Bracket:
			ResetBracketData();

			// Fire the events we built:
			OnGamesDeleted(deletedGameIDs);
			OnMatchesModified(alteredMatches);
		}

		/// <summary>
		/// Gets one Match, from its Match Number.
		/// If no matching Match is found, an exception is thrown.
		/// </summary>
		/// <param name="_matchNumber">Number of Match to find</param>
		/// <returns>relevant Match</returns>
		/// <remarks>
		/// This method is overriden in Group Stages.
		/// </remarks>
		protected virtual Match GetInternalMatch(int _matchNumber)
		{
			if (_matchNumber < 1)
			{
				throw new InvalidIndexException
					("Match number cannot be less than 1!");
			}

			if (grandFinal?.MatchNumber == _matchNumber)
			{
				return grandFinal;
			}
			if (Matches?.ContainsKey(_matchNumber) ?? false)
			{
				return Matches[_matchNumber];
			}
			if (LowerMatches?.ContainsKey(_matchNumber) ?? false)
			{
				return LowerMatches[_matchNumber];
			}

			throw new MatchNotFoundException
				("Match not found; match number may be invalid.");
		}

		/// <summary>
		/// Creates a Model of the given Match's current state,
		/// and adds this Bracket's ID.
		/// </summary>
		/// <param name="_match">Match to Model</param>
		/// <returns>Matching Model of Match</returns>
		protected MatchModel GetMatchModel(IMatch _match)
		{
			MatchModel model = (_match as Match).GetModel();
			model.BracketID = this.Id;
			return model;
		}

		/// <summary>
		/// Sorting helper method for ordering the Rankings by score metrics.
		/// This is used in scoring-type brackets, such as Round Robin.
		/// </summary>
		protected int SortRankingScores(IPlayerScore first, IPlayerScore second)
		{
			// Rankings sorting: MatchScore > OpponentsScore > GameScore > PointsScore > initial Seeding
			int compare = -1 * (first.CalculateScore(MatchWinValue, MatchTieValue, 0)
				.CompareTo(second.CalculateScore(MatchWinValue, MatchTieValue, 0)));
			compare = (compare != 0)
				? compare : -1 * (first.OpponentsScore.CompareTo(second.OpponentsScore));
			compare = (compare != 0)
				? compare : -1 * (first.GameScore.CompareTo(second.GameScore));
			compare = (compare != 0)
				? compare : -1 * (first.PointsScore.CompareTo(second.PointsScore));
			return (compare != 0)
				? compare : GetPlayerSeed(first.Id).CompareTo(GetPlayerSeed(second.Id));
		}

		/// <summary>
		/// Sorting helper method for ordering the Rankings by finish position.
		/// This is used in knockout brackets and Group Stages.
		/// </summary>
		protected int SortRankingRanks(IPlayerScore first, IPlayerScore second)
		{
			int compare = first.Rank.CompareTo(second.Rank);
			return (compare != 0)
				? compare
				: GetPlayerSeed(first.Id).CompareTo(GetPlayerSeed(second.Id));
		}
		#endregion
	}
}
