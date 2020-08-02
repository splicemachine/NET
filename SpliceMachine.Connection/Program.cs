using System;
using System.Diagnostics;
using System.Threading.Tasks;
using SpliceMachine.Drda;

namespace SpliceMachine.Connection
{
    internal static class Program
    {
        public static async Task Main()
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Error));
            Trace.AutoFlush = true;

            using (var connection = new DrdaConnection(
                new DrdaConnectionOptions
                {
                    HostName = "localhost",
                    Port = 1527,
                    UserName = "splice",
                    Password = "admin"
                }))
            {
                await connection.ConnectAsync();

                Console.WriteLine();
                Console.WriteLine("Try to execute simple SQL");
                Console.WriteLine();

                //connection.CreateStatement("SET SCHEMA SYS").Execute(); // Immediate

                var statement = connection.CreateStatement("SELECT * FROM SYS.SYSTABLES").Prepare();  // Prepared
                if (statement.Execute())
                {
                    while (statement.Fetch())
                    {
                        for (var index = 0; index < statement.Columns; ++index)
                        {
                            Console.WriteLine(
                                $"{statement.GetColumnName(index)} = {statement.GetColumnValue(index) ?? "NULL"}");
                        }

                        Console.WriteLine();
                    }
                }

                Console.ReadLine();
            }
        }
    }
}
