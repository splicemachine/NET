using System;

namespace SpliceMachine.Drda
{
    public abstract class DrdaResponseBase
    {
        internal DrdaResponseBase(
            ResponseMessage response)
        {
            RequestCorrelationId = response.RequestCorrelationId;
            CodePoint = response.Command.CodePoint;
            IsChained = response.IsChained;
        }

        public Int32 RequestCorrelationId { get; }

        public Boolean IsChained { get; }

        internal virtual Boolean Accept(
            DrdaStatementVisitor visitor) => false;

        internal CodePoint CodePoint { get; }
    }
}
