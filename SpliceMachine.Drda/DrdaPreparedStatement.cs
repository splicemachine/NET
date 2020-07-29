using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace SpliceMachine.Drda
{
    using static System.Diagnostics.Trace;

    internal sealed class DrdaPreparedStatement : IDrdaStatement
    {
        private readonly List<DrdaColumn> _columns = new List<DrdaColumn>();

        private readonly Boolean _isPackagePreparedSuccessfully;

        private readonly UInt16 _packageSerialNumber;

        private readonly DrdaConnection _connection;

        private readonly String _sqlStatement;

        private UInt64? _queryInstanceId;

        private Boolean _hasMoreData;

        public DrdaPreparedStatement(
            DrdaConnection connection, 
            String sqlStatement)
        {
            _sqlStatement = sqlStatement;
            _connection = connection;
            
            _packageSerialNumber = _connection
                .GetNextPackageSerialNumber();

            _isPackagePreparedSuccessfully = PreparePackage
                (_connection.GetStream(), 
                _connection.GetNextRequestCorrelationId());
        }

        public IDrdaStatement Prepare() => this;

        public Boolean Execute()
        {
            var stream = _connection.GetStream();

            if (!_isPackagePreparedSuccessfully ||
                !ExecutePackage(stream, _connection.GetNextRequestCorrelationId()))
            {
                return false;
            }

            while (_hasMoreData)
            {
                if (!ContinueQuery(stream, _connection.GetNextRequestCorrelationId()))
                {
                    return false;
                }
            }

            return CloseQuery(stream, _connection.GetNextRequestCorrelationId());
        }

        private Boolean PreparePackage(in NetworkStream stream, in UInt16 requestCorrelationId)
        {
            stream
                .SendRequest(
                    new PrepareSqlStatementRequest(requestCorrelationId, _packageSerialNumber))
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

                        _columns.AddRange(response.Columns);
                        break;

                    default:
                        return false;
                }
            }
            // ReSharper disable once ConstantConditionalAccessQualifier
            while (message?.IsChained ?? false);

            // TODO: olegra - add parameters description request/response processing
            return true;
        }

        private Boolean ExecutePackage(in NetworkStream stream, in UInt16 requestCorrelationId)
        {
            stream.SendRequest(
                new ExecutePreparedSqlRequest(requestCorrelationId, _packageSerialNumber, false));

            // TODO: olegra - add parameters values chaining 

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

                        _columns.AddRange(response.Columns);
                        break;

                    case RelationalDatabaseResultSetResponse response:
                        TraceInformation($"\tRSLSETRM: {response.SeverityCode}");
                        break;

                    case OpenQueryCompleteResponse response:
                        TraceInformation($"\tOPNQRYRM: {response.QueryInstanceId}");
                        _queryInstanceId = response.QueryInstanceId;
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

                        _columns.AddRange(response.Columns);
                        break;

                    case QueryAnswerSetDescResponse response:
                        // TODO: olegra - process triplets here!!!
                        break;

                    case QueryAnswerSetDataResponse response:
                        // TODO: olegra - process row data here!!!
                        _hasMoreData = response.HasMoreData;
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

            return true;
        }

        private Boolean ContinueQuery(in NetworkStream stream, in UInt16 requestCorrelationId)
        {
            if (_queryInstanceId is null)
            {
                return true;
            }

            stream.SendRequest(
                new ContinueQueryRequest(requestCorrelationId, _packageSerialNumber, _queryInstanceId.Value));

            DrdaResponseBase message;
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

            return true;
        }
        
        private Boolean CloseQuery(in NetworkStream stream, in UInt16 requestCorrelationId)
        {
            if (_queryInstanceId is null)
            {
                return true;
            }

            stream.SendRequest(
                new CloseQueryRequest(requestCorrelationId, _packageSerialNumber, _queryInstanceId.Value));

            DrdaResponseBase message;
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

            return true;
        }

    }
}
