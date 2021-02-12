using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpliceMachine.Provider;

namespace SpliceMachine.IntegrationTests
{
    [TestClass]
    public class AdoNetTest
    {
        #region Connection Settings
        private static string hostName = "localhost"; // You might want to change this.
        private static int port = 1527;
        private static string userName = "splice";
        private static string password = "admin";
        #endregion

        #region DDL TESTS

        [TestMethod]
        public void TestDDLCreateDrop()
        {
            const string DdlDropIfExists    = "DROP TABLE IF EXISTS TestTable";
            const string DdlCreateTable     = "CREATE TABLE TestTable(COL1 INT)";
            const string DdlInsertTable     = "INSERT INTO TestTable VALUES(1)";
            const string DdlCreateView      = "CREATE VIEW TestView (Col1View) AS SELECT Col1 AS Col1View FROM TestTable";
            const string DdlAlterTable      = "ALTER TABLE TestTable ADD COLUMN Col2 bigint";


            using (var connection = new SpliceDbConnection())
            {
                connection.ConnectionString = "uid=" + userName + ";pwd=" + password + ";host=" + hostName + ";port=" + port + "";
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = DdlCreateTable;
                    var result = command.ExecuteNonQuery();
                    connection.Commit();
                    command.CommandText = DdlInsertTable;
                    result = command.ExecuteNonQuery();
                    connection.Commit();
                    command.CommandText = DdlCreateView;
                    result = command.ExecuteNonQuery();
                    connection.Commit();
                    command.CommandText = DdlAlterTable;
                    result = command.ExecuteNonQuery();
                    connection.Commit();
                    command.CommandText = DdlDropIfExists;
                    result = command.ExecuteNonQuery();
                    connection.Commit();
                }

                connection.Close();
            }
        }
        #endregion

    }
}
