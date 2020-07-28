namespace SpliceMachine.Drda
{
    internal sealed class QueryAnswerSetExtraDataResponse : DrdaResponseBase
    {
        private readonly QueryAnswerSetExtraData _queryAnswerSetExtraData;

        internal QueryAnswerSetExtraDataResponse(
            ResponseMessage response)
            : base(
                response.RequestCorrelationId,
                response.IsChained)
        {
            _queryAnswerSetExtraData = response.Command as QueryAnswerSetExtraData;
        }
    }
}
