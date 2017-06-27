using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Moq;
using System.Linq;

using DatabaseLib;

namespace Tournament.Structure.Tests
{
	[TestClass]
	public class SingleElimBracketTests
	{
		#region Bracket Creation
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB Ctor")]
		public void SEBCtor_Constructs()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 2; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			Assert.AreEqual(pList, b.Players);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB Ctor")]
		public void SEBCtor_CreatesNoMatches_WithLessThanTwoPlayers()
		{
			IBracket b = new SingleElimBracket();
			//b.CreateBracket();

			Assert.AreEqual(0, b.NumberOfMatches);
		}

		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB CreateBracket")]
		public void SEBCreateBracket_CreatesFor4Players()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			Assert.AreEqual(3, b.NumberOfMatches);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB CreateBracket")]
		public void SEBCreateBracket_AssignsR1MatchNumbers()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 10; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			Assert.AreEqual(1, b.GetRound(1)[0].MatchNumber);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB CreateBracket")]
		public void SEBCreateBracket_AssignsR2MatchNumbers()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 10; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			Assert.AreEqual(3, b.GetRound(2)[0].MatchNumber);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB CreateBracket")]
		public void SEBCreateBracket_AssignsFinalRoundMatchNumber()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 10; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			Assert.AreEqual(9, b.GetRound(b.NumberOfRounds)[0].MatchNumber);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB CreateBracket")]
		public void SEBCreateBracket_4Players_DoesNotAssignPlayersToSecondRound()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			Assert.IsNull(b.GetRound(2)[0].Players[(int)PlayerSlot.Defender]);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB CreateBracket")]
		public void SEBCreateBracket_4Players_AssignsPlayersToRound1()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			Assert.AreEqual(b.Players[3],
				b.GetRound(1)[0].Players[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB CreateBracket")]
		public void SEBCreateBracket_5Players_1FirstRoundMatch_and_2SecondRoundMatches()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 5; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			Assert.IsTrue
				(b.GetRound(1).Count == 1
				&& b.GetRound(2).Count == 2);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB CreateBracket")]
		public void SEBCreateBracket_5Players_CorrectlyAssignsBye()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 5; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			Assert.IsNull(b.GetRound(2)[0].Players[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB CreateBracket")]
		public void SEBCreateBracket_29Players_CorrectlyGeneratesMatches()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 29; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			Assert.IsTrue(5 == b.NumberOfRounds
				&& 13 == b.GetRound(1).Count);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB CreateBracket")]
		public void SEBCreateBracket_29Players_CorrectlyAssignsPlayers()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 29; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			Assert.IsTrue(15 == b.GetRound(1)[0].Players[(int)PlayerSlot.Defender].Id
				&& 16 == b.GetRound(1)[0].Players[(int)PlayerSlot.Challenger].Id);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB CreateBracket")]
		public void SEBCreateBracket_SetsLowerRoundsToEmptyDict()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			List<IMatch> lowerRound1 = b.GetLowerRound(1);
			Assert.AreEqual(0, lowerRound1.Count);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB CreateBracket")]
		public void SEBCreateBracket_SetsGrandFinalToNull()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			Assert.IsNull(b.GrandFinal);
		}
		#endregion

		#region Bracket Progression
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB AddGame")]
		public void SEBAddGame_4Players_UpdatesScore()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			b.AddGame(1, 0, 20, PlayerSlot.Challenger);
			Assert.AreEqual(1, b.GetRound(1)[0].Score[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB AddGame")]
		public void SEBAddGame_FinishedMatch_DoesNotSetBracketIsFinished()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			b.AddGame(1, 1, 5, PlayerSlot.Challenger);
			Assert.IsFalse(b.IsFinished);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB AddGame")]
		public void SEBAddGame_FinishedFinals_SetsBracketIsFinished()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}
			Assert.IsTrue(b.IsFinished);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB AddGame")]
		[TestCategory("Rankings")]
		public void SEBAddGame_FinishedMatch_AddsLoserToRankingsArray()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			b.AddGame(1, 5, 3, PlayerSlot.Defender);
			Assert.AreEqual(b.Rankings[0].Id,
				b.GetMatch(1).Players[(int)PlayerSlot.Challenger].Id);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB AddGame")]
		[TestCategory("Rankings")]
		public void SEBAddGame_FinishedBracket_AddsWinnerToTopOfRankings()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}
			Assert.AreEqual(b.Rankings[0].Id,
				b.GetMatch(b.NumberOfMatches).Players[(int)PlayerSlot.Defender].Id);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB AddGame")]
		[ExpectedException(typeof(InactiveMatchException))]
		public void SEBAddGame_ThrowsInactive_WhenMatchIsAlreadyOver()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			b.AddGame(1, 0, 1, PlayerSlot.Challenger);
			b.AddGame(1, 0, 1, PlayerSlot.Challenger);
			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB AddGame")]
		public void SEBAddGame_4Players_AdvancesWinningPlayer()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			b.AddGame(2, 5, 89, PlayerSlot.Challenger);
			Assert.AreEqual(b.GetMatch(2).Players[(int)PlayerSlot.Challenger],
				b.GetRound(2)[0].Players[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB AddGame")]
		[ExpectedException(typeof(InvalidIndexException))]
		public void SEBAddGame_ThrowsInvalidIndex_WithNegativeMatchNum()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			b.AddGame(-1, 1, 0, PlayerSlot.Defender);
			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB AddGame")]
		[ExpectedException(typeof(MatchNotFoundException))]
		public void SEBAddGame_ThrowsNotFound_WhenMatchParamIsNotFound()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			b.AddGame((b.NumberOfMatches + 1), 2, 1, PlayerSlot.Defender);
			Assert.AreEqual(1, 2);
		}
		
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB UpdateGame")]
		public void SEBUpdateGame_ChangesGame()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList, 3);
			b.AddGame(1, 3, 1, PlayerSlot.Defender);

			b.UpdateGame(1, 1, 1, 2, PlayerSlot.Challenger);
			Assert.AreEqual(0, b.GetMatch(1).Score[(int)PlayerSlot.Defender]);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB UpdateGame")]
		public void SEBUpdateGame_ChangesMatchOutcome()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			b.AddGame(1, 3, 1, PlayerSlot.Defender);
			int oldWinnerId = b.GetRound(2)[0].Players[(int)PlayerSlot.Defender].Id;

			b.UpdateGame(1, 1, 1, 2, PlayerSlot.Challenger);
			Assert.AreNotEqual(oldWinnerId,
				b.GetRound(2)[0].Players[(int)PlayerSlot.Defender].Id);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB UpdateGame")]
		public void SEBUpdateGame_ChangesRankings()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.AddGame(n, 2, 1, PlayerSlot.Defender);
			}
			int oldWinnerId = b.Rankings[0].Id;

			b.UpdateGame(b.NumberOfMatches, 1, 1, 2, PlayerSlot.Challenger);
			Assert.AreNotEqual(oldWinnerId, b.Rankings[0].Id);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB UpdateGame")]
		public void SEBUpdateGame_ChangesScore()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			b.AddGame(1, 1, 0, PlayerSlot.Defender);

			int pointVal = 5;
			b.UpdateGame(1, 1, pointVal, 3, PlayerSlot.Defender);
			Assert.AreEqual(pointVal, b.GetMatch(1).Games[0].Score[(int)PlayerSlot.Defender]);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB UpdateGame")]
		[ExpectedException(typeof(GameNotFoundException))]
		public void SEBUpdateGame_ThrowsGameNotFound_WithBadGameNumberInput()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.UpdateGame(1, 5, 1, 0, PlayerSlot.Defender);
			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB UpdateGame")]
		public void SEBUpdateGame_CascadesToOtherMatchesWhenReversingAWin()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			for (int n = 1; n < b.NumberOfMatches; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}

			b.UpdateGame(1, 1, 0, 1, PlayerSlot.Challenger);
			Assert.IsFalse(b.GetMatch(b.GetMatch(1).NextMatchNumber).IsFinished);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB UpdateGame")]
		public void SEBUpdateGame_ChangesGameScoreEvenInFinishedBrackets()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}

			int pointVal = 5;
			b.UpdateGame(2, 1, pointVal, 2, PlayerSlot.Defender);
			Assert.AreEqual(pointVal, b.GetMatch(2).Games[0].Score[(int)PlayerSlot.Defender]);
		}

		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB RemoveLastGame")]
		public void SEBRemoveLastGame_SubtractsScore()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList, 3);

			b.AddGame(1, 1, 0, PlayerSlot.Defender);
			b.RemoveLastGame(1);
			Assert.AreEqual(0, b.GetMatch(1).Score[(int)PlayerSlot.Defender]);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB RemoveLastGame")]
		public void SEBRemoveLastGame_RemovesGame()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList, 3);

			b.AddGame(1, 1, 0, PlayerSlot.Defender);
			b.RemoveLastGame(1);
			Assert.AreEqual(0, b.GetMatch(1).Games.Count);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB RemoveLastGame")]
		public void SEBRemoveLastGame_ReversingFinalsVictory_SetsBracketIsNOTFinished()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.AddGame(n, 0, 20, PlayerSlot.Challenger);
			}
			b.RemoveLastGame(b.NumberOfMatches);
			Assert.IsFalse(b.IsFinished);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB RemoveLastGame")]
		[TestCategory("Rankings")]
		public void SEBRemoveLastGame_ReactivatedMatch_RemovesLoserFromRankingsList()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.AddGame(1, 1, 0, PlayerSlot.Defender);
			b.RemoveLastGame(1);
			Assert.AreEqual(0, b.Rankings.Count);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB RemoveLastGame")]
		[ExpectedException(typeof(InvalidIndexException))]
		public void SEBRemoveLastGame_ThrowsInvalidIndex_WithBadMatchNumberInput()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.RemoveLastGame(0);
			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB RemoveLastGame")]
		[ExpectedException(typeof(MatchNotFoundException))]
		public void SEBRemoveLastGame_ThrowsNotFound_WithBadMatchNumberInput()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.RemoveLastGame(b.NumberOfMatches + 1);
			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB RemoveGameNumber")]
		public void SEBRemoveGameNumber_RemovesAGame()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			b.AddGame(1, 1, 0, PlayerSlot.Defender);

			b.RemoveGameNumber(1, 1);
			Assert.AreEqual(0, b.GetMatch(1).Games.Count);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB RemoveGameNumber")]
		public void SEBRemoveGameNumber_RemovesAdvancedPlayers()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			b.AddGame(1, 1, 0, PlayerSlot.Defender);

			b.RemoveGameNumber(1, 1);
			Assert.IsNull(b.GetMatch(b.GetMatch(1).NextMatchNumber).Players[(int)PlayerSlot.Defender]);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB RemoveGameNumber")]
		public void SEBRemoveGameNumber_RemovesGameFromMiddleOfList()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList, 5);
			for (int i = 0; i < 3; ++i)
			{
				b.AddGame(1, 2, 1, PlayerSlot.Defender);
			}

			int gameNum = 2;
			b.RemoveGameNumber(1, gameNum);
			Assert.IsFalse(b.GetMatch(1).Games.Any(g => g.GameNumber == gameNum));
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB RemoveGameNumber")]
		public void SEBRemoveGameNumber_CanRemoveGameWithoutReversingMatchResult()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList, 3);
			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
				b.AddGame(n, 0, 1, PlayerSlot.Challenger);
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}

			int pId = b.GetMatch(b.GetMatch(1).NextMatchNumber).Players[(int)PlayerSlot.Defender].Id;
			b.RemoveGameNumber(1, 2);
			Assert.AreEqual(pId, b.GetMatch(b.GetMatch(1).NextMatchNumber).Players[(int)PlayerSlot.Defender].Id);
		}

		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB SetMatchWinner")]
		public void SEBSetMatchWinner_CorrectlyUpdatesMatchData()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.SetMatchWinner(1, PlayerSlot.Challenger);
			IMatch match = b.GetMatch(1);
			Assert.IsTrue(true == match.IsFinished &&
				true == match.IsManualWin &&
				PlayerSlot.Challenger == match.WinnerSlot &&
				0 == match.Games.Count);
			// IsFinished = true
			// IsManualWin = true
			// WinnerSlot sets
			// No games are added
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB SetMatchWinner")]
		public void SEBSetMatchWinner_WinnerAdvances()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.SetMatchWinner(1, PlayerSlot.Challenger);
			Assert.AreEqual(b.GetMatch(1).Players[(int)PlayerSlot.Challenger].Id,
				b.GetMatch(b.GetMatch(1).NextMatchNumber).Players[(int)PlayerSlot.Defender].Id);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB SetMatchWinner")]
		public void SEBSetMatchWinner_CanChangeTheExistingWinner()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			b.AddGame(1, 1, 0, PlayerSlot.Defender);

			PlayerSlot winnerSlot = PlayerSlot.Challenger;
			b.SetMatchWinner(1, winnerSlot);
			Assert.AreEqual(winnerSlot, b.GetMatch(1).WinnerSlot);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB SetMatchWinner")]
		public void SEBSetMatchWinner_KeepsAnyExistingGames()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList, 5);
			b.AddGame(1, 1, 0, PlayerSlot.Defender);
			b.AddGame(1, 1, 3, PlayerSlot.Challenger);
			b.AddGame(1, 2, 1, PlayerSlot.Defender);

			b.SetMatchWinner(1, PlayerSlot.Challenger);
			Assert.AreEqual(3, b.GetMatch(1).Games.Count);
		}

		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB ResetMatchScore")]
		public void SEBResetMatchScore_ResetsScore()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList, 3);

			b.AddGame(1, 1, 0, PlayerSlot.Defender);
			b.AddGame(1, 0, 1, PlayerSlot.Challenger);
			b.ResetMatchScore(1);
			Assert.AreEqual(0, b.GetMatch(1).Score[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB ResetMatchScore")]
		public void SEBResetMatchScore_RemovesGames()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList, 3);

			b.AddGame(1, 1, 0, PlayerSlot.Defender);
			b.AddGame(1, 0, 1, PlayerSlot.Challenger);
			b.ResetMatchScore(1);
			Assert.AreEqual(0, b.GetMatch(1).Games.Count);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB ResetMatchScore")]
		[TestCategory("SEB RemovePlayerFromFutureMatches")]
		public void SEBResetMatchScore_RemovesAffectedLosersFromRankingsList()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}
			b.ResetMatchScore(1);
			// Only remaining Finished match is n=2
			Assert.AreEqual(1, b.Rankings.Count);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB ResetMatchScore")]
		[ExpectedException(typeof(InvalidIndexException))]
		public void SEBResetMatchScore_ThrowsInvalidIndex_WithBadMatchNumberInput()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.ResetMatchScore(-1);
			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB ResetMatchScore")]
		[ExpectedException(typeof(MatchNotFoundException))]
		public void SEBResetMatchScore_ThrowsNotFound_WithBadMatchNumberInput()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.ResetMatchScore(b.NumberOfMatches + 1);
			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB RemovePlayerFromFutureMatches")]
		public void SEB_RPFFM_RemovesPlayer()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.AddGame(1, 1, 0, PlayerSlot.Defender);
			b.RemoveLastGame(1);
			Assert.IsNull(b.GetRound(2)[0].Players[(int)PlayerSlot.Defender]);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB RemovePlayerFromFutureMatches")]
		public void SEB_RPFFM_ResetsFutureScores()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			b.SetMaxGamesForWholeRound(2, 5);

			b.AddGame(1, 1, 0, PlayerSlot.Defender);
			b.AddGame(2, 1, 0, PlayerSlot.Defender);
			b.AddGame(3, 1, 0, PlayerSlot.Defender);
			b.AddGame(3, 1, 0, PlayerSlot.Defender);
			b.AddGame(3, 0, 1, PlayerSlot.Challenger);
			b.AddGame(3, 0, 1, PlayerSlot.Challenger);
			b.RemoveLastGame(1);
			Assert.AreEqual(0, b.GetMatch(3).Score[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB RemovePlayerFromFutureMatches")]
		public void SEB_RPFFM_OnlyRemovesPlayerInQuestionFromFuture()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.AddGame(1, 1, 0, PlayerSlot.Defender);
			b.AddGame(2, 1, 0, PlayerSlot.Defender);
			b.RemoveLastGame(1);
			Assert.AreEqual(b.GetMatch(2).Players[(int)PlayerSlot.Defender],
				b.GetMatch(3).Players[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB RemovePlayerFromFutureMatches")]
		public void SEB_RPFFM_RemovesPlayerFromManyMatches()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 32; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			for (int n = 1; n < 32; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}
			b.RemoveLastGame(1);
			Assert.IsNull(b.GetMatch(31).Players[(int)PlayerSlot.Defender]);
		}

		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB ResetMatches")]
		public void SEBResetMatches_SetsAllMatchesToNotFinished()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}

			b.ResetMatches();
			int finishedMatches = 0;
			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				if (b.GetMatch(n).IsFinished)
				{
					++finishedMatches;
				}
			}
			Assert.AreEqual(0, finishedMatches);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB ResetMatches")]
		public void SEBResetMatches_RemovesAllGames()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList, 3);
			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}

			b.ResetMatches();
			int gamesCount = 0;
			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				gamesCount += b.GetMatch(n).Games.Count;
			}
			Assert.AreEqual(0, gamesCount);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB ResetMatches")]
		public void SEBResetMatches_RemovesAllAdvancedPlayers()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.SetMatchWinner(n, PlayerSlot.Defender);
			}

			b.ResetMatches();
			int advancedPlayers = 0;
			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				IMatch match = b.GetMatch(n);
				if (1 == match.RoundIndex)
				{
					continue;
				}

				for (int i = 0; i < match.Players.Length; ++i)
				{
					if (null != match.Players[i])
					{
						++advancedPlayers;
					}
				}
			}
			Assert.AreEqual(0, advancedPlayers);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB ResetMatches")]
		public void SEBResetMatches_FiresOneMatchesEvent_AndOneGamesEvent()
		{
			int gamesDeletedEvents = 0;
			int matchesModifiedEvents = 0;

			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			b.GamesDeleted += delegate (object sender, BracketEventArgs e)
			{
				++gamesDeletedEvents;
			};
			b.MatchesModified += delegate (object sender, BracketEventArgs e)
			{
				++matchesModifiedEvents;
			};
			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}

			matchesModifiedEvents = 0;
			b.ResetMatches();
			Assert.IsTrue(1 == gamesDeletedEvents &&
				1 == matchesModifiedEvents);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB ResetMatches")]
		public void SEBResetMatches_FiresEventWithDeletedGames()
		{
			List<int> deletedGames = new List<int>();

			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			b.GamesDeleted += delegate (object sender, BracketEventArgs e)
			{
				deletedGames.AddRange(e.DeletedGameIDs);
			};
			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}

			b.ResetMatches();
			Assert.AreEqual(b.NumberOfMatches, deletedGames.Count);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB ResetMatches")]
		public void SEBResetMatches_FiresEventWithModifiedMatchModels()
		{
			List<MatchModel> modifiedMatches = new List<MatchModel>();

			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			b.MatchesModified += delegate (object sender, BracketEventArgs e)
			{
				modifiedMatches.AddRange(e.UpdatedMatches);
			};
			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}

			modifiedMatches.Clear();
			b.ResetMatches();
			Assert.AreEqual(b.NumberOfMatches, modifiedMatches.Count);
		}
		#endregion

		#region Events
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("AddGame")]
		[TestCategory("MatchesModified")]
		public void SEBAddGame_ThrowsMatchesModifiedEvent_HoldingCurrentMatch()
		{
			MatchModel model = null;

			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList, 3);
			b.MatchesModified += delegate (object sender, BracketEventArgs e)
			{
				model = e.UpdatedMatches.FirstOrDefault();
			};

			int matchNum = 1;
			b.AddGame(matchNum, 3, 2, PlayerSlot.Defender);
			Assert.AreEqual(matchNum, model.MatchNumber);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("AddGame")]
		[TestCategory("MatchesModified")]
		public void SEBAddGame_ThrowsMatchesModifiedEvent_HoldingAdvancedMatch()
		{
			List<int> modifiedMatchNums = new List<int>();

			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			b.MatchesModified += delegate (object sender, BracketEventArgs e)
			{
				modifiedMatchNums = e.UpdatedMatches
				.Select(m => m.MatchNumber).ToList();
			};

			int matchNum = 2;
			int nextMatchNum = b.GetMatch(matchNum).NextMatchNumber;
			b.AddGame(matchNum, 3, 2, PlayerSlot.Defender);
			Assert.IsTrue(modifiedMatchNums.Contains(nextMatchNum));
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("AddGame")]
		[TestCategory("MatchesModified")]
		public void SEBAddGame_ThrowsMatchesModifiedEvent_WithOnlyAffectedMatches()
		{
			int alteredMatches = 0;

			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			b.MatchesModified += delegate (object sender, BracketEventArgs e)
			{
				alteredMatches += e.UpdatedMatches.Count;
			};

			b.AddGame(1, 3, 2, PlayerSlot.Defender);
			Assert.AreEqual(2, alteredMatches);
		}

		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("RemoveLastGame")]
		[TestCategory("MatchesModified")]
		public void SEBRemoveGame_FiresMatchesModifiedEvent_WithCurrentMatch()
		{
			int matchNum = 1;
			MatchModel model = null;

			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList, 3);
			b.AddGame(matchNum, 3, 2, PlayerSlot.Defender);

			b.MatchesModified += delegate (object sender, BracketEventArgs e)
			{
				model = e.UpdatedMatches.FirstOrDefault();
			};
			b.RemoveLastGame(matchNum);
			Assert.AreEqual(matchNum, model.MatchNumber);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("RemoveLastGame")]
		[TestCategory("MatchesModified")]
		public void SEBRemoveGame_FiresMatchesModifiedEvent_WithRemovedGameID()
		{
			int removedGames = 0;

			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList, 3);
			b.MatchesModified += delegate (object sender, BracketEventArgs e)
			{
				removedGames = e.DeletedGameIDs.Count;
			};
			b.AddGame(1, 3, 2, PlayerSlot.Defender);

			b.RemoveLastGame(1);
			Assert.AreEqual(1, removedGames);
		}

		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("ResetMatchScore")]
		[TestCategory("MatchesModified")]
		public void SEBResetMatchScore_FiresMatchesModifiedEvent_WithAllAffectedMatches()
		{
			int chainLength = 1;
			int matchesAffected = -1;

			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 17; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			IMatch m = b.GetMatch(1);
			while (m.NextMatchNumber > 0)
			{
				// Follow the chain to find how many Matches we'll touch:
				m = b.GetMatch(m.NextMatchNumber);
				++chainLength;
			}
			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.SetMatchWinner(n, PlayerSlot.Defender);
			}

			b.MatchesModified += delegate (object sender, BracketEventArgs e)
			{
				matchesAffected = e.UpdatedMatches
				.Select(mm => mm.MatchNumber).Distinct().ToList().Count;
			};
			b.ResetMatchScore(1);
			Assert.AreEqual(chainLength, matchesAffected);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("ResetMatchScore")]
		[TestCategory("GamesDeleted")]
		[TestCategory("MatchesModified")]
		public void SEBResetMatchScore_FiresEvents_ForAllRemovedGames()
		{
			int chainLength = 1;
			int gamesDeleted = 0;

			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 17; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList, 3);
			b.GamesDeleted += delegate (object sender, BracketEventArgs e)
			{
				gamesDeleted += e.DeletedGameIDs.Count;
			};
			b.MatchesModified += delegate (object sender, BracketEventArgs e)
			{
				gamesDeleted += e.DeletedGameIDs.Count;
			};
			IMatch m = b.GetMatch(1);
			while (m.NextMatchNumber > 0)
			{
				// Follow the chain to find how many Matches we'll touch:
				m = b.GetMatch(m.NextMatchNumber);
				++chainLength;
			}
			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
				b.AddGame(n, 0, 1, PlayerSlot.Challenger);
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}

			b.ResetMatchScore(1);
			Assert.AreEqual(chainLength * 3, gamesDeleted);
		}

		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("ResetMatches")]
		[TestCategory("MatchesModified")]
		public void SEBResetMatches_FiresMatchesModifiedEvent_WithAllAffectedMatches()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 17; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.SetMatchWinner(n, PlayerSlot.Defender);
			}

			int matchesAffected = -1;
			b.MatchesModified += delegate (object sender, BracketEventArgs e)
			{
				matchesAffected = e.UpdatedMatches
				.Select(mm => mm.MatchNumber).Distinct().ToList().Count;
			};
			b.ResetMatches();
			Assert.AreEqual(b.NumberOfMatches, matchesAffected);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("ResetMatchScore")]
		[TestCategory("MatchesModified")]
		[TestCategory("GamesDeleted")]
		public void SEBResetMatches_FiresEvents_WithAllRemovedGames()
		{
			int deletedGames = 0;

			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 17; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList, 3);
			b.MatchesModified += delegate (object sender, BracketEventArgs e)
			{
				deletedGames += e.DeletedGameIDs.Count;
			};
			b.GamesDeleted += delegate (object sender, BracketEventArgs e)
			{
				deletedGames += e.DeletedGameIDs.Count;
			};
			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}

			b.ResetMatches();
			Assert.AreEqual(b.NumberOfMatches * 2, deletedGames);
		}
		#endregion

		#region Models
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("BracketModel")]
		public void SEBGetModel_ReturnsABracketModel()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 5; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			BracketModel bModel = b.GetModel(0);
			Assert.IsInstanceOfType(bModel, typeof(BracketModel));
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("BracketModel")]
		public void SEBGetModel_HasModelsOfAllPLayers()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 5; ++i)
			{
				IPlayer p = new Player(i + 1, "Player " + (i + 1).ToString());
				pList.Add(p);
			}
			IBracket b = new SingleElimBracket(pList);

			BracketModel bModel = b.GetModel(0);
			Assert.AreEqual(pList.Count, bModel.TournamentUsersBrackets.Count);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("BracketModel")]
		public void SEBGetModel_HasModelsOfAllMatches()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 5; ++i)
			{
				IPlayer p = new Player(i + 1, "Player " + (i + 1).ToString());
				pList.Add(p);
			}
			IBracket b = new SingleElimBracket(pList);

			BracketModel bModel = b.GetModel(0);
			Assert.AreEqual(b.NumberOfMatches, bModel.Matches.Count);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("BracketModel")]
		public void SEBGetModel_OfCompletedBracket_HasUpdatedMatchData()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 5; ++i)
			{
				IPlayer p = new Player(i + 1, "Player " + (i + 1).ToString());
				pList.Add(p);
			}
			IBracket b = new SingleElimBracket(pList);
			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.AddGame(n, 15, 5, PlayerSlot.Defender);
			}
			
			BracketModel bModel = b.GetModel(0);
			Assert.IsTrue(bModel.Matches.ToList().TrueForAll(m => m.Games.Count == 1));
		}
		#endregion

		#region Accessors & Mutators
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("Bracket Accessors")]
		public void SEBNumberOfLowerRounds_Returns0()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			Assert.AreEqual(0, b.NumberOfLowerRounds);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("Bracket Accessors")]
		public void SEBGetLowerRound_ReturnsEmptyList()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			List<IMatch> lowerRound1 = b.GetLowerRound(1);

			Assert.AreEqual(0, lowerRound1.Count);
		}

		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SetMaxGamesForWholeRound")]
		[ExpectedException(typeof(ScoreException))]
		public void SMGFWR_ThrowsScoreException_WithNegativeInput()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.SetMaxGamesForWholeRound(1, -1);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SetMaxGamesForWholeLowerRound")]
		public void SEBSMGFWLR_DoesNothing()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.SetMaxGamesForWholeLowerRound(1, 3);
			Assert.AreEqual(1, b.GetRound(1)[0].MaxGames);
		}
		#endregion
	}
}
