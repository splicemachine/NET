using System;
using System.Diagnostics;
using SpliceMachine.Drda;
using SpliceMachine.Provider;

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

        public static void Main()
        {
            TestCreateInsertSelectAdoNet();
            //TestCreateInsertSelectDrda();

            Console.ReadLine();
        }

        private static async void TestCreateInsertSelectAdoNet()
        {
            using (var connection = new SpliceDbConnection(
                "uid=splice;pwd=admin;host=localhost;port=1527"))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = DdlCreateTable;

                    var count =await command.ExecuteNonQueryAsync();
                    Console.WriteLine($"DDL command result: {count}");
                }
            }
        }

        private static async void TestCreateInsertSelectDrda()
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

                Console.WriteLine();
                Console.WriteLine("Try to execute prepared INSERT SQL");
                Console.WriteLine();

                var insert = connection.CreateStatement(SqlInsertInto).Prepare(); // Prepared

                for (var i = 0; i < 10; ++i)
                {
                    insert.SetParameterValue(0, i);
                    insert.SetParameterValue(1, "Giants");
                    insert.SetParameterValue(2, "Joe Bojangles");
                    insert.SetParameterValue(3, "C");
                    insert.SetParameterValue(4, "Little Joey");
                    insert.SetParameterValue(5, new DateTime(1911, 11, 07));

                    insert.Execute();
                }

                connection.Rollback();

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
}
