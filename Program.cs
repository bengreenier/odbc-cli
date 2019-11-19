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
            var exitOnEmptyStdin = args.LastOrDefault() == "-";

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

            var disableOutput = false;
            if (conf["disableOutput"] != null)
            {
                try
                {
                    disableOutput = conf["disableOutput"].Value<bool>();
                }
                catch(System.FormatException)
                {
                    throw new System.FormatException(".config.json invalid boolean value for 'disableOutput'.");
                }
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
                    
                    if (string.IsNullOrEmpty(input))
                    {
                        break;
                    }
                    
                    if (exitOnEmptyStdin)
                    {
                        // an extra \n makes things nicer for piped input
                        Console.WriteLine();
                    }

                    var command = new OdbcCommand(input);
                    command.Connection = connection;

                    try
                    {
                        var watch = System.Diagnostics.Stopwatch.StartNew();

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var res = new object[reader.FieldCount];
                                var read = reader.GetValues(res);

                                if (!disableOutput)
                                {
                                    // \n between entries is more readable
                                    Console.WriteLine(string.Join(",", res.Take(read)) + "\n");
                                }
                            }
                        }

                        watch.Stop();
                        Console.WriteLine($"Elapsed milliseconds: {watch.ElapsedMilliseconds}");
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
