using System;

namespace SpliceMachine.Drda
{
    public sealed class AccessSecurityDataRequest
        : DrdaRequestBase<AccessSecurityDataResponse>, IDrdaRequest
    {
        public AccessSecurityDataRequest(
            UInt16 requestCorrelationId)
            : base(
                requestCorrelationId)
        {
        }

        CompositeCommand IDrdaRequest.GetCommand() =>
            new CompositeCommand(
                CodePoint.ACCSEC,
                EncodingEbcdic.GetParameter(CodePoint.RDBNAM, WellKnownStrings.DatabaseName),
                new UInt16Parameter(CodePoint.SECMEC, 0x0003) // USRIDPWD
            );
    }
}
