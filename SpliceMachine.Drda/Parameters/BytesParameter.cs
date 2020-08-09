using System;

namespace SpliceMachine.Drda
{
    internal readonly struct BytesParameter : IDrdaMessage
    {
        private const UInt32 BaseSize = sizeof(UInt16) + sizeof(UInt16);

        public BytesParameter(
            CodePoint codePoint,
            Byte[] value)
        {
            CodePoint = codePoint;
            Value = value;
        }

        public BytesParameter(
            DrdaStreamReader reader, 
            UInt32 byteArraySize,
            CodePoint codePoint)
            : this(
                codePoint,
                reader.ReadBytes(
                    byteArraySize - BaseSize))
        {
        }

        public Byte[] Value { get; }

        public UInt32 GetSize() => BaseSize + (UInt32)Value.Length;

        public CodePoint CodePoint { get; }

        public void Write(DrdaStreamWriter writer)
        {
            writer.WriteUInt16((UInt16)GetSize());
            writer.WriteUInt16((UInt16)CodePoint);
            writer.WriteBytes(Value);
        }
    }
}
