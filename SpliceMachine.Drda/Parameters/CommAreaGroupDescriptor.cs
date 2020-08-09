using System;
using System.Diagnostics.CodeAnalysis;

namespace SpliceMachine.Drda
{
    internal sealed class CommAreaGroupDescriptor
    {
        [SuppressMessage("ReSharper", "UnusedVariable")]
        public CommAreaGroupDescriptor(
            DrdaStreamReader reader)
        {
            if (reader.ReadUInt8() == 0xFF)
            {
                return;
            }

            SqlCode = reader.ReadUInt32();
            SqlState = reader.ReadString(5);
            var sqlErrProc = reader.ReadString(8);

            if (reader.ReadUInt8() != 0xFF)
            {
                RowsFetched = reader.ReadUInt64();
                RowsUpdated = reader.ReadUInt32();

                var sqlErrs = reader.ReadBytes(12); // 3 * sizeof(UInt32)
                var sqlWarn = reader.ReadBytes(11); // 11 * sizeof(Byte)

                var rdbName = reader.ReadUInt16();

                SqlMessage = reader.ReadVcmVcs();
            }

            if (reader.ReadUInt8() != 0xFF)
            {
                // WORKWORK
            }
        }

        public UInt32 SqlCode { get; }

        public String SqlState { get; }

        public UInt32 RowsUpdated { get; }

        public UInt64 RowsFetched { get; }

        public String SqlMessage { get; }
    }
}
