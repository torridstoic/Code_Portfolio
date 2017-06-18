using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.Data
{
    public abstract class Table
    {
        private String tableName;
        protected List<String> fields;
        protected DateTime UTCTime;

        public Table (String tableName)
        {
            this.tableName = tableName;
            fields = new List<string>();
            UTCTime = new DateTime(1970, 1, 1);
            LoadFields();
        }

        public String TableName
        {
            get { return tableName; }
        }

        protected int SecondsFromUTC()
        {
            return (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        protected void AddField(String field)
        {
            this.fields.Add(field);
        }

        public String getFieldsAsString(String delimiter)
        {
            String fieldString = "";
            foreach (String field in fields)
            {
                fieldString += field + delimiter;
            }

            return fieldString.Substring(0, fieldString.Length - delimiter.Length);
        }

        public String getValuesAsString(String delimiter)
        {
            String valueString = "";
            foreach (Object value in getValues())
            {
                if (value == null)
                {
                    valueString += "NULL" + delimiter;
                }
                else
                {
                    if (value.GetType() == typeof(Boolean))
                    {
                        valueString += Convert.ToInt32(value) + delimiter;
                    }
                    else
                    {
                        valueString += "'" + value + "'" + delimiter;
                    }
                }
            }

            return valueString.Substring(0, valueString.Length - delimiter.Length);
        }

        public String MapFields(String delimiter, bool AllowNulls = true)
        {
            //Acquire the values
            List<Object> values = getValues();

            String data = "";

            //Now loop through and make sure all fields are set
            for (int i = 0; i < fields.Count; i++)
            {
                if (AllowNulls)
                {
                    if (values[i].GetType() == typeof(Boolean))
                    {
                        data += fields[i] + " = " + Convert.ToInt32(values[i]) + delimiter;
                    }
                    else {
                        data += fields[i] + " = '" + values[i] + "'" + delimiter;
                    }
                }
                else
                {
                    if (values[i] != null)
                    {
                        if (values[i].GetType() == typeof(Boolean))
                        {
                            data += fields[i] + " = " + Convert.ToInt32(values[i]) + delimiter;
                        }
                        else
                        {
                            data += fields[i] + " = '" + values[i] + "'" + delimiter;
                        }
                    }
                }
            }

            return data.Substring(0, data.Length - delimiter.Length);
        }

        abstract public int Id
        {
            get;
        }
        abstract protected void LoadFields(); // Loads non ID fields
        abstract protected List<Object> getValues();
        abstract public Boolean Validation();
    }
}
