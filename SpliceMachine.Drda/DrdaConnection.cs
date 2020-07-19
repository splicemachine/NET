using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SpliceMachine.Drda
{
    public sealed class DrdaConnection : IDisposable
    {
        private readonly TcpClient _client = new TcpClient(AddressFamily.InterNetwork);

        private readonly DrdaConnectionOptions _options;

        private Int32 _requestCorrelationId;

        public DrdaConnection(
            DrdaConnectionOptions options)
        {
            _options = options;
        }

        public void Dispose() => (_client as IDisposable)?.Dispose();

        public async Task ConnectAsync()
        {
            await _client.ConnectAsync(_options.HostName, _options.Port).ConfigureAwait(false);

            var stream = _client.GetStream();

            stream.RequestResponseSequence<ExchangeServerAttributesRequest>(
                new ExchangeServerAttributesRequest(++_requestCorrelationId));

            stream.RequestResponseSequence<AccessSecurityDataRequest>(
                new AccessSecurityDataRequest(++_requestCorrelationId));

            stream.RequestResponseSequence<SecurityCheckRequest>(
                new SecurityCheckRequest(++_requestCorrelationId, _options.UserName, _options.Password));

            stream.RequestResponseSequence<AccessRelationalDatabaseRequest>(
                new AccessRelationalDatabaseRequest(++_requestCorrelationId, _client.Client.LocalEndPoint));
        }
    }
}
