using System;

namespace SpliceMachine.Drda
{
    public sealed class ExchangeServerAttributesRequest
        : DrdaRequestBase<ExchangeServerAttributesResponse>, IDrdaRequest
    {
        private readonly struct ManagerLevels : IDrdaMessage
        {
            private const UInt32 BaseSize = sizeof(UInt16) + sizeof(UInt16);

            private static readonly UInt16[] Values = 
            {
                0x1403, 0x0007, // AGENT, DDM_Level7
                0x2407, 0x0007, // SQLAM, DDM_Level7
                0x240F, 0x0007, // RDB, DDM_Level7
                0x1440, 0x0007, // SECMGR, DDM_Level7
                0x1C08, 1208 // UNICODEMGR, CCSID_1208
            };

            public UInt32 GetSize() => BaseSize + sizeof(UInt16) * (UInt32) Values.Length;

            public void Write(DrdaStreamWriter writer)
            {
                writer.WriteUInt16((UInt16)GetSize());
                writer.WriteUInt16((UInt16)CodePoint.MGRLVLLS); 

                foreach (var value in Values)
                {
                    writer.WriteUInt16(value);
                }
            }
        }

        public ExchangeServerAttributesRequest(
            UInt16 requestCorrelationId)
            : base(
                requestCorrelationId)
        {
        }

        CompositeCommand IDrdaRequest.GetCommand() =>
            new CompositeCommand(
                CodePoint.EXCSAT,
                EncodingEbcdic.GetParameter(CodePoint.EXTNAM, "derbydncmain"),
                EncodingEbcdic.GetParameter(CodePoint.SRVCLSNM, "SM/WIN32"),
                EncodingEbcdic.GetParameter(CodePoint.SRVNAM, "Derby"),
                EncodingEbcdic.GetParameter(CodePoint.SRVRLSLV, "SNC1009-/10.9.1.0 - (1344872)"),
                new ManagerLevels());
    }
}
