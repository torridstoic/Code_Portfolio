using System;
using System.Collections.Generic;

namespace MoneyManager.Data
{
    public class Wallet : Table
    {
        private int id;
        private int userId;
        private int colorArgb;
        private int walletTypeId;
        private float amount;
        private String name;

        public Wallet(ResultData data) : base("Wallets")
        {
            this.id             = Convert.ToInt32(data.Value("Id"));
            this.userId         = Convert.ToInt32(data.Value("UserId"));
            this.ColorArgb      = Convert.ToInt32(data.Value("ColorArgb"));
            this.Name           = Convert.ToString(data.Value("Name"));
            this.amount         = (float)Convert.ToDouble(data.Value("Amount"));
            this.walletTypeId   = Convert.ToInt32(data.Value("WalletTypeId"));
        }

        public Wallet(User user) : base("Wallets")
        {
            this.id = -1;
            this.userId = user.Id;
            this.WalletTypeId = (int)WalletType.Types.Default;
        }

        public override int Id
        {
            get { return id; }
        }
        public int UserId
        {
            get { return userId;  }
        }
        public int WalletTypeId
        {
            get { return walletTypeId; }
            set { walletTypeId = value; }
        }
        public String Name
        {
            get { return this.name; }
            set { this.name = value; }
        }
        public float Amount
        {
            get { return amount; }
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
            AddField("WalletTypeId");
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