using System;
using System.Collections;
using System.Collections.Generic;

namespace SpliceMachine.Drda
{
    internal readonly struct ReaderCommand : ICommand
    {
        private const Int32 BaseSize = sizeof(UInt16) + sizeof(UInt16);

        private readonly Int32 _totalByteLength;

        public ReaderCommand(
            DrdaStreamReader reader,
            Int32 totalByteLength,
            CodePoint codePoint)
        {
            _totalByteLength = totalByteLength - BaseSize;
            CodePoint = codePoint;
            Reader = reader;
        }

        public DrdaStreamReader Reader { get; }

        public Int32 GetSize() => BaseSize + _totalByteLength;

        public CodePoint CodePoint { get; }

        public void Write(DrdaStreamWriter writer)
        {
            writer.WriteUInt16((UInt16)GetSize());
            writer.WriteUInt16((UInt16)CodePoint);
            writer.WriteBytes(Reader?.ReadBytes(_totalByteLength));
        }

        public IEnumerator<IDrdaMessage> GetEnumerator()
        {
            yield return this;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void ConsumeAllBytes() => 
            Reader?.ReadBytes(_totalByteLength);
    }
}
