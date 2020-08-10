using System;
using System.Diagnostics;
using System.IO;

namespace SpliceMachine.Drda
{
    public sealed class DrdaStreamReader //-V3072
    {
        private readonly Stream _stream;

        public DrdaStreamReader(
            Stream stream) =>
            _stream = stream;

        public Byte ReadUInt8() => (Byte)_stream.ReadByte();

        public UInt16 ReadUInt16()
        {
            var result = _stream.ReadByte();
            return (UInt16)((result << 8) | _stream.ReadByte());
        }

        public UInt32 ReadUInt32()
        {
            var result = _stream.ReadByte();
            result = (UInt16)((result << 8) | _stream.ReadByte());
            result = (UInt16)((result << 8) | _stream.ReadByte());
            return (UInt16)((result << 8) | _stream.ReadByte());
        }

        public UInt64 ReadUInt64()
        {
            var result = (UInt64)_stream.ReadByte();
            result = (result << 8) | (UInt32)_stream.ReadByte();
            result = (result << 8) | (UInt32)_stream.ReadByte();
            result = (result << 8) | (UInt32)_stream.ReadByte();
            result = (result << 8) | (UInt32)_stream.ReadByte();
            result = (result << 8) | (UInt32)_stream.ReadByte();
            result = (result << 8) | (UInt32)_stream.ReadByte();
            return (result << 8) | (UInt32)_stream.ReadByte();
        }

        public Byte[] ReadBytes(UInt32 parameterSize)
        {
            var buffer = new Byte[parameterSize];
            if (parameterSize > 0)
            {
                var x = _stream.Read(buffer, 0, buffer.Length);
                if (x != buffer.Length)
                {
                    Trace.TraceInformation($"Read({buffer.Length}) -> {x}");
                }
            }
            return buffer;
        }
    }
}
