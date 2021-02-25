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
            Int32 declaredPrecision,
            Int32 declaredScale)
        {
            var buffer = new Byte[declaredPrecision / 2 + 1];

            int[] bits = decimal.GetBits(value);
            byte scale = (byte)((bits[3] >> 16) & 0x7F);
            int bigScale = scale;
            String unscaledStr = (value * IntPow(10,(uint)bigScale)).ToString("0");
            int bigPrecision = unscaledStr.Length;

            int bigWholeIntegerLength = bigPrecision - bigScale;
            if ((bigWholeIntegerLength > 0) && (!unscaledStr.Equals("0")))
            {
                // if whole integer part exists, check if overflow.
                int declaredWholeIntegerLength = declaredPrecision - declaredScale;
                if (bigWholeIntegerLength > declaredWholeIntegerLength)
                {

                }
            }

            // convert the unscaled value to a packed decimal bytes.

            // get unicode '0' value.
            int zeroBase = '0';

            // start index in target packed decimal.
            int packedIndex = declaredPrecision - 1;

            // start index in source big decimal.
            int bigIndex;

            if (bigScale >= declaredScale)
            {
                // If target scale is less than source scale,
                // discard excessive fraction.

                // set start index in source big decimal to ignore excessive fraction.
                bigIndex = bigPrecision - 1 - (bigScale - declaredScale);

                if (bigIndex < 0)
                {
                    // all digits are discarded, so only process the sign nybble.
                    buffer[(packedIndex + 1) / 2] =
                            (byte)((Math.Sign(value) >= 0) ? 12 : 13); // sign nybble
                }
                else
                {
                    // process the last nybble together with the sign nybble.
                    buffer[(packedIndex + 1) / 2] =
                            (byte)(((unscaledStr[bigIndex] - zeroBase) << 4) + // last nybble
                            ((Math.Sign(value) >= 0) ? 12 : 13)); // sign nybble
                }
                packedIndex -= 2;
                bigIndex -= 2;
            }
            else
            {
                // If target scale is greater than source scale,
                // pad the fraction with zero.

                // set start index in source big decimal to pad fraction with zero.
                bigIndex = declaredScale - bigScale - 1;

                // process the sign nybble.
                buffer[(packedIndex + 1) / 2] =
                        (byte)((Math.Sign(value) >= 0) ? 12 : 13); // sign nybble

                for (packedIndex -= 2, bigIndex -= 2; bigIndex >= 0; packedIndex -= 2, bigIndex -= 2)
                {
                    buffer[(packedIndex + 1) / 2] = (byte)0;
                }

                if (bigIndex == -1)
                {
                    buffer[(packedIndex + 1) / 2] =
                            (byte)((unscaledStr[bigPrecision - 1] - zeroBase) << 4); // high nybble

                    packedIndex -= 2;
                    bigIndex = bigPrecision - 3;
                }
                else
                {
                    bigIndex = bigPrecision - 2;
                }
            }

            // process the rest.
            for (; bigIndex >= 0; packedIndex -= 2, bigIndex -= 2)
            {
                buffer[(packedIndex + 1) / 2] =
                        (byte)(((unscaledStr[bigIndex] - zeroBase) << 4) + // high nybble
                        (unscaledStr[bigIndex + 1] - zeroBase)); // low nybble
            }

            // process the first nybble when there is one left.
            if (bigIndex == -1)
            {
                buffer[(packedIndex + 1) / 2] =
                        (byte)(unscaledStr[0] - zeroBase);

                packedIndex -= 2;
            }

            // pad zero in front of the big decimal if necessary.
            for (; packedIndex >= -1; packedIndex -= 2)
            {
                buffer[(packedIndex + 1) / 2] = (byte)0;
            }
            writer.WriteBytes(buffer);            
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

        static int IntPow(int x, uint pow)
        {
            int ret = 1;
            while (pow != 0)
            {
                if ((pow & 1) == 1)
                    ret *= x;
                x *= x;
                pow >>= 1;
            }
            return ret;
        }

    }
}
