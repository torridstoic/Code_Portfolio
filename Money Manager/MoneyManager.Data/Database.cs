using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;

namespace MoneyManager.Data
{
    public abstract class Database
    {
        //private SqlConnection mssql;
        protected string connectionString;

        public Database()
        {
        }

        abstract protected void CreateTables();
        //abstract protected Boolean isInserted();
    }
}
