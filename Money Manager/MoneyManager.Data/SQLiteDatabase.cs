using MoneyManager.Data.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace MoneyManager.Data
{
    public class SQLiteDatabase : Database
    {
        private SQLiteConnection sqlite;
        private String databaseName;

        public SQLiteDatabase(String filePath = null, String database = "MoneyManager", bool forceReset = false, bool useTestData = false)
        {
            if (filePath == null) filePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + Settings.Default.ApplicationName;
            this.databaseName = filePath + "/" + database + ".sqlite";
            this.connectionString = "Data Source=" + this.databaseName + "; Version = 3;";

            //Make sure the connection has a valid file or force it
            if (!File.Exists(this.databaseName) || forceReset)
            {
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                SQLiteConnection.CreateFile(this.databaseName);
                this.sqlite = new SQLiteConnection(connectionString);
                forceReset = true;
            }

            this.sqlite = new SQLiteConnection(connectionString);
            //Open the database 
            this.OpenConnection();

            //Force the reset of the database tables
            if (forceReset)
            {
                this.CreateTables();
            }

            //Create test records for accuracy
            if (useTestData)
            {
                this.InsertTestData();
            }
        }

        override protected void CreateTables()
        {
            //Create Users
            using (SQLiteCommand comm = new SQLiteCommand(this.sqlite))
            {
                List<String> queries = new List<String>();

                //Open the database file
                StreamReader reader = new StreamReader("database.txt");
                String queryString = "";
                while(!reader.EndOfStream)
                {
                    String lineData = reader.ReadLine();

                    if (lineData != "")
                    {
                        queryString += lineData;

                        if (queryString.Contains(";"))
                        {
                            //Put this into the queries list
                            queries.Add(queryString);
                            queryString = "";
                        }
                    }
                }

                foreach (String query in queries)
                {
                    comm.CommandText = query;
                    comm.ExecuteNonQuery();
                }
            }
        }

        private void InsertTestData()
        {

            using (SQLiteCommand comm = new SQLiteCommand(this.sqlite))
            {
                List<String> queries = new List<string>();

                //Open the database file
                StreamReader reader = new StreamReader("databaseTestData.txt");
                String queryString = "";
                while (!reader.EndOfStream)
                {
                    String lineData = reader.ReadLine();

                    if (lineData != "")
                    {
                        queryString += lineData;

                        if (queryString.Contains(";"))
                        {
                            //Put this into the queries list
                            queries.Add(queryString);
                            queryString = "";
                        }
                    }
                }

                foreach (String query in queries)
                {
                    this.Query(query);
                }
            }
        }

        public void ResetDatabase()
        {
            this.CreateTables();
        }

        public void AddTestData()
        {
            this.InsertTestData();
        }

        //SELECT QUERIES

        public ResultData GetTable(String table, String fields, String where = "", String group = "", String order = "", int limit = -1)
        {
            if (fields == "") fields = "*";
            String query = "SELECT " + fields + " FROM " + table + " WHERE " + where;

            query += group != String.Empty ? " GROUP BY " + group : "";
            query += order != String.Empty ? " ORDER BY " + order : "";
            query += limit > 0 ? " LIMIT " + limit : "";

            ResultData data = this.Query(query);
            return data;
        }

        public List<Wallet> GetWallets(User user, int WalletId = -1)
        {
            List<Wallet> wallets = new List<Wallet>();
            if (user.Id > 0)
            {
                string query = "SELECT *, (SELECT TOTAL(Amount) FROM Transactions WHERE w.Id = WalletId) as BudgetUsed FROM Wallets w WHERE UserId = " + user.Id;
                query += WalletId > 0 ? " AND WalletId = " + WalletId : "";

                ResultData data = this.Query(query);
                while (data.Read())
                {
                    wallets.Add(new Wallet(data));
                }
            }

            return wallets;
        }

        public Wallet GetWallet(User user, int walletId)
        {
            String query = "SELECT * FROM Wallets WHERE Id = "+walletId+" AND UserId = " + user.Id;
            ResultData data = this.Query(query);
            if (data.Read())
            {
                return new Wallet(data);
            }
            else
            {
                return null;
            }
        }
        
        public ResultData GetUsers(int Id = -1, String email = "")
        {
            string query = "SELECT * FROM Users";
            string where = "";
            if (Id > 0)
            {
                where += "Id = " + Id;
            }
            if (email.Length > 0)
            {
                where += "Email = '" + email + "'";
            }

            return this.Query(query + " WHERE " + where);
        }

        public List<Transaction> GetTransactions(User user, Wallet wallet, TransactionType.Types type = TransactionType.Types.Unknown)
        {
            List<Transaction> transactions = new List<Transaction>();

            if (user.Id > 0)
            {
                string query = "SELECT *, w.Name as WalletName, (SELECT Name FROM Stores WHERE t.StoreId = Id) as StoreName FROM Transactions t INNER JOIN Wallets w ON t.WalletId = w.Id WHERE w.UserId = " + user.Id;
                if (wallet != null) query += " AND w.Id = " + wallet.Id;
                if (type != TransactionType.Types.Unknown) query += " AND TransactionTypeId = " + (int)type;

                ResultData data = this.Query(query);
                while (data.Read())
                {
                    transactions.Add(new Transaction(data));
                }
            }

            return transactions;
        }

        public List<RecurringTransaction> GetRecurringTransactions(User user, Wallet wallet, TransactionType.Types transactionType)
        {
            List<RecurringTransaction> recurTran = new List<RecurringTransaction>();

            if (user != null)
            {
                string query = "SELECT * FROM RecurringTransactions WHERE WalletId = " + wallet.Id;
                if (transactionType != TransactionType.Types.Unknown) query += " AND TransactionTypeId = " + (int)transactionType;

                ResultData data = this.Query(query);
                while (data.Read())
                {
                    recurTran.Add(new RecurringTransaction(data));
                }
            }

            return recurTran;
        }


        public List<WalletType> GetWalletTypes()
        {
            List<WalletType> types = new List<WalletType>();

            string query = "SELECT * FROM WalletTypes";
            ResultData data = this.Query(query);
            while (data.Read())
            {
                types.Add(new WalletType(data));
            }

            return types;
        }

        public List<TransactionType> GetTransactionTypes()
        {
            List<TransactionType> types = new List<TransactionType>();

            string query = "SELECT * FROM TransactionTypes";
            ResultData data = this.Query(query);
            while (data.Read())
            {
                types.Add(new TransactionType(data));
            }

            return types;
        }

        public List<Store> GetStores(String Name = "")
        {
            List<Store> store = new List<Store>();

            string query = "SELECT * FROM Stores";
            query += Name != String.Empty ? " WHERE Name = '" + Name + "'" : "";
            ResultData data = this.Query(query);
            while (data.Read())
            {
                store.Add(new Store(data));
            }

            return store;
        }
        
        public Store GetStore(int Id)
        {
            String query = "SELECT * FROM Stores WHERE Id = " + Id;
            ResultData data = this.Query(query);
            if (data.Read())
            {
                return new Store(data);
            }
            else
            {
                return null;
            }
        }

        public List<Budget> GetBudgets(User user, Wallet wallet = null)
        {
            List<Budget> budgets = new List<Budget>();

            if (user.Id > 0)
            {
                String query = "SELECT * FROM Budgets WHERE ";

                if (wallet != null)
                {
                    query += " WalletId = " + wallet.Id;
                }
                else
                {
                    query += "WalletId IN (SELECT Id FROM Wallets WHERE UserId = " + user.Id + ")";
                }

                ResultData data = this.Query(query);
                while(data.Read())
                {
                    budgets.Add(new Budget(data));
                }
            }

            return budgets;
        }

		public List<RecurringBudget> GetRecurringBudgets(int WalletId)
		{
			List<RecurringBudget> rbudgets = new List<RecurringBudget>();

			if (WalletId > 0)
			{
				string query = "SELECT * FROM RecurringBudgets WHERE WalletId = " + WalletId;

				ResultData data = this.Query(query);
				while (data.Read())
				{
					rbudgets.Add(new RecurringBudget(data));
				}
			}

			return rbudgets;
		}

        //DELETE QUERIES
        public Boolean Delete(Table data)
        {
            String query = "DELETE FROM " + data.TableName + " WHERE Id = " + data.Id;
            return this.Query(query).getAffectedRows() == 1;
        }

        //UPDATE QUERIES
        public Boolean UpdateRecord(Table data)
        {
            if (data.Validation())
            {
                String query = "UPDATE " + data.TableName + " SET " + data.MapFields(",") + " WHERE Id = " + data.Id;
                return this.Query(query).getAffectedRows() == 1;
            }
            else
            {
                return false;
            }
        }

        //CREATION QUERIES
        //Returns boolean if successful or not\
        public Boolean CreateRecord(Table data)
        {
            if (data.Validation())
            {
                String query = "INSERT INTO " + data.TableName + " (" + data.getFieldsAsString(",") + ") VALUES (" + data.getValuesAsString(",") + ")";
                return this.Query(query).getAffectedRows() == 1;
            }
            else
            {
                return false;
            }
        }

        //HELPER FUNCTIONS
        //Opens the connection
        public void OpenConnection()
        {
            this.sqlite.Open();
        }

        //Closes the connection
        public void CloseConnection()
        {
            this.sqlite.Close();
        }

        //Used for custom queries and more flexability
        public ResultData Query(string query)
        {
            ResultData data = new ResultData();

            try
            {
                SQLiteCommand comm = new SQLiteCommand(query, this.sqlite);
                
                if (query.ToLower().Contains("select"))
                {
                    data.SetData(comm.ExecuteReader());
                }
                else
                {
                    data.SetAffectedRows(comm.ExecuteNonQuery());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Invalid Query: " + e.Message);
            }

            return data;
        }

        //Grabs the last inserted ID for any table
        public int GetLastInsertedId()
        {
            if (this.sqlite.State != ConnectionState.Open)
            {
                this.sqlite.Open();
            }

            SQLiteCommand comm = new SQLiteCommand(this.sqlite);
            comm.CommandText = "SELECT last_insert_rowid()";
            Object data = comm.ExecuteScalar();
            if (data != null)
            {
                return Convert.ToInt32(data);
            }
            else
            {
                return -1;
            }
        }
    }
}
