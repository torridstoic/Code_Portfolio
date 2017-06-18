using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.Data
{
    public class Transaction : Table
    {
        private int id;
        private int walletId;
        private int storeId;
        private float amount;
        private double created;
        private String notes;
        private String walletName;
        private Boolean flagged;
        private Boolean posted;
        private Boolean isPositive;
        private String storeName;

        public Transaction(ResultData data) : base("Transactions")
        {
            this.id             = Convert.ToInt32(data.Value("Id"));
            this.WalletId       = Convert.ToInt32(data.Value("WalletId"));
            this.StoreId        = Convert.ToInt32(data.Value("StoreId"));
            this.Amount         = (float)Convert.ToDecimal(data.Value("Amount"));
            this.Created        = Convert.ToDouble(data.Value("Created"));
            this.Notes          = Convert.ToString(data.Value("Notes"));
            this.Flagged        = Convert.ToInt32(data.Value("Flagged")) == 1 ? true : false;
            this.Posted         = Convert.ToInt32(data.Value("Posted")) == 1 ? true : false;
            this.isPositive     = Convert.ToInt32(data.Value("IsPositive")) == 1 ? true : false;
            this.walletName     = Convert.ToString(data.Value("WalletName"));
            this.storeName      = Convert.ToString(data.Value("StoreName"));
        }

        public Transaction(int walletId) : base("Transactions")
        {
            this.id = -1;
            this.walletId = walletId;
        }

		// DEMO-ONLY CTOR
		public Transaction(int walletId, int id) : base("Transactions")
		{
			this.id = id;
			this.walletId = walletId;
		}

        public override int Id
        {
            get { return id; }
        }
        public int WalletId
        {
            get { return walletId; }
            set { walletId = value; }
        }
        public int StoreId
        {
            get { return storeId; }
            set { storeId = value; }
        }
        public float Amount
        {
            get { return amount; }
            set { amount = value; }
        }
        public double Created
        {
            get { return created; }
            set { created = value; }
        }
        public String Notes
        {
            get { return notes; }
            set { notes = value; }
        }
        public String WalletName
        {
            get { return walletName; }
            set { walletName = value; }
        }
        public String StoreName
        {
            get { return storeName; }
            set { storeName = value; }
        }
        public Boolean Flagged
        {
            get { return flagged; }
            set { flagged = value; }
        }
        public Boolean Posted
        {
            get { return posted; }
            set { posted = value; }
        }
        public Boolean IsPositive
        {
            get { return isPositive; }
            set { isPositive = value; }
        }

        protected override void LoadFields()
        {
            AddField("WalletId");
            AddField("StoreId");
            AddField("Amount");
            AddField("Created");
            AddField("Notes");
            AddField("Flagged");
            AddField("Posted");
            AddField("IsPositive");
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
            if (WalletId < 1 || StoreId < 1 || Created < 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //public String StoreName
        //{
        //    get { return storeName; }
        //}

        //public override string ToString()
        //{
        //    return this.StoreName + " $" + this.Amount;
        //}
    }
}
