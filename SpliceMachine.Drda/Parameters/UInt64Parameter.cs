using System;

namespace SpliceMachine.Drda
{
    internal class UInt64Parameter : IDrdaMessage
    {
        private const UInt32 BaseSize = sizeof(UInt16) + sizeof(UInt16) + sizeof(UInt64);

        public UInt64Parameter(
            CodePoint codePoint,
            UInt64 value)
        {
            CodePoint = codePoint;
            Value = value;
        }

        public UInt64Parameter(
            DrdaStreamReader reader, 
            CodePoint codePoint)
            : this(
                codePoint,
                reader.ReadUInt64())
        {
        }

        public UInt64 Value { get; }

        public UInt32 GetSize() => BaseSize;

        public CodePoint CodePoint { get; }

        public void Write(DrdaStreamWriter writer)
        {
            writer.WriteUInt16((UInt16)GetSize());
            writer.WriteUInt16((UInt16)CodePoint);
            writer.WriteUInt64(Value);
        }
    }
}
