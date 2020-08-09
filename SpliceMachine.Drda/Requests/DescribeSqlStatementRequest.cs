using System;

namespace SpliceMachine.Drda
{
    internal sealed class DescribeSqlStatementRequest : IDrdaRequest
    {
        private readonly UInt16 _packageSerialNumber;

        public DescribeSqlStatementRequest(
            UInt16 requestCorrelationId,
            UInt16 packageSerialNumber)
        {
            _packageSerialNumber = packageSerialNumber;
            RequestCorrelationId = requestCorrelationId;
        }
        
        public UInt16 RequestCorrelationId { get; }

        MessageFormat IDrdaRequest.Format => MessageFormat.Request;

        public void CheckResponseType(DrdaResponseBase response)
        {
        }

        CompositeCommand IDrdaRequest.GetCommand() =>
            new CompositeCommand(
                CodePoint.DSCSQLSTT,
                new PackageSerialNumber(_packageSerialNumber),
                new UInt8Parameter(CodePoint.TYPSQLDA, 5)); // SQLDA_EI
    }
}
