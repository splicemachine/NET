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
            CommAreaRowDescMessage message)
        {
            TraceInformation(
                $"SQLCARD: '{String.Join(" / ", message.SqlMessages)}' [{message.RowsUpdated}]");
            return true;
        }

        public Boolean Visit(
            DescAreaRowDescMessage message)
        {
            foreach (var column in message.Columns)
            {
                TraceInformation(
                    $"Column: {column.BaseName}.{column.Name}");
            }

            _context.Columns.AddRange(message.Columns);
            return true;
        }

        public Boolean Visit(
            QueryAnswerSetDataMessage message)
        {
            TraceInformation($"QRYDTA: {message.RequestCorrelationId}");
            message.Process(_context);
            return true;
        }

        public Boolean Visit(
            QueryAnswerSetExtraMessage message)
        {
            TraceInformation($"EXTDTA: {message.RequestCorrelationId}");
            return true;
        }

        public Boolean Visit(
            QueryAnswerSetDescMessage message)
        {
            TraceInformation($"QRYDSC: {message.RequestCorrelationId}");
            message.Process(_context);
            return true;
        }

        public Boolean Visit(
            SqlResultSetColumnsMessage message)
        {
            if (_context.IsColumnsListFinalized)
            {
                return true;
            }

            foreach (var column in message.Columns)
            {
                TraceInformation(
                    $"Column: {column.BaseName}.{column.Name}");
            }

            _context.Columns.AddRange(message.Columns);
            return true;
        }

        public Boolean Visit(
            SqlResultSetDataMessage message)
        {
            foreach (var resultSet in message.ResultSets)
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