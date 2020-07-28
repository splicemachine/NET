using System;

namespace SpliceMachine.Drda
{
    internal sealed class QueryAnswerSetDataResponse : DrdaResponseBase
    {
        private readonly QueryAnswerSetData _queryAnswerSetData;

        internal QueryAnswerSetDataResponse(
            ResponseMessage response)
            : base(
                response.RequestCorrelationId,
                response.IsChained)
        {
            Console.WriteLine($"RCID: {RequestCorrelationId}, CP: {response.Command.CodePoint}");

            _queryAnswerSetData = response.Command as QueryAnswerSetData;
        }

        public Boolean HasMoreData => false; // TODO: olegra - implement it correctly
    }
}
