using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatabaseLib;

namespace Tournament.Structure
{
	public class GSLGroups : GroupStage
	{
		private class GSLBracket : DoubleElimBracket
		{
			#region Variables & Properties
			//public int Id
			//public BracketType BracketType
			//public bool IsFinalized
			//public bool IsFinished
			//public List<IPlayer> Players
			//public List<IPlayerScore> Rankings
			//public int MaxRounds = 0
			//protected Dictionary<int, Match> Matches
			//public int NumberOfRounds
			//protected Dictionary<int, Match> LowerMatches
			//public int NumberOfLowerRounds
			//protected Match grandFinal = null
			//public IMatch GrandFinal = null
			//public int NumberOfMatches
			//protected int MatchWinValue = 0
			//protected int MatchTieValue = 0
			//protected List<IBracket> Groups = empty
			//public int NumberOfGroups = 0
			#endregion

			#region Ctors
			public GSLBracket(List<IPlayer> _players, int _maxGamesPerMatch = 1)
				: base(_players, _maxGamesPerMatch)
			{
				//BracketType = BracketType.GSL;
			}
			public GSLBracket()
				: this(new List<IPlayer>())
			{ }
			public GSLBracket(BracketModel _model)
			{
				throw new NotImplementedException();
			}
			#endregion

			#region Public Methods
			public override void CreateBracket(int _gamesPerMatch = 1)
			{
				base.CreateBracket(_gamesPerMatch);
				grandFinal = null;
				--NumberOfMatches;
			}
			public override bool Validate()
			{
				if ((Players?.Count ?? 0) != 4 &&
					Players.Count != 8)
				{
					return false;
				}

				return true;
			}
			#endregion

			#region Private Methods
			protected override List<MatchModel> ApplyWinEffects(int _matchNumber, PlayerSlot _slot)
			{
				List<MatchModel> alteredMatches = new List<MatchModel>();

				int nextWinnerNumber;
				int nextLoserNumber;
				IMatch match = GetMatchData(_matchNumber, out nextWinnerNumber, out nextLoserNumber);

				if (match.NextMatchNumber <= NumberOfMatches)
				{
					// Case 1: Not a final/endpoint match. Treat like a DEB:
					alteredMatches.AddRange(base.ApplyWinEffects(_matchNumber, _slot));
				}
				else if (match.IsFinished)
				{
					if (nextLoserNumber > 0)
					{
						// Case 2: UB Finals.
						// Advance loser to lower bracket:
						PlayerSlot loserSlot = (PlayerSlot.Defender == match.WinnerSlot)
							? PlayerSlot.Challenger
							: PlayerSlot.Defender;
						GetInternalMatch(nextLoserNumber).AddPlayer
							(match.Players[(int)loserSlot], PlayerSlot.Defender);
						alteredMatches.Add(GetMatchModel(nextLoserNumber));
						// Check lower bracket completion:
						if (GetLowerRound(NumberOfLowerRounds)[0].IsFinished)
						{
							this.IsFinished = true;
						}
					}
					else
					{
						// Case 3: LB Finals.
						// Check upper bracket completion:
						if (GetRound(NumberOfRounds)[0].IsFinished)
						{
							this.IsFinished = true;
						}
					}
				}

				return alteredMatches;
			}
			// void ApplyGameRemovalEffects() just uses DEB's version.
			protected override void UpdateScore(int _matchNumber, List<GameModel> _games, bool _isAddition, MatchModel _oldMatch)
			{
				// The base method will add all match losers to the Rankings:
				base.UpdateScore(_matchNumber, _games, _isAddition, _oldMatch);

				// Now, check for special cases (winners to add):
				if (_isAddition &&
					_oldMatch.WinnerID.GetValueOrDefault(-1) < 0)
				{
					int nextWinnerNumber;
					int nextLoserNumber;
					IMatch match = GetMatchData(_matchNumber, out nextWinnerNumber, out nextLoserNumber);

					if (match.IsFinished &&
						nextWinnerNumber > NumberOfMatches)
					{
						if (nextLoserNumber < 0)
						{
							// Case 1: Lower bracket finals.
							// Add winner (rank 2):
							Rankings.Add(new PlayerScore
								(match.Players[(int)(match.WinnerSlot)].Id,
								match.Players[(int)(match.WinnerSlot)].Name,
								2));
						}
						else
						{
							// Case 2: Upper bracket finals.
							// Add winner (rank 1):
							Rankings.Add(new PlayerScore
								(match.Players[(int)(match.WinnerSlot)].Id,
								match.Players[(int)(match.WinnerSlot)].Name,
								1));
						}

						Rankings.Sort(SortRankingRanks);
					}
				}
			}

			protected override void RecalculateRankings()
			{
				// The base method will add all losers to rankings.
				// It will also add the LB winner as Rank 1...
				base.RecalculateRankings();

				if (Rankings.Count > 0)
				{
					if (1 == Rankings[0].Rank)
					{
						// The LB winner was erroneously added as Rank 1.
						// Fix it (change to 2):
						Rankings[0].Rank = 2;
					}

					IMatch upperFinal = GetRound(NumberOfRounds)[0];
					if (upperFinal.IsFinished)
					{
						// Add the UB winner as Rank 1:
						Rankings.Add(new PlayerScore
							(upperFinal.Players[(int)(upperFinal.WinnerSlot)].Id,
							upperFinal.Players[(int)(upperFinal.WinnerSlot)].Name,
							1));

						Rankings.Sort(SortRankingRanks);
					}
				}
			}

			protected override List<MatchModel> RemovePlayerFromFutureMatches(int _matchNumber, int _playerId)
			{
				if (_matchNumber > NumberOfMatches)
				{
					return new List<MatchModel>();
				}
				return base.RemovePlayerFromFutureMatches(_matchNumber, _playerId);
			}
			#endregion
		}

		#region Variables & Properties
		//public int Id
		//public BracketType BracketType
		//public bool IsFinalized
		//public bool IsFinished
		//public List<IPlayer> Players
		//public List<IPlayerScore> Rankings
		//public int MaxRounds
		//protected Dictionary<int, Match> Matches = empty
		//public int NumberOfRounds
		//protected Dictionary<int, Match> LowerMatches = empty
		//public int NumberOfLowerRounds = 0
		//protected Match grandFinal = null
		//public IMatch GrandFinal = null
		//public int NumberOfMatches
		//protected int MatchWinValue
		//protected int MatchTieValue
		//protected List<IBracket> Groups
		//public int NumberOfGroups
		#endregion

		#region Ctors
		public GSLGroups(List<IPlayer> _players, int _numberOfGroups, int _maxGamesPerMatch = 1)
		{
			if (null == _players)
			{
				throw new ArgumentNullException("_players");
			}

			Players = _players;
			Id = 0;
			BracketType = BracketType.GSLGROUP;
			NumberOfGroups = _numberOfGroups;

			CreateBracket(_maxGamesPerMatch);
		}
		public GSLGroups()
			: this(new List<IPlayer>(), 2)
		{ }
		public GSLGroups(BracketModel _model)
		{
			SetDataFromModel(_model);
		}
		#endregion

		#region Public Methods
		public override void CreateBracket(int _gamesPerMatch = 1)
		{
			ResetBracketData();

			for (int b = 0; b < NumberOfGroups; ++b)
			{
				List<IPlayer> pList = new List<IPlayer>();
				for (int p = 0; (p + b) < Players.Count; p += NumberOfGroups)
				{
					pList.Add(Players[p + b]);
				}

				//Groups.Add(new GSLBracket(pList, _gamesPerMatch));
			}
			//SubscribeToGroupEvents();

			//foreach (IBracket group in Groups)
			//{
			//	NumberOfMatches += group.NumberOfMatches;
			//	NumberOfRounds = Math.Max(this.NumberOfRounds, group.NumberOfRounds);
			//}
		}

		public override bool CheckForTies()
		{
			return false;
		}
		public override bool GenerateTiebreakers()
		{
			throw new NotImplementedException
				("Not applicable for knockout brackets!");
		}
		#endregion

		#region Private Methods
		protected override List<MatchModel> ApplyWinEffects(int _matchNumber, PlayerSlot _slot)
		{
			throw new NotImplementedException();
		}
		protected override List<MatchModel> ApplyGameRemovalEffects(int _matchNumber, List<GameModel> _games, PlayerSlot _formerMatchWinnerSlot)
		{
			throw new NotImplementedException();
		}
		protected override void UpdateScore(int _matchNumber, List<GameModel> _games, bool _isAddition, MatchModel _oldMatch)
		{
			throw new NotImplementedException();
		}

		protected override void RecalculateRankings()
		{
			throw new NotImplementedException();
		}
		protected override void UpdateRankings()
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
