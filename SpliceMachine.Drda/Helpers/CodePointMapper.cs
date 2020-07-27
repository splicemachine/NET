using System;

namespace SpliceMachine.Drda
{
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
                case CodePoint.EXSATRM:
                case CodePoint.ACCSECRM:
                case CodePoint.SECCHKRM:
                case CodePoint.ACCRDBRM:
                case CodePoint.CMDCHKRM:
                case CodePoint.SQLERRRM:
                case CodePoint.RDBUPDRM:
                case CodePoint.SYNTAXRM:
                case CodePoint.PBSD:
                    return new CompositeParameter(reader, size, codePoint);

                case CodePoint.SQLCARD:
                    return new CommAreaGroupDescriptor(reader, size);

                case CodePoint.SQLDARD:
                    return new DescAreaGroupDescriptor(reader, size);

                case CodePoint.SECMEC:
                case CodePoint.SRVCOD:
                    return new UInt16Parameter(reader, codePoint);

                case CodePoint.PBSD_ISO:
                case CodePoint.SYNERRCD:
                    return new UInt8Parameter(reader, codePoint);

                default:
                    return new BytesParameter(reader, size, codePoint);
            }
        }
    }
}
