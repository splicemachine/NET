using System;

namespace SpliceMachine.Drda
{
    public sealed class AccessSecurityDataRequest : DrdaRequestBase
    {
        public AccessSecurityDataRequest(
            Int32 requestCorrelationId)
            : base(
                requestCorrelationId)
        {
        }

        internal override CompositeParameter GetCommand() =>
            new CompositeParameter(
                CodePoint.ACCSEC,
                EncodingEbcdic.GetParameter(CodePoint.RDBNAM, WellKnownStrings.DatabaseName),
                new UInt16Parameter(CodePoint.SECMEC, 0x0003) // USRIDPWD
            );
    }
}
