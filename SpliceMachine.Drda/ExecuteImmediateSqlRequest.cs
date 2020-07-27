using System;
using System.Text;
using static SpliceMachine.Drda.MessageFormat;

namespace SpliceMachine.Drda
{
    public sealed class ExecuteImmediateSqlRequest : IDrdaRequest
    {
        private readonly struct PackageSerialNumber : IDrdaMessage
        {
            private const Int32 BaseSize = sizeof(UInt16) + sizeof(UInt16);

            private static readonly String CollationId = "NULLID".PadRight(18, ' ');

            private static readonly String PackageId = "SYSLH000".PadRight(18, ' ');

            private static readonly String Token = "SYSLVL01";

            private readonly UInt16 _packageSerialNumber;

            public PackageSerialNumber(
                UInt16 packageSerialNumber) => 
                _packageSerialNumber = packageSerialNumber;

            public Int32 GetSize() => 
                BaseSize + WellKnownStrings.DatabaseName.Length +
                CollationId.Length + PackageId.Length + Token.Length +
                sizeof(UInt16);

            public void Write(
                DrdaStreamWriter writer)
            {
                writer.WriteUInt16((UInt16)GetSize());
                writer.WriteUInt16((UInt16)CodePoint.PKGNAMCSN);

                writer.WriteBytes(Encoding.UTF8.GetBytes(WellKnownStrings.DatabaseName));
                writer.WriteBytes(Encoding.UTF8.GetBytes(CollationId));
                writer.WriteBytes(Encoding.UTF8.GetBytes(PackageId));
                writer.WriteBytes(Encoding.UTF8.GetBytes(Token));

                writer.WriteUInt16(_packageSerialNumber);
            }
        }

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
