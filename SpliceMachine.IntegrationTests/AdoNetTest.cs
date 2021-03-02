using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpliceMachine.Provider;
using System;
using System.Data;

namespace SpliceMachine.IntegrationTests
{
    [TestClass]
    public class AdoNetTest
    {
        #region Connection Settings
        private static string hostName = "192.168.0.27"; // You might want to change this.
        private static int port = 1527;
        private static string userName = "splice";
        private static string password = "admin";
        #endregion

        #region DDL TESTS

        [TestMethod]
        public void TestDDLCreateDrop()
        {
            const string DdlCreateTable = "CREATE TABLE TestTable(COL1 BLOB)";
            const string DdlInsertTable = "INSERT INTO TestTable VALUES(?)";
            const string DdlAlterTable = "ALTER TABLE TestTable ADD COLUMN Col2 bigint";
            const string DdlCreateView = "CREATE VIEW TestView (Col1View) AS SELECT Col1 AS Col1View FROM TestTable";
            const string DdlDropIfExists = "DROP TABLE IF EXISTS TestTable";
            //var byteArray = new byte[10];
            using (var connection = new SpliceDbConnection())
            {
                connection.ConnectionString = "uid=" + userName + ";pwd=" + password + ";host=" + hostName + ";port=" + port + "";
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    //command.CommandText = DdlCreateTable;
                    //var result = command.ExecuteNonQuery();
                    //connection.Commit();
                    command.CommandText = DdlInsertTable;
                    var byt = new byte[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
                    command.Parameters.Add(new SpliceDbParameter { Value = byt, DbType = DbType.Binary, IsNullable = true });
                    var result = command.ExecuteNonQuery();
                    connection.Commit();
                    //command.CommandText = DdlAlterTable;
                    //result = command.ExecuteNonQuery();
                    //connection.Commit();
                    //command.CommandText = DdlCreateView;
                    //result = command.ExecuteNonQuery();
                    //connection.Commit();
                    //command.CommandText = DdlDropIfExists;
                    //result = command.ExecuteNonQuery();
                    //connection.Commit();
                }
                connection.Close();
            }
        }
        #endregion

        #region SELECT Execution
        [TestMethod]
        public void TestDDLSelect()
        {
            const string DdlSelectTable = "SELECT * FROM TestTable";
            const string DdlSelectView = "SELECT * FROM TESTVIEW";
            using (var connection = new SpliceDbConnection())
            {
                connection.ConnectionString = "uid=" + userName + ";pwd=" + password + ";host=" + hostName + ";port=" + port + "";
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = DdlSelectTable;
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var obj1 = reader[0];
                    }
                }
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = DdlSelectView;
                    var spliceDbAdapt = new SpliceDbDataAdapter();
                    spliceDbAdapt.SelectCommand = command;
                    var dataTble = new DataTable();
                    spliceDbAdapt.Fill(dataTble);
                }
                connection.Close();
            }
        }

        #endregion

        #region Procedure Execution

        [TestMethod]
        public void TestProcedures()
        {
            const string DdlProcedureCall1 = "CALL SYSCS_UTIL.SHOW_CREATE_TABLE( 'SPLICE', 'TESTTABLE')";
            const string DdlProcedureCall2 = "CALL SYSCS_UTIL.SYSCS_GET_ALL_PROPERTIES()";
            using (var connection = new SpliceDbConnection())
            {
                connection.ConnectionString = "uid=" + userName + ";pwd=" + password + ";host=" + hostName + ";port=" + port + "";
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = DdlProcedureCall1;
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var obj1 = reader[0];
                    }

                }
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = DdlProcedureCall2;
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var obj1 = reader[0];
                        var obj2 = reader[1];
                        var obj3 = reader[2];
                    }

                }
                connection.Close();
            }
        }

        #endregion

        #region DML SELECT

        [TestMethod]
        public void TestDML()
        {
            const string DmlCreateQuery = "CREATE TABLE TestTable(Col1 BIGINT,Col2 BLOB,Col3 BOOLEAN,Col4 CHAR(2), Col5 CLOB(6535),Col6 DATE,Col7 DECIMAL,Col8 DOUBLE,Col9 FLOAT,Col10 INT,Col11 NUMERIC, Col12 REAL, Col13 SMALLINT, Col14 TEXT(500),Col15 TIME, Col16 TIMESTAMP, Col17 TINYINT, Col18 VARCHAR(200))";
            const string DmlInsertQuery = "INSERT INTO TestTable VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";

            using (var connection = new SpliceDbConnection())
            {
                connection.ConnectionString = "uid=" + userName + ";pwd=" + password + ";host=" + hostName + ";port=" + port + "";
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = DmlCreateQuery;
                    var result = command.ExecuteNonQuery();
                    command.CommandText = DmlInsertQuery;
                    var blobSample = new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };
                    command.Parameters.Add(new SpliceDbParameter() { Value = 10 });
                    command.Parameters.Add(new SpliceDbParameter() { Value = blobSample });
                    command.Parameters.Add(new SpliceDbParameter() { Value = true });
                    command.Parameters.Add(new SpliceDbParameter() { Value = 'A' });
                    command.Parameters.Add(new SpliceDbParameter() { Value = "TUlNRS1WZXJzaW9uOiAxLjANClgtTWFpbGVyOiBNYWlsQmVlLk5FVCA4LjAuNC40MjgNClN1YmplY3Q6IHRlc3Qgc3ViamVjdA0KVG86IGtldmlubUBkYXRhbW90aW9uLmNvbQ0KQ29udGVudC1UeXBlOiBtdWx0aXBhcnQvYWx0ZXJuYXRpdmU7DQoJYm91bmRhcnk9Ii0tLS09X05leHRQYXJ0XzAwMF9BRTZCXzcyNUUwOUFGLjg4QjdGOTM0Ig0KDQoNCi0tLS0tLT1fTmV4dFBhcnRfMDAwX0FFNkJfNzI1RTA5QUYuODhCN0Y5MzQNCkNvbnRlbnQtVHlwZTogdGV4dC9wbGFpbjsNCgljaGFyc2V0PSJ1dGYtOCINCkNvbnRlbnQtVHJhbnNmZXItRW5jb2Rpbmc6IHF1b3RlZC1wcmludGFibGUNCg0KdGVzdCBib2R5DQotLS0tLS09X05leHRQYXJ0XzAwMF9BRTZCXzcyNUUwOUFGLjg4QjdGOTM0DQpDb250ZW50LVR5cGU6IHRleHQvaHRtbDsNCgljaGFyc2V0PSJ1dGYtOCINCkNvbnRlbnQtVHJhbnNmZXItRW5jb2Rpbmc6IHF1b3RlZC1wcmludGFibGUNCg0KPHByZT50ZXN0IGJvZHk8L3ByZT4NCi0tLS0tLT1fTmV4dFBhcnRfMDAwX0FFNkJfNzI1RTA5QUYuODhCN0Y5MzQtLQ0K" });
                    command.Parameters.Add(new SpliceDbParameter() { Value = "02/16/2021" });
                    command.Parameters.Add(new SpliceDbParameter() { Value = 2.1 });
                    command.Parameters.Add(new SpliceDbParameter() { Value = 2.2 });
                    command.Parameters.Add(new SpliceDbParameter() { Value = 2.3 });
                    command.Parameters.Add(new SpliceDbParameter() { Value = 5 });
                    command.Parameters.Add(new SpliceDbParameter() { Value = 4 });
                    command.Parameters.Add(new SpliceDbParameter() { Value = 6 });
                    command.Parameters.Add(new SpliceDbParameter() { Value = 1 });
                    command.Parameters.Add(new SpliceDbParameter() { Value = "TUlNRS1WZXJzaW9uOiAxLjANClgtTWFpbGVyOiBNYWlsQmVlLk5FVCA4LjAuNC40MjgNClN1YmplY3Q6IHRlc3Qgc3ViamVjdA0KVG86IGtldmlubUBkYXRhbW90aW9uLmNvbQ0KQ29udGVudC1UeXBlOiBtdWx0aXBhcnQvYWx0ZXJuYXRpdmU7DQoJYm91bmRhcnk9Ii0tLS09X05leHRQYXJ0XzAwMF9BRTZCXzcyNUUwOUFGLjg4QjdGOTM0Ig0KDQoNCi0tLS0tLT1fTmV4dFBhcnRfMDAwX0FFNkJfNzI1RTA5QUYuODhCN0Y5MzQNCkNvbnRlbnQtVHlwZTogdGV4dC9wbGFpbjsNCgljaGFyc2V0PSJ1dGYtOCINCkNvbnRlbnQtVHJhbnNmZXItRW5jb2Rpbmc6IHF1b3RlZC1wcmludGFibGUNCg0KdGVzdCBib2R5DQotLS0tLS09X05leHRQYXJ0XzAwMF9BRTZCXzcyNUUwOUFGLjg4QjdGOTM0DQpDb250ZW50LVR5cGU6IHRleHQvaHRtbDsNCgljaGFyc2V0PSJ1dGYtOCINCkNvbnRlbnQtVHJhbnNmZXItRW5jb2Rpbmc6IHF1b3RlZC1wcmludGFibGUNCg0KPHByZT50ZXN0IGJvZHk8L3ByZT4NCi0tLS0tLT1fTmV4dFBhcnRfMDAwX0FFNkJfNzI1RTA5QUYuODhCN0Y5MzQtLQ0K" });
                    command.Parameters.Add(new SpliceDbParameter() { Value = "15:09:02" });
                    command.Parameters.Add(new SpliceDbParameter() { Value = "02/16/2021" });
                    command.Parameters.Add(new SpliceDbParameter() { Value = 1 });
                    command.Parameters.Add(new SpliceDbParameter() { Value = "HELLO" });
                    var result1 = command.ExecuteNonQuery();

                }

                connection.Close();
            }
        }
        #endregion

    }
}
