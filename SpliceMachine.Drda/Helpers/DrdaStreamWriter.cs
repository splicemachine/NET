using System;
using System.IO;

namespace SpliceMachine.Drda
{
    internal sealed class DrdaStreamWriter
    {
        private readonly Stream _stream;

        public DrdaStreamWriter(Stream stream) => 
            _stream = stream;

        public void WriteByte(Byte value) => _stream.WriteByte(value);

        public void WriteUint16(UInt16 value)
        {
            _stream.WriteByte((Byte)((value >> 8) & 0xFF));
            _stream.WriteByte((Byte)(value & 0xFF));
        }

        public void WriteBytes(Byte[] value) => 
            _stream.Write(value, 0, value.Length);
    }
}
