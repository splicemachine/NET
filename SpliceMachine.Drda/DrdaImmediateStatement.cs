using System;
using System.Collections.Generic;

namespace SpliceMachine.Drda
{
    using static CodePoint;

    internal sealed class DrdaImmediateStatement : IDrdaStatement
    {
        private static readonly ISet<CodePoint> AllowedCodePoints =
            new SortedSet<CodePoint>
                { SQLERRRM, CMDCHKRM, RDBUPDRM, SQLCARD, PBSD };

        private readonly DrdaConnection _connection;

        private readonly String _sqlStatement;
        private UInt32 _rowsUpdated;

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
            var stream = _connection.GetStream();
            var context = new QueryContext(_connection, false);

            var requestCorrelationId = _connection.GetNextRequestCorrelationId();

            stream
                .SendRequest(new ExecuteImmediateSqlRequest(
                    requestCorrelationId, context.PackageSerialNumber))
                .SendRequest(new SqlStatementRequest(
                    requestCorrelationId, _sqlStatement));
            var drdaStatementVisitor = new DrdaStatementVisitor(AllowedCodePoints, context);
            var execResult = drdaStatementVisitor.ProcessChainedResponses(stream);
            _rowsUpdated += drdaStatementVisitor.RowsUpdated;
            return execResult;
        }

        public Boolean Fetch() => false;

        public Int32 Columns => 0;
        public Int32 ParametersLength => 0;

        public String GetColumnName(Int32 index) => 
            throw new InvalidOperationException();

        public String GetColumnLabel(Int32 index) =>
            throw new InvalidOperationException();

        public long GetColumnSize(Int32 index) =>
            throw new InvalidOperationException();

        public String GetSchemaName(Int32 index) =>
            throw new InvalidOperationException();

        public Object GetColumnValue(Int32 index) => 
            throw new InvalidOperationException();

        public void SetParameterValue(Int32 index, Object value) =>
            throw new InvalidOperationException();
        public UInt32 RowsUpdated => _rowsUpdated;

        public String[] GetParameterMetaData(Int32 index) => throw new InvalidOperationException();

    }
}
