using System;

namespace SpliceMachine.Drda
{
    using static System.Diagnostics.Trace;

    internal static class CodePointMapper
    {
        public static IDrdaMessage Deserialize(
            DrdaStreamReader reader)
        {
            var size = (Int32)reader.ReadUInt16();
            var codePoint = (CodePoint)reader.ReadUInt16();

            if (size == 0x8004)
            {
                // TODO: olegra - check how we can obtain real message length here
            }
            else if ((size & 0x8000) == 0x8000)
            {
                size = reader.ReadUInt32();
            }

            // TODO: olegra - find the better way for handling code points here

            switch (codePoint)
            {
                case CodePoint.PBSD:
                case CodePoint.EXSATRM:
                case CodePoint.ACCSECRM:
                case CodePoint.SECCHKRM:
                case CodePoint.ACCRDBRM:
                case CodePoint.CMDCHKRM:
                case CodePoint.SQLERRRM:
                case CodePoint.RDBUPDRM:
                case CodePoint.SYNTAXRM:
                case CodePoint.RSLSETRM:
                case CodePoint.OPNQRYRM:
                case CodePoint.ENDUOWRM:
                    return new CompositeParameter(reader, size, codePoint);

                case CodePoint.SQLCARD:
                    return new CommAreaGroupDescriptor(reader, size);

                case CodePoint.SQLDARD:
                    return new DescAreaGroupDescriptor(reader, size);

                case CodePoint.SQLRSLRD:
                    return new SqlResultSetData(reader, size);

                case CodePoint.SQLCINRD:
                    return new SqlResultSetColumnInfo(reader, size);

                case CodePoint.QRYDSC:
                    return new QueryAnswerSetDescriptor(reader, size);

                case CodePoint.QRYDTA:
                    return new QueryAnswerSetData(reader, size);

                case CodePoint.EXTDTA:
                    return new QueryAnswerSetExtraData(reader, size);

                case CodePoint.SECMEC:
                case CodePoint.SRVCOD:
                    return new UInt16Parameter(reader, codePoint);

                case CodePoint.PBSD_ISO:
                case CodePoint.SYNERRCD:
                    return new UInt8Parameter(reader, codePoint);

                case CodePoint.QRYINSID:
                    return new UInt64Parameter(reader, codePoint);

                default:
                    TraceWarning(codePoint.ToString("X"));
                    return new BytesParameter(reader, size, codePoint);
            }
        }
    }
}
