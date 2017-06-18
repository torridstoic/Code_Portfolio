using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.Data
{
	public class RecurringBudget : Table
	{
		private int id;
		private int walletId;
		private float amount;
		private int period; // 0 = monthly, 1 = quarterly, 2 = yearly
		private double currentStartDate;
		private double currentEndDate;

		public RecurringBudget(ResultData data) : base("RecurringBudgets")
		{
			id = Convert.ToInt32(data.Value("Id"));
			walletId = Convert.ToInt32(data.Value("WalletId"));
			amount = (float)Convert.ToDouble(data.Value("Amount"));
			period = Convert.ToInt32(data.Value("Period"));
			currentStartDate = Convert.ToDouble(data.Value("CurrentStartDate"));
			currentEndDate = Convert.ToDouble(data.Value("CurrentEndDate"));
		}

		public RecurringBudget(int walletId, float amount) : base("RecurringBudgets")
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

		// period: 0 = monthly, 1 = quarterly, 2 = yearly
		public int Period
		{
			get { return period; }
			set { period = value; }
		}

		public double CurrentStartDate
		{
			get { return currentStartDate; }
			set { currentStartDate = value; }
		}

		public double CurrentEndDate
		{
			get { return currentEndDate; }
			set { currentEndDate = value; }
		}

		protected override void LoadFields()
		{
			AddField("WalletId");
			AddField("Amount");
			AddField("Period");
			AddField("CurrentStartDate");
			AddField("CurrentEndDate");
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
