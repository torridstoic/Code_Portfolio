using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatabaseLib;

namespace Tournament.Structure
{
	public interface ITournament
	{
		#region Variables & Properties
		string Title { get; set; }
		List<IPlayer> Players { get; }
		List<IBracket> Brackets { get; }
		float PrizePool { get; set; }
		#endregion

		#region Methods
		/// <summary>
		/// Get the number of Players in the tournament.
		/// </summary>
		/// <returns>Players.Count</returns>
		int NumberOfPlayers();

		/// <summary>
		/// Clear and set a new list of Players.
		/// </summary>
		/// <param name="_players">New list of Player-type objects</param>
		void SetNewPlayerlist(List<IPlayer> _players);

		/// <summary>
		/// Advance Players from one Bracket to another.
		/// Players will be seeded by their final Ranking.
		/// </summary>
		/// <param name="_firstBracket">Finished Bracket</param>
		/// <param name="_secondBracket">New Bracket</param>
		void AdvancePlayersByRanking(IBracket _firstBracket, IBracket _secondBracket);

		/// <summary>
		/// Add a Player to this Tournament.
		/// </summary>
		/// <param name="_player">Player-type object to add</param>
		void AddPlayer(IPlayer _player);

		/// <summary>
		/// Replace a player/slot in the playerlist.
		/// Also replaces old Player in any brackets.
		/// </summary>
		/// <param name="_player">Player-type object to add.</param>
		/// <param name="_index">Slot in list to replace.</param>
		void ReplacePlayer(IPlayer _player, int _index);

		/// <summary>
		/// Remove a Player from the tournament.
		/// Also removes from all brackets.
		/// </summary>
		/// <param name="_playerId">ID of Player to remove</param>
		void RemovePlayer(int _playerId);

		/// <summary>
		/// Clear the tournament's player list.
		/// Also clears all bracket playerlists.
		/// </summary>
		void ResetPlayers();

		/// <summary>
		/// Get the number of Brackets in the tournament.
		/// </summary>
		/// <returns>Number of Brackets</returns>
		int NumberOfBrackets();

		/// <summary>
		/// Add a Bracket.
		/// </summary>
		/// <param name="_bracket">Bracket-type object to add.</param>
		void AddBracket(IBracket _bracket);

		/// <summary>
		/// Re-creates the correct type of IBracket
		/// from the passed-in BracketModel.
		/// </summary>
		/// <param name="_model">BracketModel-type object</param>
		/// <returns>Correct type of Bracket</returns>
		IBracket RestoreBracket(BracketModel _model);

		/// <summary>
		/// Removes a Bracket from the tournament.
		/// </summary>
		/// <param name="_bracket">Bracket-type object to remove.</param>
		void RemoveBracket(IBracket _bracket);

		/// <summary>
		/// Clears the tournament's bracket list.
		/// </summary>
		void ResetBrackets();

		#region Bracket Creation Methods
		/// <summary>
		/// Adds a new Single Elimination bracket to the tournament.
		/// </summary>
		/// <param name="_playerList">List of Players for the Bracket</param>
		/// <param name="_maxGamesPerMatch">Length of each Match</param>
		void AddSingleElimBracket(List<IPlayer> _playerList, int _maxGamesPerMatch = 1);
#if false
		void AddSingleElimBracket(int _numPlayers);
#endif

		/// <summary>
		/// Adds a new Double Elimination bracket to the tournament.
		/// </summary>
		/// <param name="_playerList">List of Players for the Bracket.</param>
		/// <param name="_maxGamesPerMatch">Length of each Match</param>
		void AddDoubleElimBracket(List<IPlayer> _playerList, int _maxGamesPerMatch = 1);
#if false
		void AddDoubleElimBracket(int _numPlayers);
#endif

		/// <summary>
		/// Adds a new Stepladder bracket to the tournament.
		/// </summary>
		/// <param name="_playerList">List of Players for the Bracket</param>
		/// <param name="_maxGamesPerMatch">Length of each Match</param>
		void AddStepladderBracket(List<IPlayer> _playerList, int _maxGamesPerMatch = 1);

		/// <summary>
		/// Adds a new Round Robin stage to the tournament.
		/// </summary>
		/// <param name="_playerList">List of Players for the stage</param>
		/// <param name="_maxGamesPerMatch">Length of each Match</param>
		/// <param name="_numRounds">Limit of rounds for the stage
		/// 0 = no limit (default round robin)</param>
		void AddRoundRobinBracket(List<IPlayer> _playerList, int _maxGamesPerMatch = 1, int _numRounds = 0);
#if false
		void AddRoundRobinBracket(int _numPlayers, int _numRounds = 0);
#endif

		/// <summary>
		/// Adds a new Swiss bracket to the tournament.
		/// </summary>
		/// <param name="_playerList">List of Players for the stage</param>
		/// <param name="_pairingMethod">Method of pairing players within each round</param>
		/// <param name="_maxGamesPerMatch">Length of each Match</param>
		/// <param name="_numRounds">Limit of rounds for the stage
		/// 0 = no limit (default)</param>
		void AddSwissBracket(List<IPlayer> _playerList, PairingMethod _pairingMethod = PairingMethod.Slide, int _maxGamesPerMatch = 1, int _numRounds = 0);

		/// <summary>
		/// Adds a new Round Robin Group Stage to the tournament.
		/// </summary>
		/// <param name="_playerList">List of Players for the stage</param>
		/// <param name="_numGroups">Number of groups to divide players into</param>
		/// <param name="_maxGamesPerMatch">Length of each Match</param>
		/// <param name="_maxRounds">Limit of rounds for the stage
		/// 0 = no limit (default round robin)</param>
		void AddRRGroupStage(List<IPlayer> _playerList, int _numGroups = 2, int _maxGamesPerMatch = 1, int _maxRounds = 0);
#if false
		void AddRRGroupStageBracket(int _numPlayers, int _numGroups = 2);
#endif

		/// <summary>
		/// Adds a new GSL-style Group Stage to the tournament.
		/// </summary>
		/// <param name="_playerList">List of Players for the stage</param>
		/// <param name="_numGroups">Number of groups to divide players into</param>
		/// <param name="_maxGamesPerMatch">Length of each Match</param>
		void AddGSLGroupStage(List<IPlayer> _playerList, int _numGroups = 2, int _maxGamesPerMatch = 1);
		#endregion
		#endregion
	}
}
