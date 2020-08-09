using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SpliceMachine.Drda
{
    internal readonly struct ReaderCommand : ICommand
    {
        private const UInt32 BaseSize = sizeof(UInt16) + sizeof(UInt16);

        private readonly UInt32 _totalByteLength;

        public ReaderCommand(
            DrdaStreamReader reader,
            UInt32 totalByteLength,
            CodePoint codePoint)
        {
            _totalByteLength = totalByteLength - BaseSize;
            CodePoint = codePoint;
            Reader = reader;
        }

        public DrdaStreamReader Reader { get; }

        public UInt32 GetSize() => BaseSize + _totalByteLength;

        public CodePoint CodePoint { get; }

        public void Write(DrdaStreamWriter writer)
        {
            writer.WriteUInt16((UInt16)GetSize());
            writer.WriteUInt16((UInt16)CodePoint);
            writer.WriteBytes(Reader?.ReadBytes(_totalByteLength));
        }

        public IEnumerator<IDrdaMessage> GetEnumerator() => 
            Enumerable.Empty<IDrdaMessage>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public Byte[] GetMessageBytes() => 
            Reader?.ReadBytes(_totalByteLength);
    }
}
