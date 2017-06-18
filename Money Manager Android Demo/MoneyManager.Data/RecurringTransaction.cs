using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.Data
{
    public class RecurringTransaction : Table
    {
        private int id;
        private int walletId;
		private int storeId;
        private float amount;
        private double processDate;
        private int processPeriod;
		private Boolean isPositive;

        public RecurringTransaction(ResultData data) : base("RecurringTransactions")
        {
            this.id = Convert.ToInt32(data.Value("Id"));
            this.WalletId = Convert.ToInt32(data.Value("WalletId"));
			this.StoreId = Convert.ToInt32(data.Value("StoreId"));
			this.Amount = (float)Convert.ToDouble(data.Value("Amount"));
			this.ProcessDate = Convert.ToDouble(data.Value("ProcessDate"));
            this.ProcessPeriod = Convert.ToInt32(data.Value("ProcessPeriod"));
			this.isPositive = Convert.ToInt32(data.Value("IsPositive")) == 1 ? true : false;
        }
        
        public RecurringTransaction(int walletId) : base("RecurringTransactions")
        {
			this.id = -1;
            this.WalletId = walletId;
        }

		// DEMO-ONLY CTOR
		public RecurringTransaction(int walletId, int id) : base("RecurringTransactions")
		{
			this.id = id;
			this.walletId = walletId;
		}

        public override int Id
        {
            get { return this.id; }
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
            get { return this.amount; }
            set { this.amount = value; }
        }
        public double ProcessDate
        {
            get { return this.processDate; }
            set { this.processDate = value; }
        }
        public int ProcessPeriod
        {
            get { return this.processPeriod; }
            set { this.processPeriod = value; }
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
			AddField("ProcessDate");
            AddField("ProcessPeriod");
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
            if (WalletId < 1 || StoreId < 1 || Amount < 0 || ProcessDate < 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
