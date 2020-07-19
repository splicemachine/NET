using System;

namespace SpliceMachine.Drda
{
    internal readonly struct BytesParameter : IDrdaMessage
    {
        private const Int32 BaseSize = sizeof(UInt16) + sizeof(UInt16);

        public BytesParameter(
            CodePoint codePoint,
            Byte[] value)
        {
            CodePoint = codePoint;
            Value = value;
        }

        public BytesParameter(
            DrdaStreamReader reader, 
            UInt16 byteArraySize,
            CodePoint codePoint)
            : this(
                codePoint,
                reader.ReadBytes(
                    byteArraySize - BaseSize))
        {
        }
        public Byte[] Value { get; }

        public Int32 GetSize() => BaseSize + Value.Length;

        public CodePoint CodePoint { get; }

        public void Write(DrdaStreamWriter writer)
        {
            writer.WriteUint16((UInt16)GetSize());
            writer.WriteUint16((UInt16)CodePoint);
            writer.WriteBytes(Value);
        }
    }
}
