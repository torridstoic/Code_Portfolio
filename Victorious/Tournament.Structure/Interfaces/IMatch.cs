using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatabaseLib;

namespace Tournament.Structure
{
	public interface IMatch
	{
		#region Variables & Properties
		int Id { get; }

		/// <summary>
		/// Is the Match set/ready to play?
		/// </summary>
		bool IsReady { get; }

		/// <summary>
		/// Is the Match finished/won?
		/// </summary>
		bool IsFinished { get; }

		/// <summary>
		/// Is the Match winner manually set?
		/// </summary>
		bool IsManualWin { get; }

		/// <summary>
		/// Max number of games that MAY be played.
		/// (ex: BO3 = 3 max games)
		/// </summary>
		int MaxGames { get; }

		IPlayer[] Players { get; }

		/// <summary>
		/// Slot of winning player:
		/// Defender, Challenger, or unspecified.
		/// </summary>
		PlayerSlot WinnerSlot { get; }

		/// <summary>
		/// Ordered list of completed/recorded Games.
		/// </summary>
		List<IGame> Games { get; }

		/// <summary>
		/// 2-sized Array, indexes correspond to Players array.
		/// </summary>
		int[] Score { get; }

		/// <summary>
		/// For Group Stages; which group this Match is part of.
		/// (1-indexed)
		/// </summary>
		int GroupNumber { get; }

		/// <summary>
		/// 1-indexed
		/// </summary>
		int RoundIndex { get; }

		/// <summary>
		/// 1-indexed
		/// </summary>
		int MatchIndex { get; }

		/// <summary>
		/// First Match = 1
		/// </summary>
		int MatchNumber { get; }

		/// <summary>
		/// Match Numbers of Matches sending winners to this Match.
		/// 2-sized Array, default values are -1.
		/// Indexes correspond to Players array.
		/// </summary>
		int[] PreviousMatchNumbers { get; }

		/// <summary>
		/// Number of Match this winner is sent to.
		/// </summary>
		int NextMatchNumber { get; }

		/// <summary>
		/// Number of Match this loser is sent to.
		/// (-1 if not applicable)
		/// </summary>
		int NextLoserMatchNumber { get; }
		#endregion

		#region Methods
		/// <summary>
		/// Create a Model for this Match.
		/// </summary>
		/// <returns>MatchModel-type object</returns>
		MatchModel GetModel();

		[System.Obsolete("Set Max Games on (NumRounds+1) instead", false)]
		void SetMaxGames(int _numberOfGames);
		#endregion
	}
}
