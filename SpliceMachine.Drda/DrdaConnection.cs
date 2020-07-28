using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SpliceMachine.Drda
{
    using static System.Diagnostics.Trace;

    public sealed class DrdaConnection : IDisposable
    {
        private readonly TcpClient _client = new TcpClient(AddressFamily.InterNetwork);

        private readonly DrdaConnectionOptions _options;

        private Int32 _requestCorrelationId;

        private UInt16 _packageSerialNumber;

        public DrdaConnection(
            DrdaConnectionOptions options)
        {
            _options = options;
        }

        public void Dispose() => (_client as IDisposable)?.Dispose();

        public async Task ConnectAsync()
        {
            await _client
                .ConnectAsync(_options.HostName, _options.Port)
                .ConfigureAwait(false);

            var stream = _client.GetStream();

            stream
                .RequestResponseSequence(
                    new ExchangeServerAttributesRequest(++_requestCorrelationId), out _)
                .RequestResponseSequence(
                    new AccessSecurityDataRequest(++_requestCorrelationId), out _)
                .RequestResponseSequence(
                    new SecurityCheckRequest(++_requestCorrelationId,
                        _options.UserName, _options.Password), out _)
                .RequestResponseSequence(
                    new AccessRelationalDatabaseRequest(++_requestCorrelationId,
                        _client.Client.LocalEndPoint), out var isChained);

            if (isChained &&
                stream.ReadResponse() is PiggyBackSchemaDescResponse response)
            {
                TraceInformation($"\tPBSD: {response.IsolationLevel} @ {response.Schema}");
            }
        }

        public Boolean ExecuteImmediateSql(
            String sqlStatement)
        {
            var requestCorrelationId = ++_requestCorrelationId;
            var packageSerialNumber = ++_packageSerialNumber;

            var stream = _client.GetStream();
            stream
                .SendRequest(
                    new ExecuteImmediateSqlRequest(requestCorrelationId, packageSerialNumber))
                .SendRequest(
                    new SqlStatementRequest(requestCorrelationId, sqlStatement));

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
                        return false;
                }
            }
            // ReSharper disable once ConstantConditionalAccessQualifier
            while (message?.IsChained ?? false);

            return true;
        }

        public Boolean ExecutePreparedSql(
            String sqlStatement)
        {
            List<DrdaColumn> columns = new List<DrdaColumn>();

            var requestCorrelationId = ++_requestCorrelationId;
            var packageSerialNumber = ++_packageSerialNumber;

            var stream = _client.GetStream();
            stream
                .SendRequest(
                    new PrepareSqlStatementRequest(requestCorrelationId, packageSerialNumber))
                .SendRequest(
                    new SqlStatementRequest(requestCorrelationId, sqlStatement));

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

                    case CommAreaRowDescResponse response:
                        TraceInformation(
                            $"\tSQLCARD: '{String.Join(" / ", response.SqlMessages)}' [{response.RowsUpdated}]");
                        break;

                    case DescAreaRowDescResponse response:
                        foreach (var column in response.Columns)
                        {
                            TraceInformation(
                                $"\tColumn: {column.BaseName}.{column.Name}");
                        }

                        columns.AddRange(response.Columns);
                        break;

                    default:
                        return false;
                }
            }
            // ReSharper disable once ConstantConditionalAccessQualifier
            while (message?.IsChained ?? false);

            // TODO: olegra - add parameters description request/response processing

            requestCorrelationId = ++_requestCorrelationId;

            stream
                .SendRequest(
                    new ExecutePreparedSqlRequest(requestCorrelationId, packageSerialNumber, false));

            // TODO: olegra - add parameters values chaining 

            var hasMoreData = false;
            UInt64? queryInstanceId = null;

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

                    case CommAreaRowDescResponse response:
                        TraceInformation(
                            $"\tSQLCARD: '{String.Join(" / ", response.SqlMessages)}' [{response.RowsUpdated}]");
                        break;

                    case DescAreaRowDescResponse response:
                        foreach (var column in response.Columns)
                        {
                            TraceInformation(
                                $"\tColumn: {column.BaseName}.{column.Name}");
                        }
                        columns.AddRange(response.Columns);
                        break;

                    case RelationalDatabaseResultSetResponse response:
                        TraceInformation($"\tRSLSETRM: {response.SeverityCode}");
                        break;

                    case OpenQueryCompleteResponse response:
                        TraceInformation($"\tOPNQRYRM: {response.QueryInstanceId}");
                        queryInstanceId = response.QueryInstanceId;
                        break;

                    case SqlResultSetDataResponse response:
                        foreach (var resultSet in response.ResultSets)
                        {
                            TraceInformation(
                                $"\tCursor: {resultSet.CursorName} -> {resultSet.Rows}");
                        }
                        break;

                    case SqlResultSetColumnInfoResponse response:
                        foreach (var column in response.Columns)
                        {
                            TraceInformation(
                                $"\tColumn: {column.BaseName}.{column.Name}");
                        }
                        columns.AddRange(response.Columns);
                        break;
                    
                    case QueryAnswerSetDescResponse response:
                        // TODO: olegra - process triplets here!!!
                        break;
                                        
                    case QueryAnswerSetDataResponse response:
                        // TODO: olegra - process row data here!!!
                        hasMoreData = response.HasMoreData;
                        break;

                    case EndUnitOfWorkResponse response:
                        TraceInformation($"\tRSLSETRM: {response.SeverityCode}");
                        break;

                    case PiggyBackSchemaDescResponse response:
                        TraceInformation($"\tPBSD: {response.IsolationLevel} @ {response.Schema}");
                        break;

                    default:
                        return false;
                }
            }
            // ReSharper disable once ConstantConditionalAccessQualifier
            while (message?.IsChained ?? false);

            while (hasMoreData && queryInstanceId.HasValue)
            {
                stream.SendRequest(new
                    ContinueQueryRequest(++_requestCorrelationId, _packageSerialNumber, queryInstanceId.Value));
                do
                {
                    message = stream.ReadResponse();
                    switch (message)
                    {
                        case SqlErrorResponse response:
                            TraceInformation($"\tSQLERRRM: {response.SeverityCode}");
                            break;

                        case CommAreaRowDescResponse response:
                            TraceInformation(
                                $"\tSQLCARD: '{String.Join(" / ", response.SqlMessages)}' [{response.RowsUpdated}]");
                            break;
                                            
                        case QueryAnswerSetDataResponse response:
                            // TODO: olegra - process row data here!!!
                            break;
                                            
                        case QueryAnswerSetExtraDataResponse response:
                            // TODO: olegra - process extra row data here!!!
                            break;

                        default:
                            return false;
                    }
                }
                // ReSharper disable once ConstantConditionalAccessQualifier
                while (message?.IsChained ?? false);
            }

            if (queryInstanceId.HasValue)
            {
                stream.SendRequest(
                    new CloseQueryRequest(++_requestCorrelationId, _packageSerialNumber, queryInstanceId.Value));
                do
                {
                    message = stream.ReadResponse();
                    switch (message)
                    {
                        case SqlErrorResponse response:
                            TraceInformation($"\tSQLERRRM: {response.SeverityCode}");
                            break;

                        case CommAreaRowDescResponse response:
                            TraceInformation(
                                $"\tSQLCARD: '{String.Join(" / ", response.SqlMessages)}' [{response.RowsUpdated}]");
                            break;

                        default:
                            return false;
                    }
                }
                // ReSharper disable once ConstantConditionalAccessQualifier
                while (message?.IsChained ?? false);
            }

            return true;
        }
    }
}
