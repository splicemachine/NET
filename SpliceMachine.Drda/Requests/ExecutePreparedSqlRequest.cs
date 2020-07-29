using System;
using static SpliceMachine.Drda.MessageFormat;

namespace SpliceMachine.Drda
{
    internal sealed class ExecutePreparedSqlRequest : IDrdaRequest
    {
        private readonly UInt16 _packageSerialNumber;

        private readonly Boolean _hasParameters;

        public ExecutePreparedSqlRequest(
            UInt16 requestCorrelationId,
            UInt16 packageSerialNumber,
            Boolean hasParameters)
        {
            _packageSerialNumber = packageSerialNumber;
            _hasParameters = hasParameters;
            RequestCorrelationId = requestCorrelationId;
        }
        
        public UInt16 RequestCorrelationId { get; }

        MessageFormat IDrdaRequest.Format => 
            _hasParameters
                ? Request | Chained | Correlated
                : Request;

        public void CheckResponseType(DrdaResponseBase response)
        {
        }

        CompositeParameter IDrdaRequest.GetCommand() =>
            new CompositeParameter(
                CodePoint.EXCSQLSTT,
                new PackageSerialNumber(_packageSerialNumber),
                new UInt32Parameter(CodePoint.QRYBLKSZ, 0x100000),
                new UInt16Parameter(CodePoint.MAXBLKEXT, 0xFFFF),
                new UInt8Parameter(CodePoint.RDBCMTOK, 0xF1));
    }
}
