using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.Data
{
	public class Budget : Table
	{
		private int id;
		private int walletId;
		private float amount;
		private double startDate;
		private double endDate;

		public Budget(ResultData data) : base("Budgets")
		{
			id = Convert.ToInt32(data.Value("Id"));
			walletId = Convert.ToInt32(data.Value("WalletId"));
			amount = (float)Convert.ToDouble(data.Value("Amount"));
			startDate = Convert.ToDouble(data.Value("StartDate"));
			endDate = Convert.ToDouble(data.Value("EndDate"));
		}

		public Budget(int walletId, float amount) : base("Budgets")
		{
			this.id = -1;
			this.walletId = walletId;
			this.amount = amount;
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

		public float Amount
		{
			get { return amount; }
			set { amount = value; }
		}

		public double StartDate
		{
			get { return startDate; }
			set { startDate = value; }
		}

		public double EndDate
		{
			get { return endDate; }
			set { endDate = value; }
		}

        protected override void LoadFields()
        {
            AddField("WalletId");
            AddField("Amount");
			AddField("StartDate");
			AddField("EndDate");
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
            if (WalletId < 0 || Amount < 0)
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
