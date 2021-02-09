using System;
using System.Data;
using System.Diagnostics;
using SpliceMachine.Drda;
using SpliceMachine.Provider;

namespace SpliceMachine.Connection
{
    internal static class Program
    {
        private const string DdlDropIfExists = "DROP TABLE IF EXISTS Players";
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
        private const String SqlUpdate = 
            @"UPDATE PLAYERS SET NAME='Joe Updated'";

        private const String SqlALTER =
            @"ALTER TABLE Players ADD COLUMN Updated TIMESTAMP";

        private const String SqlSelect = "select * from players";

        public static void Main()
        {
            //TestDropAdoNet();
            //TestCreateAdoNet();
            //TestInsertAdoNet();
            //TestUpdateAdoNet();
            //TestAlterAdoNet();
            TestSelectAdoNet();
            //TestCreateInsertSelectDrda();

            Console.ReadLine();
        }

        private static async void TestDropAdoNet()
        {
            using (var connection = new SpliceDbConnection(
                "uid=splice;pwd=admin;host=localhost;port=1527"))
            {
                await connection.OpenAsync();
                
                using (var command = connection.CreateCommand())
                {                    
                    command.CommandText = DdlDropIfExists;
                    var count = await command.ExecuteNonQueryAsync();
                    Console.WriteLine($"DDL command result: {count}");
                }
            }
        }

        private static async void TestCreateAdoNet()
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

        private static async void TestInsertAdoNet()
        {
            using (var connection = new SpliceDbConnection(
                "uid=splice;pwd=admin;host=localhost;port=1527"))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = SqlInsertInto;
                    command.Parameters.Add( new SpliceDbParameter() { Value =DateTime.Now.Second});
                    command.Parameters.Add(new SpliceDbParameter() { Value = "Giants" });
                    command.Parameters.Add( new SpliceDbParameter() { Value = "Joe Bojangles" });
                    command.Parameters.Add( new SpliceDbParameter() { Value = "C" });
                    command.Parameters.Add(new SpliceDbParameter() { Value = "Little Joey" });
                    command.Parameters.Add(new SpliceDbParameter() { Value = new DateTime(1911, 11, 07) });
                    var count = await command.ExecuteNonQueryAsync();
                    Console.WriteLine($"Insert command result: {count}");
                }
            }
        }

        private static async void TestUpdateAdoNet()
        {
            using (var connection = new SpliceDbConnection(
                "uid=splice;pwd=admin;host=localhost;port=1527"))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = SqlUpdate;
                    var count = await command.ExecuteNonQueryAsync();
                    Console.WriteLine($"Update command result: {count}");
                }
            }
        }

        private static async void TestAlterAdoNet()
        {
            using (var connection = new SpliceDbConnection(
                "uid=splice;pwd=admin;host=localhost;port=1527"))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = SqlALTER;
                    var count = await command.ExecuteNonQueryAsync();
                    Console.WriteLine($"Alter command result: {count}");
                }
            }
        }

        private static async void TestSelectAdoNet()
        {
            using (var connection = new SpliceDbConnection(
                "uid=splice;pwd=admin;host=localhost;port=1527"))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = SqlSelect;
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Console.WriteLine("Select Query Result" + String.Format("{0}", reader[0]));
                        Console.WriteLine("Select Query Result" + String.Format("{0}", reader[1]));
                        Console.WriteLine("Select Query Result" + String.Format("{0}", reader[2]));
                    }
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
