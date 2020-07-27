using System;

namespace SpliceMachine.Drda
{
    internal interface IDrdaRequest
    {
        Int32 RequestCorrelationId { get; }

        MessageFormat Format { get; }

        void CheckResponseType(
            DrdaResponseBase response);

        CompositeParameter GetCommand();
    }
}
