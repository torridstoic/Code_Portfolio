using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoneyManager.Data;


namespace MoneyManager.Forms.v2
{
	public static class Global
	{
        public static SQLiteDatabase db;
		public static User user;
		public static string Name = "Money Manager V1.0 Underwater";
		public static DateTime UnixTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static int SecondsFromUTC()
        {
            return (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

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
        
		public static DateTime ConvertTimeStampToDateTime(double timestamp)
		{
			if (timestamp == 0)
			{
				return UnixTime;
			}
			else
			{
                DateTime date = UnixTime.AddSeconds(timestamp).Date;
                return date;
			}
		}

        public static int TimeLeft(double end)
        {
            DateTime timeNow = DateTime.UtcNow.Date;
            DateTime endDate = UnixTime.AddSeconds(end);

            return (endDate - timeNow).Days;
        }
	}
}
