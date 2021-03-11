using System;
using System.Text;

namespace SpliceMachine.Drda
{
    internal static class DrdaStreamReaderExtensions
    {
        private static readonly Decimal[] Scales =
        {
            1M,
            0.1M,
            0.01M,
            0.001M,
            0.0001M,
            0.00001M,
            0.000001M,
            0.0000001M,
            0.00000001M,
            0.000000001M,
            0.0000000001M,
            0.00000000001M,
            0.000000000001M,
            0.0000000000001M,
            0.00000000000001M,
            0.000000000000001M,
            0.0000000000000001M,
            0.00000000000000001M,
            0.000000000000000001M,
            0.0000000000000000001M,
            0.00000000000000000001M,
            0.000000000000000000001M,
            0.0000000000000000000001M,
            0.00000000000000000000001M,
            0.000000000000000000000001M,
            0.0000000000000000000000001M,
            0.00000000000000000000000001M,
            0.000000000000000000000000001M,
            0.0000000000000000000000000001M
        };

        public static Object ReadColumnValue(
            this DrdaStreamReader reader,
            DrdaColumn column) =>
            column.ReadColumnValue(reader);

        public static Decimal ReadDecimal(
            this DrdaStreamReader reader,
            Int32 precision,
            Int32 scale)
        {
            var value = 0M;

            var bytes = precision / 2;
            for (var index = 0; index < bytes; ++index)
            {
                var nybbles = reader.ReadUInt8();

                var hiNybble = (nybbles >> 4) & 0xF;
                var lowNybble = nybbles & 0xF;

                value = value * 10 + hiNybble;
                value = value * 10 + lowNybble;
            }

            var lastNybbles = reader.ReadUInt8();
            if (precision % 2 == 1)
            {
                var hiNybble = (lastNybbles >> 4) & 0xF;
                value = value * 10 + hiNybble;
            }

            if ((lastNybbles & 0x0F) == 0x0D)
            {
                value = Decimal.Negate(value);
            }

            return value * Scales[scale-1];
        }

        public static String ReadString(
            this DrdaStreamReader reader,
            Int32 maxLength)
        {
            var buffer = new StringBuilder(maxLength);

            do
            {
                var character = (Char) reader.ReadUInt8();
                if (character == 0x00)
                {
                    break;
                }

                buffer.Append(character);
            } while (--maxLength != 0);

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

        public static String ReadVcmVcs(
            this DrdaStreamReader reader,
            Byte hiByte)
        {
            var value = reader.ReadVarString(hiByte);

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

        private static String ReadVarString(
            this DrdaStreamReader reader,
            Byte hiByte)
        {
            var size = (hiByte << 8) | reader.ReadUInt8();
            var bytes = reader.ReadBytes((UInt32) size);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
