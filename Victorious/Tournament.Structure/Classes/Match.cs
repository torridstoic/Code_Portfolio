using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatabaseLib;

namespace Tournament.Structure
{
	/// <summary>
	/// Matches combine, in different combinations, to form Brackets.
	/// Similarly, Match objects are always associated with a containing Bracket,
	/// and contain information such as the two competing Players, a List of Games, and score.
	/// </summary>
	public class Match : IMatch
	{
		#region Variables & Properties
		public int Id
		{ get; private set; }
		public bool IsReady
		{ get; private set; }
		public bool IsFinished
		{ get; private set; }
		public bool IsManualWin
		{ get; private set; }
		public int MaxGames
		{ get; private set; }
		public IPlayer[] Players
		{ get; private set; }
		public PlayerSlot WinnerSlot
		{ get; private set; }
		public List<IGame> Games
		{ get; private set; }
		public int[] Score
		{ get; private set; }
		public int GroupNumber
		{ get; private set; }
		public int RoundIndex
		{ get; private set; }
		public int MatchIndex
		{ get; private set; }
		public int MatchNumber
		{ get; private set; }
		public int[] PreviousMatchNumbers
		{ get; private set; }
		public int NextMatchNumber
		{ get; private set; }
		public int NextLoserMatchNumber
		{ get; private set; }
		#endregion

		#region Ctors
		public Match()
		{
			Id = 0;

			IsReady = false;
			IsFinished = false;
			IsManualWin = false;
			MaxGames = 1;

			Players = new IPlayer[2] { null, null };
			WinnerSlot = PlayerSlot.unspecified;
			Games = new List<IGame>();
			Score = new int[2] { 0, 0 };

			GroupNumber = -1;
			RoundIndex = -1;
			MatchIndex = -1;
			MatchNumber = -1;
			PreviousMatchNumbers = new int[2] { -1, -1 };
			NextMatchNumber = -1;
			NextLoserMatchNumber = -1;
		}
		[System.Obsolete("Copy Ctor is out-of-date: update before using.", true)]
		public Match(IMatch _match)
		{
			if (null == _match)
			{
				throw new ArgumentNullException("_match");
			}

			this.Id = _match.Id;
			this.IsReady = _match.IsReady;
			this.IsFinished = _match.IsFinished;
			this.IsManualWin = _match.IsManualWin; // NOTE : This needs fixing.
			this.MaxGames = _match.MaxGames;
			this.WinnerSlot = _match.WinnerSlot;
			this.GroupNumber = _match.GroupNumber;
			this.RoundIndex = _match.RoundIndex;
			this.MatchIndex = _match.MatchIndex;
			this.MatchNumber = _match.MatchNumber;
			this.NextMatchNumber = _match.NextMatchNumber;
			this.NextLoserMatchNumber = _match.NextLoserMatchNumber;

			this.Players = new IPlayer[2];
			this.Score = new int[2];
			this.PreviousMatchNumbers = new int[2];
			for (int i = 0; i < 2; ++i)
			{
				this.Players[i] = _match.Players[i];
				//this.Score[i] = _match.Score[i];
				this.PreviousMatchNumbers[i] = _match.PreviousMatchNumbers[i];
			}

			this.Games = new List<IGame>();
			foreach (IGame game in _match.Games.OrderBy(g => g.GameNumber))
			{
				//this.Games.Add(game);
				this.AddGame(game);
			}
		}
		public Match(MatchModel _model)
		{
			this.SetFromModel(_model);
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Creates a Model of this Match's current state.
		/// Any contained objects are also converted into Models.
		/// </summary>
		/// <returns>Matching MatchModel</returns>
		public MatchModel GetModel()
		{
			MatchModel model = new MatchModel();

			model.MatchID = this.Id;
			model.GroupNumber = this.GroupNumber;
			model.RoundIndex = this.RoundIndex;
			model.MatchIndex = this.MatchIndex;
			model.MatchNumber = this.MatchNumber;
			model.NextMatchNumber = this.NextMatchNumber;
			model.NextLoserMatchNumber = this.NextLoserMatchNumber;
			model.PrevDefenderMatchNumber = this.PreviousMatchNumbers[(int)PlayerSlot.Defender];
			model.PrevChallengerMatchNumber = this.PreviousMatchNumbers[(int)PlayerSlot.Challenger];
			model.MaxGames = this.MaxGames;

			model.ChallengerID = Players[(int)PlayerSlot.Challenger]?.Id
				?? -1;
			model.DefenderID = Players[(int)PlayerSlot.Defender]?.Id
				?? -1;
			model.WinnerID = (PlayerSlot.unspecified == WinnerSlot)
				? -1 : Players[(int)WinnerSlot].Id;
#if false
			// Create and add PlayerModel objects if applicable:
			model.Defender = Players[(int)PlayerSlot.Defender]?.GetTournamentUserModel();
			model.Challenger = Players[(int)PlayerSlot.Challenger]?.GetTournamentUserModel();
#endif

			model.ChallengerScore = this.Score[(int)PlayerSlot.Challenger];
			model.DefenderScore = this.Score[(int)PlayerSlot.Defender];
			//model.Games = new List<GameModel>();
			foreach (IGame game in this.Games)
			{
				model.Games.Add(GetGameModel(game));
			}

			return model;
		}

		/// <summary>
		/// Sets this object's values from a related MatchModel.
		/// Convert's any null values into appropriate defaults.
		/// Note: passing an unrealistic MatchModel can cause impossible situations.
		/// </summary>
		/// <param name="_model">MatchModel with data</param>
		public void SetFromModel(MatchModel _model)
		{
			if (null == _model)
			{
				throw new ArgumentNullException("_model");
			}

			this.Id = _model.MatchID;
			this.MaxGames = _model.MaxGames.GetValueOrDefault(1);
			this.WinnerSlot = PlayerSlot.unspecified;

			// Copy Players:
			this.Players = new IPlayer[2];
			Players[(int)PlayerSlot.Defender] = (null == _model.Defender)
				? null : new Player(_model.Defender);
			Players[(int)PlayerSlot.Challenger] = (null == _model.Challenger)
				? null : new Player(_model.Challenger);
			// Match is "Ready" if both player slots are filled
			this.IsReady = (null != Players[0] && null != Players[1]);

			this.Games = new List<IGame>();
			this.Score = new int[2] { 0, 0 };
			this.IsManualWin = false;
			if (_model.IsManualWin)
			{
				// If match has a manually set winner,
				// call the SetWinner method.
				// This prevents any Games from affecting the Score.
				PlayerSlot winnerSlot = (_model.WinnerID == _model.DefenderID)
					? PlayerSlot.Defender : PlayerSlot.Challenger;
				SetWinner(winnerSlot);
			}

			foreach (GameModel gameModel in _model.Games.OrderBy(m => m.GameNumber))
			{
				// Adding Games with the AddGame method handles things for us.
				// Data like score and finish status will auto update.
				AddGame(gameModel);
			}

			this.GroupNumber = _model.GroupNumber.GetValueOrDefault(-1);
			this.RoundIndex = _model.RoundIndex.GetValueOrDefault(-1);
			this.MatchIndex = _model.MatchIndex.GetValueOrDefault(-1);
			this.MatchNumber = _model.MatchNumber;
			this.NextMatchNumber = _model.NextMatchNumber.GetValueOrDefault(-1);
			this.NextLoserMatchNumber = _model.NextLoserMatchNumber.GetValueOrDefault(-1);

			this.PreviousMatchNumbers = new int[2];
			PreviousMatchNumbers[(int)PlayerSlot.Defender] =
				_model.PrevDefenderMatchNumber.GetValueOrDefault(-1);
			PreviousMatchNumbers[(int)PlayerSlot.Challenger] =
				_model.PrevChallengerMatchNumber.GetValueOrDefault(-1);
		}

		#region Player Methods
		/// <summary>
		/// Adds a Player to the Match, in the specified slot.
		/// If no PlayerSlot is given, the Player is added to the first empty slot.
		/// If the specified slot is full or invalid, an exception is thrown.
		/// </summary>
		/// <param name="_player">Player to add</param>
		/// <param name="_slot">Slot to fill</param>
		public void AddPlayer(IPlayer _player, PlayerSlot _slot = PlayerSlot.unspecified)
		{
			if (null == _player)
			{
				throw new ArgumentNullException("_player");
			}
			if (_slot != PlayerSlot.unspecified &&
				_slot != PlayerSlot.Defender &&
				_slot != PlayerSlot.Challenger)
			{
				throw new InvalidSlotException
					("PlayerSlot must be -1, 0, or 1!");
			}
			if (Players[0]?.Id == _player.Id ||
				Players[1]?.Id == _player.Id)
			{
				throw new DuplicateObjectException
					("Match already contains this Player!");
			}

			for (int i = 0; i < 2; ++i)
			{
				if ((int)_slot == i || _slot == PlayerSlot.unspecified)
				{
					if (null == Players[i])
					{
						// The given slot is empty.
						// Add the Player:
						Players[i] = _player;
						// Check Match readiness (two players present):
						IsReady = (null != Players[0] && null != Players[1]);
						// Return successfully
						return;
					}
				}
			}

			throw new SlotFullException
				("Match cannot add Player; there is already a Player in this Slot!");
		}

		/// <summary>
		/// Removes a Player from this Match and replaces with a new Player.
		/// If the Player-to-replace is not present, an exception is thrown.
		/// </summary>
		/// <param name="_newPlayer">Player to add</param>
		/// <param name="_oldPlayerId">ID of Player to remove</param>
		public void ReplacePlayer(IPlayer _newPlayer, int _oldPlayerId)
		{
			if (null == _newPlayer)
			{
				throw new ArgumentNullException("_newPlayer");
			}
			
			// Check (and replace if found) Defender slot:
			if (Players[(int)PlayerSlot.Defender]?.Id == _oldPlayerId)
			{
				Players[(int)PlayerSlot.Defender] = _newPlayer;
				foreach (IGame game in Games)
				{
					// Replace removed player in all Games for this Match:
					game.PlayerIDs[(int)PlayerSlot.Defender] = _newPlayer.Id;
				}
			}
			// else, Check (and replace if found) Challenger slot:
			else if (Players[(int)PlayerSlot.Challenger]?.Id == _oldPlayerId)
			{
				Players[(int)PlayerSlot.Challenger] = _newPlayer;
				foreach (IGame game in Games)
				{
					// Replace removed player in all Games for this Match:
					game.PlayerIDs[(int)PlayerSlot.Challenger] = _newPlayer.Id;
				}
			}
			else
			{
				throw new PlayerNotFoundException
					("Player not found in this Match!");
			}
		}

		/// <summary>
		/// Removes a Player from this Match. Sets playerslot to null.
		/// This also resets the Match's state. (score, games, etc.)
		/// If the Player is not present, an exception is thrown.
		/// </summary>
		/// <param name="_playerId">ID of Player to remove</param>
		public void RemovePlayer(int _playerId)
		{
			// Check both slots:
			for (int i = 0; i < 2; ++i)
			{
				if (Players[i]?.Id == _playerId)
				{
					// Player found. Set slot to empty:
					Players[i] = null;

					// Reset the match (score, games, readiness, etc.):
					ResetScore();
					IsReady = false;
					return;
				}
			}

			throw new PlayerNotFoundException
				("Player not found in this Match!");
		}

		/// <summary>
		/// Removes both Players.
		/// This also resets the Match's state. (score, games, etc.)
		/// </summary>
		public void ResetPlayers()
		{
			if (null == Players)
			{
				Players = new IPlayer[2];
			}
			// Clear both playerslots:
			Players[0] = Players[1] = null;

			// Reset the Match (score, readiness, etc.):
			ResetScore();
			IsReady = false;
		}
		#endregion

		#region Game & Score Methods
		/// <summary>
		/// Adds a Game to this Match. (adds to Games list)
		/// Also updates score and finished status.
		/// If the Match is finished or not ready, an exception is thrown.
		/// If the Game has invalid data (such as negative score), an exception is thrown.
		/// </summary>
		/// <param name="_defenderScore">Playerslot 0's score</param>
		/// <param name="_challengerScore">Playerslot 1's score</param>
		/// <param name="_winnerSlot">Game winner</param>
		/// <returns>Model of the added Game</returns>
		public GameModel AddGame(int _defenderScore, int _challengerScore, PlayerSlot _winnerSlot)
		{
			// Find the lowest (positive) Game Number we can add.
			// Check existing games, and get the first unassigned number:
			List<int> gameNumbers = Games.Select(g => g.GameNumber).ToList();
			int gameNum = 1;
			while (gameNumbers.Contains(gameNum))
			{
				++gameNum;
			}

			// Create a new Game with the game number and this Match's ID:
			IGame game = new Game(this.Id, gameNum);
			// Add Game's data (players and score):
			for (int i = 0; i < 2; ++i)
			{
				game.PlayerIDs[i] = this.Players[i]?.Id ?? -1;
			}
			game.Score[(int)PlayerSlot.Defender] = _defenderScore;
			game.Score[(int)PlayerSlot.Challenger] = _challengerScore;
			game.WinnerSlot = _winnerSlot;

			// Use the private helper method to do the actual work.
			// This method will add the game and update the Match data:
			return AddGame(game);
		}

		/// <summary>
		/// Adds a Game to this Match. (adds to Games list)
		/// Also updates score and finished status.
		/// If the Match is finished or not ready, an exception is thrown.
		/// If the Game has invalid data (such as negative score), an exception is thrown.
		/// </summary>
		/// <param name="_gameModel">Model of Game to add</param>
		/// <returns>Model of the added Game</returns>
		public GameModel AddGame(GameModel _gameModel)
		{
			if (null == _gameModel)
			{
				throw new ArgumentNullException("_gameModel");
			}

			// Use the private helper method to do the actual work.
			// This method will add the game and update the Match data:
			return AddGame(new Game(_gameModel));
		}
#if false
		public GameModel UpdateGame(int _gameNumber, int _defenderScore, int _challengerScore, PlayerSlot _winnerSlot)
		{
			if (PlayerSlot.unspecified == _winnerSlot)
			{
				throw new NotImplementedException
					("No ties allowed / enter a winner slot!");
			}
			if (_defenderScore < 0 || _challengerScore < 0)
			{
				throw new ScoreException
					("Score cannot be negative!");
			}

			int gameIndex = Games.FindIndex(g => g.GameNumber == _gameNumber);
			if (gameIndex < 0)
			{
				throw new GameNotFoundException
					("Game not found; Game Number may be invalid!");
			}

			// Subtract the Game's current winner:
			SubtractWin(Games[gameIndex].WinnerSlot);
			// Update the Game's data:
			Games[gameIndex].Score[(int)PlayerSlot.Defender] = _defenderScore;
			Games[gameIndex].Score[(int)PlayerSlot.Challenger] = _challengerScore;
			Games[gameIndex].WinnerSlot = _winnerSlot;
			// Add the Game's new winner:
			if (!IsFinished)
			{
				if (PlayerSlot.Defender == _winnerSlot ||
					PlayerSlot.Challenger == _winnerSlot)
				{
					AddWin(_winnerSlot);
				}
			}

			return GetGameModel(Games[gameIndex]);
		}
#endif
		/// <summary>
		/// Removes the last Game of this Match.
		/// Also updates the Match's score and status.
		/// If there are no Games, an exception is thrown.
		/// </summary>
		/// <returns>Model of removed Game</returns>
		public GameModel RemoveLastGame()
		{
			int index = Games.Count - 1;
			if (index < 0)
			{
				throw new GameNotFoundException
					("No Games to remove!");
			}

			// Refer to this method's logic, which handles updating the Match state:
			return (RemoveGameNumber(Games[index].GameNumber));
		}

		/// <summary>
		/// Removes a specific Game from this Match.
		/// Also updates the Match's score and status.
		/// If the Game is not found, an exception is thrown.
		/// </summary>
		/// <param name="_gameNumber">Number of Game to remove</param>
		/// <returns>Model of removed Game</returns>
		public GameModel RemoveGameNumber(int _gameNumber)
		{
			// Search Games for one that matches the given GameNumber:
			for (int index = 0; index < Games.Count; ++index)
			{
				if (Games[index].GameNumber == _gameNumber)
				{
					// Save a Model of this Game (to return later):
					GameModel removedGame = GetGameModel(Games[index]);

					// If this match has a manual winner, we remove the game without updating score.
					if (!IsManualWin)
					{
						// Calling SubtractWin() will update the Match's score & status:
						SubtractWin(Games[index].WinnerSlot);
					}

					// Now remove the Game and return:
					Games.RemoveAt(index);
					return removedGame;
				}
			}

			throw new GameNotFoundException
				("Game not found; Game Number may be invalid!");
		}

		/// <summary>
		/// Manually sets a winner for this Match, regardless of any Games played.
		/// This can be used to signify a forfeit or win-by-default.
		/// If the Match is already finished or not ready, an exception is thrown.
		/// If an invalid slot is given, an exception is thrown.
		/// </summary>
		/// <param name="_winnerSlot">Slot of winning Player</param>
		public void SetWinner(PlayerSlot _winnerSlot)
		{
			if (!IsReady)
			{
				throw new InactiveMatchException
					("Cannot set a winner for an inactive match!");
			}
			if (IsFinished || Games.Count > 0)
			{
				throw new InactiveMatchException
					("Can't set winner! Reset match first.");
			}

			// Update the score: value of -1 signifies an auto-win.
			// In the Bracket itself, this value is translated to a win,
			// but needs to be distinct for display purposes.
			if (PlayerSlot.Defender == _winnerSlot)
			{
				Score[(int)PlayerSlot.Defender] = -1;
				Score[(int)PlayerSlot.Challenger] = 0;
			}
			else if (PlayerSlot.Challenger == _winnerSlot)
			{
				Score[(int)PlayerSlot.Defender] = 0;
				Score[(int)PlayerSlot.Challenger] = -1;
			}
			else
			{
				throw new InvalidSlotException
					("Winner must be Defender or Challenger!");
			}

			WinnerSlot = _winnerSlot;
			IsFinished = true;
			IsManualWin = true;
		}

		/// <summary>
		/// Resets this Match's score and finished status.
		/// Manually set winners are also reversed.
		/// All Games are removed.
		/// </summary>
		/// <returns>Models (list) of all removed Games</returns>
		public List<GameModel> ResetScore()
		{
			if (null == Score)
			{
				Score = new int[2];
			}

			IsFinished = IsManualWin = false;
			Score[0] = Score[1] = 0;
			WinnerSlot = PlayerSlot.unspecified;

			List<GameModel> modelList = new List<GameModel>();
			foreach (IGame game in Games)
			{
				modelList.Add(GetGameModel(game));
			}
			Games.Clear();
			return modelList;
		}
		#endregion

		#region Mutators
		/// <summary>
		/// Sets maximum amount of Games that can be played for this Match.
		/// Ex: A "best of 3" has 3 max games.
		/// If the given value is invalid (negative) or the Match is already finished,
		/// an exception is thrown.
		/// </summary>
		/// <param name="_numberOfGames">Max Games</param>
		public void SetMaxGames(int _numberOfGames)
		{
			if (IsFinished)
			{
				throw new InactiveMatchException
					("Match is finished; cannot change victory conditions.");
			}
			if (_numberOfGames < 1)
			{
				throw new ScoreException
					("Total games cannot be less than 1!");
			}

			MaxGames = _numberOfGames;
		}

		/// <summary>
		/// Sets the group number for this Match.
		/// This is only needed for Group Stage-type brackets.
		/// If this has already been set, an exception is thrown.
		/// If an invalid (negative) index is given, an exception is thrown.
		/// </summary>
		/// <param name="_groupNumber">Group number</param>
		public void SetGroupNumber(int _groupNumber)
		{
			if (GroupNumber > 0)
			{
				throw new AlreadyAssignedException
					("Group Number is already set!");
			}
			if (_groupNumber < 1)
			{
				throw new InvalidIndexException
					("Group Number cannot be less than 1!");
			}

			GroupNumber = _groupNumber;
		}

		/// <summary>
		/// Sets the round number for this Match.
		/// If this has already been set, an exception is thrown.
		/// If an invalid (negative) index is given, an exception is thrown.
		/// </summary>
		/// <param name="_index">Round number</param>
		public void SetRoundIndex(int _index)
		{
			if (RoundIndex > -1)
			{
				throw new AlreadyAssignedException
					("Round Index is already set!");
			}
			if (_index < 1)
			{
				throw new InvalidIndexException
					("Round Index cannot be less than 1!");
			}

			RoundIndex = _index;
		}

		/// <summary>
		/// Sets the match index (within the round) for this Match.
		/// If this has already been set, an exception is thrown.
		/// If an invalid (negative) index is given, an exception is thrown.
		/// </summary>
		/// <param name="_index">New match index</param>
		public void SetMatchIndex(int _index)
		{
			if (MatchIndex > -1)
			{
				throw new AlreadyAssignedException
					("Match Index is already set!");
			}
			if (_index < 1)
			{
				throw new InvalidIndexException
					("Match Index cannot be less than 1!");
			}

			MatchIndex = _index;
		}

		/// <summary>
		/// Sets the match number (within the bracket) for this Match.
		/// If this has already been set, an exception is thrown.
		/// If an invalid (negative) number is given, an exception is thrown.
		/// </summary>
		/// <param name="_number">New match number</param>
		public void SetMatchNumber(int _number)
		{
			if (MatchNumber > -1)
			{
				throw new AlreadyAssignedException
					("Match Number is already set!");
			}
			if (_number < 1)
			{
				throw new InvalidIndexException
					("Match Number cannot be less than 1!");
			}

			MatchNumber = _number;
		}

		/// <summary>
		/// Adds a reference (MatchNumber) to a previous Match: Where this Match's Players came from.
		/// Each Match can hold up to two of these previous match numbers.
		/// If two match numbers have already been added, an exception is thrown.
		/// If an invalid (negative) match number is given, an exception is thrown.
		/// </summary>
		/// <param name="_number">Number of previous match</param>
		/// <param name="_slot">Which playerslot is coming from that Match</param>
		public void AddPreviousMatchNumber(int _number, PlayerSlot _slot = PlayerSlot.unspecified)
		{
			if (_number < 1)
			{
				throw new InvalidIndexException
					("Match Number cannot be less than 1!");
			}

			// Previous Match Numbers are stored in a 2-length array.
			// Make sure the correct slot (given as _slot) is not already assigned before doing so:
			if ((PlayerSlot.unspecified == _slot || PlayerSlot.Defender == _slot)
				&& PreviousMatchNumbers[(int)PlayerSlot.Defender] < 0)
			{
				PreviousMatchNumbers[(int)PlayerSlot.Defender] = _number;
			}
			else if ((PlayerSlot.unspecified == _slot || PlayerSlot.Challenger == _slot)
				&& PreviousMatchNumbers[(int)PlayerSlot.Challenger] < 0)
			{
				PreviousMatchNumbers[(int)PlayerSlot.Challenger] = _number;
			}
			else
			{
				throw new AlreadyAssignedException
					("Previous Match Numbers are already set!");
			}
		}

		/// <summary>
		/// Set a reference (MatchNumber) to the Match this WINNER will advance to.
		/// If this is already set, an exception is thrown.
		/// If an invalid (negative) match number is given, an exception is thrown.
		/// </summary>
		/// <param name="_number">Number of next Match</param>
		public void SetNextMatchNumber(int _number)
		{
			if (NextMatchNumber > -1)
			{
				throw new AlreadyAssignedException
					("Next Match Number is already set!");
			}
			if (_number < 1)
			{
				throw new InvalidIndexException
					("Match Number cannot be less than 1!");
			}

			NextMatchNumber = _number;
		}

		/// <summary>
		/// Set a reference (MatchNumber) to the Match this LOSER will advance to.
		/// This only applies to double-elimination-style brackets.
		/// If this is already set, an exception is thrown.
		/// If an invalid (negative) match number is given, an exception is thrown.
		/// </summary>
		/// <param name="_number">Number of next Match</param>
		public void SetNextLoserMatchNumber(int _number)
		{
			if (NextLoserMatchNumber > -1)
			{
				throw new AlreadyAssignedException
					("Next Loser Match Number is already set!");
			}
			if (_number < 1)
			{
				throw new InvalidIndexException
					("Match Number cannot be less than 1!");
			}

			NextLoserMatchNumber = _number;
		}
		#endregion
		#endregion

		#region Private Methods
		/// <summary>
		/// Creates a GameModel of the given Game.
		/// This Model includes the appropriate Match ID.
		/// </summary>
		/// <param name="_game">Game-type object</param>
		/// <returns>Model of given Game</returns>
		private GameModel GetGameModel(IGame _game)
		{
			if (null == _game)
			{
				throw new ArgumentNullException("_game");
			}

			GameModel gameModel = _game.GetModel();
			gameModel.MatchID = this.Id;
			return gameModel;
		}

		/// <summary>
		/// Helper method, handles logic for adding a Game to this Match.
		/// Adds the Game to the list, updates score and finished status.
		/// Also sorts the Games list by GameNumber.
		/// If the Match is finished or not ready, an exception is thrown.
		/// If the new GameNumber is already present, an exception is thrown.
		/// If the Game has invalid (negative) score, an exception is thrown.
		/// </summary>
		/// <param name="_game">Game to add</param>
		/// <returns>Model of added Game</returns>
		private GameModel AddGame(IGame _game)
		{
			if (!IsReady)
			{
				throw new InactiveMatchException
					("Cannot add games to an inactive match!");
			}
			if (IsFinished && !IsManualWin)
			{
				throw new InactiveMatchException
					("Cannot add games to a finished match!");
			}
			if (PlayerSlot.unspecified == _game.WinnerSlot)
			{
				throw new NotImplementedException
					("No tie games allowed / enter a winner slot!");
			}
			if (_game.Score[0] < 0 || _game.Score[1] < 0)
			{
				throw new ScoreException
					("Score cannot be negative!");
			}
			if (Games.Any(g => g.GameNumber == _game.GameNumber))
			{
				throw new DuplicateObjectException
					("Match already contains duplicate game data!");
			}

			// If the Match is a "Manual Win," Games can still be added,
			// though the score will NOT be updated.
			if (!IsFinished)
			{
				if (PlayerSlot.Defender == _game.WinnerSlot ||
					PlayerSlot.Challenger == _game.WinnerSlot)
				{
					// The AddWin method handles the logic of updating the Match.
					// Call this to update the score and match status:
					AddWin(_game.WinnerSlot);
				}
			}

			// Add the new Game to the list and sort it, then return a Model:
			Games.Add(_game);
			Games.Sort((first, second) => first.GameNumber.CompareTo(second.GameNumber));
			return GetGameModel(_game);
		}

		/// <summary>
		/// Adds a "win" to this Match.
		/// This updates the score and finished status.
		/// If the Match is already finished or not ready, an exception is thrown.
		/// If the winning slot is inalid, an exception is thrown.
		/// </summary>
		/// <param name="_slot">Playerslot to add a win to</param>
		private void AddWin(PlayerSlot _slot)
		{
			if (IsFinished)
			{
				throw new InactiveMatchException
					("Match is finished; can't add more wins!");
			}
			if (!IsReady)
			{
				throw new InactiveMatchException
					("Match is not begun; can't add a win!");
			}
			if (PlayerSlot.unspecified == _slot)
			{
				// "Adding" a tie: do nothing to Score
				return;
			}
			if (_slot != PlayerSlot.Defender &&
				_slot != PlayerSlot.Challenger)
			{
				throw new InvalidSlotException
					("PlayerSlot must be 0 or 1!");
			}

			Score[(int)_slot] += 1;

			// Use MaxGames to calculate how many wins are needed to finish the Match:
			int winsNeeded = MaxGames / 2 + 1;
			if (Score[(int)_slot] >= winsNeeded)
			{
				// The new winner has enough game-wins to win the match
				WinnerSlot = _slot;
				IsFinished = true;
			}
			else if (Score[0] + Score[1] >= MaxGames)
			{
				// MaxGames has been played without a winner: Match is over (TIE)
				WinnerSlot = PlayerSlot.unspecified;
				IsFinished = true;
			}
		}

		/// <summary>
		/// Subtracts a "win" from this Match.
		/// This updates the score and finished status.
		/// If the Match is not yet ready, an exception is thrown.
		/// If this subtraction would lower score under 0, an exception is thrown.
		/// </summary>
		/// <param name="_slot">Playerslot to subtract a win from</param>
		private void SubtractWin(PlayerSlot _slot)
		{
			if (!IsReady)
			{
				throw new InactiveMatchException
					("Match is not begun; can't subtract wins!");
			}
			if (PlayerSlot.unspecified == _slot)
			{
				// "Removing" a tie: do nothing to Score
				return;
			}
			if (_slot != PlayerSlot.Defender &&
				_slot != PlayerSlot.Challenger)
			{
				throw new InvalidSlotException
					("PlayerSlot must be 0 or 1!");
			}
			if (Score[(int)_slot] <= 0)
			{
				throw new ScoreException
					("Score is already 0; can't subtract wins!");
			}

			// If the given slot is the Match winner, the Match will be reactivated.
			// Remove the winner and reset IsFinished:
			if (WinnerSlot == _slot)
			{
				IsFinished = false;
				WinnerSlot = PlayerSlot.unspecified;
			}

			// Now we can subtract the point:
			Score[(int)_slot] -= 1;
		}
		#endregion
	}
}
