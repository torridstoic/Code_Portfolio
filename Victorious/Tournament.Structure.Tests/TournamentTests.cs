using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

using Tournament.Structure;

namespace Tournament.Structure.Tests
{
	[TestClass]
	public class TournamentTests
	{
#if false
		[TestMethod]
		[TestCategory("Tournament")]
		[TestCategory("Tournament Constructor")]
		public void DefaultCtor_Constructs()
		{
			Tournament t = new Tournament();

			Assert.AreEqual(false, t.IsPublic);
		}
		[TestMethod]
		[TestCategory("Tournament")]
		[TestCategory("Tournament Constructor")]
		public void FullCtor_Constructs()
		{
			string str = "title";
			List<IPlayer> pList = new List<IPlayer>();
			List<IBracket> bList = new List<IBracket>();
			Tournament t = new Tournament(str, pList, bList, 1.0f, true);

			Assert.AreEqual(str, t.Title);
		}
#endif
	}
}
