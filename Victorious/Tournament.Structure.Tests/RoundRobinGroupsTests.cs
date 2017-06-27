using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Moq;
using System.Linq;

using DatabaseLib;

namespace Tournament.Structure.Tests
{
	[TestClass]
	public class RoundRobinGroupsTests
	{
		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("RRG Ctor")]
		public void RRGCtor_Constructs()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinGroups(pList, 2);

			Assert.IsInstanceOfType(b, typeof(RoundRobinGroups));
		}
		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("RRG Ctor")]
		[ExpectedException(typeof(ArgumentNullException))]
		public void RRGCtor_ThrowsNullException()
		{
			IBracket b = new RoundRobinGroups(null, 2);

			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("RRG CreateBracket")]
		public void RRGCreateBracket_CreatesFourGroups()
		{
			int numGroups = 4;
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 32; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinGroups(pList, numGroups);

			Assert.AreEqual((4 * 7 * 4), b.NumberOfMatches);
		}
		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("RRG CreateBracket")]
		public void RRGCreateBracket_CreatesUnevenGroupsForOddPlayercounts()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 7; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinGroups(pList, 2);

			Assert.AreEqual((2 * 3 + 3), b.NumberOfMatches);
		}

		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("RRG Overloaded Accessors")]
		public void RRGGetRound_ReturnsMatchesFromAllGroups()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 32; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinGroups(pList, 4);

			Assert.AreEqual((4 * 4), b.GetRound(1).Count);
		}
		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("RRG Overloaded Accessors")]
		[ExpectedException(typeof(InvalidIndexException))]
		public void RRGGetRound_ThrowsInvalidIndex_WithNegativeRound()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinGroups(pList, 2);

			b.GetRound(-1);
			Assert.AreEqual(1, 2);
		}

		#region Bracket Progression
		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("RRG AddGame")]
		public void RRGAddGame_AddsWinToFirstGroup()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinGroups(pList, 2);

			b.AddGame(1, 0, 1, PlayerSlot.Challenger);
			Assert.AreEqual(1, b.GetMatch(1).Score[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("RRG AddGame")]
		public void RRGAddGame_AddsWinToAnyGroup()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinGroups(pList, 2);

			int matchNum = b.NumberOfMatches;
			b.AddGame(matchNum, 0, 1, PlayerSlot.Challenger);
			Assert.AreEqual(1, b.GetMatch(matchNum).Score[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("RRG AddGame")]
		public void RRGAddGame_CanFinishBracket()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinGroups(pList, 2);

			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}
			Assert.IsTrue(b.IsFinished);
		}

		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("RRG RemoveLastGame")]
		public void RRGRemoveLastGame_Removes()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinGroups(pList, 2);

			b.AddGame(1, 0, 1, PlayerSlot.Challenger);
			b.RemoveLastGame(1);
			Assert.AreEqual(0, b.GetMatch(1).Score[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("RRG RemoveLastGame")]
		public void RRGRemoveLastGame_RemovesFromAllGroups()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinGroups(pList, 2);

			int matchNum = b.NumberOfMatches;
			b.AddGame(matchNum, 0, 1, PlayerSlot.Challenger);
			b.RemoveLastGame(matchNum);
			Assert.AreEqual(0, b.GetMatch(matchNum).Score[(int)PlayerSlot.Challenger]);
		}

		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("RRG ResetMatchScore")]
		public void RRGResetMatchScore_Resets()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinGroups(pList, 2);

			b.AddGame(1, 0, 1, PlayerSlot.Challenger);
			b.ResetMatchScore(1);
			Assert.AreEqual(0, b.GetMatch(1).Score[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("RRG ResetMatchScore")]
		public void RRGResetMatchScore_ResetsFromAllGroups()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinGroups(pList, 2);

			int matchNum = b.NumberOfMatches;
			b.AddGame(matchNum, 0, 1, PlayerSlot.Challenger);
			b.ResetMatchScore(matchNum);
			Assert.AreEqual(0, b.GetMatch(matchNum).Score[(int)PlayerSlot.Challenger]);
		}

		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("RRG ResetMatches")]
		public void RRGResetMatches_FiresMatchesModifiedEventsForEveryMatch()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 12; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinGroups(pList, 3);
			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}

			int matchesModded = 0;
			b.MatchesModified += delegate (object sender, BracketEventArgs e)
			{
				matchesModded += e.UpdatedMatches.Count;
			};
			b.ResetMatches();
			Assert.AreEqual(b.NumberOfMatches, matchesModded);
		}

		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("CheckForTies")]
		public void RRGCheckForTies_ReturnsFalseIfNoTiesFound()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 10; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinGroups(pList, 5);
			b.SetMatchWinner(5, PlayerSlot.Defender);

			Assert.IsFalse(b.CheckForTies());
		}
		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("CheckForTies")]
		public void RRGCheckForTies_ReturnsTrueIfAnyTieIsFound()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 12; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinGroups(pList, 3);
			int matchesPerGroup = b.NumberOfMatches / 3;
			for (int n = 1; n <= matchesPerGroup; ++n)
			{
				b.SetMatchWinner(n, PlayerSlot.Defender);
			}

			Assert.IsTrue(b.CheckForTies());
		}

		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("GenerateTiebreakers")]
		public void RRGGenerateTiebreakers_ReturnsFalse_WhenNoGroupsAreFinished()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 12; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinGroups(pList, 3);

			Assert.IsFalse(b.GenerateTiebreakers());
		}
		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("GenerateTiebreakers")]
		public void RRGGenerateTiebreakers_ReturnsFalseWhenNoTies()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 10; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinGroups(pList, 5);
			b.SetMatchWinner(4, PlayerSlot.Challenger);

			Assert.IsFalse(b.GenerateTiebreakers());
		}
		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("GenerateTiebreakers")]
		[TestCategory("RoundAdded")]
		public void RRGGenerateTiebreakers_ThrowsRoundAddedEvent()
		{
			bool roundAdded = false;

			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 12; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinGroups(pList, 3);
			b.RoundAdded += delegate
			{
				roundAdded = true;
			};
			int matchesPerGroup = b.NumberOfMatches / (b as IGroupStage).NumberOfGroups;
			for (int n = 1; n <= matchesPerGroup; ++n)
			{
				b.SetMatchWinner(n, PlayerSlot.Defender);
			}

			b.GenerateTiebreakers();
			Assert.IsTrue(roundAdded);
		}
		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("GenerateTiebreakers")]
		[TestCategory("RoundAdded")]
		public void RRGGenerateTiebreakers_RoundAddedEventContainsAllNewMatchModels()
		{
			int newMatchModels = 0;

			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 12; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinGroups(pList, 3);
			int numMatches = b.NumberOfMatches;
			b.RoundAdded += delegate(object sender, BracketEventArgs e)
			{
				newMatchModels += e.UpdatedMatches.Count;
			};
			int matchesPerGroup = numMatches / (b as IGroupStage).NumberOfGroups;
			for (int n = 1; n <= matchesPerGroup; ++n)
			{
				b.SetMatchWinner(n, PlayerSlot.Defender);
			}

			b.GenerateTiebreakers();
			Assert.AreEqual(b.NumberOfMatches - numMatches, newMatchModels);
		}

		#endregion

		#region Models
		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("GetModel")]
		public void RRGGetModel_HasModelsOfAllPlayers()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 17; ++i)
			{
				IPlayer p = new Player(i + 1, "Player " + (i + 1).ToString());
				pList.Add(p);
			}
			IBracket b = new RoundRobinGroups(pList, 4);

			BracketModel bModel = b.GetModel(0);
			Assert.AreEqual(pList.Count, bModel.TournamentUsersBrackets.Count);
		}
		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("GetModel")]
		public void RRGGetModel_HasModelsOfAllMatches()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 17; ++i)
			{
				IPlayer p = new Player(i + 1, "Player " + (i + 1).ToString());
				pList.Add(p);
			}
			IBracket b = new RoundRobinGroups(pList, 4);

			BracketModel bModel = b.GetModel(0);
			Assert.AreEqual(b.NumberOfMatches, bModel.Matches.Count);
		}

		#endregion
	}
}

#if false
#region Models
		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("GetModel")]
		public void RRGGetModel_AddsGroupOffsetsToMatchNumbers()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 17; ++i)
			{
				IPlayer p = new Player(i + 1, "Player " + (i + 1).ToString());
				pList.Add(p);
			}
			IBracket b = new RoundRobinGroups(pList, 4);

			BracketModel bModel = b.GetModel();
			List<int> bModelMatchNumbers = bModel.Matches
				.Select(m => m.MatchNumber).ToList();
			List<int> distinctMatchNumbers = bModelMatchNumbers
				.Distinct().ToList();
			Assert.AreEqual(distinctMatchNumbers.Count, bModelMatchNumbers.Count);
		}

		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("GetModel")]
		[TestCategory("Model Constructor")]
		public void RRGModelCtor_SetsAndCreatesGroups()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 17; ++i)
			{
				IPlayer p = new Player(i + 1, "Player " + (i + 1).ToString());
				pList.Add(p);
			}
			IBracket b = new RoundRobinGroups(pList, 4);

			BracketModel bModel = b.GetModel();
			IBracket b2 = new RoundRobinGroups(bModel);
			Assert.AreEqual((b as IGroupStage).NumberOfGroups,
				(b2 as IGroupStage).NumberOfGroups);
		}
		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("GetModel")]
		[TestCategory("Model Constructor")]
		public void RRGModelCtor_RedividesPlayersIntoCorrectGroups()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 17; ++i)
			{
				IPlayer p = new Player(i + 1, "Player " + (i + 1).ToString());
				pList.Add(p);
			}
			IBracket b = new RoundRobinGroups(pList, 4);

			BracketModel bModel = b.GetModel();
			IBracket b2 = new RoundRobinGroups(bModel);
			bool samePlayersAssigned = true;

			for (int n = 1; n <= (b as IGroupStage).NumberOfGroups; ++n)
			{
				List<int> bPlayerIDs = (b as IGroupStage).GetGroup(n)
					.Players
					.Select(p => p.Id).ToList();
				List<int> b2PlayerIDs = (b2 as IGroupStage).GetGroup(n)
					.Players
					.Select(p => p.Id).ToList();
				if (bPlayerIDs.Count != b2PlayerIDs.Count)
				{
					samePlayersAssigned = false;
					break;
				}
				for (int i = 0; i < bPlayerIDs.Count; ++i)
				{
					if (bPlayerIDs[i] != b2PlayerIDs[i])
					{
						samePlayersAssigned = false;
						break;
					}
				}
				//if (bPlayerIDs.Except(b2PlayerIDs).ToList().Count > 0)
				//{
				//	samePlayersAssigned = false;
				//	break;
				//}
			}

			Assert.IsTrue(samePlayersAssigned);
		}
		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("GetModel")]
		[TestCategory("Model Constructor")]
		public void RRGModelCtor_ReassignsAllMatches_AndMatchNumbers()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 17; ++i)
			{
				IPlayer p = new Player(i + 1, "Player " + (i + 1).ToString());
				pList.Add(p);
			}
			IBracket b = new RoundRobinGroups(pList, 4);

			BracketModel bModel = b.GetModel();
			IBracket b2 = new RoundRobinGroups(bModel);

			// Test that all IMatches are fetchable from the main groupstage:
			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				IMatch match = b.GetMatch(n);
			}
			// Test that all IMatches are fetchable from each group:
			for (int n = 1; n <= (b as IGroupStage).NumberOfGroups; ++n)
			{
				IBracket group = (b as IGroupStage).GetGroup(n);
				for (int m = 1; m <= group.NumberOfMatches; ++m)
				{
					IMatch match = group.GetMatch(m);
				}
			}

			// Test that all old matches are recreated
			Assert.AreEqual(b.NumberOfMatches, b2.NumberOfMatches);
		}

		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("GetModel")]
		[TestCategory("Model Constructor")]
		public void RRGModelCtor_SetsMatchScores()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 17; ++i)
			{
				IPlayer p = new Player(i + 1, "Player " + (i + 1).ToString());
				pList.Add(p);
			}
			IBracket b = new RoundRobinGroups(pList, 4);
			for (int n = 1; n <= b.NumberOfMatches; n += 2)
			{
				// Add Games to all ODD matches:
				b.AddGame(n, 15, 1, PlayerSlot.Defender);
			}

			BracketModel bModel = b.GetModel();
			IBracket b2 = new RoundRobinBracket(bModel);

			bool allOddMatchesHaveGames = true;
			for (int n = 1; n <= b2.NumberOfMatches; ++n)
			{
				IMatch match = b2.GetMatch(n);
				if ((n % 2 == 0 && match.Games.Count > 0) ||
					(n % 2 > 0 && match.Games.Count != 1))
				{
					allOddMatchesHaveGames = false;
					break;
				}
			}
			Assert.IsTrue(allOddMatchesHaveGames);
		}
		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("GetModel")]
		[TestCategory("Model Constructor")]
		public void RRGModelCtor_CreatesAModifiableBracket()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 17; ++i)
			{
				IPlayer p = new Player(i + 1, "Player " + (i + 1).ToString());
				pList.Add(p);
			}
			IBracket b = new RoundRobinGroups(pList, 4);
			for (int n = 1; n <= b.NumberOfMatches; n += 2)
			{
				// Add Games to all ODD matches:
				b.AddGame(n, 15, 1, PlayerSlot.Defender);
			}

			BracketModel bModel = b.GetModel();
			IBracket b2 = new RoundRobinBracket(bModel);

			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				if (!(b.GetMatch(n).IsFinished))
				{
					b.SetMatchWinner(n, PlayerSlot.Challenger);
				}
			}
			Assert.IsTrue(b.IsFinished);
		}
		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("GetModel")]
		[TestCategory("Model Constructor")]
		public void RRGModelCtor_CorrectlyRegeneratesMatchNumbers()
		{
			int matchNum = 10;
			int modifiedMatchNum = -1;

			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				IPlayer p = new Player(i + 1, "Player " + (i + 1).ToString());
				pList.Add(p);
			}
			IBracket b = new RoundRobinGroups(pList, 2);
			b.MatchesModified += delegate (object sender, BracketEventArgs e)
			{
				modifiedMatchNum = e.UpdatedMatches.First().MatchNumber;
			};
			b.AddGame(matchNum, 1, 0, PlayerSlot.Defender);
			//Assert.AreEqual(matchNum, modifiedMatchNum);

			BracketModel bModel = b.GetModel();
			//foreach (TournamentUsersBracketModel userModel in bModel.TournamentUsersBrackets)
			//{
			//	userModel.TournamentUser = new TournamentUserModel();
			//	userModel.TournamentUser.AccountID = userModel.TournamentUserID;
			//	userModel.TournamentUser.TournamentUserID = userModel.TournamentUserID;
			//	userModel.TournamentUser.Name = "Player " + (userModel.TournamentUserID).ToString();
			//	userModel.TournamentUser.TournamentID = -1;
			//}
			IBracket b2 = new RoundRobinGroups(bModel);

			b.AddGame(++matchNum, 1, 0, PlayerSlot.Defender);
			Assert.AreEqual(matchNum, modifiedMatchNum);
		}
		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("GetModel")]
		[TestCategory("Model Constructor")]
		public void RRGModelCtor_LoadsAFinishedGroupStage()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 17; ++i)
			{
				IPlayer p = new Player(i + 1, "Player " + (i + 1).ToString());
				pList.Add(p);
			}
			IBracket b = new RoundRobinGroups(pList, 4);
			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.AddGame(n, 15, 1, PlayerSlot.Defender);
			}

			BracketModel bModel = b.GetModel();
			IBracket b2 = new RoundRobinBracket(bModel);
			Assert.IsTrue(b2.IsFinished);
		}

		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("RRG ResetMatches")]
		public void RRGModelCtor_CorrectlyRelaysGroupEvents()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 12; ++i)
			{
				IPlayer p = new Player(i + 1, (i + 1).ToString());
				pList.Add(p);
			}
			IBracket b = new RoundRobinGroups(pList, 3);
			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}
			BracketModel model = b.GetModel();

			IBracket b2 = new RoundRobinGroups(model);
			int eventsFired = 0;
			b2.MatchesModified += delegate
			{
				++eventsFired;
			};
			b2.ResetMatches();
			Assert.AreEqual((b2 as IGroupStage).NumberOfGroups, eventsFired);
		}
}
#endregion
#endif
