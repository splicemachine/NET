using System;

namespace SpliceMachine.Drda
{
    public sealed class RelationalDatabaseRollbackRequest : IDrdaRequest
    {
        public RelationalDatabaseRollbackRequest(
            UInt16 requestCorrelationId)
        {
            RequestCorrelationId = requestCorrelationId;
        }
        
        public UInt16 RequestCorrelationId { get; }

        MessageFormat IDrdaRequest.Format => MessageFormat.Request;

        public void CheckResponseType(DrdaResponseBase response)
        {
        }

        CompositeCommand IDrdaRequest.GetCommand() =>
            new CompositeCommand(CodePoint.RDBRLLBCK);
    }
}
