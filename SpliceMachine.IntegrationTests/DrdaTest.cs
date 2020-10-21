using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpliceMachine.Drda;

namespace SpliceMachine.IntegrationTests
{
    [TestClass]
    public class DrdaTest
    {
        private static string hostName = "localhost"; // You might want to change this.
        private static int port = 1527;
        private static string userName = "splice";
        private static string password = "admin";

        [TestMethod]
        public void TestInsertSelectInt() // the returned Integers are Uint32 though.
        {
            const string DdlDropIfExists = "DROP TABLE IF EXISTS TestInsertSelectInt";
            const string DdlCreateTable = "CREATE TABLE TestInsertSelectInt(COL1 INT)";
            const string SqlInsertInto = "INSERT INTO TestInsertSelectInt(COL1) VALUES (?)";
            const string SqlSelect = "SELECT * FROM TestInsertSelectInt ORDER BY COL1 ASC";

            using(var connection = new DrdaConnection(new DrdaConnectionOptions { HostName = hostName, Port = port, UserName = userName, Password = password }))
            {
                connection.ConnectAsync().Wait();
                connection.CreateStatement(DdlDropIfExists).Execute();
                connection.CreateStatement(DdlCreateTable).Execute();
                var preparedInsert = connection.CreateStatement(SqlInsertInto).Prepare();
                preparedInsert.SetParameterValue(0, 0);
                preparedInsert.Execute();
                preparedInsert.SetParameterValue(0, 1);
                preparedInsert.Execute();
                preparedInsert.SetParameterValue(0, 2);
                preparedInsert.Execute();
                connection.Commit();
                var selectStatement = connection.CreateStatement(SqlSelect).Prepare();
                Assert.IsTrue(selectStatement.Execute());
                Assert.IsTrue(selectStatement.Fetch());
                Assert.AreEqual(0U, selectStatement.GetColumnValue(0));
                Assert.IsTrue(selectStatement.Fetch());
                Assert.AreEqual(1U, selectStatement.GetColumnValue(0));
                Assert.IsTrue(selectStatement.Fetch());
                Assert.AreEqual(2U, selectStatement.GetColumnValue(0));
                Assert.IsFalse(selectStatement.Fetch());
            }
        }

        [TestMethod]
        public void TestInsertSelectBigInt() // the returned Integers are Uint32 though.
        {
            const string DdlDropIfExists = "DROP TABLE IF EXISTS TestInsertSelectBigInt";
            const string DdlCreateTable = "CREATE TABLE TestInsertSelectBigInt(COL1 BIGINT)";
            const string SqlInsertInto = "INSERT INTO TestInsertSelectBigInt(COL1) VALUES (?)";
            const string SqlSelect = "SELECT * FROM TestInsertSelectBigInt ORDER BY COL1 ASC";

            using (var connection = new DrdaConnection(new DrdaConnectionOptions { HostName = hostName, Port = port, UserName = userName, Password = password }))
            {
                connection.ConnectAsync().Wait();
                connection.CreateStatement(DdlDropIfExists).Execute();
                connection.CreateStatement(DdlCreateTable).Execute();
                var preparedInsert = connection.CreateStatement(SqlInsertInto).Prepare();
                preparedInsert.SetParameterValue(0, 1L + int.MaxValue);
                preparedInsert.Execute();
                preparedInsert.SetParameterValue(0, 2L + int.MaxValue);
                preparedInsert.Execute();
                preparedInsert.SetParameterValue(0, 3L + int.MaxValue);
                preparedInsert.Execute();
                connection.Commit();
                var selectStatement = connection.CreateStatement(SqlSelect).Prepare();
                Assert.IsTrue(selectStatement.Execute());
                Assert.IsTrue(selectStatement.Fetch());
                Assert.AreEqual(1UL + int.MaxValue, selectStatement.GetColumnValue(0));
                Assert.IsTrue(selectStatement.Fetch());
                Assert.AreEqual(2UL + int.MaxValue, selectStatement.GetColumnValue(0));
                Assert.IsTrue(selectStatement.Fetch());
                Assert.AreEqual(3UL + int.MaxValue, selectStatement.GetColumnValue(0));
                Assert.IsFalse(selectStatement.Fetch());
            }
        }

        [TestMethod]
        public void TestInsertSelectChar() // the returned Integers are Uint32 though.
        {
            const string DdlDropIfExists = "DROP TABLE IF EXISTS TestInsertSelectChar";
            const string DdlCreateTable = "CREATE TABLE TestInsertSelectChar(COL1 CHAR)";
            const string SqlInsertInto = "INSERT INTO TestInsertSelectChar(COL1) VALUES (?)";
            const string SqlSelect = "SELECT * FROM TestInsertSelectChar ORDER BY COL1 ASC";

            using (var connection = new DrdaConnection(new DrdaConnectionOptions { HostName = hostName, Port = port, UserName = userName, Password = password }))
            {
                connection.ConnectAsync().Wait();
                connection.CreateStatement(DdlDropIfExists).Execute();
                connection.CreateStatement(DdlCreateTable).Execute();
                var preparedInsert = connection.CreateStatement(SqlInsertInto).Prepare();
                preparedInsert.SetParameterValue(0, 'a');
                preparedInsert.Execute();
                preparedInsert.SetParameterValue(0, 'b');
                preparedInsert.Execute();
                preparedInsert.SetParameterValue(0, 'c');
                preparedInsert.Execute();
                connection.Commit();
                var selectStatement = connection.CreateStatement(SqlSelect).Prepare();
                Assert.IsTrue(selectStatement.Execute());
                Assert.IsTrue(selectStatement.Fetch());
                Assert.AreEqual("a", selectStatement.GetColumnValue(0));
                Assert.IsTrue(selectStatement.Fetch());
                Assert.AreEqual("b", selectStatement.GetColumnValue(0));
                Assert.IsTrue(selectStatement.Fetch());
                Assert.AreEqual("c", selectStatement.GetColumnValue(0));
                Assert.IsFalse(selectStatement.Fetch());
            }
        }

        [TestMethod]
        public void TestInsertSelectVarchar() // the returned Integers are Uint32 though.
        {
            const string DdlDropIfExists = "DROP TABLE IF EXISTS TestInsertSelectVarchar";
            const string DdlCreateTable = "CREATE TABLE TestInsertSelectVarchar(COL1 VARCHAR(20))";
            const string SqlInsertInto = "INSERT INTO TestInsertSelectVarchar(COL1) VALUES (?)";
            const string SqlSelect = "SELECT * FROM TestInsertSelectVarchar ORDER BY COL1 ASC";

            using (var connection = new DrdaConnection(new DrdaConnectionOptions { HostName = hostName, Port = port, UserName = userName, Password = password }))
            {
                connection.ConnectAsync().Wait();
                connection.CreateStatement(DdlDropIfExists).Execute();
                connection.CreateStatement(DdlCreateTable).Execute();
                var preparedInsert = connection.CreateStatement(SqlInsertInto).Prepare();
                preparedInsert.SetParameterValue(0, "abc");
                preparedInsert.Execute();
                preparedInsert.SetParameterValue(0, "def");
                preparedInsert.Execute();
                preparedInsert.SetParameterValue(0, "ghi");
                preparedInsert.Execute();
                connection.Commit();
                var selectStatement = connection.CreateStatement(SqlSelect).Prepare();
                Assert.IsTrue(selectStatement.Execute());
                Assert.IsTrue(selectStatement.Fetch());
                Assert.AreEqual("abc", selectStatement.GetColumnValue(0));
                Assert.IsTrue(selectStatement.Fetch());
                Assert.AreEqual("def", selectStatement.GetColumnValue(0));
                Assert.IsTrue(selectStatement.Fetch());
                Assert.AreEqual("ghi", selectStatement.GetColumnValue(0));
                Assert.IsFalse(selectStatement.Fetch());
            }
        }

        [Ignore] // fails with the following error:
                 // Test method SpliceMachine.IntegrationTests.DrdaTest.TestInsertSelectTimestamp threw exception: 
                 // System.ArgumentOutOfRangeException: Index and length must refer to a location within the string.
                 // Parameter name: length
        [TestMethod]
        public void TestInsertSelectTimestamp() // the returned Integers are Uint32 though.
        {
            const string DdlDropIfExists = "DROP TABLE IF EXISTS TestInsertSelectTimestamp";
            const string DdlCreateTable = "CREATE TABLE TestInsertSelectTimestamp(COL1 TIMESTAMP)";
            const string SqlInsertInto = "INSERT INTO TestInsertSelectTimestamp(COL1) VALUES (?)";
            const string SqlSelect = "SELECT * FROM TestInsertSelectTimestamp ORDER BY COL1 ASC";

            using (var connection = new DrdaConnection(new DrdaConnectionOptions { HostName = hostName, Port = port, UserName = userName, Password = password }))
            {
                connection.ConnectAsync().Wait();
                connection.CreateStatement(DdlDropIfExists).Execute();
                connection.CreateStatement(DdlCreateTable).Execute();
                var preparedInsert = connection.CreateStatement(SqlInsertInto).Prepare();
                preparedInsert.SetParameterValue(0, new DateTime(2020, 10, 20, 15, 15, 15));
                preparedInsert.Execute();
                preparedInsert.SetParameterValue(0, new DateTime(2020, 10, 20, 15, 15, 16));
                preparedInsert.Execute();
                preparedInsert.SetParameterValue(0, new DateTime(2020, 10, 20, 15, 15, 17));
                preparedInsert.Execute();
                connection.Commit();
                var selectStatement = connection.CreateStatement(SqlSelect).Prepare();
                Assert.IsTrue(selectStatement.Execute());
                Assert.IsTrue(selectStatement.Fetch());
                Assert.AreEqual(new DateTime(2020, 10, 20, 15, 15, 15), selectStatement.GetColumnValue(0));
                Assert.IsTrue(selectStatement.Fetch());
                Assert.AreEqual(new DateTime(2020, 10, 20, 15, 15, 16), selectStatement.GetColumnValue(0));
                Assert.IsTrue(selectStatement.Fetch());
                Assert.AreEqual(new DateTime(2020, 10, 20, 15, 15, 17), selectStatement.GetColumnValue(0));
                Assert.IsFalse(selectStatement.Fetch());
            }
        }
    }
}
