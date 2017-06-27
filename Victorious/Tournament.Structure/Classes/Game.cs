using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatabaseLib;

namespace Tournament.Structure
{
	/// <summary>
	/// Each Match outcome is determined by one or more Game objects.
	/// Games are always associated with a containing Match,
	/// and contain information such as Player ID's and score.
	/// </summary>
	public class Game : IGame
	{
		#region Variables & Properties
		public int Id
		{ get; private set; }
		public int MatchId
		{ get; set; }
		public int GameNumber
		{ get; set; }
		public int[] PlayerIDs
		{ get; set; }
		public PlayerSlot WinnerSlot
		{ get; set; }
		public int[] Score
		{ get; set; }
		#endregion

		#region Ctors
		public Game(int _matchId, int _gameNumber, int[] _playerIDs, PlayerSlot _winnerSlot, int[] _score)
		{
			if (null == _playerIDs)
			{
				throw new ArgumentNullException("_playerIDs");
			}
			if (null == _score)
			{
				throw new ArgumentNullException("_score");
			}

			Id = -1;
			MatchId = _matchId;
			GameNumber = _gameNumber;
			WinnerSlot = _winnerSlot;

			// ID values default to -1, indicating "no player" (an error)
			PlayerIDs = new int[2] { -1, -1 };
			Score = new int[2] { 0, 0 };
			for (int i = 0; i < 2; ++i)
			{
				PlayerIDs[i] = _playerIDs[i];
				Score[i] = _score[i];
			}
		}
		public Game(int _matchId, int _gameNumber)
			: this(
				  _matchId, // associated Match ID
				  _gameNumber, // Game Number (should be incremental)
				  new int[2] { -1, -1 }, // Player ID's
				  PlayerSlot.unspecified, // Winner Slot
				  new int[2] // Score
				  )
		{ }
		public Game()
			: this(
				  -1, // Match ID
				  0 // Game Number
				  )
		{
			// The provided values for the default constructor (-1, 0) should NEVER be kept.
		}
		public Game(GameModel _model)
		{
			if (null == _model)
			{
				throw new ArgumentNullException("_model");
			}

			this.Id = _model.GameID;
			this.MatchId = _model.MatchID.GetValueOrDefault(-1);
			this.GameNumber = _model.GameNumber;

			this.PlayerIDs = new int[2] { _model.DefenderID, _model.ChallengerID };
			this.Score = new int[2] { _model.DefenderScore, _model.ChallengerScore };

			// Translate the Model's "WinnerID int" to our "WinnerSlot PlayerSlot":
			if (_model.WinnerID == PlayerIDs[(int)PlayerSlot.Defender])
			{
				this.WinnerSlot = PlayerSlot.Defender;
			}
			else if (_model.WinnerID == PlayerIDs[(int)PlayerSlot.Challenger])
			{
				this.WinnerSlot = PlayerSlot.Challenger;
			}
			else
			{
				this.WinnerSlot = PlayerSlot.unspecified;
			}
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Creates a Model of this Game's current state.
		/// </summary>
		/// <returns>Matching GameModel</returns>
		public GameModel GetModel()
		{
			GameModel model = new GameModel();
			model.GameID = this.Id;
			model.ChallengerID = this.PlayerIDs[(int)PlayerSlot.Challenger];
			model.DefenderID = this.PlayerIDs[(int)PlayerSlot.Defender];
			model.WinnerID = (PlayerSlot.unspecified == this.WinnerSlot)
				? -1 : this.PlayerIDs[(int)WinnerSlot];
			model.MatchID = this.MatchId;
			model.GameNumber = this.GameNumber;
			model.ChallengerScore = this.Score[(int)PlayerSlot.Challenger];
			model.DefenderScore = this.Score[(int)PlayerSlot.Defender];

			return model;
		}
		#endregion
	}
}
