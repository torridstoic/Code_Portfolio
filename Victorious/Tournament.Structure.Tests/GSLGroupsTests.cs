using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Moq;
using System.Linq;

using DatabaseLib;

namespace Tournament.Structure.Tests
{
#if false
	[TestClass]
	public class GSLGroupsTests
	{
		#region Bracket Creation
		[TestMethod]
		[TestCategory("GSLGroups")]
		[TestCategory("GSL Ctor")]
		public void GSLCtor_Constructs()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new GSLGroups(pList, 2);

			Assert.IsInstanceOfType(b, typeof(GSLGroups));
		}
		[TestMethod]
		[TestCategory("GSLGroups")]
		[TestCategory("GSL Ctor")]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void GSLCtor_ThrowsOutOfRange_WithTooFewGroups()
		{
			IBracket b = new GSLGroups(new List<IPlayer>(), 1);
			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("GSLGroups")]
		[TestCategory("GSL Ctor")]
		[ExpectedException(typeof(ArgumentNullException))]
		public void GSLCtor_ThrowsNull_WithNullParameter()
		{
			IBracket b = new GSLGroups(null, 2);
			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("GSLGroups")]
		[TestCategory("GSL Ctor")]
		[ExpectedException(typeof(BracketException))]
		public void GSLCtor_ThrowsBracketExcep_WithNonStandardPlayerCount()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 6; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new GSLGroups(pList, 2);
		}

		[TestMethod]
		[TestCategory("GSLGroups")]
		[TestCategory("GSL CreateBracket")]
		[ExpectedException(typeof(BracketException))]
		public void GSLCreateBracket_ThrowsBracketExcep_WithNegativeGamesPerMatch()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new GSLGroups(pList, 2, 0);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("GSLGroups")]
		[TestCategory("GSL CreateBracket")]
		public void GSLCreateBracket_CreatesTenMatches_ForTwoGroupsOfFour()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new GSLGroups(pList, 2);

			Assert.AreEqual(10, b.NumberOfMatches);
		}
		[TestMethod]
		[TestCategory("GSLGroups")]
		[TestCategory("GSL CreateBracket")]
		public void GSLCreateBracket_CorrectlyAssignsNumberOfRounds()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new GSLGroups(pList, 2);

			Assert.AreEqual(2, b.NumberOfRounds);
		}
		[TestMethod]
		[TestCategory("GSLGroups")]
		[TestCategory("GSL CreateBracket")]
		public void GSLCreateBracket_GroupStageStoresAllEightPlayers()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new GSLGroups(pList, 2);

			Assert.AreEqual(pList.Count, b.NumberOfPlayers());
		}
		[TestMethod]
		[TestCategory("GSLGroups")]
		[TestCategory("GSL CreateBracket")]
		public void GSLCreateBracket_EachGroupTakesFourPlayers()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new GSLGroups(pList, 2);

			Assert.AreEqual(4, (b as IGroupStage).GetGroup(1).NumberOfPlayers());
		}
		#endregion

		#region Bracket Progression
		[TestMethod]
		[TestCategory("GSLGroups")]
		[TestCategory("GSL AddGame")]
		public void GSLAddGame_DoesNotTryToAdvancePlayersToGrandFinal()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new GSLGroups(pList, 2);

			for (int n = 1; n <= 3; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}
			Assert.AreEqual(PlayerSlot.Defender, b.GetMatch(3).WinnerSlot);
		}
		[TestMethod]
		[TestCategory("GSLGroups")]
		[TestCategory("GSL AddGame")]
		public void GSLAddGame_CorrectlyAddsGamesToAllRemainingMatches()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new GSLGroups(pList, 2);

			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}
			Assert.AreEqual(b.NumberOfPlayers(), b.Rankings.Count);
		}
		[TestMethod]
		[TestCategory("GSLGroups")]
		[TestCategory("GSL AddGame")]
		public void GSLAddGame_SetsGroupStageAsFinishedWhenAllMatchesAreDone()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new GSLGroups(pList, 2);

			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}
			Assert.IsTrue(b.IsFinished);
		}
		[TestMethod]
		[TestCategory("GSLGroups")]
		[TestCategory("GSL AddGame")]
		public void GSLAddGame_HasOneFirstPlaceRankerForEachGroup()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new GSLGroups(pList, 2);

			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}
			Assert.AreEqual((b as IGroupStage).NumberOfGroups,
				b.Rankings.Where(r => r.Rank == 1).ToList().Count);
		}
		#endregion
	}
#endif
}
