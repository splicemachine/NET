using System;

namespace SpliceMachine.Drda
{
    public sealed class RelationalDatabaseCommitRequest : IDrdaRequest
    {
        public RelationalDatabaseCommitRequest(
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
            new CompositeCommand(CodePoint.RDBCMM);
    }
}
