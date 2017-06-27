using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Moq;

using DatabaseLib;

namespace Tournament.Structure.Tests
{
	[TestClass]
	public class MatchTests
	{
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Ctor")]
		public void MatchDefaultCtor_Constructs()
		{
			Match m = new Match();

			Assert.AreEqual(1, m.MaxGames);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Ctor")]
		[TestCategory("MatchModel")]
		[ExpectedException(typeof(ArgumentNullException))]
		public void MatchModelCtor_ThrowsExceptionOnNullParam()
		{
			MatchModel mod = null;
			Match m = new Match(mod);

			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match GetModel")]
		[TestCategory("MatchModel")]
		public void GetModel_AddsDefaultPlayerIDs_WhenMatchIsMissingPlayers()
		{
			Match m = new Match();
			MatchModel model = m.GetModel();

			Assert.AreEqual(-1, model.ChallengerID);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match GetModel")]
		[TestCategory("MatchModel")]
		public void GetModel_CorrectlyCopiesMatchData()
		{
			Match m = new Match();
			m.SetMaxGames(3);
			MatchModel model = m.GetModel();

			Assert.AreEqual(m.MaxGames, model.MaxGames);
		}

		#region Player Updates
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match AddPlayer")]
		public void AddPlayer_AddsAPlayer()
		{
			IPlayer p = new Mock<IPlayer>().Object;
			Match m = new Match();
			m.AddPlayer(p);

			Assert.AreEqual(p, m.Players[(int)PlayerSlot.Defender]);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match AddPlayer")]
		public void AddPlayer_DoesNotSetMatchIsReady()
		{
			Match m = new Match();
			m.AddPlayer(new Mock<IPlayer>().Object);

			Assert.AreEqual(false, m.IsReady);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match AddPlayer")]
		public void AddPlayer_AddsTwoPlayers()
		{
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(10);
			var p2 = new Mock<IPlayer>();
			p2.Setup(p => p.Id).Returns(20);

			Match m = new Match();
			m.AddPlayer(p1.Object);
			m.AddPlayer(p2.Object);

			Assert.AreEqual(p2.Object, m.Players[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match AddPlayer")]
		public void AddPlayer_SecondPlayerSetsMatchIsReady()
		{
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(10);
			var p2 = new Mock<IPlayer>();
			p2.Setup(p => p.Id).Returns(20);

			Match m = new Match();
			m.AddPlayer(p1.Object);
			m.AddPlayer(p2.Object);

			Assert.AreEqual(true, m.IsReady);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match AddPlayer")]
		public void AddPlayer_OverloadedMethodAddsPlayerToCorrectSlot()
		{
			var p2 = new Mock<IPlayer>();

			Match m = new Match();
			m.AddPlayer(p2.Object, PlayerSlot.Challenger);

			Assert.AreEqual(p2.Object, m.Players[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match AddPlayer")]
		[ExpectedException(typeof(InvalidSlotException))]
		public void AddPlayer_ThrowsInvalidSlot_WithBadSlotParam()
		{
			Match m = new Match();
			m.AddPlayer(new Mock<IPlayer>().Object, (PlayerSlot)4);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match AddPlayer")]
		[ExpectedException(typeof(SlotFullException))]
		public void AddPlayer_ThrowsException_WhenAddingPlayerToSameSpot()
		{
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(10);
			var p2 = new Mock<IPlayer>();
			p2.Setup(p => p.Id).Returns(20);

			Match m = new Match();
			m.AddPlayer(p1.Object, PlayerSlot.Defender);
			m.AddPlayer(p2.Object, PlayerSlot.Defender);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match AddPlayer")]
		[ExpectedException(typeof(DuplicateObjectException))]
		public void AddPlayer_ThrowsDuplicate_WhenAddingSamePlayerTwice()
		{
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(1);

			Match m = new Match();
			m.AddPlayer(p1.Object, PlayerSlot.Challenger);
			m.AddPlayer(p1.Object, PlayerSlot.Defender);

			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match ReplacePlayer")]
		public void ReplacePlayer_Replaces()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 3; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}

			Match m = new Match();
			m.AddPlayer(pList[0]);
			m.AddPlayer(pList[1]);
			m.ReplacePlayer(pList[2], pList[(int)PlayerSlot.Challenger].Id);

			Assert.AreEqual(pList[2].Id, m.Players[(int)PlayerSlot.Challenger].Id);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match ReplacePlayer")]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ReplacePlayer_ThrowsNullException_WithNullParam()
		{
			Match m = new Match();
			m.ReplacePlayer(null, 1);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match ReplacePlayer")]
		[ExpectedException(typeof(PlayerNotFoundException))]
		public void ReplacePlayer_ThrowsNotFound_IfIDIsNotInMatch()
		{
			Match m = new Match();
			m.ReplacePlayer(new Mock<IPlayer>().Object, 10);

			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match RemovePlayer")]
		public void RemovePlayer_Removes()
		{
			int playerId = 10;
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(playerId);

			Match m = new Match();
			m.AddPlayer(p1.Object, PlayerSlot.Defender);
			m.RemovePlayer(playerId);

			Assert.IsNull(m.Players[(int)PlayerSlot.Defender]);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match RemovePlayer")]
		public void RemovePlayer_SetsMatchNotReady()
		{
			int playerId = 10;
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(playerId);
			var p2 = new Mock<IPlayer>();
			p2.Setup(p => p.Id).Returns(20);

			Match m = new Match();
			m.AddPlayer(p1.Object);
			m.AddPlayer(p2.Object);
			m.RemovePlayer(playerId);

			Assert.AreEqual(false, m.IsReady);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match RemovePlayer")]
		[ExpectedException(typeof(PlayerNotFoundException))]
		public void RemovePlayer_ThrowsNotFound_IfPlayerToRemoveDoesntExist()
		{
			Match m = new Match();
			m.RemovePlayer(10);

			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match ResetPlayers")]
		public void ResetPlayers_ResetsArr1()
		{
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(10);
			var p2 = new Mock<IPlayer>();
			p2.Setup(p => p.Id).Returns(20);

			Match m = new Match();
			m.AddPlayer(p1.Object);
			m.AddPlayer(p2.Object);
			m.ResetPlayers();

			Assert.IsNull(m.Players[(int)PlayerSlot.Defender]);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match ResetPlayers")]
		public void ResetPlayers_ResetsArr2()
		{
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(10);
			var p2 = new Mock<IPlayer>();
			p2.Setup(p => p.Id).Returns(20);

			Match m = new Match();
			m.AddPlayer(p1.Object);
			m.AddPlayer(p2.Object);
			m.ResetPlayers();

			Assert.IsNull(m.Players[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match ResetPlayers")]
		public void ResetPlayers_SetsMatchInactive()
		{
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(10);
			var p2 = new Mock<IPlayer>();
			p2.Setup(p => p.Id).Returns(20);

			Match m = new Match();
			m.AddPlayer(p1.Object);
			m.AddPlayer(p2.Object);
			m.ResetPlayers();

			Assert.AreEqual(false, m.IsReady);
		}
		#endregion

		#region Score Updates
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match AddGame")]
		[TestCategory("Game")]
		public void AddGame_AddsToGameList()
		{
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(10);
			var p2 = new Mock<IPlayer>();
			p2.Setup(p => p.Id).Returns(20);
			Match m = new Match();
			m.AddPlayer(p1.Object);
			m.AddPlayer(p2.Object);

			m.AddGame(0, 1, PlayerSlot.Challenger);
			Assert.AreEqual(1, m.Games.Count);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match AddGame")]
		[TestCategory("Game")]
		public void AddGame_UpdatesScore()
		{
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(10);
			var p2 = new Mock<IPlayer>();
			p2.Setup(p => p.Id).Returns(20);
			Match m = new Match();
			m.AddPlayer(p1.Object);
			m.AddPlayer(p2.Object);

			m.AddGame(1, 0, PlayerSlot.Defender);
			Assert.AreEqual(1, m.Score[(int)PlayerSlot.Defender]);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match AddGame")]
		[TestCategory("Game")]
		[ExpectedException(typeof(InactiveMatchException))]
		public void AddGame_ThrowsInactiveIfMatchIsFinished()
		{
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(10);
			var p2 = new Mock<IPlayer>();
			p2.Setup(p => p.Id).Returns(20);
			Match m = new Match();
			m.AddPlayer(p1.Object);
			m.AddPlayer(p2.Object);

			m.AddGame(1, 0, PlayerSlot.Defender);
			m.AddGame(0, 1, PlayerSlot.Challenger);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match AddGame")]
		[TestCategory("Game")]
		[ExpectedException(typeof(InactiveMatchException))]
		public void AddGame_ThrowsInactiveIfTooFewPlayers()
		{
			var p1 = new Mock<IPlayer>();
			Match m = new Match();
			m.AddPlayer(p1.Object);

			m.AddGame(1, 0, PlayerSlot.Defender);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match AddGame")]
		[TestCategory("Game")]
		public void AddGame_AddsCorrectlyAfterMaxGamesIsIncreased()
		{
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(10);
			var p2 = new Mock<IPlayer>();
			p2.Setup(p => p.Id).Returns(20);
			Match m = new Match();
			m.AddPlayer(p1.Object);
			m.AddPlayer(p2.Object);
			m.SetMaxGames(3);

			m.AddGame(1, 0, PlayerSlot.Defender);
			m.AddGame(1, 0, PlayerSlot.Defender);
			Assert.AreEqual(2, m.Score[(int)PlayerSlot.Defender]);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match AddGame")]
		[TestCategory("Game")]
		public void AddGame_CorrectlySetsMatchIsFinished()
		{
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(10);
			var p2 = new Mock<IPlayer>();
			p2.Setup(p => p.Id).Returns(20);
			Match m = new Match();
			m.AddPlayer(p1.Object);
			m.AddPlayer(p2.Object);
			m.SetMaxGames(3);

			m.AddGame(1, 0, PlayerSlot.Defender);
			m.AddGame(1, 0, PlayerSlot.Defender);
			Assert.IsTrue(m.IsFinished);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match AddGame")]
		[TestCategory("Game")]
		public void AddGame_CorrectlySetsWinnerSlot()
		{
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(10);
			var p2 = new Mock<IPlayer>();
			p2.Setup(p => p.Id).Returns(20);
			Match m = new Match();
			m.AddPlayer(p1.Object);
			m.AddPlayer(p2.Object);
			m.SetMaxGames(3);

			m.AddGame(1, 0, PlayerSlot.Defender);
			m.AddGame(1, 0, PlayerSlot.Defender);
			Assert.AreEqual(PlayerSlot.Defender, m.WinnerSlot);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match AddGame")]
		[TestCategory("Game")]
		[ExpectedException(typeof(NotImplementedException))]
		public void AddGame_ThrowsExceptionIfScoreIsTied()
		{
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(10);
			var p2 = new Mock<IPlayer>();
			p2.Setup(p => p.Id).Returns(20);
			Match m = new Match();
			m.AddPlayer(p1.Object);
			m.AddPlayer(p2.Object);

			m.AddGame(1, 1, PlayerSlot.unspecified);

			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match RemoveLastGame")]
		[TestCategory("Game")]
		[ExpectedException(typeof(GameNotFoundException))]
		public void RemoveLastGame_ThrowsNotFound_WhenMatchHasNoGames()
		{
			Match m = new Match();
			m.RemoveLastGame();

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match RemoveLastGame")]
		[TestCategory("Game")]
		public void RemoveLastGame_ReturnsRemovedGame()
		{
			var p1 = new Mock<IPlayer>();
			var p2 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(10);
			p2.Setup(p => p.Id).Returns(20);
			Match m = new Match();
			m.AddPlayer(p1.Object);
			m.AddPlayer(p2.Object);

			m.AddGame(1, 0, PlayerSlot.Defender);
			var g = m.RemoveLastGame();
			Assert.IsInstanceOfType(g, typeof(GameModel));
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match RemoveLastGame")]
		[TestCategory("Game")]
		public void RemoveLastGame_CorrectlySetsMatchIsNotFinished()
		{
			var p1 = new Mock<IPlayer>();
			var p2 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(10);
			p2.Setup(p => p.Id).Returns(20);
			Match m = new Match();
			m.AddPlayer(p1.Object);
			m.AddPlayer(p2.Object);

			m.AddGame(1, 0, PlayerSlot.Defender);
			m.RemoveLastGame();
			Assert.IsFalse(m.IsFinished);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match RemoveLastGame")]
		[TestCategory("Game")]
		public void RemoveLastGame_ResetsWinnerSlot()
		{
			var p1 = new Mock<IPlayer>();
			var p2 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(10);
			p2.Setup(p => p.Id).Returns(20);
			Match m = new Match();
			m.AddPlayer(p1.Object);
			m.AddPlayer(p2.Object);

			m.AddGame(1, 0, PlayerSlot.Defender);
			m.RemoveLastGame();
			Assert.AreEqual(PlayerSlot.unspecified, m.WinnerSlot);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match RemoveLastGame")]
		[TestCategory("Game")]
		public void RemoveLastGame_OnlyRemovesOneGame()
		{
			var p1 = new Mock<IPlayer>();
			var p2 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(10);
			p2.Setup(p => p.Id).Returns(20);
			Match m = new Match();
			m.AddPlayer(p1.Object);
			m.AddPlayer(p2.Object);
			m.SetMaxGames(3);

			m.AddGame(1, 0, PlayerSlot.Defender);
			m.AddGame(0, 2, PlayerSlot.Challenger);
			m.AddGame(5, 4, PlayerSlot.Defender);
			m.RemoveLastGame();
			Assert.AreEqual(2, m.Games.Count);
		}

		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match ResetScore")]
		public void ResetScore_ResetsToZero()
		{
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(10);
			var p2 = new Mock<IPlayer>();
			p2.Setup(p => p.Id).Returns(20);

			Match m = new Match();
			m.SetMaxGames(3);
			m.AddPlayer(p1.Object);
			m.AddPlayer(p2.Object);
			m.AddGame(1, 0, PlayerSlot.Defender);
			m.AddGame(25, 90, PlayerSlot.Challenger);
			m.ResetScore();

			Assert.AreEqual(0, m.Score[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match ResetScore")]
		public void ResetScore_ClearsGamesList()
		{
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(10);
			var p2 = new Mock<IPlayer>();
			p2.Setup(p => p.Id).Returns(20);

			Match m = new Match();
			m.SetMaxGames(3);
			m.AddPlayer(p1.Object);
			m.AddPlayer(p2.Object);
			m.AddGame(1, 0, PlayerSlot.Defender);
			m.AddGame(25, 90, PlayerSlot.Challenger);
			m.ResetScore();

			Assert.AreEqual(0, m.Games.Count);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match ResetScore")]
		public void ResetScore_SetsMatchUnfinished()
		{
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(10);
			var p2 = new Mock<IPlayer>();
			p2.Setup(p => p.Id).Returns(20);

			Match m = new Match();
			m.SetMaxGames(3);
			m.AddPlayer(p1.Object);
			m.AddPlayer(p2.Object);
			m.AddGame(1, 0, PlayerSlot.Defender);
			m.AddGame(90, 89, PlayerSlot.Defender);
			m.ResetScore();

			Assert.IsFalse(m.IsFinished);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match ResetScore")]
		public void ResetScore_ResetsWinnerIndex()
		{
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(10);
			var p2 = new Mock<IPlayer>();
			p2.Setup(p => p.Id).Returns(20);

			Match m = new Match();
			m.SetMaxGames(3);
			m.AddPlayer(p1.Object);
			m.AddPlayer(p2.Object);
			m.AddGame(1, 0, PlayerSlot.Defender);
			m.AddGame(90, 89, PlayerSlot.Defender);
			m.ResetScore();

			Assert.AreEqual(PlayerSlot.unspecified, m.WinnerSlot);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match RemovePlayer")]
		[TestCategory("Match ResetScore")]
		public void RemovePlayer_ResetsScore()
		{
			int playerId = 10;
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(playerId);
			var p2 = new Mock<IPlayer>();
			p2.Setup(p => p.Id).Returns(20);

			Match m = new Match();
			m.AddPlayer(p1.Object, PlayerSlot.Defender);
			m.AddPlayer(p2.Object, PlayerSlot.Challenger);
			m.AddGame(1, 0, PlayerSlot.Defender);
			m.RemovePlayer(playerId);

			Assert.AreEqual(0, m.Score[(int)PlayerSlot.Defender]);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match RemovePlayer")]
		[TestCategory("Match ResetScore")]
		public void RemovePlayer_ResetScore_ClearsGamesList()
		{
			int playerId = 10;
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(playerId);
			var p2 = new Mock<IPlayer>();
			p2.Setup(p => p.Id).Returns(20);

			Match m = new Match();
			m.AddPlayer(p1.Object, PlayerSlot.Defender);
			m.AddPlayer(p2.Object, PlayerSlot.Challenger);
			m.AddGame(1, 0, PlayerSlot.Defender);
			m.RemovePlayer(playerId);

			Assert.AreEqual(0, m.Games.Count);
		}
		#endregion

		#region Mutators
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Mutators")]
		[ExpectedException(typeof(ScoreException))]
		public void SetMaxGames_ThrowsScoreException_WithZeroInput()
		{
			Match m = new Match();
			m.SetMaxGames(0);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Mutators")]
		[ExpectedException(typeof(InactiveMatchException))]
		public void SetWinsNeeded_ThrowsInactive_WhenCalledOnFinishedMatch()
		{
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(10);
			var p2 = new Mock<IPlayer>();
			p2.Setup(p => p.Id).Returns(20);

			Match m = new Match();
			m.AddPlayer(p1.Object);
			m.AddPlayer(p2.Object);
			m.AddGame(1, 0, PlayerSlot.Defender);
			m.SetMaxGames(5);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Mutators")]
		[ExpectedException(typeof(AlreadyAssignedException))]
		public void SetRoundIndex_ThrowsAlreadyAssigned_WhenCalledTwice()
		{
			Match m = new Match();
			m.SetRoundIndex(2);
			m.SetRoundIndex(3);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Mutators")]
		[ExpectedException(typeof(AlreadyAssignedException))]
		public void SetMatchIndex_ThrowsAlreadyAssigned_WhenCalledTwice()
		{
			Match m = new Match();
			m.SetMatchIndex(2);
			m.SetMatchIndex(3);

			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Mutators")]
		public void SetMatchNumber_Sets()
		{
			int n = 5;
			Match m = new Match();
			m.SetMatchNumber(n);

			Assert.AreEqual(n, m.MatchNumber);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Mutators")]
		[ExpectedException(typeof(AlreadyAssignedException))]
		public void SetMatchNumber_ThrowsAlreadyAssigned_WhenCalledTwice()
		{
			Match m = new Match();
			m.SetMatchNumber(1);
			m.SetMatchNumber(2);

			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Mutators")]
		public void AddPreviousMatchNumber_Adds()
		{
			int i = 14;
			Match m = new Match();
			m.AddPreviousMatchNumber(i);
			m.AddPreviousMatchNumber(2);

			Assert.IsTrue(m.PreviousMatchNumbers[0] == i
				|| m.PreviousMatchNumbers[1] == i);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Mutators")]
		[ExpectedException(typeof(AlreadyAssignedException))]
		public void AddPreviousMatchNumber_ThrowsAlreadyAssigned_AfterMoreThanTwoCalls()
		{
			Match m = new Match();
			m.AddPreviousMatchNumber(1);
			m.AddPreviousMatchNumber(2);
			m.AddPreviousMatchNumber(3);

			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Mutators")]
		public void SetNextMatchNumber_Sets()
		{
			int n = 5;
			Match m = new Match();
			m.SetNextMatchNumber(n);

			Assert.AreEqual(n, m.NextMatchNumber);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Mutators")]
		[ExpectedException(typeof(AlreadyAssignedException))]
		public void SetNextMatchNumber_ThrowsAlreadyAssigned_WhenCalledTwice()
		{
			Match m = new Match();
			m.SetNextMatchNumber(1);
			m.SetNextMatchNumber(2);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Mutators")]
		public void SetNextLoserMatchNumber_Sets()
		{
			int n = 7;
			Match m = new Match();
			m.SetNextLoserMatchNumber(n);

			Assert.AreEqual(n, m.NextLoserMatchNumber);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Mutators")]
		[ExpectedException(typeof(AlreadyAssignedException))]
		public void SetNextLoserMatchNumber_ThrowsAlreadyAssigned_WhenCalledTwice()
		{
			Match m = new Match();
			m.SetNextLoserMatchNumber(1);
			m.SetNextLoserMatchNumber(2);

			Assert.AreEqual(1, 2);
		}
		#endregion
	}
}
