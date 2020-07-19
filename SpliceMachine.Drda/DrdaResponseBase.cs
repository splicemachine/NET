using System;

namespace SpliceMachine.Drda
{
    public abstract class DrdaResponseBase
    {
        protected DrdaResponseBase(
            Int32 requestCorrelationId) =>
            RequestCorrelationId = requestCorrelationId;

        public Int32 RequestCorrelationId { get; }
    }
}
