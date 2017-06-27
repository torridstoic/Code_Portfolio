// #define ENABLE_TIEBREAKERS

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Moq;
using System.Linq;

using DatabaseLib;

namespace Tournament.Structure.Tests
{
	[TestClass]
	public class RoundRobinBracketTests
	{
		#region Bracket Creation
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB Ctor")]
		public void RRBCtor_Constructs()
		{
			IBracket b = new RoundRobinBracket();

			Assert.IsInstanceOfType(b, typeof(RoundRobinBracket));
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB Ctor")]
		public void RRBCtor_CreatesNoMatches_WithLessThanTwoPlayers()
		{
			IBracket b = new RoundRobinBracket();

			Assert.AreEqual(0, b.NumberOfMatches);
		}

		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB CreateBracket")]
		public void RRBCreateBracket_CreatesFor4Players()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			Assert.AreEqual(3 * 2, b.NumberOfMatches);
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB CreateBracket")]
		public void RRBCreateBracket_AssignsCorrectMatchesPerPlayer()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			int numMatchesForPlayerOne = 0;
			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				IMatch m = b.GetMatch(n);
				if (b.Players[0] == m.Players[(int)PlayerSlot.Defender]
					|| b.Players[0] == m.Players[(int)PlayerSlot.Challenger])
				{
					++numMatchesForPlayerOne;
				}
			}

			Assert.AreEqual(3, numMatchesForPlayerOne);
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB CreateBracket")]
		public void RRBCreateBracket_CreatesFiveRounds_ForFivePlayers()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 5; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			Assert.AreEqual(5, b.NumberOfRounds);
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB CreateBracket")]
		public void RRBCreateBracket_CreatesTenMatches_ForFivePlayers()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 5; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			Assert.AreEqual(5 * 2, b.NumberOfMatches);
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB CreateBracket")]
		public void RRBCreateBracket_OverloadedCtorLimitsRoundCount()
		{
			int maxRounds = 4;
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 16; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList, 1, maxRounds);

			Assert.AreEqual(maxRounds, b.NumberOfRounds);
		}
		#endregion

		#region Bracket Progression
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB AddGame")]
		[TestCategory("Rankings")]
		public void RRBAddGame_UpdatesRankings()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			b.AddGame(1, 0, 1, PlayerSlot.Challenger);
			Assert.AreEqual(b.GetMatch(1).Players[(int)(b.GetMatch(1).WinnerSlot)].Id,
				b.Rankings[0].Id);
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB AddGame")]
		[ExpectedException(typeof(InvalidIndexException))]
		public void RRBAddGame_ThrowsInvalidIndex_WithBadMatchNumber()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			b.AddGame(-1, 1, 0, PlayerSlot.Defender);
			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB AddGame")]
		[ExpectedException(typeof(MatchNotFoundException))]
		public void RRBAddGame_ThrowsNotFound_WithBadMatchNumber()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			b.AddGame(b.NumberOfMatches + 1, 0, 1, PlayerSlot.Challenger);
			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB UpdateGame")]
		[TestCategory("Rankings")]
		public void RRBUpdateGame_UpdatesPointsScore_InRankings()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);
			b.AddGame(1, 15, 10, PlayerSlot.Defender);

			int oldPoints = b.Rankings[0].PointsScore;
			b.UpdateGame(1, 1, 25, 10, PlayerSlot.Defender);
			Assert.AreNotEqual(oldPoints, b.Rankings[0].PointsScore);
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB UpdateGame")]
		[TestCategory("Rankings")]
		public void RRBUpdateGame_NewMatchWinnerUpdatesInRankings()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);
			b.AddGame(1, 15, 10, PlayerSlot.Defender);

			int oldRankingsLeader = b.Rankings[0].Id;
			b.UpdateGame(1, 1, 10, 25, PlayerSlot.Challenger);
			Assert.AreNotEqual(oldRankingsLeader, b.Rankings[0].Id);
		}

		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("SetMatchWinner")]
		public void RRBSetMatchWinner_FinishesMatch()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 9; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList, 5);

			b.SetMatchWinner(1, PlayerSlot.Challenger);
			Assert.IsTrue(b.GetMatch(1).IsFinished);
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("SetMatchWinner")]
		public void RRBSetMatchWinner_AddsWinValueToRankings()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 9; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList, 5);

			b.SetMatchWinner(1, PlayerSlot.Challenger);
			int rIndex = b.Rankings.FindIndex(r => r.Id == b.GetMatch(1).Players[(int)PlayerSlot.Challenger].Id);
			Assert.AreEqual(1, b.Rankings[rIndex].Wins);
		}

		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB RemoveLastGame")]
		[ExpectedException(typeof(InvalidIndexException))]
		public void RRBRemoveLastGame_ThrowsInvalidIndex_WithBadMatchNumber()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			b.RemoveLastGame(-1);
			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB RemoveLastGame")]
		[ExpectedException(typeof(MatchNotFoundException))]
		public void RRBRemoveLastGame_ThrowsNotFound_WithBadMatchNumber()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			b.RemoveLastGame(b.NumberOfMatches + 1);
			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB ResetMatchScore")]
		[ExpectedException(typeof(InvalidIndexException))]
		public void RRBResetScore_ThrowsInvalidIndex_WithBadInput()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			b.ResetMatchScore(-1);
			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB ResetMatchScore")]
		[ExpectedException(typeof(MatchNotFoundException))]
		public void RRBResetScore_ThrowsNotFound_WithBadInput()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			b.ResetMatchScore(b.NumberOfMatches + 1);
			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("ResetMatchScore")]
		[TestCategory("SetMatchWinner")]
		public void RRBResetScore_RemovesRankingsWin_FromManualMatchWinner()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 9; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList, 5);

			b.AddGame(1, 1, 4, PlayerSlot.Challenger);
			b.SetMatchWinner(1, PlayerSlot.Challenger);
			b.ResetMatchScore(1);
			Assert.AreEqual(0, b.Rankings[0].Wins);
		}

		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("Rankings")]
		public void RRB_ScoreAndRankingTracker()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				IMatch m = b.GetMatch(n);
				if (m.Players[(int)PlayerSlot.Defender].Id > m.Players[(int)PlayerSlot.Challenger].Id)
				{
					b.AddGame(n, 1, 0, PlayerSlot.Defender);
				}
				else
				{
					b.AddGame(n, 0, 1, PlayerSlot.Challenger);
				}
			}

			Assert.AreEqual(1, 1);
		}

		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("ResetMatches")]
		public void RRBResetMatches_ResetsRankings()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 9; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);
			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.AddGame(n, 1, 3, PlayerSlot.Challenger);
			}

			b.ResetMatches();
			bool rankingsReset = true;
			foreach (IPlayerScore score in b.Rankings)
			{
				if (score.Rank != 1 || score.W != 0)
				{
					rankingsReset = false;
				}
			}
			Assert.IsTrue(rankingsReset);
		}

		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("ReplacePlayer")]
		public void RRB_ReplacePlayer_Replaces()
		{
			Mock<IPlayer> playerMoq = new Mock<IPlayer>();
			playerMoq.Setup(p => p.Id).Returns(10);

			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			int pIndex = 3;
			b.ReplacePlayer(playerMoq.Object, pIndex);
			Assert.AreEqual(playerMoq.Object, b.Players[pIndex]);
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("ReplacePlayer")]
		public void RRB_ReplacePlayer_ReplacesPlayerInRankings()
		{
			Mock<IPlayer> playerMoq = new Mock<IPlayer>();
			playerMoq.Setup(p => p.Id).Returns(10);

			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList, 3);
			List<IMatch> firstRound = b.GetRound(1);
			foreach (IMatch match in firstRound)
			{
				b.AddGame(match.MatchNumber, 1, 0, PlayerSlot.Defender);
				b.AddGame(match.MatchNumber, 0, 1, PlayerSlot.Challenger);
				b.AddGame(match.MatchNumber, 1, 0, PlayerSlot.Defender);
			}

			int pIndex = 3;
			int rankIndex = b.Rankings.FindIndex(r => r.Id == b.Players[pIndex].Id);
			b.ReplacePlayer(playerMoq.Object, pIndex);
			Assert.AreEqual(playerMoq.Object.Id, b.Rankings[rankIndex].Id);
		}
		#endregion

		#region Events
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("AddGame")]
		[TestCategory("MatchesModified")]
		public void RRBAddGame_ThrowsMatchesModifiedEvent_HoldingCurrentMatch()
		{
			MatchModel model = null;

			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 9; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);
			b.MatchesModified += delegate (object sender, BracketEventArgs e)
			{
				model = e.UpdatedMatches.FirstOrDefault();
			};

			int matchNum = 5;
			b.AddGame(matchNum, 3, 2, PlayerSlot.Defender);
			Assert.AreEqual(matchNum, model.MatchNumber);
		}

		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("ResetMatchScore")]
		[TestCategory("MatchesModified")]
		public void RRBResetMatchScore_FiresMatchesModifiedEvent_WithOnlyOneMatch()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);
			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}

			int affectedMatches = 0;
			b.MatchesModified += delegate (object sender, BracketEventArgs e)
			{
				affectedMatches += e.UpdatedMatches.Count;
			};

			b.ResetMatchScore(1);
			Assert.AreEqual(1, affectedMatches);
		}

		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("ResetMatches")]
		[TestCategory("MatchesModified")]
		public void RRBResetMatches_FiresMatchesModifiedEvent_WithAllAffectedMatches()
		{
			int matchesTouched = 5;

			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 7; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);
			for (int n = 1; n <= matchesTouched; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}

			int affectedMatches = 0;
			b.MatchesModified += delegate (object sender, BracketEventArgs e)
			{
				affectedMatches += e.UpdatedMatches.Count;
			};

			b.ResetMatches();
			Assert.AreEqual(matchesTouched, affectedMatches);
		}

		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("CheckForTies")]
		public void RRBCheckForTies_ReturnsTrueWhenTieExists()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 3; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);
			b.AddGame(1, 100, 0, PlayerSlot.Defender);
			b.AddGame(2, 15, 2, PlayerSlot.Defender);
			b.AddGame(3, 20, 10, PlayerSlot.Defender);

			Assert.IsTrue(b.CheckForTies());
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("CheckForTies")]
		[ExpectedException(typeof(BracketException))]
		public void RRBCheckForTies_ThrowsBracketExcep_OnUnfinishedBracket()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			b.CheckForTies();
			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("CheckForTies")]
		public void RRBCheckForTies_ReturnsFalseIfNoTies()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 2; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);
			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.SetMatchWinner(n, PlayerSlot.Defender);
			}

			Assert.IsFalse(b.CheckForTies());
		}

		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("GenerateTiebreakers")]
		[ExpectedException(typeof(BracketException))]
		public void RRBGenerateTiebreakers_ThrowsBracketExcep_OnUnfinishedBracket()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 2; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			b.GenerateTiebreakers();
			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("GenerateTiebreakers")]
		public void RRBGenerateTiebreakers_ReturnsFalseIfNoTies()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 2; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);
			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.SetMatchWinner(n, PlayerSlot.Defender);
			}

			Assert.IsFalse(b.GenerateTiebreakers());
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("GenerateTiebreakers")]
		public void RRBGenerateTiebreakers_ReturnsTrueWhenGenerating()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);
			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.SetMatchWinner(n, PlayerSlot.Defender);
			}

			Assert.IsTrue(b.GenerateTiebreakers());
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("GenerateTiebreakers")]
		[TestCategory("RoundAdded")]
		public void RRBGenerateTiebreakers_FiresRoundAddedEvent()
		{
			bool roundAdded = false;

			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);
			b.RoundAdded += delegate
			{
				roundAdded = true;
			};
			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.SetMatchWinner(n, PlayerSlot.Defender);
			}

			b.GenerateTiebreakers();
			Assert.IsTrue(roundAdded);
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("GenerateTiebreakers")]
		[TestCategory("RoundAdded")]
		public void RRBGenerateTiebreakers_RoundAddedEventContainsModelsForAllNewMatches()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 3; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);
			int modelsAdded = 0;
			int oldMatchCount = b.NumberOfMatches;
			b.RoundAdded += delegate (object sender, BracketEventArgs e)
			{
				modelsAdded += e.UpdatedMatches.Count;
			};

			b.AddGame(1, 100, 0, PlayerSlot.Defender);
			b.AddGame(2, 15, 2, PlayerSlot.Defender);
			b.AddGame(3, 20, 10, PlayerSlot.Defender);

			b.GenerateTiebreakers();
			Assert.AreEqual(b.NumberOfMatches - oldMatchCount, modelsAdded);
		}
		#endregion
	}
}
