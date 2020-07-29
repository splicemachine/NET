using System;

namespace SpliceMachine.Drda
{
    internal interface IDrdaRequest
    {
        UInt16 RequestCorrelationId { get; }

        MessageFormat Format { get; }

        void CheckResponseType(
            DrdaResponseBase response);

        CompositeCommand GetCommand();
    }
}
