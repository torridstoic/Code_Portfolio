﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.Data
{
    public class TransactionType : Table
    {
        private int id;
        private String type;

        public enum Types
        {
            Unknown = 0,
            Payment = 1,
            Credit = 2,
            Savings = 3
        }

        public TransactionType(ResultData data) : base("WalletTypes")
        {
            this.id = Convert.ToInt32(data.Value("Id"));
            this.type = Convert.ToString(data.Value("Type"));
        }

        public TransactionType(String type) : base("WalletTypes")
        {
            this.Type = type;
        }

        public override int Id
        {
            get { return id; }
        }

        public String Type
        {
            get { return type; }
            set { type = value; }
        }

        protected override void LoadFields()
        {
            AddField("Type");
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
            if (Type == String.Empty)
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
            return Type;
        }
    }
}
