using System;
using System.Collections.Generic;

namespace MoneyManager.Data
{
    public class Store : Table
    {
        private int id;
        private String storeName;
        private int colorArgb;

        public Store(ResultData data) : base("Stores")
        {
            this.id = Convert.ToInt32(data.Value("Id"));
            this.Name = Convert.ToString(data.Value("Name"));
            this.colorArgb = Convert.ToInt32(data.Value("ColorArgb"));
        }

        public Store(String name) : base("Stores")
        {
            this.id = 0;
            this.Name = name;
        }

		// DEMO-ONLY CTOR
		public Store(String name, int id) : base("Stores")
		{
			this.id = id;
			this.Name = name;
		}

        public override int Id
        {
            get { return id; }
        }

        public String Name
        {
            get { return storeName; }
            set { this.storeName = value; }
        }

        public int ColorArgb
        {
            get { return colorArgb; }
            set { colorArgb = value; }
        }

        protected override void LoadFields()
        {
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
            if (Name == String.Empty)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
