using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.Data
{
    public class ResultData
    {
        private Dictionary<int, Dictionary<String, Object>> results;
        private Dictionary<String, Object> resultRow;
        private List<String> fieldNames;
        private bool hasRows;
        private int totalRows;
        private int currentRow;
        private int affectedRows;
        private int fieldCount;

        public ResultData()
        {
            this.currentRow = -1;
            this.totalRows = 0;
            this.affectedRows = 0;
            this.fieldCount = 0;
            this.hasRows = false;
            results = new Dictionary<int, Dictionary<String, Object>>();
            resultRow = new Dictionary<String, Object>();
            fieldNames = new List<string>();
        }

        public ResultData(SQLiteDataReader reader)
        {
            results = new Dictionary<int, Dictionary<String, Object>>();
            resultRow = new Dictionary<String, Object>();
            fieldNames = new List<string>();

            this.SetData(reader);
        }

        public void SetData(SQLiteDataReader reader)
        {
            this.currentRow = -1;
            this.totalRows = 0;
            this.affectedRows = 0;
            this.hasRows = reader.HasRows;
            this.fieldCount = reader.FieldCount;
            this.affectedRows = reader.RecordsAffected;

            if (this.hasRows)
            {
                //Set the fieldNames
                for (int i = 0; i < this.fieldCount; i++)
                {
                    this.fieldNames.Add(reader.GetName(i));
                }
                
                while (reader.Read())
                {
                    Dictionary<String, Object> rowData = new Dictionary<String, Object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        rowData[reader.GetName(i)] = reader.GetValue(i);
                    }

                    this.results[totalRows] = rowData;
                    this.totalRows++;
                }
            }
        }

        public String getFieldName(int i)
        {
            return this.fieldNames[i];
        }

        public Object Value(String key)
        {
            if (this.resultRow.ContainsKey(key))
            {
                return this.resultRow[key];
            }
            else
            {
                return null;
            }
        }

        public bool Read()
        {
            currentRow++;
            if (currentRow >= totalRows)
            {
                this.resultRow = null;
                return false;
            }
            else
            {
                this.resultRow = this.results[currentRow];
                return true;
            }
        }

        ////Will set the row data.
        //public object this[int i]
        //{
        //    set
        //    {
        //        this.resultRow = this.results[i];
        //    }
        //}

        public void ResetCurrentRow()
        {
            this.currentRow = -1;
        }

        public void SetAffectedRows(int rows)
        {
            this.affectedRows = rows;
        }

        public bool HasRows()
        {
            return this.hasRows;
        }

        public int getTotalRows()
        {
            return this.totalRows;
        }

        public int getCurrentRow()
        {
            return this.currentRow;
        }

        public int getAffectedRows()
        {
            return this.affectedRows;
        }

        public int getFieldCount()
        {
            return this.fieldCount;
        }
    }
}
