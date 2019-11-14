using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Linq;

namespace odbc_test
{
    class Program
    {
        /// <summary>
        /// Id index mapper
        /// Need this because databricks fields names are all _c{value} garbage
        /// </summary>
        static Dictionary<string, int> IdMap = new Dictionary<string, int>()
        {
            { "Id", 0 },
            { "Name", 1 }
        };

        static string ReadString(OdbcDataReader reader, string id)
        {
            return reader.GetString(IdMap[id]);
        }

        static void Main(string[] args)
        {
            IEnumerable<string> conf;
            
            if (File.Exists("./cli.conf"))
            {
                conf = args.Concat(File.ReadAllLines("./cli.conf"));
            }
            else
            {
                conf = args;
            }

            string connStr = null;
            string query = null;
            for (var i = 0; i < conf.Count(); i++)
            {
                if (conf.ElementAt(i).EndsWith("conn-str"))
                {
                    connStr = conf.ElementAt(i + 1);
                }
                else if (conf.ElementAt(i).EndsWith("query"))
                {
                    query = conf.ElementAt(i + 1);
                }
            }

            if (string.IsNullOrEmpty(connStr))
            {
                throw new InvalidOperationException("Missing connection string");
            }

            if (string.IsNullOrEmpty(query))
            {
                throw new InvalidOperationException("Missing query");
            }

            var queryFields = new string[] { "Id", "Name" };

            using (var connection = new OdbcConnection(connStr))
            {
                connection.Open();

                while (true)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("> ");
                    var input = Console.ReadLine();
                    Console.ForegroundColor = ConsoleColor.Blue;

                    var command = new OdbcCommand(input);
                    command.Connection = connection;

                    try
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var res = new object[reader.FieldCount];
                                var read = reader.GetValues(res);

                                // \n between entries is more readable
                                Console.WriteLine(string.Join(",", res.Take(read)) + "\n");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("ERROR: " + ex.Message + "\n" + ex.StackTrace);
                    }
                }
            }
        }
    }
}
