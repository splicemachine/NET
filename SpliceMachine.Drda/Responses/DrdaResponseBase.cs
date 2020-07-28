using System;

namespace SpliceMachine.Drda
{
    public abstract class DrdaResponseBase
    {
        protected DrdaResponseBase(
            Int32 requestCorrelationId,
            Boolean isChained)
        {
            RequestCorrelationId = requestCorrelationId;
            IsChained = isChained;
        }

        public Int32 RequestCorrelationId { get; }

        public Boolean IsChained { get; }
    }
}
