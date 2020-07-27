﻿using System;
using System.IO;

namespace SpliceMachine.Drda
{
    public sealed class DrdaStreamReader
    {
        private readonly Stream _stream;

        public DrdaStreamReader(
            Stream stream)
        {
            _stream = stream;
        }

        public Byte ReadUInt8() => (Byte)_stream.ReadByte();

        public UInt16 ReadUInt16()
        {
            var result = _stream.ReadByte();
            return (UInt16)((result << 8) | _stream.ReadByte());
        }

        public UInt16 ReadUInt32()
        {
            var result = _stream.ReadByte();
            result = (UInt16)((result << 8) | _stream.ReadByte());
            result = (UInt16)((result << 8) | _stream.ReadByte());
            return (UInt16)((result << 8) | _stream.ReadByte());
        }

        public Byte[] ReadBytes(Int32 parameterSize)
        {
            var buffer = new Byte[parameterSize];
            _stream.Read(buffer, 0, parameterSize);
            return buffer;
        }
    }
}
