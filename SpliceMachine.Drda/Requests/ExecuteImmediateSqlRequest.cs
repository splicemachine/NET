using System;
using static SpliceMachine.Drda.MessageFormat;

namespace SpliceMachine.Drda
{
    public sealed class ExecuteImmediateSqlRequest : IDrdaRequest
    {
        private readonly UInt16 _packageSerialNumber;

        public ExecuteImmediateSqlRequest(
            Int32 requestCorrelationId,
            UInt16 packageSerialNumber)
        {
            _packageSerialNumber = packageSerialNumber;
            RequestCorrelationId = requestCorrelationId;
        }

        public Int32 RequestCorrelationId { get; }

        MessageFormat IDrdaRequest.Format => 
            Request | Chained | Correlated ;

        public void CheckResponseType(
            DrdaResponseBase response)
        {
        }

        CompositeParameter IDrdaRequest.GetCommand() =>
            new CompositeParameter(
                CodePoint.EXCSQLIMM,
                new PackageSerialNumber(_packageSerialNumber),
                new UInt8Parameter(CodePoint.RDBCMTOK, 0xF1));
    }
}
