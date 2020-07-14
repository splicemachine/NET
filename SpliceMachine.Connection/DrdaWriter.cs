using System;
using System.IO;
using System.Text;

namespace SpliceMachine.Connection
{
    internal sealed class DrdaWriter : IDisposable
    {
        private static readonly Encoding EbcdicEncoding = Encoding.GetEncoding(37);

        private readonly Stream _stream = new MemoryStream();

        public void Dispose() => _stream.Dispose();

        public void WriteByte(Byte value) => _stream.WriteByte(value);

        private void WriteBytes(Byte[] value) => 
            _stream.Write(value, 0, value.Length);

        public void WriteUint16(UInt16 value)
        {
            _stream.WriteByte((Byte)((value >> 8) & 0xFF));
            _stream.WriteByte((Byte)(value & 0xFF));
        }

        public void WriteString(String value) => WriteString(value, EbcdicEncoding);

        public void WriteString(String value, Encoding encoding) => WriteBytes(encoding.GetBytes(value));

        public void WriteParameter(UInt16 codePoint, UInt16 value)
        {
            WriteUint16(codePoint);
            WriteUint16(value);
        }

        public void WriteParameter(UInt16 codePoint, String value) => 
            WriteParameter(codePoint, value, EbcdicEncoding);

        public void WriteParameter(UInt16 codePoint, String value, Encoding encoding)
        {
            using (var buffer = new DrdaWriter())
            {
                buffer.WriteUint16(codePoint);
                buffer.WriteString(value, encoding);

                WriteBuffer(buffer);
            }
        }

        public void WriteParameter(UInt16 codePoint, Byte[] value)
        {
            using (var buffer = new DrdaWriter())
            {
                buffer.WriteUint16(codePoint);
                buffer.WriteBytes(value);

                WriteBuffer(buffer);
            }
        }

        public void WriteBuffer(DrdaWriter drdaWriter) => drdaWriter.CopyTo(_stream);

        public void CopyTo(Stream target)
        {
            _stream.Flush();

            var length = _stream.Length + 2;

            target.WriteByte((Byte)((length >> 8) & 0xFF));
            target.WriteByte((Byte)(length & 0xFF));

            _stream.Seek(0, SeekOrigin.Begin);
            _stream.CopyTo(target);

            target.Flush();
        }
    }
}