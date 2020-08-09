using System;
using System.Text;

namespace SpliceMachine.Drda
{
    internal static class DrdaStreamWriterExtensions
    {
        private static readonly Decimal[] Scales =
        {
            1M,
            10M,
            100M,
            1000M,
            10000M,
            100000M,
            1000000M,
            10000000M,
            100000000M,
            1000000000M,
            10000000000M,
            100000000000M,
            1000000000000M,
            10000000000000M,
            100000000000000M,
            1000000000000000M,
            10000000000000000M,
            100000000000000000M,
            1000000000000000000M,
            10000000000000000000M,
            100000000000000000000M,
            1000000000000000000000M,
            10000000000000000000000M,
            100000000000000000000000M,
            1000000000000000000000000M,
            10000000000000000000000000M,
            100000000000000000000000000M,
            1000000000000000000000000000M,
            10000000000000000000000000000M
        };

        public static void WriteDecimal(
            this DrdaStreamWriter writer,
            Decimal value,
            Int32 precision,
            Int32 scale)
        {
            var bytes = new Byte[precision / 2 + 1];

            value *= Scales[scale];

            var nybble = value < 0 ? 0x0d : 0x0c;

            value = Decimal.Floor(Decimal.Negate(value));

            var i = bytes.Length - 1;

            if(precision % 2 == 1) 
            {
                nybble |= (Int32)(value % 10) << 4;
                value /= 10;
            }
            bytes[i] = (Byte)(nybble & 0xFF);

            for(--i; i >= 0; --i) 
            {
                nybble = (Int32)(value % 10);
                value /= 10;

                nybble |= (Int32)(value % 10) << 4;
                value /= 10;

                bytes[i] = (Byte)(nybble & 0xFF);
            }

            writer.WriteBytes(bytes);
        }

        public static void WriteString(
            this DrdaStreamWriter writer,
            String value,
            Int32 maxLength)
        {
            value = value?.Substring(0, maxLength) ?? String.Empty;
            var bytes = Encoding.UTF8.GetBytes(value);
            writer.WriteBytes(bytes);
        }

        public static void WriteVarString(
            this DrdaStreamWriter writer,
            String value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            writer.WriteUInt16((UInt16)bytes.Length);
            writer.WriteBytes(bytes);
        }
    }
}
