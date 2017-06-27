using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Moq;
using System.Linq;

using DatabaseLib;

namespace Tournament.Structure.Tests
{
	[TestClass]
	public class SwissBracketTests
	{
		#region Bracket Creation
		[TestMethod]
		[TestCategory("SwissBracket")]
		[TestCategory("Swiss Ctor")]
		public void SwissCtor_Constructs()
		{
			IBracket b = new SwissBracket();

			Assert.IsInstanceOfType(b, typeof(SwissBracket));
		}
		[TestMethod]
		[TestCategory("SwissBracket")]
		[TestCategory("Swiss Ctor")]
		public void SwissCtor_CreatesNoMatches_WithLessThanTwoPlayers()
		{
			IBracket b = new SwissBracket();

			Assert.AreEqual(0, b.NumberOfMatches);
		}
		[TestMethod]
		[TestCategory("SwissBracket")]
		[TestCategory("Swiss Ctor")]
		[ExpectedException(typeof(ArgumentNullException))]
		public void SwissCtor_ThrowsNull_WithNullParameter()
		{
			IBracket b = new SwissBracket(null, PairingMethod.Slide, 1);

			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("SwissBracket")]
		[TestCategory("Swiss CreateBracket")]
		public void SwissCreateBracket_GeneratesFirstRound()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SwissBracket(pList);

			Assert.AreEqual(pList.Count / 2, b.GetRound(1).Count);
		}
		[TestMethod]
		[TestCategory("SwissBracket")]
		[TestCategory("Swiss CreateBracket")]
		public void SwissCreateBracket_GeneratesDefaultNumberOfRounds()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SwissBracket(pList);

			Assert.AreEqual(3, b.NumberOfRounds);
		}
		[TestMethod]
		[TestCategory("SwissBracket")]
		[TestCategory("Swiss CreateBracket")]
		public void SwissCreateBracket_AddsPlayersOnlyToFirstRound()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SwissBracket(pList);

			List<int> roundsWithPlayers = new List<int>();
			for (int r = 1; r < b.NumberOfRounds; ++r)
			{
				if (b.GetRound(r).Any(m => m.Players.Contains(null)))
				{
					break;
				}
				roundsWithPlayers.Add(r);
			}
			Assert.AreEqual(1, roundsWithPlayers.Count);
		}
		[TestMethod]
		[TestCategory("SwissBracket")]
		[TestCategory("Swiss CreateBracket")]
		[TestCategory("AddSwissRound")]
		public void SwissCreateBracket_UsesStandardSlideMatchups()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SwissBracket(pList);

			// Match(1) is Player[0] vs Player[4]
			Assert.AreEqual(b.Players[b.NumberOfPlayers() / 2].Id,
				b.GetMatch(1).Players[(int)PlayerSlot.Challenger].Id);
		}
		[TestMethod]
		[TestCategory("SwissBracket")]
		[TestCategory("Swiss CreateBracket")]
		[TestCategory("AddSwissRound")]
		public void SwissCreateBracket_GivesFirstRoundByeToTopSeed()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 9; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 3);
				pList.Add(moq.Object);
			}
			IBracket b = new SwissBracket(pList);

			// Match(1) is Player[second] vs Player[last]
			Assert.AreEqual(b.Players[1].Id,
				b.GetMatch(1).Players[(int)PlayerSlot.Defender].Id);
		}
		[TestMethod]
		[TestCategory("SwissBracket")]
		[TestCategory("Swiss CreateBracket")]
		[TestCategory("AddSwissRound")]
		public void SwissCreateBracket_ByePlayerGetsPointsForAWin()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 9; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SwissBracket(pList);

			Assert.IsTrue(b.Rankings[0].Wins > 0);
		}
		[TestMethod]
		[TestCategory("SwissBracket")]
		[TestCategory("Swiss CreateBracket")]
		public void SwissCreateBracket_LimitsMaxRounds()
		{
			int maxRounds = 15;

			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 7; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new SwissBracket(pList, PairingMethod.Slide, 1, maxRounds);

			Assert.IsTrue(b.MaxRounds < maxRounds);
		}
		#endregion

		#region Bracket Progression
		[TestMethod]
		[TestCategory("SwissBracket")]
		[TestCategory("AddSwissRound")]
		public void AddSwissRound_PopulatesSecondRoundWhenFirstFinishes()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SwissBracket(pList);
			List<IMatch> round1 = b.GetRound(1);

			foreach (IMatch match in round1)
			{
				b.SetMatchWinner(match.MatchNumber, PlayerSlot.Defender);
			}
			Assert.IsFalse(b.GetRound(2).Select(m => m.Players).Contains(null));
		}
		[TestMethod]
		[TestCategory("SwissBracket")]
		[TestCategory("AddSwissRound")]
		public void AddSwissRound_MatchesWinnersAgainstEachOther()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SwissBracket(pList);
			int firstRoundMatches = b.NumberOfMatches;
			for (int n = 1; n <= firstRoundMatches; ++n)
			{
				b.AddGame(n, 0, 1, PlayerSlot.Challenger);
			}

			Assert.IsTrue(b.GetMatch(1).Players[(int)(b.GetMatch(1).WinnerSlot)].Id == b.GetMatch(4).Players[(int)PlayerSlot.Defender].Id &&
				b.GetMatch(2).Players[(int)(b.GetMatch(2).WinnerSlot)].Id == b.GetMatch(4).Players[(int)PlayerSlot.Challenger].Id);
		}
		[TestMethod]
		[TestCategory("SwissBracket")]
		[TestCategory("AddSwissRound")]
		public void AddSwissRound_GivesSecondRoundByeToTopSeed_ButDoesntRepeatBye()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 5; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SwissBracket(pList);
			int firstRoundMatches = b.NumberOfMatches;
			for (int n = 1; n <= firstRoundMatches; ++n)
			{
				b.AddGame(n, 0, 1, PlayerSlot.Challenger);
			}

			Assert.AreNotEqual(b.Players[0].Id, b.Rankings[0].Id);
		}

		[TestMethod]
		[TestCategory("SwissBracket")]
		[TestCategory("ReplacePlayer")]
		public void SwissReplacePlayer_ReplacesPlayerIDinByesList()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 7; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new SwissBracket(pList);

			int pId = 50;
			Mock<IPlayer> player = new Mock<IPlayer>();
			player.Setup(p => p.Id).Returns(pId);
			b.ReplacePlayer(player.Object, 0);
			// 0-index had a first-round bye.

			List<IMatch> round1 = b.GetRound(1);
			foreach (IMatch match in round1)
			{
				b.SetMatchWinner(match.MatchNumber, PlayerSlot.Defender);
			}
			// Second round is now populated.
			// If new player was correctly added to the Byes list...
			// He will be in exactly 1 second-round match:
			List<IMatch> round2 = b.GetRound(2);
			Assert.AreEqual(1, round2.Where(m => m.Players.Select(p => p.Id).Contains(pId)).ToList().Count);
		}

		[TestMethod]
		[TestCategory("SwissBracket")]
		[TestCategory("UpdateGame")]
		public void SwissUpdateGame_DoesNotResetNextRound_IfMatchWinnerRemains()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 9; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new SwissBracket(pList);
			int matchesPerRound = b.GetRound(1).Count;
			for (int n = 1; n <= matchesPerRound; ++n)
			{
				b.AddGame(n, 10, 5, PlayerSlot.Defender);
			}
			b.SetMatchWinner(matchesPerRound + 1, PlayerSlot.Challenger);

			b.UpdateGame(1, 1, 15, 5, PlayerSlot.Defender);
			Assert.IsTrue(b.GetMatch(matchesPerRound + 1).IsFinished);
		}
		[TestMethod]
		[TestCategory("SwissBracket")]
		[TestCategory("UpdateGame")]
		public void SwissUpdateGame_ResetsNextRound_IfMatchWinnerChanges()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 9; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new SwissBracket(pList);
			int matchesPerRound = b.GetRound(1).Count;
			for (int n = 1; n <= matchesPerRound; ++n)
			{
				b.AddGame(n, 10, 5, PlayerSlot.Defender);
			}
			b.SetMatchWinner(matchesPerRound + 1, PlayerSlot.Challenger);

			b.UpdateGame(1, 1, 5, 15, PlayerSlot.Challenger);
			Assert.IsFalse(b.GetMatch(matchesPerRound + 1).IsFinished);
		}

		[TestMethod]
		[TestCategory("SwissBracket")]
		[TestCategory("RemoveLastGame")]
		public void SwissRemoveLastGame_ClearsAllFutureRounds()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 32; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new SwissBracket(pList);
			int matchesPerRound = b.GetRound(1).Count;
			for (int i = 0; i < 3; ++i)
			{
				for (int n = 1; n <= matchesPerRound; ++n)
				{
					b.AddGame(n + (i * matchesPerRound), 1, 0, PlayerSlot.Defender);
				}
			}

			b.RemoveLastGame(1);
			bool roundTwoHasPlayers = b.GetRound(2)
				.Any(m => !(m.Players.Contains(null)));
			Assert.IsFalse(roundTwoHasPlayers);
		}
		[TestMethod]
		[TestCategory("SwissBracket")]
		[TestCategory("RemoveGameNumber")]
		public void SwissRemoveGameNumber_ClearsAllFutureRounds()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 32; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new SwissBracket(pList, PairingMethod.Slide, 3);
			int matchesPerRound = b.GetRound(1).Count;
			for (int i = 0; i < 3; ++i)
			{
				for (int n = 1; n <= matchesPerRound; ++n)
				{
					b.AddGame(n + (i * matchesPerRound), 1, 0, PlayerSlot.Defender);
					b.AddGame(n + (i * matchesPerRound), 1, 0, PlayerSlot.Defender);
				}
			}

			b.RemoveGameNumber(1, 1);
			bool roundTwoHasPlayers = b.GetRound(2)
				.Any(m => !(m.Players.Contains(null)));
			Assert.IsFalse(roundTwoHasPlayers);
		}
		[TestMethod]
		[TestCategory("SwissBracket")]
		[TestCategory("ResetMatchScore")]
		public void SwissResetMatchScore_ClearsAllFutureRounds()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 32; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new SwissBracket(pList, PairingMethod.Slide, 3);
			int matchesPerRound = b.GetRound(1).Count;
			for (int i = 0; i < 3; ++i)
			{
				for (int n = 1; n <= matchesPerRound; ++n)
				{
					b.AddGame(n + (i * matchesPerRound), 1, 0, PlayerSlot.Defender);
					b.AddGame(n + (i * matchesPerRound), 1, 0, PlayerSlot.Defender);
				}
			}

			b.ResetMatchScore(1);
			bool roundTwoHasPlayers = b.GetRound(2)
				.Any(m => !(m.Players.Contains(null)));
			Assert.IsFalse(roundTwoHasPlayers);
		}
		[TestMethod]
		[TestCategory("SwissBracket")]
		[TestCategory("ResetMatchScore")]
		public void SwissResetMatchScore_DeletesFollowingRound_IfMatchWinIsReversed()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 32; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new SwissBracket(pList, PairingMethod.Slide, 3);
			int matchesPerRound = b.NumberOfMatches;
			for (int n = 1; n <= matchesPerRound; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}

			b.ResetMatchScore(matchesPerRound);
			Assert.AreEqual(matchesPerRound, b.NumberOfMatches);
		}
		[TestMethod]
		[TestCategory("SwissBracket")]
		[TestCategory("ResetMatchScore")]
		public void SwissResetMatchScore_RecalculatesRankings_IfMatchWinIsReversed()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 33; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new SwissBracket(pList, PairingMethod.Slide, 3);
			int matchesPerRound = b.GetRound(1).Count;
			for (int n = 1; n <= matchesPerRound; ++n)
			{
				b.SetMatchWinner(n, PlayerSlot.Challenger);
			}
			for (int n = 1; n < matchesPerRound; ++n)
			{
				b.AddGame(n + matchesPerRound, 25, 15, PlayerSlot.Defender);
				b.AddGame(n + matchesPerRound, 25, 15, PlayerSlot.Defender);
			}

			b.ResetMatchScore(matchesPerRound + 1);
			Assert.AreEqual(2, b.Rankings[0].Wins);
		}

		[TestMethod]
		[TestCategory("SwissBracket")]
		[TestCategory("ResetMatches")]
		public void SwissResetMatches_RemovesPlayersFromAllMatchesAfterFirstRound()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 32; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new SwissBracket(pList);
			int matchesPerRound = b.GetRound(1).Count;
			for (int i = 0; i < 3; ++i)
			{
				for (int n = 1; n <= matchesPerRound; ++n)
				{
					b.SetMatchWinner(n + (i * matchesPerRound), PlayerSlot.Defender);
				}
			}

			b.ResetMatches();
			int matchesWithPlayers = 0;
			for (int n = 1; n < b.NumberOfMatches; ++n)
			{
				if (!(b.GetMatch(n).Players.Contains(null)))
				{
					++matchesWithPlayers;
				}
			}
			Assert.AreEqual(matchesPerRound, matchesWithPlayers);
		}
		[TestMethod]
		[TestCategory("SwissBracket")]
		[TestCategory("ResetMatches")]
		public void SwissResetMatches_ReAddsAutowinToPlayerWithBye()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 9; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new SwissBracket(pList);
			int matchesPerRound = b.GetRound(1).Count;
			for (int i = 0; i < 2; ++i)
			{
				for (int n = 1; n <= matchesPerRound; ++n)
				{
					b.SetMatchWinner(n + (i * matchesPerRound), PlayerSlot.Defender);
				}
			}

			b.ResetMatches();
			int rIndex = b.Rankings.FindIndex(r => r.Id == b.Players[0].Id);
			Assert.AreEqual(1, b.Rankings[rIndex].Wins);
		}

		//[TestMethod]
		//public void Swiss_RoundCountTester()
		//{
		//	List<IPlayer> pList = new List<IPlayer>();
		//	for (int i = 0; i < 13; ++i)
		//	{
		//		Mock<IPlayer> moq = new Mock<IPlayer>();
		//		moq.Setup(p => p.Id).Returns(i + 5);
		//		pList.Add(moq.Object);
		//	}
		//	IBracket b = new SwissBracket(pList);

		//	for (int n = 1; n <= b.NumberOfMatches; ++n)
		//	{
		//		b.AddGame(n, 1, 0, PlayerSlot.Defender);
		//	}
		//	Assert.AreEqual(1, 1);
		//}
		#endregion

		#region Events
		[TestMethod]
		[TestCategory("SwissBracket")]
		[TestCategory("AddGame")]
		[TestCategory("MatchesModified")]
		public void SwissAddGame_FiresMatchesModifiedEvent_WithAllMatchesAffectedAndNextRound()
		{
			int matchesModified = 0;

			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 9; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new SwissBracket(pList);
			b.MatchesModified += delegate (object sender, BracketEventArgs e)
			{
				matchesModified += e.UpdatedMatches.Count;
			};

			int matchesPerRound = b.GetRound(1).Count;
			for (int i = 0; i < 2; ++i)
			{
				for (int n = 1; n <= matchesPerRound; ++n)
				{
					b.SetMatchWinner(n + (matchesPerRound * i), PlayerSlot.Challenger);
				}
			}
			Assert.AreEqual(2 * 2 * matchesPerRound, matchesModified);
		}

		[TestMethod]
		[TestCategory("SwissBracket")]
		[TestCategory("ResetMatchScore")]
		[TestCategory("MatchesModified")]
		public void SwissResetMatchScore_FiresMatchesModifiedEvent_WithResetRoundModels()
		{
			int matchesReset = 0;

			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 9; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new SwissBracket(pList);

			int matchesPerRound = b.GetRound(1).Count;
			for (int n = 1; n <= matchesPerRound; ++n)
			{
				b.SetMatchWinner(n, PlayerSlot.Challenger);
			}

			b.MatchesModified += delegate (object sender, BracketEventArgs e)
			{
				matchesReset += e.UpdatedMatches.Count;
			};
			b.ResetMatchScore(1);
			Assert.AreEqual(matchesPerRound + 1, matchesReset);
		}
		[TestMethod]
		[TestCategory("SwissBracket")]
		[TestCategory("ResetMatchScore")]
		[TestCategory("RoundDeleted")]
		public void SwissResetMatchScore_FiresMatchesModifiedEvent_WithOnlyOneMatchModel()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 9; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new SwissBracket(pList);
			int matchesPerRound = b.GetRound(1).Count;
			for (int n = 1; n < matchesPerRound; ++n)
			{
				b.SetMatchWinner(n, PlayerSlot.Challenger);
			}

			int matchesModified = 0;
			b.MatchesModified += delegate (object sender, BracketEventArgs e)
			{
				matchesModified += e.UpdatedMatches.Count;
			};
			b.ResetMatchScore(1);
			Assert.AreEqual(1, matchesModified);
		}
		[TestMethod]
		[TestCategory("SwissBracket")]
		[TestCategory("ResetMatchScore")]
		[TestCategory("GamesDeleted")]
		public void SwissResetMatchScore_FiresGamesDeletedEvents_WithAllGamesInClearedRound()
		{
			int deletedGames = 0;

			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 9; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new SwissBracket(pList);
			b.GamesDeleted += delegate (object sender, BracketEventArgs e)
			{
				deletedGames += e.DeletedGameIDs.Count;
			};
			b.MatchesModified += delegate (object sender, BracketEventArgs e)
			{
				deletedGames += e.DeletedGameIDs.Count;
			};
			int matchesPerRound = b.GetRound(1).Count;
			for (int n = 1; n <= (2 + matchesPerRound); ++n)
			{
				b.AddGame(n, 2, 1, PlayerSlot.Defender);
			}

			b.ResetMatchScore(1);
			// Delete games: 1 from first match, 2 from second round (removed)
			Assert.AreEqual(1 + 2, deletedGames);
		}
		#endregion
	}
}
