using System;
using System.IO;

namespace SpliceMachine.Drda
{
    internal sealed class DrdaStreamWriter //-V3072
    {
        private readonly Stream _stream;

        public DrdaStreamWriter(
            Stream stream) => 
            _stream = stream;

        public void WriteUInt8(Byte value) => _stream.WriteByte(value);

        public void WriteUInt16(UInt16 value)
        {
            _stream.WriteByte((Byte)((value >> 8) & 0xFF));
            _stream.WriteByte((Byte)(value & 0xFF));
        }

        public void WriteUInt32(UInt32 value)
        {
            _stream.WriteByte((Byte)((value >> 24) & 0xFF));
            _stream.WriteByte((Byte)((value >> 16) & 0xFF));
            _stream.WriteByte((Byte)((value >> 8) & 0xFF));
            _stream.WriteByte((Byte)(value & 0xFF));
        }

        public void WriteUInt64(UInt64 value)
        {
            _stream.WriteByte((Byte)((value >> 56) & 0xFF));
            _stream.WriteByte((Byte)((value >> 48) & 0xFF));
            _stream.WriteByte((Byte)((value >> 40) & 0xFF));
            _stream.WriteByte((Byte)((value >> 32) & 0xFF));
            _stream.WriteByte((Byte)((value >> 24) & 0xFF));
            _stream.WriteByte((Byte)((value >> 16) & 0xFF));
            _stream.WriteByte((Byte)((value >> 8) & 0xFF));
            _stream.WriteByte((Byte)(value & 0xFF));
        }

        public void WriteBytes(Byte[] value) => 
            _stream.Write(value, 0, value.Length);
    }
}
