using System;

namespace SpliceMachine.Drda
{
    using static System.Diagnostics.Trace;
    using static CodePoint;

    internal static class CodePointMapper
    {
        public static IDrdaMessage Deserialize(
            DrdaStreamReader reader)
        {
            var size = (UInt32)reader.ReadUInt16();
            var codePoint = (CodePoint)reader.ReadUInt16();

            if (size == 0x8004)
            {
                // TODO: olegra - check how we can obtain real message length here
            }
            else if ((size & 0x8000) == 0x8000)
            {
                size = reader.ReadUInt32() + sizeof(UInt16) + sizeof(UInt16);
            }

            // TODO: olegra - find the better way for handling code points here

            switch (codePoint)
            {
                case PBSD:
                case EXSATRM:
                case ACCSECRM:
                case SECCHKRM:
                case ACCRDBRM:
                case CMDCHKRM:
                case SQLERRRM:
                case RDBUPDRM:
                case SYNTAXRM:
                case RSLSETRM:
                case OPNQRYRM:
                case ENDUOWRM:
                    return new CompositeCommand(reader, size, codePoint);

                // In fact they are composite but we don't need them right now
                case MGRLVLLS:
                case TYPDEFOVR:
                case PKGSNLST:
                    return new BytesParameter(reader, size, codePoint); //-V3139

                case QRYDSC:
                case QRYDTA:
                case EXTDTA:
                case SQLCARD:
                case SQLDARD:
                case SQLCINRD:
                case SQLRSLRD:
                    return new ReaderCommand(reader, size, codePoint);

                case PBSD_ISO:
                case SYNERRCD:
                case SECCHKCD:
                case SQLCSRHLD:
                case QRYATTUPD:
                    return new UInt8Parameter(reader, codePoint);

                case SECMEC:
                case SRVCOD:
                case QRYPRCTYP:
                    return new UInt16Parameter(reader, codePoint);

                case QRYINSID:
                    return new UInt64Parameter(reader, codePoint);

                case PRDID:
                case RDBNAM:
                case EXTNAM:
                case SRVNAM:
                case SRVCLSNM:
                case SRVRLSLV:
                case TYPDEFNAM:
                case PBSD_SCHEMA:
                    // TODO: olegra - create string parameter type?
                    return new BytesParameter(reader, size, codePoint);

                default:
                    TraceWarning("Unknown code point value: 0x{0:X}", codePoint);
                    return new BytesParameter(reader, size, codePoint);
            }
        }
    }
}
