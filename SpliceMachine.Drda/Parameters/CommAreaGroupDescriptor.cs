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
                var sqlErr1 = reader.ReadUInt32();
                var sqlErr2 = reader.ReadUInt32();
                RowsUpdated = reader.ReadUInt32();
                var sqlErr4 = reader.ReadUInt32();
                var sqlErr5 = reader.ReadUInt32();
                var sqlErr6 = reader.ReadUInt32();

                var sqlWarn = reader.ReadString(11);
                var rdbName = reader.ReadVarString();

                SqlMessage = reader.ReadVcmVcs();
            }

            if (reader.ReadUInt8() != 0xFF)
            {
                // WORKWORK
            }
        }

        public Int32 SqlCode { get; }

        public String SqlState { get; }

        public Int32 RowsUpdated { get; }

        public String SqlMessage { get; }
    }
}
