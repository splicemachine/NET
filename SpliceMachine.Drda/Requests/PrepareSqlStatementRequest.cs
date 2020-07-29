using System;
using static SpliceMachine.Drda.MessageFormat;

namespace SpliceMachine.Drda
{
    public sealed class PrepareSqlStatementRequest : IDrdaRequest
    {
        private readonly UInt16 _packageSerialNumber;

        public PrepareSqlStatementRequest(
            UInt16 requestCorrelationId,
            UInt16 packageSerialNumber)
        {
            _packageSerialNumber = packageSerialNumber;
            RequestCorrelationId = requestCorrelationId;
        }
        
        public UInt16 RequestCorrelationId { get; }

        MessageFormat IDrdaRequest.Format => 
            Request | Chained | Correlated;

        public void CheckResponseType(DrdaResponseBase response)
        {
        }

        CompositeCommand IDrdaRequest.GetCommand() =>
            new CompositeCommand(
                CodePoint.PRPSQLSTT,
                new PackageSerialNumber(_packageSerialNumber),
                new UInt8Parameter(CodePoint.RTNSQLDA, 0xF1),
                new UInt8Parameter(CodePoint.TYPSQLDA, 4));
    }
}
