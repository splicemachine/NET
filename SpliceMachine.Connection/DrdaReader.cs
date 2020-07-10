using System;
using System.IO;

namespace SpliceMachine.Connection
{
    internal sealed class DrdaReader
    {
        private readonly DrdaReader _parent;

        private readonly Stream _stream;

        private Int32 _bytesToRead;

        public DrdaReader(
            Stream stream)
        {
            _stream = stream;
            Size = ReadUInt16();
            _bytesToRead = Size - 2;
        }
        
        public DrdaReader(
            DrdaReader parent)
            : this(parent._stream)
        {
            _parent = parent;
            _parent.Advance(2);
        }

        public UInt16 Size { get; }

        public Boolean HasMoreData => _bytesToRead > 0;

        public Byte ReadByte()
        {
            --_bytesToRead;
            _parent?.Advance(1);
            return (Byte) _stream.ReadByte();
        }

        public UInt16 ReadUInt16()
        {
            _bytesToRead -= 2;
            _parent?.Advance(2);
            var result = _stream.ReadByte();
            return (UInt16)((result << 8) | _stream.ReadByte());
        }

        public Byte[] ReadBytes(Int32 parameterSize)
        {
            _bytesToRead -= parameterSize;
            _parent?.Advance(parameterSize);

            var buffer = new Byte[parameterSize];
            _stream.Read(buffer, 0, parameterSize);
            return buffer;
        }

        private void Advance(Int32 offset) => _bytesToRead -= offset;
    }
}