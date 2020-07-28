using System;

namespace SpliceMachine.Drda
{
    internal readonly struct UInt32Parameter : IDrdaMessage
    {
        private const Int32 BaseSize = sizeof(UInt16) + sizeof(UInt16) + sizeof(UInt32);

        public UInt32Parameter(
            CodePoint codePoint,
            UInt32 value)
        {
            CodePoint = codePoint;
            Value = value;
        }

        public UInt32Parameter(
            DrdaStreamReader reader, 
            CodePoint codePoint)
            : this(
                codePoint,
                reader.ReadUInt32())
        {
        }

        public UInt32 Value { get; }

        public Int32 GetSize() => BaseSize;

        public CodePoint CodePoint { get; }

        public void Write(DrdaStreamWriter writer)
        {
            writer.WriteUInt16((UInt16)GetSize());
            writer.WriteUInt16((UInt16)CodePoint);
            writer.WriteUInt32(Value);
        }
    }
}
