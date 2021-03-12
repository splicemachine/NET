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
        private static string hostName = "localhost";
        private static int port = 1527;
        private static string userName = "splice";
        private static string password = "admin";
        #endregion

        #region CREATE/DROP Views/Tables

        [TestMethod]
        public void TestCreateDrop()
        {
            const string QryCreateTable = "CREATE TABLE TestTable(COL1 INT)";
            const string QryInsertTable = "INSERT INTO TestTable VALUES(1)";
            const string QryAlterTable = "ALTER TABLE TestTable ADD COLUMN Col2 bigint";
            const string QryCreateView = "CREATE VIEW TestView (Col1View) AS SELECT Col1 AS Col1View FROM TestTable";
            const string QryDropIfExists = "DROP TABLE IF EXISTS TestTable";

            using (var connection = new SpliceDbConnection())
            {
                connection.ConnectionString = "uid=" + userName + ";pwd=" + password + ";host=" + hostName + ";port=" + port + "";
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = QryCreateTable;
                    var result = command.ExecuteNonQuery();
                    command.CommandText = QryInsertTable;
                    result = command.ExecuteNonQuery();
                }
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = QryAlterTable;
                    var result = command.ExecuteNonQuery();
                    command.CommandText = QryCreateView;
                    result = command.ExecuteNonQuery();
                    command.CommandText = QryDropIfExists;
                    result = command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        #endregion

        #region SELECT TABLES/VIEWS
        [TestMethod]
        public void TestSelect()
        {
            const string QrySelectTable = "SELECT * FROM TestTable";
            const string QrySelectView = "SELECT * FROM TESTVIEW";
            using (var connection = new SpliceDbConnection())
            {
                connection.ConnectionString = "uid=" + userName + ";pwd=" + password + ";host=" + hostName + ";port=" + port + "";
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = QrySelectTable;
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var obj1 = reader[0];
                    }
                }
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = QrySelectView;
                    var spliceDbAdapt = new SpliceDbDataAdapter();
                    spliceDbAdapt.SelectCommand = command;
                    var dataTble = new DataTable();
                    spliceDbAdapt.Fill(dataTble);
                }
                connection.Close();
            }
        }

        #endregion

        #region Execute procedures/functions

        [TestMethod]
        public void TestProcedures()
        {
            const string QryProcedureCall1 = "CALL SYSCS_UTIL.SHOW_CREATE_TABLE(?,?)";
            const string QryProcedureCall2 = "CALL SYSCS_UTIL.SYSCS_GET_ALL_PROPERTIES()";
            using (var connection = new SpliceDbConnection())
            {
                connection.ConnectionString = "uid=" + userName + ";pwd=" + password + ";host=" + hostName + ";port=" + port + "";
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = QryProcedureCall1;
                    command.Parameters.Add(new SpliceDbParameter() { Value = "SPLICE" });
                    command.Parameters.Add(new SpliceDbParameter() { Value = "TESTTABLE" });
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var obj1 = reader[0];
                    }

                }
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = QryProcedureCall2;
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

        #region All data type checks

        [TestMethod]
        //This method will create a table with all data types
        //Do a insert into the table with test values
        //Select a table which has all data type values
        public void TestAllDataTypes()
        {
            const string QryCreateQuery = "CREATE TABLE TestTable(Col1 BIGINT,Col3 BOOLEAN,Col4 CHAR(2), Col6 DATE,Col7 DECIMAL(5,2),Col8 DOUBLE,Col9 FLOAT,Col10 INT,Col11 NUMERIC(5,2), Col12 REAL, Col13 SMALLINT, Col15 TIME, Col16 TIMESTAMP, Col17 TINYINT, Col18 VARCHAR(200))";
            const string QryInsertQuery = "INSERT INTO TestTable VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";
            const string QrySelectQuery = "Select * from TESTTABLE";

            using (var connection = new SpliceDbConnection())
            {
                connection.ConnectionString = "uid=" + userName + ";pwd=" + password + ";host=" + hostName + ";port=" + port + "";
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = QryCreateQuery;
                    var result = command.ExecuteNonQuery();
                    command.CommandText = QryInsertQuery;
                    command.Parameters.Add(new SpliceDbParameter() { Value = 10 });//BIGINT
                    command.Parameters.Add(new SpliceDbParameter() { Value = true });//BOOL
                    command.Parameters.Add(new SpliceDbParameter() { Value = 'A' });//CHAR
                    command.Parameters.Add(new SpliceDbParameter() { Value = "02/16/2021" });//DATE
                    command.Parameters.Add(new SpliceDbParameter() { Value = 2.1 });//DECIMAL
                    command.Parameters.Add(new SpliceDbParameter() { Value = 2.2 });//DOUBLE
                    command.Parameters.Add(new SpliceDbParameter() { Value = 2.3 });//FLOAT
                    command.Parameters.Add(new SpliceDbParameter() { Value = 5 });//INT
                    command.Parameters.Add(new SpliceDbParameter() { Value = 4 });//NUMERIC(5,2)
                    command.Parameters.Add(new SpliceDbParameter() { Value = 6 });//REAL
                    command.Parameters.Add(new SpliceDbParameter() { Value = 1 });//SMALLINT
                    command.Parameters.Add(new SpliceDbParameter() { Value = "03:09:02" });//TIME
                    command.Parameters.Add(new SpliceDbParameter() { Value = DateTime.Now });//TIMESTAMP
                    command.Parameters.Add(new SpliceDbParameter() { Value = 1 });//TINYINT                    
                    command.Parameters.Add(new SpliceDbParameter() { Value = "HELLO" });
                    result = command.ExecuteNonQuery();
                }
                using (var command1 = connection.CreateCommand())
                {
                    command1.CommandText = QrySelectQuery;
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
                    command2.CommandText = QrySelectQuery;
                    var spliceDbAdapt = new SpliceDbDataAdapter();
                    spliceDbAdapt.SelectCommand = command2;
                    var dataTble = new DataTable();
                    spliceDbAdapt.Fill(dataTble);
                }
                connection.Close();
            }
        }
        #endregion

        #region Null tests

        [TestMethod]
        public void TestNullValues()
        {
            const string QryCreateTable = "CREATE TABLE TESTTABLE(Col1 BIGINT,Col2 BOOLEAN)";
            const string QryInsertNull = "INSERT INTO TESTTABLE VALUES(?,?)";
            const string QrySelectNull = "SELECT * FROM TESTTABLE";
            using (var connection = new SpliceDbConnection())
            {
                connection.ConnectionString = "uid=" + userName + ";pwd=" + password + ";host=" + hostName + ";port=" + port + "";
                connection.Open();
                //Create table with two columns
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = QryCreateTable;
                    var result = command.ExecuteNonQuery();
                }
                //Insert null value to table along with a simple bool value
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = QryInsertNull;
                    command.Parameters.Add(new SpliceDbParameter { Value = null });
                    command.Parameters.Add(new SpliceDbParameter { Value = true });
                    var result = command.ExecuteNonQuery();
                }
                //Select the table 
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = QrySelectNull;
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var obj1 = reader[0];
                        var obj2 = reader[0];
                    }

                }
                connection.Close();
            }
        }

        #endregion

        #region Sequence tests

        [TestMethod]
        public void TestSequnces()
        {
            const string QryCreateTable = "CREATE TABLE TESTTABLE(Col1 BIGINT,Col2 BOOLEAN)";
            const string QryCreateSequence = "CREATE SEQUENCE test_sequence_id AS BIGINT START WITH 100";
            const string QryInsertSequnce = "INSERT INTO TESTTABLE VALUES(NEXT VALUE FOR test_sequence_id,?)";
            using (var connection = new SpliceDbConnection())
            {
                connection.ConnectionString = "uid=" + userName + ";pwd=" + password + ";host=" + hostName + ";port=" + port + "";
                connection.Open();
                //Create Table
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = QryCreateTable;
                    var result = command.ExecuteNonQuery();
                }
                //Create Sequnce
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = QryCreateSequence;
                    var result = command.ExecuteNonQuery();
                }
                //Insert Table with sequence
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = QryInsertSequnce;
                    command.Parameters.Add(new SpliceDbParameter() { Value = null });
                    var result = command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        #endregion

        #region Scale/Precesion/Tests 

        [TestMethod]
        public void TestScalePrecesion()
        {
            const string QryCreateTable = "CREATE TABLE TESTTABLE(COL1 BIGINT,COL2 INT,COL3 DECIMAL(16,5))";
            const string QryInsertValue = "INSERT INTO TESTTABLE VALUES(?,?,?)";
            using (var connection = new SpliceDbConnection())
            {
                connection.ConnectionString = "uid=" + userName + ";pwd=" + password + ";host=" + hostName + ";port=" + port + "";
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = QryCreateTable;
                    var reader = command.ExecuteNonQuery();
                }
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = QryInsertValue;
                    command.Parameters.Add(new SpliceDbParameter() { Value = 92233720368547758  });
                    command.Parameters.Add(new SpliceDbParameter() { Value = 2147483647 });
                    command.Parameters.Add(new SpliceDbParameter() { Value = 2147483647.21474 });
                    var reader = command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        #endregion

    }
}
