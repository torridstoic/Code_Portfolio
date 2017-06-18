using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoneyManager.Data;


namespace MoneyManager.Android
{
	public static class Global
	{
		/// <summary>
		/// DEMO-ONLY SECTION
		/// </summary>
		private static int uniqueId = 0;
		public static List<Wallet> gWallets = new List<Wallet>();
		public static List<Transaction> gTransactions = new List<Transaction>();
		public static List<RecurringTransaction> gRecurTransactions = new List<RecurringTransaction>();
		public static List<Store> gStores = new List<Store>();
		public static int GetUniqueId()
		{
			++uniqueId;
			if (uniqueId <= 0)
				uniqueId = 1;
			return uniqueId;
		}
		/// END DEMO-ONLY

        public static SQLiteDatabase db;
		public static User user;
		public static string Name = "Money Manager V1.0 Underwater";
		public static DateTime UnixTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

		public static int ConvertToUnixTimeStamp(DateTime date)
		{
			if (date > UnixTime)
			{
				return (int)(date - UnixTime).TotalSeconds;
			}
			else
			{
				return 0;
			}
		}

        public static int SecondsFromUTC()
        {
            return (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        public static int ConvertToUnixTimeStamp()
		{
			DateTime date = new DateTime();
			date = DateTime.UtcNow;

			if (date > UnixTime)
			{
				return (int)(date - UnixTime).TotalSeconds;
			}
			else
			{
				return 0;
			}
		}

		public static DateTime ConvertTimeStampToDateTime(double timestamp)
		{
			if (timestamp == 0)
			{
				return UnixTime;
			}
			else
			{
				DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestamp).ToLocalTime();
				return date.ToLocalTime();
			}
		}
	}
}
