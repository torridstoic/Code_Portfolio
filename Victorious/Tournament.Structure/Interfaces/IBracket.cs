using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatabaseLib;

namespace Tournament.Structure
{
	public interface IBracket
	{
		#region Variables & Properties
		int Id { get; }
		BracketType BracketType { get; }

		/// <summary>
		/// Is Bracket creation finalized; ready to play?
		/// </summary>
		bool IsFinalized { get; }

		/// <summary>
		/// Is Bracket play finished & winner determined?
		/// </summary>
		bool IsFinished { get; }

		/// <summary>
		/// Players list, ordered by seed.
		/// </summary>
		List<IPlayer> Players { get; }

		/// <summary>
		/// Ordered Rankings list, containing:
		/// Player ID, name, score, and rank.
		/// </summary>
		List<IPlayerScore> Rankings { get; }

		/// <summary>
		/// How many Players will advance from this Bracket.
		/// </summary>
		int AdvancingPlayers { get; }

		/// <summary>
		/// Limit on the number of rounds;
		/// for RoundRobin-type brackets.
		/// </summary>
		int MaxRounds { get; set; }

		/// <summary>
		/// Number of rounds in the upper bracket.
		/// (if "upper" is N/A, this is total rounds)
		/// </summary>
		int NumberOfRounds { get; }

		/// <summary>
		/// Returns 0 if there is no lower bracket.
		/// </summary>
		int NumberOfLowerRounds { get; }

		/// <summary>
		/// NULL if not applicable.
		/// </summary>
		IMatch GrandFinal { get; }

		/// <summary>
		/// TOTAL number of matches.
		/// Includes Lower Bracket & Grand Final, when available.
		/// </summary>
		int NumberOfMatches { get; }
		#endregion

		#region Events
		/// <summary>
		/// Fired on creation of a new round of Matches. (Swiss)
		/// Contains Models of Matches to add.
		/// </summary>
		event EventHandler<BracketEventArgs> RoundAdded;

		/// <summary>
		/// Fired on deletion of one or more rounds. (Swiss)
		/// Contains Models of Matches to delete.
		/// </summary>
		event EventHandler<BracketEventArgs> RoundDeleted;

		/// <summary>
		/// Modifications were made. Update any contained Matches and Games.
		/// </summary>
		event EventHandler<BracketEventArgs> MatchesModified;

		/// <summary>
		/// Games were deleted. Use these ID's to update the database.
		/// </summary>
		event EventHandler<BracketEventArgs> GamesDeleted;
		#endregion

		#region Methods
		/// <summary>
		/// Generates the bracket (NEW rounds & matches).
		/// </summary>
		/// <param name="_gamesPerMatch">Max games played each match</param>
		void CreateBracket(int _gamesPerMatch = 1);

		/// <summary>
		/// Checks if this bracket's fields are legal.
		/// This should be called before Finalizing.
		/// </summary>
		/// <returns>true if legal, false if any errors</returns>
		bool Validate();

		/// <summary>
		/// Resets EVERY Match to a pre-play state (no games played).
		/// </summary>
		void ResetMatches();

		#region Player Methods
		/// <summary>
		/// Gets the number of Players in the Bracket.
		/// </summary>
		/// <returns>Count of Players</returns>
		int NumberOfPlayers();

		/// <summary>
		/// Gets a Player's seed in this Bracket.
		/// </summary>
		/// <param name="_playerId">ID of specified Player</param>
		/// <returns>Seed of specified Player</returns>
		int GetPlayerSeed(int _playerId);

		/// <summary>
		/// Randomizes seed values of Player's in this Bracket.
		/// (Deletes all Matches)
		/// </summary>
		void RandomizeSeeds();

		/// <summary>
		/// Replace this bracket's Players (if any)
		/// with the passed-in list.
		/// (Deletes all Matches)
		/// </summary>
		/// <param name="_players">List of Player-type objects to store</param>
		void SetNewPlayerlist(List<IPlayer> _players);

		/// <summary>
		/// Replace this bracket's Players (if any)
		/// with the passed-in list.
		/// (Deletes all Matches)
		/// </summary>
		/// <param name="_players">Seeded list of Player Models</param>
		void SetNewPlayerlist(ICollection<TournamentUsersBracketModel> _players);

		/// <summary>
		/// Add a Player to the Bracket.
		/// (Deletes all Matches)
		/// </summary>
		/// <param name="_player">Player-type object to add</param>
		void AddPlayer(IPlayer _player);

		/// <summary>
		/// Replaces a player/slot in the playerlist
		/// with the new indicated Player.
		/// Also replaces Player in all Matches.
		/// </summary>
		/// <param name="_player">Player-type object to add</param>
		/// <param name="_index">Slot in list to replace (0-indexed)</param>
		void ReplacePlayer(IPlayer _player, int _index);

		/// <summary>
		/// Remove a Player from the bracket.
		/// (Deletes all Matches)
		/// </summary>
		/// <param name="_playerId">ID of Player to remove</param>
		void RemovePlayer(int _playerId);

		/// <summary>
		/// Swaps two Players' seeds/positions.
		/// (Deletes all Matches)
		/// </summary>
		/// <param name="_index1">P1's index (0-indexed)</param>
		/// <param name="_index2">P2's index (0-indexed)</param>
		void SwapPlayers(int _index1, int _index2);

		/// <summary>
		/// Moves a Player's seed/position in the playerlist,
		/// adjusting all other Players.
		/// (Deletes all Matches)
		/// </summary>
		/// <param name="_oldIndex">Player's current index (0-indexed)</param>
		/// <param name="_newIndex">New index (0-indexed)</param>
		void ReinsertPlayer(int _oldIndex, int _newIndex);

		/// <summary>
		/// Clears the bracket's player list.
		/// (Deletes all Matches)
		/// </summary>
		void ResetPlayers();
		#endregion

		#region Match & Game Methods
		/// <summary>
		/// Add/record a finished Game.
		/// </summary>
		/// <param name="_matchNumber">Match to contain this Game</param>
		/// <param name="_defenderScore">Score for Defender-slot Player</param>
		/// <param name="_challengerScore">Score for Challenger-slot Player</param>
		/// <param name="_winnerSlot">Slot of winner (Defender/Challenger)</param>
		/// <returns>Model of the new Game</returns>
		GameModel AddGame(int _matchNumber, int _defenderScore, int _challengerScore, PlayerSlot _winnerSlot);

		/// <summary>
		/// Replaces a Game with new data.
		/// </summary>
		/// <param name="_matchNumber">Match to contain this Game</param>
		/// <param name="_gameNumber">Game Number to replace</param>
		/// <param name="_defenderScore">Score for Defender-slot Player</param>
		/// <param name="_challengerScore">Score for Challenger-slot Player</param>
		/// <param name="_winnerSlot">Slot of winner (Defender/Challenger)</param>
		/// <returns>Model of the new Game</returns>
		GameModel UpdateGame(int _matchNumber, int _gameNumber, int _defenderScore, int _challengerScore, PlayerSlot _winnerSlot);

		/// <summary>
		/// Delete/un-record a Match's most recent Game.
		/// </summary>
		/// <param name="_matchNumber">Number of Match to modify</param>
		/// <returns>Model of removed Game</returns>
		GameModel RemoveLastGame(int _matchNumber);

		/// <summary>
		/// Delete/un-record a Game from within a Match.
		/// </summary>
		/// <param name="_matchNumber">Number of Match to modify</param>
		/// <param name="_gameNumber">Number of Game to delete</param>
		/// <param name="_updateInstead">true if updating the game, false if removing</param>
		/// <returns>Model of removed Game</returns>
		GameModel RemoveGameNumber(int _matchNumber, int _gameNumber, bool _updateInstead = false);

		/// <summary>
		/// Manually set a winner for specified Match.
		/// Winner's score will be -1.
		/// </summary>
		/// <param name="_matchNumber">Number of Match to affect</param>
		/// <param name="_winnerSlot">Slot of winner (Defender/Challenger)</param>
		void SetMatchWinner(int _matchNumber, PlayerSlot _winnerSlot);

		/// <summary>
		/// Reset score for the specified match.
		/// Resets any affected "future" matches.
		/// </summary>
		/// <param name="_matchNumber">Number of specified match</param>
		/// <returns>List of Models of removed Games</returns>
		List<GameModel> ResetMatchScore(int _matchNumber);

		/// <summary>
		/// Check a *finished* Bracket for tied Players.
		/// Only used for round robin-types.
		/// </summary>
		/// <returns>True if tie found, false otherwise</returns>
		bool CheckForTies();

		/// <summary>
		/// Add applicable tiebreaker Matches to this Bracket.
		/// </summary>
		/// <returns>True if Matches added, false otherwise</returns>
		bool GenerateTiebreakers();
		#endregion

		#region Accessors & Mutators
		/// <summary>
		/// Get a Model of current Bracket.
		/// </summary>
		/// <param name="_tournamentID">ID of this Bracket's Tournament</param>
		/// <returns>BracketModel</returns>
		BracketModel GetModel(int _tournamentID);

		/// <summary>
		/// Get all Matches in specified round.
		/// (_index=1 returns FIRST round)
		/// </summary>
		/// <param name="_round">Round number to get (1-indexed)</param>
		/// <returns>List of Matches in the round</returns>
		List<IMatch> GetRound(int _round);

		/// <summary>
		/// Get all Matches in specified lower bracket round.
		/// (_index=1 returns FIRST round)
		/// </summary>
		/// <param name="_round">Round number to get (1-indexed)</param>
		/// <returns>List of Matches in the round</returns>
		List<IMatch> GetLowerRound(int _round);

		/// <summary>
		/// Get a specific Match object from:
		/// Upper & Lower Brackets and Grand Final.
		/// </summary>
		/// <param name="_matchNumber">Match Number of the desired Match</param>
		/// <returns>Specified Match object</returns>
		IMatch GetMatch(int _matchNumber);

		/// <summary>
		/// Retrieve a Model of the specified Match.
		/// This will include the Match's BracketID.
		/// </summary>
		/// <param name="_matchNumber">Number of desired Match</param>
		/// <returns>Model of specified Match</returns>
		MatchModel GetMatchModel(int _matchNumber);

		/// <summary>
		/// Set the max number of Games PER MATCH for one round.
		/// </summary>
		/// <param name="_roundIndex">Round of Matches to modify</param>
		/// <param name="_maxGamesPerMatch">How many Games each Match may last</param>
		void SetMaxGamesForWholeRound(int _round, int _maxGamesPerMatch);

		/// <summary>
		/// Set the max number of Games PER MATCH for one lower round.
		/// </summary>
		/// <param name="_roundIndex">Round (lower bracket) of Matches to modify</param>
		/// <param name="_maxGamesPerMatch">How many Games each Match may last</param>
		void SetMaxGamesForWholeLowerRound(int _round, int _maxGamesPerMatch);
		#endregion
		#endregion
	}
}
