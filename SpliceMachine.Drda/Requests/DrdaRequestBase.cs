using System;

namespace SpliceMachine.Drda
{
    public abstract class DrdaRequestBase<TResponse> : IDrdaRequest
        where TResponse : DrdaResponseBase
    {
        protected DrdaRequestBase(
            UInt16 requestCorrelationId) =>
            RequestCorrelationId = requestCorrelationId;

        public UInt16 RequestCorrelationId { get; }

        MessageFormat IDrdaRequest.Format =>
            MessageFormat.Request;

        void IDrdaRequest.CheckResponseType(
            DrdaResponseBase response)
        {
            if (!(response is TResponse))
            {
                throw new InvalidOperationException();
            }
        }

        CompositeParameter IDrdaRequest.GetCommand() => default;
    }
}
