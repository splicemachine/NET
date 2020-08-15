using System.Data.Common;
using Simba.ADO.Net;

namespace SpliceMachine.Provider
{
    public sealed class SpliceDbFactory : SFactory
    {
        public static readonly SpliceDbFactory Instance = new SpliceDbFactory();

        private SpliceDbFactory()
        {
        }

        public override DbCommand CreateCommand() => 
            new SpliceDbCommand();

        public override DbCommandBuilder CreateCommandBuilder() => 
            new SpliceDbCommandBuilder();

        public override DbConnection CreateConnection() => 
            new SpliceDbConnection();

        public override DbConnectionStringBuilder CreateConnectionStringBuilder() => 
            new SpliceDbConnectionStringBuilder();

        public override DbDataAdapter CreateDataAdapter() => 
            new SpliceDbDataAdapter();

        public override DbParameter CreateParameter() => 
            new SpliceDbParameter();
    }
}
