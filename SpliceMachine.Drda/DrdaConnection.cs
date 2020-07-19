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
            await _client
                .ConnectAsync(_options.HostName, _options.Port)
                .ConfigureAwait(false);

            _client.GetStream()
                .RequestResponseSequence(
                    new ExchangeServerAttributesRequest(++_requestCorrelationId))
                .RequestResponseSequence(
                    new AccessSecurityDataRequest(++_requestCorrelationId))
                .RequestResponseSequence(
                    new SecurityCheckRequest(++_requestCorrelationId,
                        _options.UserName, _options.Password))
                .RequestResponseSequence(
                    new AccessRelationalDatabaseRequest(++_requestCorrelationId,
                        _client.Client.LocalEndPoint));
        }
    }
}
