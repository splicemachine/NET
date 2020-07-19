using System;
using System.Text;

namespace SpliceMachine.Drda
{
    internal static class EncodingEbcdic
    {
        private static readonly Encoding Instance = Encoding.GetEncoding(37);

        public static String GetString(
            Byte[] bytes) => Instance.GetString(bytes);

        public static BytesParameter GetParameter(
            CodePoint codePoint,
            String value) =>
            new BytesParameter(codePoint, Instance.GetBytes(value));
    }
}
