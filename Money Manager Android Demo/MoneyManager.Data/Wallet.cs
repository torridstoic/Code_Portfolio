using System;
using System.Collections.Generic;

namespace MoneyManager.Data
{
    public class Wallet : Table
    {
        private int         id;
        private int         userId;
        private String      name;
        private int         colorArgb;


        public Wallet(ResultData data) : base("Wallets")
        {
            this.id             = Convert.ToInt32(data.Value("Id"));
            this.userId         = Convert.ToInt32(data.Value("UserId"));
            Name                = Convert.ToString(data.Value("Name"));
            ColorArgb           = Convert.ToInt32(data.Value("ColorArgb"));
        }

        public Wallet(User user) : base("Wallets")
        {
            this.id = -1;
            this.userId = user.Id;
        }

		// DEMO-ONLY CTOR
		public Wallet(User user, int id) : base("Wallets")
		{
			this.id = id;
			this.userId = user.Id;
		}

        public override int Id
        {
            get { return id; }
        }
        public int UserId
        {
            get { return userId;  }
        }
        public String Name
        {
            get { return this.name; }
            set { this.name = value; }
        }
        public int ColorArgb
        {
            get { return colorArgb; }
            set { colorArgb = value; }
        }

        protected override void LoadFields()
        {
            AddField("UserId");
            AddField("Name");
            AddField("ColorArgb");
        }

        protected override List<Object> getValues()
        {
            List<Object> values = new List<Object>();

            foreach (String field in fields)
            {
                Object value = this.GetType().GetProperty(field).GetValue(this);
                values.Add(value);
            }

            return values;
        }

        public override bool Validation()
        {
            if (UserId < 1 || Name == String.Empty)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public override String ToString()
        {
            return this.Name;
        }
    }
}