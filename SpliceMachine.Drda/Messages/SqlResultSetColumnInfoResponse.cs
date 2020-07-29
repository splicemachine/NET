using System;
using System.Collections.Generic;

namespace SpliceMachine.Drda
{
    internal sealed class SqlResultSetColumnInfoResponse : DrdaResponseBase
    {
        private readonly SqlResultSetColumnInfo _sqlResultSetColumnInfo;

        internal SqlResultSetColumnInfoResponse(
            ResponseMessage response)
            : base(response)
        {
            _sqlResultSetColumnInfo = response.Command as SqlResultSetColumnInfo;
        }

        public IReadOnlyList<DrdaColumn> Columns => _sqlResultSetColumnInfo.Columns;

        internal override Boolean Accept(
            DrdaStatementVisitor visitor) => visitor.Visit(this);
    }
}
