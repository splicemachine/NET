﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace SpliceMachine.Drda
{
    using static CodePoint;

    internal sealed class DrdaPreparedStatement : IDrdaStatement
    {
        private static readonly ISet<CodePoint> AllowedCodePointsForPreparePackage =
            new SortedSet<CodePoint>
                { SQLERRRM, CMDCHKRM, SQLCARD, SQLDARD };

        private static readonly ISet<CodePoint> AllowedCodePointsForDescribeQuery =
            new SortedSet<CodePoint>
                { SQLERRRM, CMDCHKRM, SQLCARD, SQLDARD };

        private static readonly ISet<CodePoint> AllowedCodePointsForExecutePackage =
            new HashSet<CodePoint>
                { SQLERRRM, CMDCHKRM, SQLCARD, SQLDARD, PBSD, QRYDSC, RSLSETRM,
                    QRYDTA, OPNQRYRM, RSLSETRM, SQLRSLRD, SQLCINRD };

        private static readonly ISet<CodePoint> AllowedCodePointsForContinueQuery =
            new SortedSet<CodePoint>
                { SQLERRRM, SQLCARD, QRYDTA, EXTDTA };

        private static readonly ISet<CodePoint> AllowedCodePointsForCloseQuery =
            new SortedSet<CodePoint>
                { SQLERRRM, SQLCARD };

        private readonly Boolean _isPackagePreparedSuccessfully;

        private readonly DrdaConnection _connection;

        private readonly QueryContext _context;

        private readonly String _sqlStatement;

        public DrdaPreparedStatement(
            DrdaConnection connection, 
            String sqlStatement)
        {
            _sqlStatement = sqlStatement;
            _connection = connection;

            _context = new QueryContext(
                _connection, _sqlStatement.Contains("?"));

            _isPackagePreparedSuccessfully = PreparePackage
                (_connection.GetStream(), 
                _connection.GetNextRequestCorrelationId());
        }

        public IDrdaStatement Prepare() => this;

        public Boolean Execute()
        {
            _context.ColumnMode = ColumnMode.Ignore;

            var stream = _connection.GetStream();

            if (!_isPackagePreparedSuccessfully ||
                !ExecutePackage(stream, _connection.GetNextRequestCorrelationId()))
            {
                return false;
            }

            while (_context.HasMoreData)
            {
                if (!ContinueQuery(stream, _connection.GetNextRequestCorrelationId()))
                {
                    return false;
                }
            }

            return CloseQuery(stream, _connection.GetNextRequestCorrelationId());
        }

        public Boolean Fetch() => _context.Fetch();

        public Int32 Columns => _context.Columns.Count;

        public String GetColumnName(Int32 index) => _context.Columns[index].Name;

        public Object GetColumnValue(Int32 index) => _context[index];

        public void SetParameterValue(Int32 index, Object value) => 
            _context.Parameters[index].Value = value;

        private Boolean PreparePackage(
            in NetworkStream stream,
            in UInt16 requestCorrelationId)
        {
            stream
                .SendRequest(new PrepareSqlStatementRequest(
                    requestCorrelationId, _context.PackageSerialNumber))
                .SendRequest(new SqlStatementRequest(
                    requestCorrelationId, _sqlStatement));

            _context.ColumnMode = ColumnMode.Columns;

            if (!new DrdaStatementVisitor(AllowedCodePointsForPreparePackage, _context)
                .ProcessChainedResponses(stream))
            {
                return false;
            }

            if (_context.HasParameters)
            {
                stream.SendRequest(new DescribeSqlStatementRequest(
                    _connection.GetNextRequestCorrelationId(),
                    _context.PackageSerialNumber));

                _context.ColumnMode = ColumnMode.Parameters;

                return new DrdaStatementVisitor(AllowedCodePointsForDescribeQuery, _context)
                    .ProcessChainedResponses(stream);
            }

            return true;
        }

        private Boolean ExecutePackage(
            in NetworkStream stream,
            in UInt16 requestCorrelationId)
        {
            stream.SendRequest(new ExecutePreparedSqlRequest(
                requestCorrelationId, _context.PackageSerialNumber, _context.HasParameters));

            if (_context.HasParameters)
            {
                stream.SendRequest(new SqlRowDataDescMessage(
                    requestCorrelationId, _context.Parameters));

                // TODO: olegra - send BLOB parameters data here
            }

            return new DrdaStatementVisitor(AllowedCodePointsForExecutePackage, _context)
                .ProcessChainedResponses(stream);
        }

        private Boolean ContinueQuery(
            in NetworkStream stream, 
            in UInt16 requestCorrelationId)
        {
            if (!_context.IsQueryOpened)
            {
                return true;
            }

            stream.SendRequest(new ContinueQueryRequest(
                requestCorrelationId, _context.PackageSerialNumber, _context.QueryInstanceId ?? 0));

            return new DrdaStatementVisitor(AllowedCodePointsForContinueQuery, _context)
                .ProcessChainedResponses(stream);
        }
        
        private Boolean CloseQuery(in NetworkStream stream, in UInt16 requestCorrelationId)
        {
            if (!_context.IsQueryOpened)
            {
                return true;
            }

            stream.SendRequest(new CloseQueryRequest(
                requestCorrelationId, _context.PackageSerialNumber, _context.QueryInstanceId ?? 0));

            return new DrdaStatementVisitor(AllowedCodePointsForCloseQuery, _context)
                .ProcessChainedResponses(stream);
        }
    }
}
