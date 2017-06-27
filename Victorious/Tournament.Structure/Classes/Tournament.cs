using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatabaseLib;

namespace Tournament.Structure
{
	public class Tournament : ITournament
	{
		#region Variables & Properties
		public string Title
		{ get; set; }
		public List<IPlayer> Players
		{ get; private set; }
		public List<IBracket> Brackets
		{ get; private set; }
		public float PrizePool
		{ get; set; }
		#endregion

		#region Ctors
		public Tournament()
		{
			Title = "";
			Players = new List<IPlayer>();
			Brackets = new List<IBracket>();
			PrizePool = 0.0f;
		}
		public Tournament(TournamentModel _model)
		{
			if (null == _model)
			{
				throw new ArgumentNullException("_model");
			}

			this.Title = _model.Title;

			this.Players = new List<IPlayer>();
			foreach (TournamentUserModel userModel in _model.TournamentUsers)
			{
				Players.Add(new Player(userModel));
			}

			this.Brackets = new List<IBracket>();
			foreach (BracketModel bModel in _model.Brackets)
			{
				AddBracket(RestoreBracket(bModel));
			}

			this.PrizePool = (float)(_model.PrizePurse);
		}
		#endregion

		#region Public Methods
		#region Player Methods
		public int NumberOfPlayers()
		{
			if (null == Players)
			{
				Players = new List<IPlayer>();
			}
			return Players.Count;
		}

		/// <summary>
		/// Clears the existing playerlist,
		/// replacing it with the given list.
		/// </summary>
		/// <param name="_players">New list of Players</param>
		public void SetNewPlayerlist(List<IPlayer> _players)
		{
			if (null == _players)
			{
				throw new ArgumentNullException("_players");
			}
			if (null == Players)
			{
				Players = new List<IPlayer>();
			}

			Players = _players;
			foreach (IBracket bracket in Brackets)
			{
				bracket.ResetPlayers();
			}
		}

		/// <summary>
		/// Takes the ordered rankings from a *finished* Bracket,
		/// and adds those Players to a new Bracket.
		/// Moves the amount of players specified by the first bracket's data.
		/// If the first bracket is not finished, an exception is thrown.
		/// If the first bracket's "AdvancingPlayers" value is invalid, an exception is thrown.
		/// </summary>
		/// <param name="_firstBracket">Finished Bracket</param>
		/// <param name="_secondBracket">New Bracket</param>
		public void AdvancePlayersByRanking(IBracket _firstBracket, IBracket _secondBracket)
		{
			if (!(_firstBracket.IsFinished))
			{
				throw new BracketException
					("Can't retrieve seeds from an unfinished bracket!");
			}
			if (_firstBracket.AdvancingPlayers <= 0 ||
				_firstBracket.AdvancingPlayers > _firstBracket.NumberOfPlayers())
			{
				throw new BracketException
					("Invalid value for advancing player!");
			}

			// How many Players are we advancing?
			List<IPlayer> pList = new List<IPlayer>();
			pList.Capacity = _firstBracket.AdvancingPlayers;
			foreach (IPlayerScore pScore in _firstBracket.Rankings)
			{
				// For each (ordered) Rankings entry, add the relevant Player to a list:
				pList.Add(_firstBracket.Players
					.Find(p => p.Id == pScore.Id));

				if (pList.Count == _firstBracket.AdvancingPlayers)
				{
					break;
				}
			}

			// Apply the playerlist to the new Bracket.
			// This also clears the new Bracket's playerlist, if it isn't empty:
			_secondBracket.SetNewPlayerlist(pList);
		}

		/// <summary>
		/// Adds a Player to this Tournament.
		/// Note: this doesn't add the Player to any Brackets.
		/// If this Tournament already contains the Player, an exception is thrown.
		/// </summary>
		/// <param name="_player">Player to add</param>
		public void AddPlayer(IPlayer _player)
		{
			if (null == _player)
			{
				throw new NullReferenceException
					("New Player cannot be null!");
			}
			if (null == Players)
			{
				throw new NullReferenceException
					("Playerlist is null; this shouldn't happen...");
			}
			if (Players.Contains(_player))
			{
				throw new DuplicateObjectException
					("Tournament already contains this Player!");
			}

			Players.Add(_player);
		}

		/// <summary>
		/// Removes a Player, and replaces him with a given Player.
		/// This also replaces the Player in any applicable Brackets.
		/// If the given index is invalid, an exception is thrown.
		/// </summary>
		/// <param name="_player">Player to add</param>
		/// <param name="_index">Index (in playerlist) of Player to remove</param>
		public void ReplacePlayer(IPlayer _player, int _index)
		{
			if (null == _player)
			{
				throw new NullReferenceException
					("New Player cannot be null!");
			}
			if (_index < 0 || _index >= Players.Count)
			{
				throw new InvalidIndexException
					("Can't replace; Index is out of playerlist's bounds!");
			}

			if (null != Players[_index])
			{
				// Get the ID of the Player we're removing...
				int pId = Players[_index].Id;
				foreach (IBracket bracket in Brackets)
				{
					for (int i = 0; i < bracket.Players.Count; ++i)
					{
						// Find that Player in every Bracket,
						// and replace him with the new Player:
						if (bracket.Players[i].Id == pId)
						{
							// Calling this Bracket method will handle replacing the Player in the Bracket,
							// as well as in Matches and Games:
							bracket.ReplacePlayer(_player, i);
							break;
						}
					}
				}
			}

			// Now we can actually replace the Player for this Tournament:
			Players[_index] = _player;
		}

		/// <summary>
		/// Removes a Player from this Tournament.
		/// If the Player is not found, an exception is thrown.
		/// Also removes this Player from all contained Brackets!
		/// Note: any affected Brackets have their Matches cleared!
		/// </summary>
		/// <param name="_playerId">ID of Player to remove</param>
		public void RemovePlayer(int _playerId)
		{
			if (null == Players)
			{
				throw new NullReferenceException
					("Playerlist is null; this shouldn't happen...");
			}

			int pIndex = Players.FindIndex(p => p.Id == _playerId);
			if (pIndex < 0)
			{
				throw new PlayerNotFoundException
					("Player not found in this tournament!");
			}

			Players.RemoveAt(pIndex);
			List<IBracket> currBrackets = Brackets
				.Where(b => b.Players.Any(p => p.Id == _playerId))
				.ToList();
			foreach (IBracket bracket in currBrackets)
			{
				bracket.RemovePlayer(_playerId);
			}
		}

		/// <summary>
		/// Clears the playerlist.
		/// Also clears the playerlist for all Brackets.
		/// </summary>
		public void ResetPlayers()
		{
			if (null == Players)
			{
				Players = new List<IPlayer>();
			}

			Players.Clear();
			foreach (IBracket bracket in Brackets)
			{
				bracket.ResetPlayers();
			}
		}
		#endregion

		#region Bracket Methods
		/// <summary>
		/// Gets the count of Brackets.
		/// If Brackets is null, initializes a new list.
		/// </summary>
		/// <returns>Count</returns>
		public int NumberOfBrackets()
		{
			if (null == Brackets)
			{
				Brackets = new List<IBracket>();
			}
			return Brackets.Count;
		}

		/// <summary>
		/// Adds a Bracket object to the list of Brackets.
		/// If the list already contains this Bracket, an exception is thrown.
		/// </summary>
		/// <param name="_bracket">Bracket to add</param>
		public void AddBracket(IBracket _bracket)
		{
			if (null == _bracket)
			{
				throw new ArgumentNullException("_bracket");
			}
			if (null == Brackets)
			{
				throw new NullReferenceException
					("Bracket list is null; this shouldn't happen...");
			}
			if (Brackets.Contains(_bracket))
			{
				throw new DuplicateObjectException
					("Tournament already contains this Bracket!");
			}

			Brackets.Add(_bracket);
		}

		/// <summary>
		/// (Re)creates a Bracket object from the given BracketModel.
		/// Determines the BracketType from the Model's data,
		/// and calls the appropriate constructor.
		/// If the type is not identifiable, an exception is thrown.
		/// </summary>
		/// <param name="_model">Model of an existing Bracket</param>
		/// <returns>Recreated Bracket</returns>
		public IBracket RestoreBracket(BracketModel _model)
		{
			if (null == _model)
			{
				throw new ArgumentNullException("_model");
			}

			IBracket ret = null;
			switch (_model.BracketType.Type)
			{
				case (BracketType.SINGLE):
					ret = new SingleElimBracket(_model);
					break;
				case (BracketType.DOUBLE):
					ret = new DoubleElimBracket(_model);
					break;
				case (BracketType.STEP):
					ret = new StepladderBracket(_model);
					break;
				case (BracketType.ROUNDROBIN):
					ret = new RoundRobinBracket(_model);
					break;
				case (BracketType.SWISS):
					ret = new SwissBracket(_model);
					break;
				case (BracketType.RRGROUP):
					ret = new RoundRobinGroups(_model);
					break;
				case (BracketType.GSLGROUP):
					throw new NotImplementedException();
					//ret = new GSLGroups(_model);
					//break;
				default:
					throw new NotImplementedException();
			}
			if (0 == ret.NumberOfMatches)
			{
				// If the Model has no Matches,
				// use the data we have to generate them:
				ret.CreateBracket();
			}

			return ret;
		}

		/// <summary>
		/// Removes a Bracket from the bracketlist.
		/// If the given Bracket is not in the list, an exception is thrown.
		/// </summary>
		/// <param name="_bracket">Bracket to remove</param>
		public void RemoveBracket(IBracket _bracket)
		{
			if (null == _bracket)
			{
				throw new ArgumentNullException("_bracket");
			}
			if (null == Brackets)
			{
				throw new NullReferenceException
					("Bracket list is null; this shouldn't happen...");
			}
			if (false == Brackets.Remove(_bracket))
			{
				throw new BracketNotFoundException
					("Bracket not found in this tournament!");
			}
		}

		/// <summary>
		/// Clears the bracketlist.
		/// If the list is null, creates an empty list.
		/// NOTE: This can leave a lot of data hanging loose in the database!
		/// </summary>
		public void ResetBrackets()
		{
			if (null == Brackets)
			{
				Brackets = new List<IBracket>();
			}
			Brackets.Clear();
		}
		#endregion

		#region Bracket Creation Methods
		/// <summary>
		/// Creates a new Single-Elim Bracket, and adds it to the bracketlist.
		/// This is just a wrapper method that calls the bracket ctor
		/// and adds the resulting Bracket object to the list.
		/// </summary>
		/// <param name="_playerList">List of Players</param>
		/// <param name="_maxGamesPerMatch">Max games, applied to every Match</param>
		public void AddSingleElimBracket(List<IPlayer> _playerList, int _maxGamesPerMatch = 1)
		{
			Brackets.Add(new SingleElimBracket(_playerList, _maxGamesPerMatch));
		}
#if false
		public void AddSingleElimBracket(int _numPlayers)
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < _numPlayers; ++i)
			{
				pList.Add(new User());
			}
			AddSingleElimBracket(pList);
		}
#endif

		/// <summary>
		/// Creates a new Double-Elim Bracket, and adds it to the bracketlist.
		/// This is just a wrapper method that calls the bracket ctor
		/// and adds the resulting Bracket object to the list.
		/// </summary>
		/// <param name="_playerList">List of Players</param>
		/// <param name="_maxGamesPerMatch">Max games, applied to every Match</param>
		public void AddDoubleElimBracket(List<IPlayer> _playerList, int _maxGamesPerMatch = 1)
		{
			Brackets.Add(new DoubleElimBracket(_playerList, _maxGamesPerMatch));
		}
#if false
		public void AddDoubleElimBracket(int _numPlayers)
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < _numPlayers; ++i)
			{
				pList.Add(new User());
			}
			AddDoubleElimBracket(pList);
		}
#endif

		/// <summary>
		/// Creates a new Stepladder Bracket, and adds it to the bracketlist.
		/// This is just a wrapper method that calls the bracket ctor
		/// and adds the resulting Bracket object to the list.
		/// </summary>
		/// <param name="_playerList">List of Players</param>
		/// <param name="_maxGamesPerMatch">Max games, applied to every Match</param>
		public void AddStepladderBracket(List<IPlayer> _playerList, int _maxGamesPerMatch = 1)
		{
			Brackets.Add(new StepladderBracket(_playerList, _maxGamesPerMatch));
		}

		/// <summary>
		/// Creates a new Round Robin Bracket, and adds it to the bracketlist.
		/// This is just a wrapper method that calls the bracket ctor
		/// and adds the resulting Bracket object to the list.
		/// </summary>
		/// <param name="_playerList">List of Players</param>
		/// <param name="_maxGamesPerMatch">Max games, applied to every Match</param>
		/// <param name="_numRounds">Limit of rounds to generate (0 = full round robin)</param>
		public void AddRoundRobinBracket(List<IPlayer> _playerList, int _maxGamesPerMatch = 1, int _numRounds = 0)
		{
			Brackets.Add(new RoundRobinBracket(_playerList, _maxGamesPerMatch, _numRounds));
		}
#if false
		public void AddRoundRobinBracket(int _numPlayers, int _numRounds = 0)
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < _numPlayers; ++i)
			{
				pList.Add(new User());
			}
			AddRoundRobinBracket(pList, _numRounds);
		}
#endif

		/// <summary>
		/// Creates a new Swiss-style Bracket, and adds it to the bracketlist.
		/// This is just a wrapper method that calls the bracket ctor
		/// and adds the resulting Bracket object to the list.
		/// </summary>
		/// <param name="_playerList">List of Players</param>
		/// <param name="_pairingMethod">Method for matchup pairing: Slide, Fold, or Adjacent</param>
		/// <param name="_maxGamesPerMatch">Max games, applied to every Match</param>
		/// <param name="_numRounds">Limit of rounds to generate (0 = full Swiss)</param>
		public void AddSwissBracket(List<IPlayer> _playerList, PairingMethod _pairingMethod = PairingMethod.Slide, int _maxGamesPerMatch = 1, int _numRounds = 0)
		{
			Brackets.Add(new SwissBracket(_playerList, _pairingMethod, _maxGamesPerMatch, _numRounds));
		}

		/// <summary>
		/// Creates a new Round Robin "Group Stage," and adds it to the bracketlist.
		/// This is just a wrapper method that calls the bracket ctor
		/// and adds the resulting Bracket object to the list.
		/// </summary>
		/// <param name="_playerList">List of Players</param>
		/// <param name="_numGroups">How many groups to divide Players into</param>
		/// <param name="_maxGamesPerMatch">Max games, applied to every Match</param>
		/// <param name="_maxRounds">Limit of rounds to generate (0 = full round robin)</param>
		public void AddRRGroupStage(List<IPlayer> _playerList, int _numGroups = 2, int _maxGamesPerMatch = 1, int _maxRounds = 0)
		{
			Brackets.Add(new RoundRobinGroups(_playerList, _numGroups, _maxGamesPerMatch, _maxRounds));
		}
#if false
		public void AddRRGroupStageBracket(int _numPlayers, int _numGroups = 2)
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < _numPlayers; ++i)
			{
				pList.Add(new User());
			}
			AddGroupStageBracket(pList, _numGroups);
		}
#endif

		/// <summary>
		/// Creates a new GSL-style "Group Stage," and adds it to the bracketlist.
		/// This is just a wrapper method that calls the bracket ctor
		/// and adds the resulting Bracket object to the list.
		/// </summary>
		/// <param name="_playerList">List of Players</param>
		/// <param name="_numGroups">How many groups to divide Players into</param>
		/// <param name="_maxGamesPerMatch">Max games, applied to every Match</param>
		public void AddGSLGroupStage(List<IPlayer> _playerList, int _numGroups = 2, int _maxGamesPerMatch = 1)
		{
			throw new NotImplementedException();
			//Brackets.Add(new GSLGroups(_playerList, _numGroups, _maxGamesPerMatch));
		}
		#endregion
		#endregion
	}
}
