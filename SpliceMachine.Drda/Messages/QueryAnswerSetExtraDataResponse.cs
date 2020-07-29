using System;

namespace SpliceMachine.Drda
{
    internal sealed class QueryAnswerSetExtraDataResponse : DrdaResponseBase
    {
        private readonly QueryAnswerSetExtraData _queryAnswerSetExtraData;

        internal QueryAnswerSetExtraDataResponse(
            ResponseMessage response)
            : base(response)
        {
            _queryAnswerSetExtraData = response.Command as QueryAnswerSetExtraData;
        }

        internal override Boolean Accept(
            DrdaStatementVisitor visitor) => visitor.Visit(this);
    }
}
