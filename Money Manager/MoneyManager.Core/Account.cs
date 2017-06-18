using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.Core
{
	public class Account
	{
		//////////////
		// Attributes
		private ushort mPeriodLength;
		private ushort mDaysElapsed;
		// list of Wallets?

		//////////////
		// Ctors
		public Account()
		{
			mPeriodLength = mDaysElapsed = 0;
		}
		public Account(ushort _period)
		{
			SetPeriodLength(_period);
			SetDaysElapsed(0);
		}

		//////////////
		// Accessors
		public ushort GetPeriodLength() { return mPeriodLength; }
		public ushort GetDaysElapsed() { return mDaysElapsed; }

		//////////////
		// Mutators
		public void SetPeriodLength(ushort _val) { mPeriodLength = _val; }
		public void SetDaysElapsed(ushort _val) { mDaysElapsed = _val; }

		//////////////
		// Other Functions
		public void Setup()
		{
			// This function is where the user can set up a new account:
			// Add wallets and time period, etc.
		}
		public void Display()
		{
			// Display report data, like PeriodLength
			// probably want to call each Wallet's Display function here
		}
		public void AddTransaction()
		{
			// Manually create a transaction, and
			//AllocateTransaction(t);
		}
		public void Swipe()
		{
			// pull data from the CC purchase to create a Transaction
			// Allocate it if the store matches a database Wallet entry,
			// otherwise leave it un-allocated?
		}
	}
}
