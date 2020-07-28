using System;
using System.Diagnostics.CodeAnalysis;

namespace SpliceMachine.Drda
{
    internal sealed class DrdaResultSet : IDrdaMessage
    {
        [SuppressMessage("ReSharper", "UnusedVariable")]
        public DrdaResultSet(
            DrdaStreamReader reader)
        {
            Locator = reader.ReadUInt32();
            CursorName = reader.ReadVcmVcs();
            Rows = reader.ReadUInt32();
        }

        public UInt16 Locator { get; }

        public String CursorName { get; }

        public UInt16 Rows { get; }

        public Int32 GetSize() => 0;

        public void Write(DrdaStreamWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
