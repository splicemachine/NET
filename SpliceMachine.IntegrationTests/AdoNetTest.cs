﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpliceMachine.Provider;
using System;
using System.Data;

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
            const string DdlCreateTable = "CREATE TABLE TestTable(COL1 INT)";
            const string DdlInsertTable = "INSERT INTO TestTable VALUES(1)";
            const string DdlAlterTable = "ALTER TABLE TestTable ADD COLUMN Col2 bigint";
            const string DdlCreateView = "CREATE VIEW TestView (Col1View) AS SELECT Col1 AS Col1View FROM TestTable";
            const string DdlDropIfExists = "DROP TABLE IF EXISTS TestTable";

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
                    command.CommandText = DdlAlterTable;
                    result = command.ExecuteNonQuery();
                    connection.Commit();
                    command.CommandText = DdlCreateView;
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
            const string DmlCreateQuery = "CREATE TABLE TestTable(Col1 BIGINT,Col2 BLOB,Col3 BOOLEAN,Col4 CHAR(2), Col5 CLOB(6535),Col6 DATE,Col7 DECIMAL,Col8 DOUBLE,Col9 FLOAT,Col10 INT,Col11 NUMERIC, Col12 REAL, Col13 SMALLINT,Col15 TIME, Col16 TIMESTAMP, Col17 TINYINT, Col18 VARCHAR(200))";
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
                    
                }

                connection.Close();
            }
        }
        #endregion

        #region All data types test

        [TestMethod]
        public void TestAllDtypes()
        {
            const string DmlCreateQuery = "CREATE TABLE TestTable(Col1 BIGINT,Col3 BOOLEAN,Col4 CHAR(2), Col6 DATE,Col7 DECIMAL(5,2),Col8 DOUBLE,Col9 FLOAT,Col10 INT,Col11 NUMERIC(5,2), Col12 REAL, Col13 SMALLINT, Col15 TIME, Col16 TIMESTAMP, Col17 TINYINT, Col18 VARCHAR(200))";
            const string DmlInsertQuery = "INSERT INTO TestTable VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";
            const string DdlSelectQuery = "Select * from TESTTABLE";

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
                    command.Parameters.Add(new SpliceDbParameter() { Value = 10 });//BIGINT
                    //command.Parameters.Add(new SpliceDbParameter() { Value = blobSample });//BLOB
                    command.Parameters.Add(new SpliceDbParameter() { Value = true });//BOOL
                    command.Parameters.Add(new SpliceDbParameter() { Value = 'A' });//CHAR
                    //command.Parameters.Add(new SpliceDbParameter() { Value = "TUlNRS1WZXJzaW9uOiAxLjANClgtTWFpbGVyOiBNYWlsQmVlLk5FVCA4LjAuNC40MjgNClN1YmplY3Q6IHRlc3Qgc3ViamVjdA0KVG86IGtldmlubUBkYXRhbW90aW9uLmNvbQ0KQ29udGVudC1UeXBlOiBtdWx0aXBhcnQvYWx0ZXJuYXRpdmU7DQoJYm91bmRhcnk9Ii0tLS09X05leHRQYXJ0XzAwMF9BRTZCXzcyNUUwOUFGLjg4QjdGOTM0Ig0KDQoNCi0tLS0tLT1fTmV4dFBhcnRfMDAwX0FFNkJfNzI1RTA5QUYuODhCN0Y5MzQNCkNvbnRlbnQtVHlwZTogdGV4dC9wbGFpbjsNCgljaGFyc2V0PSJ1dGYtOCINCkNvbnRlbnQtVHJhbnNmZXItRW5jb2Rpbmc6IHF1b3RlZC1wcmludGFibGUNCg0KdGVzdCBib2R5DQotLS0tLS09X05leHRQYXJ0XzAwMF9BRTZCXzcyNUUwOUFGLjg4QjdGOTM0DQpDb250ZW50LVR5cGU6IHRleHQvaHRtbDsNCgljaGFyc2V0PSJ1dGYtOCINCkNvbnRlbnQtVHJhbnNmZXItRW5jb2Rpbmc6IHF1b3RlZC1wcmludGFibGUNCg0KPHByZT50ZXN0IGJvZHk8L3ByZT4NCi0tLS0tLT1fTmV4dFBhcnRfMDAwX0FFNkJfNzI1RTA5QUYuODhCN0Y5MzQtLQ0K" });//CLOB
                    command.Parameters.Add(new SpliceDbParameter() { Value = "02/16/2021" });//DATE
                    command.Parameters.Add(new SpliceDbParameter() { Value = 2.1 });//DECIMAL
                    command.Parameters.Add(new SpliceDbParameter() { Value = 2.2 });//DOUBLE
                    command.Parameters.Add(new SpliceDbParameter() { Value = 2.3 });//FLOAT
                    command.Parameters.Add(new SpliceDbParameter() { Value = 5 });//INT
                    command.Parameters.Add(new SpliceDbParameter() { Value = 4 });//NUMERIC(5,2)
                    command.Parameters.Add(new SpliceDbParameter() { Value = 6 });//REAL
                    command.Parameters.Add(new SpliceDbParameter() { Value = 1 });//SMALLINT
                    //command.Parameters.Add(new SpliceDbParameter() { Value = "TUlNRS1WZXJzaW9uOiAxLjANClgtTWFpbGVyOiBNYWlsQmVlLk5FVCA4LjAuNC40MjgNClN1YmplY3Q6IHRlc3Qgc3ViamVjdA0KVG86IGtldmlubUBkYXRhbW90aW9uLmNvbQ0KQ29udGVudC1UeXBlOiBtdWx0aXBhcnQvYWx0ZXJuYXRpdmU7DQoJYm91bmRhcnk9Ii0tLS09X05leHRQYXJ0XzAwMF9BRTZCXzcyNUUwOUFGLjg4QjdGOTM0Ig0KDQoNCi0tLS0tLT1fTmV4dFBhcnRfMDAwX0FFNkJfNzI1RTA5QUYuODhCN0Y5MzQNCkNvbnRlbnQtVHlwZTogdGV4dC9wbGFpbjsNCgljaGFyc2V0PSJ1dGYtOCINCkNvbnRlbnQtVHJhbnNmZXItRW5jb2Rpbmc6IHF1b3RlZC1wcmludGFibGUNCg0KdGVzdCBib2R5DQotLS0tLS09X05leHRQYXJ0XzAwMF9BRTZCXzcyNUUwOUFGLjg4QjdGOTM0DQpDb250ZW50LVR5cGU6IHRleHQvaHRtbDsNCgljaGFyc2V0PSJ1dGYtOCINCkNvbnRlbnQtVHJhbnNmZXItRW5jb2Rpbmc6IHF1b3RlZC1wcmludGFibGUNCg0KPHByZT50ZXN0IGJvZHk8L3ByZT4NCi0tLS0tLT1fTmV4dFBhcnRfMDAwX0FFNkJfNzI1RTA5QUYuODhCN0Y5MzQtLQ0K" });//TEXT
                    command.Parameters.Add(new SpliceDbParameter() { Value = "03:09:02" });//TIME
                    command.Parameters.Add(new SpliceDbParameter() { Value = DateTime.Now });//TIMESTAMP
                    command.Parameters.Add(new SpliceDbParameter() { Value = 1 });//TINYINT                    
                    command.Parameters.Add(new SpliceDbParameter() { Value = "HELLO" });
                    result = command.ExecuteNonQuery();
                    using (var command1 = connection.CreateCommand())
                    {
                        command1.CommandText = DdlSelectQuery;
                        var reader = command1.ExecuteReader();
                        while (reader.Read())
                        {
                            var obj1 = reader[0];
                            var obj2 = reader[1];
                            var obj3 = reader[2];
                            var obj4 = reader[3];
                            var obj5 = reader[4];
                            var obj6 = reader[5];
                            var obj7 = reader[6];
                            var obj8 = reader[7];
                            var obj9 = reader[8];
                            var obj10 = reader[9];
                            var obj11 = reader[10];
                            var obj12 = reader[11];
                            var obj13 = reader[12];
                            var obj14 = reader[13];
                            var obj15 = reader[14];
                        }

                    }
                    using (var command2 = connection.CreateCommand())
                    {
                        command2.CommandText = DdlSelectQuery;
                        var spliceDbAdapt = new SpliceDbDataAdapter();
                        spliceDbAdapt.SelectCommand = command2;
                        var dataTble = new DataTable();
                        spliceDbAdapt.Fill(dataTble);
                    }
                }

                connection.Close();
            }
        }
        #endregion

    }
}
