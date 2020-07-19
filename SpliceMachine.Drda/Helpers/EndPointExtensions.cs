using System;
using System.Globalization;
using System.Net;
using System.Text;

namespace SpliceMachine.Drda
{
    internal static class EndPointExtensions
    {
        public static String GetCorrelationToken(
            this EndPoint endPoint)
        {
            return endPoint is IPEndPoint ipEndPoint
                ? GetCorrelationToken(ipEndPoint)
                : throw new InvalidOperationException();
        }

        private static String GetCorrelationToken(IPEndPoint endPoint)
        {
            // TODO: olegra - generate token properly for both IPv4 and IPv6 modes
            var correlationToken = new StringBuilder(61);

            correlationToken.Append("9F00A8C0"); // endPoint.Address
            correlationToken.Append('.');

            var port = endPoint.Port;
            correlationToken.Append((port & 0xFF).ToString("X2", CultureInfo.InvariantCulture));
            correlationToken.Append(((port & 0xFF00) >> 8).ToString("X2", CultureInfo.InvariantCulture));

            // correlationToken.Append('.');
            correlationToken.Append("012345"); // "0123456789AB"

            return correlationToken.ToString();
        }
    }
}
