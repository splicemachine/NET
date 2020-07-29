using System;

namespace SpliceMachine.Drda
{
    using static System.Diagnostics.Trace;

    internal sealed class DrdaImmediateStatement : IDrdaStatement
    {
        private readonly DrdaConnection _connection;

        private readonly String _sqlStatement;

        public DrdaImmediateStatement(
            DrdaConnection connection, 
            String sqlStatement)
        {
            _sqlStatement = sqlStatement;
            _connection = connection;
        }

        public IDrdaStatement Prepare() => 
            new DrdaPreparedStatement(_connection, _sqlStatement);

        public Boolean Execute()
        {
            var requestCorrelationId = _connection.GetNextRequestCorrelationId();
            var packageSerialNumber = _connection.GetNextPackageSerialNumber();

            var stream = _connection.GetStream();
            stream
                .SendRequest(
                    new ExecuteImmediateSqlRequest(requestCorrelationId, packageSerialNumber))
                .SendRequest(
                    new SqlStatementRequest(requestCorrelationId, _sqlStatement));

            DrdaResponseBase message;
            do
            {
                message = stream.ReadResponse();
                switch (message)
                {
                    case CommandCheckResponse response:
                        TraceInformation($"\tCMDCHKRM: {response.SeverityCode}");
                        break;

                    case SqlErrorResponse response:
                        TraceInformation($"\tSQLERRRM: {response.SeverityCode}");
                        break;

                    case RelationalDatabaseUpdateResponse response:
                        TraceInformation($"\tRDBUPDRM: {response.SeverityCode}");
                        break;

                    case CommAreaRowDescResponse response:
                        TraceInformation(
                            $"\tSQLCARD: '{String.Join(" / ", response.SqlMessages)}' [{response.RowsUpdated}]");
                        break;

                    case PiggyBackSchemaDescResponse response:
                        TraceInformation($"\tPBSD: {response.IsolationLevel} @ {response.Schema}");
                        break;

                    default:
                        TraceWarning($"\tUnknown message: {message.RequestCorrelationId}");
                        return false;
                }
            }
            // ReSharper disable once ConstantConditionalAccessQualifier
            while (message?.IsChained ?? false);

            return true;
        }
    }
}