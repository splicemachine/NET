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

            return new DrdaStatementVisitor(AllowedCodePoints, context)
                .ProcessChainedResponses(stream);
        }

        public Boolean Fetch() => false;

        public Int32 Columns => 0;

        public String GetColumnName(Int32 index) => 
            throw new InvalidOperationException();

        public Object GetColumnValue(Int32 index) => 
            throw new InvalidOperationException();

        public void SetParameterValue(Int32 index, Object value) =>
            throw new InvalidOperationException();
    }
}
