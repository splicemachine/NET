using System;
using System.Text;

namespace SpliceMachine.Drda
{
    internal static class EncodingExtensions
    {
        public static IDrdaMessage GetParameter(
            this Encoding encoding,
            CodePoint codePoint,
            String value) =>
            new BytesParameter(codePoint, encoding.GetBytes(value));
    }
}
