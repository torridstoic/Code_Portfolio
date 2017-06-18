using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoneyManager.Data;

namespace MoneyManager.DatabaseConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            SQLiteDatabase db = new SQLiteDatabase();
            bool keepRunning = true;

            while (keepRunning)
            {
                string input;

                Console.Write("Query> ");
                input = Console.ReadLine();

                if (input == "exit")
                {
                    break;
                }
                else if (input == "Reset")
                {
                    db.ResetDatabase();
                }
                else if (input == "Test")
                {
                    db.AddTestData();
                }
                else
                {
                    ResultData data = db.Query(input);
                    if (data.HasRows())
                    {
                        while (data.Read())
                        {

                            for (int i = 0; i < data.getFieldCount(); i++)
                            {
                                Console.WriteLine(data.getFieldName(i) + ": " + data.Value(data.getFieldName(i)));
                            }
                        }
                    }

                    else if (data.getAffectedRows() > 0)
                    {
                        Console.WriteLine(data.getAffectedRows() + " rows affected.");
                    }
                    else
                    {
                        Console.WriteLine("No results found.");
                    }
                }

                Console.WriteLine();
            }
        }
    }
}
