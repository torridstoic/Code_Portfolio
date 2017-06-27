using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Moq;

namespace Tournament.Structure.Tests
{
	[TestClass]
	public class BracketTests
	{
		#region Player Methods
		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket Accessors")]
		public void NumberOfPlayers_GetsCorrectNumber()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 2; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			Assert.AreEqual(2, b.NumberOfPlayers());
		}
		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket GetPlayerSeed")]
		public void GetPlayerSeed_ReturnsCorrectSeedValue()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i * 10);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			Assert.AreEqual(3, b.GetPlayerSeed(30));
		}
		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket GetPlayerSeed")]
		[ExpectedException(typeof(PlayerNotFoundException))]
		public void GetPlayerSeed_ThrowsNotFoundWithBadIDInput()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i * 10);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			b.GetPlayerSeed(11);

			Assert.AreEqual(3, 2);
		}
		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket RandomizeSeeds")]
		public void RandomizeSeeds_Randomizes_ThisIsJustForDebugging()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i * 10);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			b.RandomizeSeeds();

			Assert.AreEqual(2, 2);
		}

		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket SetNewPlayerlist")]
		public void SetNewPlayerlist_ReplacesOldPlayerlistWithNew()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 12; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList.Where(p => p.Id <= 8).ToList());

			b.SetNewPlayerlist(pList.Where(p => p.Id > 8).ToList());
			Assert.AreEqual(4, b.Players.Count);
		}
#if false
		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket SetNewPlayerlist")]
		[ExpectedException(typeof(ArgumentNullException))]
		public void SetNewPlayerlist_ThrowsNullReferenceWithNullParam()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.SetNewPlayerlist(null);
			Assert.AreEqual(1, 2);
		}
#endif

		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket AddPlayer")]
		public void AddPlayer_Adds()
		{
			Mock<IPlayer> m = new Mock<IPlayer>();
			m.Setup(p => p.Id).Returns(10);
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 2; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			b.AddPlayer(m.Object);

			Assert.IsTrue(b.Players.Contains(m.Object));
		}
		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket AddPlayer")]
		[ExpectedException(typeof(DuplicateObjectException))]
		public void AddPlayer_ThrowsDuplicate_OnTryingToAddTwice()
		{
			IPlayer ip = new Mock<IPlayer>().Object;
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 2; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.AddPlayer(ip);
			b.AddPlayer(ip);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket AddPlayer")]
		[ExpectedException(typeof(ArgumentNullException))]
		public void AddPlayer_ThrowsArgNullExcept_OnNullParam()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 2; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.AddPlayer(null);
			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket AddPlayer")]
		[TestCategory("Bracket ResetBracket")]
		public void AddPlayer_ResetsBracketMatches()
		{
			IPlayer ip = new Mock<IPlayer>().Object;
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 2; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			b.AddPlayer(ip);

			Assert.AreEqual(0, b.NumberOfMatches);
		}

		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket ReplacePlayer")]
		public void ReplacePlayer_Replaces()
		{
			IPlayer ip = new Mock<IPlayer>().Object;
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			int pIndex = 2;
			b.ReplacePlayer(ip, pIndex);

			Assert.AreEqual(ip, b.Players[pIndex]);
		}
		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket ReplacePlayer")]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ReplacePlayer_ThrowsArgNullExcept_OnNullParam()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			b.ReplacePlayer(null, 2);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket ReplacePlayer")]
		[ExpectedException(typeof(InvalidIndexException))]
		public void ReplacePlayer_ThrowsInvalidIndex_WithBadIndexParam()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.ReplacePlayer(new Mock<IPlayer>().Object, 6);
			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket ReplacePlayer")]
		[TestCategory("DoubleElimBracket")]
		public void ReplacePlayer_ReplacesPlayerInAllMatches()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			for (int n = 1; n < b.NumberOfMatches; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}

			Mock<IPlayer> m = new Mock<IPlayer>();
			m.Setup(p => p.Id).Returns(9);
			b.ReplacePlayer(m.Object, 0);

			Assert.AreEqual(b.Players[0].Id, b.GrandFinal.Players[(int)PlayerSlot.Defender].Id);
		}

		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket SwapPlayers")]
		public void SwapPlayers_Swaps()
		{
			int i1 = 1;
			int i2 = 6;

			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 8; ++i)
			{
				var moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			int playerId = b.Players[i1].Id;
			b.SwapPlayers(i1, i2);
			Assert.AreEqual(playerId, b.Players[i2].Id);
		}
		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket SwapPlayers")]
		[ExpectedException(typeof(InvalidIndexException))]
		public void SwapPlayers_ThrowsInvalidIndexWithBadParameter()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 8; ++i)
			{
				var moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.SwapPlayers(0, -2);
			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket ReinsertPlayer")]
		public void ReinsertPlayer_MovesPlayerUpwardToDestinationSlot()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 8; ++i)
			{
				var moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			int playerId = b.Players[6].Id;
			b.ReinsertPlayer(6, 2);
			Assert.AreEqual(playerId, b.Players[2].Id);
		}
		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket ReinsertPlayer")]
		public void ReinsertPlayer_ShiftsAffectedPlayersDownward()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 8; ++i)
			{
				var moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			int playerId = b.Players[4].Id;
			b.ReinsertPlayer(6, 2);
			Assert.AreEqual(playerId, b.Players[5].Id);
		}
		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket ReinsertPlayer")]
		public void ReinsertPlayer_MovesPlayerDownwardToDestinationSlot()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 8; ++i)
			{
				var moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			int playerId = b.Players[2].Id;
			b.ReinsertPlayer(2, 6);
			Assert.AreEqual(playerId, b.Players[6].Id);
		}
		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket ReinsertPlayer")]
		public void ReinsertPlayer_ShiftsAffectedPlayersUpward()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 8; ++i)
			{
				var moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			int playerId = b.Players[5].Id;
			b.ReinsertPlayer(2, 6);
			Assert.AreEqual(playerId, b.Players[4].Id);
		}
		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket ReinsertPlayer")]
		[ExpectedException(typeof(InvalidIndexException))]
		public void ReinsertPlayer_ThrowsInvalidIndex_WithBadIndexInput()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 8; ++i)
			{
				var moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.ReinsertPlayer(2, -2);
			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket RemovePlayer")]
		public void RemovePlayer_Removes()
		{
			var ip = new Mock<IPlayer>();
			ip.Setup(p => p.Id).Returns(10);
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.AddPlayer(ip.Object);
			b.RemovePlayer(ip.Object.Id);

			Assert.IsFalse(b.Players.Contains(ip.Object));
		}
		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket RemovePlayer")]
		[ExpectedException(typeof(PlayerNotFoundException))]
		public void RemovePlayer_ThrowsNotFound_IfPlayerIsntPresent()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			b.RemovePlayer(-5);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket RemovePlayer")]
		[TestCategory("Bracket ResetBracket")]
		public void RemovePlayer_ClearsBracketMatches()
		{
			IPlayer ip = new Mock<IPlayer>().Object;
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			b.AddPlayer(ip);
			b.CreateBracket();
			b.RemovePlayer(ip.Id);

			Assert.AreEqual(0, b.NumberOfMatches);
		}

		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket ResetPlayers")]
		public void ResetPlayers_ClearsPlayerList()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 4; ++i)
			{
				var moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.ResetPlayers();
			Assert.AreEqual(0, b.Players.Count);
		}
		#endregion

		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket Accessors")]
		public void NumberOfRounds_ReturnsNumber()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			Assert.AreEqual(2, b.NumberOfRounds);
		}

		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket GetRound")]
		public void GetRound_ReturnsCorrectMatchList()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			Assert.AreEqual(4, b.GetRound(1).Count);
		}
		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket GetRound")]
		[ExpectedException(typeof(InvalidIndexException))]
		public void GetRound_ThrowsInvalidIndex_WithBadParam()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			var l = b.GetRound(-1);

			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket GetMatch")]
		public void GetMatch_ReturnsCorrectMatch()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			IMatch m = b.GetMatch(2);

			Assert.AreEqual(1, m.Players[(int)PlayerSlot.Defender].Id);
		}
		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket GetMatch")]
		[ExpectedException(typeof(InvalidIndexException))]
		public void GetMatch_ThrowsInvalidIndex_WithNegativeParam()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			var m = b.GetMatch(-1);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket GetMatch")]
		[ExpectedException(typeof(MatchNotFoundException))]
		public void GetMatch_ThrowsNotFound_IfMatchNumberNotFound()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			var m = b.GetMatch(15);

			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("SetMaxGamesForWholeRound")]
		public void SMGFWR_UpdatesSingleElimBrackets()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 16; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			int games = 5;
			b.SetMaxGamesForWholeRound(2, games);
			Assert.AreEqual(games, b.GetRound(2)[1].MaxGames);
		}
		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("SetMaxGamesForWholeRound")]
		[ExpectedException(typeof(ScoreException))]
		public void SMGFWR_ThrowsScoreExcept_IfInputIsNegative()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.SetMaxGamesForWholeRound(1, 0);
			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("SetMaxGamesForWholeRound")]
		[ExpectedException(typeof(InactiveMatchException))]
		public void SMGFWR_ThrowsInactiveMatch_IfAffectedMatchIsFinished()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.AddGame(2, 1, 0, PlayerSlot.Defender);
			b.SetMaxGamesForWholeRound(1, 3);
			Assert.AreEqual(1, 2);
		}
	}
}
