using System;
using System.Collections.Generic;

namespace SpliceMachine.Drda
{
    internal sealed class SqlResultSetDataResponse : DrdaResponseBase
    {
        private readonly SqlResultSetData _sqlResultSetData;

        internal SqlResultSetDataResponse(
            ResponseMessage response)
            : base(response)
        {
            _sqlResultSetData = response.Command as SqlResultSetData;
        }

        public IReadOnlyList<DrdaResultSet> ResultSets => _sqlResultSetData.ResultSets;

        internal override Boolean Accept(
            DrdaStatementVisitor visitor) => visitor.Visit(this);
    }
}
