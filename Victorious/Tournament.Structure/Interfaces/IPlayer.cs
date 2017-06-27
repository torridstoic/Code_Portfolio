using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatabaseLib;

namespace Tournament.Structure
{
	/// <summary>
	/// Interface used to fill slots for Users or Teams.
	/// </summary>
	public interface IPlayer
	{
		#region Variables & Properties
		int Id { get; }
		string Name { get; set; }
		string Email { get; set; }
		#endregion

		#region Public Methods
		/// <summary>
		/// Creates an AccountModel of this Player.
		/// </summary>
		AccountModel GetAccountModel();
#if false
		/// <summary>
		/// Creates a TournamentUserModel of this Player.
		/// </summary>
		/// <param name="_tournamentId">ID of containing Tournament</param>
		TournamentUserModel GetTournamentUserModel(int _tournamentId);
#endif
		/// <summary>
		/// Creates a TournamentUsersBracketModel of this Player.
		/// This Model will also contain the Player's bracketID and seed.
		/// </summary>
		/// <param name="_bracketId">ID of containing Bracket</param>
		/// <param name="_seed">Player's seed-value within the Bracket</param>
		/// <param name="_tournamentId">ID of containing Tournament</param>
		TournamentUsersBracketModel GetTournamentUsersBracketModel(int _bracketId, int _seed, int _tournamentId);
		#endregion
	}
}
