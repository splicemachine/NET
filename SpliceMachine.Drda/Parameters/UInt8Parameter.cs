using System;

namespace SpliceMachine.Drda
{
    internal readonly struct UInt8Parameter : IDrdaMessage
    {
        private const UInt32 BaseSize = sizeof(UInt16) + sizeof(UInt16) + sizeof(Byte);

        public UInt8Parameter(
            CodePoint codePoint,
            Byte value)
        {
            CodePoint = codePoint;
            Value = value;
        }

        public UInt8Parameter(
            DrdaStreamReader reader, 
            CodePoint codePoint)
            : this(
                codePoint,
                reader.ReadUInt8())
        {
        }

        public Byte Value { get; }

        public UInt32 GetSize() => BaseSize;

        public CodePoint CodePoint { get; }

        public void Write(DrdaStreamWriter writer)
        {
            writer.WriteUInt16((UInt16)GetSize());
            writer.WriteUInt16((UInt16)CodePoint);
            writer.WriteUInt8(Value);
        }
    }
}
