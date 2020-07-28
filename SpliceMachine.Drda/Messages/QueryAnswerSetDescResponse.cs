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
            Console.WriteLine($"RCID: {RequestCorrelationId}, CP: {response.Command.CodePoint}");

            _queryAnswerSetDescriptor = response.Command as QueryAnswerSetDescriptor;
        }
    }
}
