using Newtonsoft.Json.Linq;
using System;
using System.Data.Odbc;
using System.IO;
using System.Linq;

namespace odbc_cli
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!File.Exists(".config.json"))
            {
                throw new InvalidOperationException(".config.json could not be found.");
            }

            var conf = JObject.Parse(File.ReadAllText(".config.json"));
            var connStr = conf["connectionString"].Value<string>();

            if (string.IsNullOrEmpty(connStr))
            {
                throw new InvalidOperationException(".config.json missing 'connectionString'.");
            }

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
