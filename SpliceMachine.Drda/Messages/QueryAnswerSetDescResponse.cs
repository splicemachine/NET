using System;

namespace SpliceMachine.Drda
{
    public sealed class QueryAnswerSetDescResponse : DrdaResponseBase
    {
        private readonly QueryAnswerSetDescriptor _queryAnswerSetDescriptor;

        internal QueryAnswerSetDescResponse(
            ResponseMessage response)
            : base(response)
        {
            _queryAnswerSetDescriptor = response.Command as QueryAnswerSetDescriptor;
        }

        internal override Boolean Accept(
            DrdaStatementVisitor visitor) => visitor.Visit(this);
    }
}
