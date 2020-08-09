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

        //private const String SqlInsertInto =
        //    @"INSERT INTO Players VALUES
        //        (99, 'Giants', 'Joe Bojangles', 'C', 'Little Joey', '07/11/1991'),
        //        (73, 'Giants', 'Lester Johns', 'P', 'Big John', '06/09/1984'),
        //        (27, 'Cards', 'Earl Hastings', 'OF', 'Speedy Earl', '04/22/1982')";

        private const String SqlInsertInto = 
            @"INSERT INTO Players 
                (ID, Team, Name, Position, DisplayName, BirthDate)
                VALUES (?, ?, ?, ?, ?, ?)";

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

                TestCreateInsertSelect(connection);

                Console.ReadLine();
            }
        }


        private static void TestCreateInsertSelect(DrdaConnection connection)
        {
            Console.WriteLine();
            Console.WriteLine("Try to execute simple SQL");
            Console.WriteLine();

            connection.CreateStatement(DdlCreateTable).Execute(); // Immediate

            Console.WriteLine();
            Console.WriteLine("Try to execute prepared INSERT SQL");
            Console.WriteLine();

            var insert = connection.CreateStatement(SqlInsertInto).Prepare(); // Prepared

            // TODO: implement level-A chunking properly and increase this number for testing
            for (var i = 0; i < 573; ++i)
            {
                insert.SetParameterValue(0, i);
                insert.SetParameterValue(1, "Giants");
                insert.SetParameterValue(2, "Joe Bojangles");
                insert.SetParameterValue(3, "C");
                insert.SetParameterValue(4, "Little Joey");
                insert.SetParameterValue(5, new DateTime(1911, 11, 07));

                insert.Execute();
            }

            Console.WriteLine();
            Console.WriteLine("Try to execute prepared SELECT SQL");
            Console.WriteLine();

            var select = connection.CreateStatement(SqlSelect).Prepare(); // Prepared
            if (@select.Execute())
            {
                while (@select.Fetch())
                {
                    for (var index = 0; index < @select.Columns; ++index)
                    {
                        Console.WriteLine(
                            $"{@select.GetColumnName(index)} = {@select.GetColumnValue(index) ?? "NULL"}");
                    }

                    Console.WriteLine();
                }
            }
        }
    }
}
