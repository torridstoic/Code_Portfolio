using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Moq;
using System.Linq;

using DatabaseLib;

namespace Tournament.Structure.Tests
{
	[TestClass]
	public class StepladderBracketTests
	{
		#region Bracket Creation
		[TestMethod]
		[TestCategory("StepladderBracket")]
		[TestCategory("SB Ctor")]
		public void SBCtor_Constructs()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 9; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new StepladderBracket(pList);

			Assert.IsInstanceOfType(b, typeof(StepladderBracket));
		}

		[TestMethod]
		[TestCategory("StepladderBracket")]
		[TestCategory("CreateBracket")]
		public void SEBCreateBracket_MakesCorrectNumberOfRounds()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 9; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new StepladderBracket(pList);

			Assert.AreEqual(b.NumberOfPlayers() - 1, b.NumberOfRounds);
		}
		[TestMethod]
		[TestCategory("StepladderBracket")]
		[TestCategory("CreateBracket")]
		public void SEBCreateBracket_MakesCorrectNumberOfMatches()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 10; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new StepladderBracket(pList);

			Assert.AreEqual(b.NumberOfPlayers() - 1, b.NumberOfMatches);
		}
		#endregion

		#region Bracket Progression
		[TestMethod]
		[TestCategory("StepladderBracket")]
		[TestCategory("SetMatchWinner")]
		public void SEBSetMatchWinner_WinnersCorrectlyAdvance_AndBracketFinishes()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 9; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new StepladderBracket(pList);

			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.SetMatchWinner(n, PlayerSlot.Defender);
			}
			Assert.IsTrue(b.IsFinished);
		}
		[TestMethod]
		[TestCategory("StepladderBracket")]
		[TestCategory("ResetMatchScore")]
		public void SEBResetMatchScore_CorrectlyUpdatesFutureMatches()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 9; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new StepladderBracket(pList);
			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.SetMatchWinner(n, PlayerSlot.Defender);
			}

			b.ResetMatchScore(1);
			Assert.IsFalse(b.GetMatch(b.NumberOfMatches).IsFinished);
		}

		[TestMethod]
		[TestCategory("StepladderBracket")]
		[TestCategory("SetMatchWinner")]
		[TestCategory("Rankings")]
		public void SEBSetMatchWinner_AddsLoserToRankingsWithCorrectRank()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 9; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new StepladderBracket(pList);

			b.SetMatchWinner(1, PlayerSlot.Defender);
			Assert.AreEqual(b.NumberOfPlayers(), b.Rankings.First().Rank);
		}
		[TestMethod]
		[TestCategory("StepladderBracket")]
		[TestCategory("SetMatchWinner")]
		[TestCategory("Rankings")]
		public void SEBRankings_DoesNotDuplicateRankValues()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 9; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new StepladderBracket(pList);

			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.SetMatchWinner(n, PlayerSlot.Defender);
			}
			Assert.AreEqual(b.Rankings.Select(r => r.Rank).Distinct().Count(),
				b.Rankings.Count);
		}
		#endregion
	}
}
