using System;
using System.Threading.Tasks;
using SpliceMachine.Drda;

namespace SpliceMachine.Connection
{
    internal static class Program
    {
        public static async Task Main()
        {
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

                Console.WriteLine("Try to execute simple SQL");

                connection.ExecuteImmediateSql("SET SCHEMA SYS");
                Console.ReadLine();
            }
        }
    }
}
