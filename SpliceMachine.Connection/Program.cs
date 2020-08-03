using System;
using System.Diagnostics;
using System.Threading.Tasks;
using SpliceMachine.Drda;

namespace SpliceMachine.Connection
{
    internal static class Program
    {
        private const String DdlCreateTable =
            @"CREATE TABLE Players (
                ID SMALLINT NOT NULL PRIMARY KEY,
                Team VARCHAR(64) NOT NULL,
                Name VARCHAR(64) NOT NULL,
                Position CHAR(2),
                DisplayName VARCHAR(24),
                BirthDate DATE)";

        private const String SqlInsertInto = 
            @"INSERT INTO Players VALUES
                (99, 'Giants', 'Joe Bojangles', 'C', 'Little Joey', '07/11/1991'),
                (73, 'Giants', 'Lester Johns', 'P', 'Big John', '06/09/1984'),
                (27, 'Cards', 'Earl Hastings', 'OF', 'Speedy Earl', '04/22/1982')";

        private const String SqlSelect = "SELECT * FROM Players";

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

                connection.CreateStatement(DdlCreateTable).Execute(); // Immediate
                connection.CreateStatement(SqlInsertInto).Execute(); // Immediate

                var statement = connection.CreateStatement(SqlSelect).Prepare();  // Prepared
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
