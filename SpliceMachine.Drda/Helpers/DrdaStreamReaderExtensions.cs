using System;
using System.Text;

namespace SpliceMachine.Drda
{
    internal static class DrdaStreamReaderExtensions
    {
        public static String ReadString(
            this DrdaStreamReader reader,
            Int32 maxLength)
        {
            var buffer = new StringBuilder(maxLength);

            while (maxLength-- != 0)
            {
                var character = (Char)reader.ReadUInt8();
                if (character == 0x00)
                {
                    break;
                }

                buffer.Append(character);
            }

            return buffer.ToString();
        }

        public static String ReadVarString(
            this DrdaStreamReader reader)
        {
            var size = reader.ReadUInt16();
            var bytes = reader.ReadBytes(size);
            return Encoding.UTF8.GetString(bytes);
        }

        public static String ReadVcmVcs(
            this DrdaStreamReader reader)
        {
            var value = reader.ReadVarString();

            if (String.IsNullOrEmpty(value))
            {
                value = reader.ReadVarString();
            }
            else
            {
                reader.ReadUInt16();
            }

            return value;
        }
    }
}
