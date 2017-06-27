using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tournament.Structure.Tests
{
	[TestClass]
	public class PlayerTests
	{
		[TestMethod]
		[TestCategory("Player")]
		[TestCategory("Player Constructor")]
		public void PlayerDefaultCtor_Constructs()
		{
			IPlayer p = new Player();

			Assert.IsInstanceOfType(p, typeof(Player));
		}
		[TestMethod]
		[TestCategory("Player")]
		[TestCategory("Player Constructor")]
		public void PlayerOverloadedCtor_SavesData()
		{
			string email = "Email";
			IPlayer p = new Player(1, "user", email);

			Assert.AreEqual(email, p.Email);
		}
	}
}
