using System;

namespace SpliceMachine.Drda
{
    public sealed class QueryAnswerSetDescResponse : DrdaResponseBase
    {
        private readonly QueryAnswerSetDescriptor _queryAnswerSetDescriptor;

        internal QueryAnswerSetDescResponse(
            ResponseMessage response)
            : base(
                response.RequestCorrelationId,
                response.IsChained)
        {
            _queryAnswerSetDescriptor = response.Command as QueryAnswerSetDescriptor;
        }
    }
}
