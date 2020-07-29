using System;
using System.Collections.Generic;
using System.IO;

namespace SpliceMachine.Drda
{
    using static System.Diagnostics.Trace;
    using static SeverityCodes;

    internal sealed class DrdaStatementVisitor
    {
        private readonly ISet<CodePoint> _allowedCodePoints;

        private readonly QueryContext _context;

        public DrdaStatementVisitor(
            ISet<CodePoint> allowedCodePoints,
            QueryContext context)
        {
            _allowedCodePoints = allowedCodePoints;
            _context = context;
        }

        public Boolean ProcessChainedResponses(
            Stream stream)
        {
            var isChained = true;
            _context.SeverityCode = Info;

            while (isChained)
            {
                var message = stream.ReadResponse();

                if (_allowedCodePoints.Contains(message.CodePoint) &&
                    message.Accept(this))
                {
                    isChained = message.IsChained;
                    continue;
                }

                TraceWarning($"Unknown message: {message.CodePoint} / {message.RequestCorrelationId}");
                return false;
            }

            return _context.SeverityCode <= Warning;
        }

        public Boolean Visit(
            SqlErrorResponse response)
        {
            TraceInformation($"SQLERRRM: SC = {response.SeverityCode}");
            _context.SeverityCode = response.SeverityCode;
            return true;
        }

        public Boolean Visit(
            CommandCheckResponse response)
        {
            TraceInformation($"CMDCHKRM: SC = {response.SeverityCode}");
            _context.SeverityCode = response.SeverityCode;
            return true;
        }

        public Boolean Visit(
            RelationalDatabaseResultSetResponse response)
        {
            TraceInformation($"RSLSETRM: SC = {response.SeverityCode}");
            _context.SeverityCode = response.SeverityCode;
            return true;
        }

        public Boolean Visit(
            RelationalDatabaseUpdateResponse response)
        {
            TraceInformation($"RDBUPDRM: SC = {response.SeverityCode}");
            _context.SeverityCode = response.SeverityCode;
            return true;
        }

        public Boolean Visit(
            PiggyBackSchemaDescResponse response)
        {
            TraceInformation($"PBSD: {response.IsolationLevel} @ {response.Schema}");
            return true;
        }

        public Boolean Visit(
            CommAreaRowDescResponse response)
        {
            TraceInformation(
                $"SQLCARD: '{String.Join(" / ", response.SqlMessages)}' [{response.RowsUpdated}]");
            return true;
        }

        public Boolean Visit(
            DescAreaRowDescResponse response)
        {
            foreach (var column in response.Columns)
            {
                TraceInformation(
                    $"Column: {column.BaseName}.{column.Name}");
            }

            _context.Columns.AddRange(response.Columns);
            return true;
        }

        public Boolean Visit(
            QueryAnswerSetDataResponse response)
        {
            TraceInformation($"QRYDTA: HMD = {response.HasMoreData}");
            _context.HasMoreData = response.HasMoreData;
            return true;
        }

        public Boolean Visit(
            QueryAnswerSetExtraDataResponse response)
        {
            TraceInformation($"EXTDTA: {response.RequestCorrelationId}");
            return true;
        }

        public Boolean Visit(
            QueryAnswerSetDescResponse response)
        {
            TraceInformation($"QRYDSC: {response.RequestCorrelationId}");
            return true;
        }

        public Boolean Visit(
            SqlResultSetColumnInfoResponse response)
        {
            foreach (var column in response.Columns)
            {
                TraceInformation(
                    $"Column: {column.BaseName}.{column.Name}");
            }

            _context.Columns.AddRange(response.Columns);
            return true;
        }

        public Boolean Visit(
            SqlResultSetDataResponse response)
        {
            foreach (var resultSet in response.ResultSets)
            {
                TraceInformation(
                    $"Cursor: {resultSet.CursorName} -> {resultSet.Rows}");
            }
            return true;
        }

        public Boolean Visit(
            OpenQueryCompleteResponse response)
        {
            TraceInformation($"OPNQRYRM: {response.QueryInstanceId}");
            _context.QueryInstanceId = response.QueryInstanceId;
            return true;
        }
    }
}