using System;

namespace SpliceMachine.Drda
{
    public abstract class DrdaRequestBase
    {
        protected DrdaRequestBase(
            Int32 requestCorrelationId) =>
            RequestCorrelationId = requestCorrelationId;

        public Int32 RequestCorrelationId { get; }

        internal abstract CompositeParameter GetCommand();
    }
}
