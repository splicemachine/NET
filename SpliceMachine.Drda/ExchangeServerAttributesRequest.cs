using System;

namespace SpliceMachine.Drda
{
    public sealed class ExchangeServerAttributesRequest
        : DrdaRequestBase<ExchangeServerAttributesResponse>, IDrdaRequest
    {
        private readonly struct ManagerLevels : IDrdaMessage
        {
            private const Int32 BaseSize = sizeof(UInt16) + sizeof(UInt16);

            private static readonly UInt16[] Values = 
            {
                0x1403, 0x0007, // AGENT, DDM_Level7
                0x2407, 0x0007, // SQLAM, DDM_Level7
                0x240F, 0x0007, // RDB, DDM_Level7
                0x1440, 0x0007, // SECMGR, DDM_Level7
                0x1C08, 1208 // UNICODEMGR, CCSID_1208
            };

            public Int32 GetSize() => BaseSize + sizeof(UInt16) * Values.Length;

            public UInt16 CodePoint => 0x1404; // MGRLVLLS

            public void Write(DrdaStreamWriter writer)
            {
                writer.WriteUint16((UInt16)GetSize());
                writer.WriteUint16(CodePoint); 

                foreach (var value in Values)
                {
                    writer.WriteUint16(value);
                }
            }
        }

        public ExchangeServerAttributesRequest(
            Int32 requestCorrelationId)
            : base(
                requestCorrelationId)
        {
        }

        CompositeParameter IDrdaRequest.GetCommand() =>
            new CompositeParameter(
                CodePoint.EXCSAT,
                EncodingEbcdic.GetParameter(CodePoint.EXTNAM, "derbydncmain"),
                EncodingEbcdic.GetParameter(CodePoint.SRVCLSNM, "SM/WIN32"),
                EncodingEbcdic.GetParameter(CodePoint.SRVNAM, "Derby"),
                EncodingEbcdic.GetParameter(CodePoint.SRVRLSLV, "SNC1009-/10.9.1.0 - (1344872)"),
                new ManagerLevels());
    }
}
