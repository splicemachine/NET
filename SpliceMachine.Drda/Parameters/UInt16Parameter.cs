using System;

namespace SpliceMachine.Drda
{
    internal readonly struct UInt16Parameter : IDrdaMessage
    {
        private const Int32 BaseSize = sizeof(UInt16) + sizeof(UInt16) + sizeof(UInt16);

        public UInt16Parameter(
            CodePoint codePoint,
            UInt16 value)
        {
            CodePoint = codePoint;
            Value = value;
        }

        public UInt16Parameter(
            DrdaStreamReader reader, 
            CodePoint codePoint)
            : this(
                codePoint,
                reader.ReadUInt16())
        {
        }

        public UInt16 Value { get; }

        public Int32 GetSize() => BaseSize;

        public CodePoint CodePoint { get; }

        public void Write(DrdaStreamWriter writer)
        {
            writer.WriteUInt16((UInt16)GetSize());
            writer.WriteUInt16((UInt16)CodePoint);
            writer.WriteUInt16(Value);
        }
    }
}
