using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatabaseLib;

namespace Tournament.Structure
{
	public interface IGame
	{
		#region Variables & Properties
		int Id { get; }

		/// <summary>
		/// ID of this game's Match.
		/// </summary>
		int MatchId { get; set; }

		/// <summary>
		/// Number of Game within this Match.
		/// First game = 1, increments from there.
		/// </summary>
		int GameNumber { get; set; }

		/// <summary>
		/// Length-2 array. Default values of -1.
		/// </summary>
		int[] PlayerIDs { get; set; }

		/// <summary>
		/// Slot of winner: Defender or Challenger.
		/// </summary>
		PlayerSlot WinnerSlot { get; set; }

		int[] Score { get; set; }
		#endregion

		#region Public Methods
		/// <summary>
		/// Get a Model of this Game.
		/// </summary>
		/// <returns>GameModel</returns>
		GameModel GetModel();
		#endregion
	}
}
