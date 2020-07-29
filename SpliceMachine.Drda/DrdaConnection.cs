using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SpliceMachine.Drda
{
    using static System.Diagnostics.Trace;

    public sealed class DrdaConnection : IDisposable
    {
        private readonly TcpClient _client = new TcpClient(AddressFamily.InterNetwork);

        private readonly DrdaConnectionOptions _options;

        private UInt16 _requestCorrelationId;

        private UInt16 _packageSerialNumber;

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

            var stream = _client.GetStream();

            stream
                .RequestResponseSequence(
                    new ExchangeServerAttributesRequest(++_requestCorrelationId), out _)
                .RequestResponseSequence(
                    new AccessSecurityDataRequest(++_requestCorrelationId), out _)
                .RequestResponseSequence(
                    new SecurityCheckRequest(++_requestCorrelationId,
                        _options.UserName, _options.Password), out _)
                .RequestResponseSequence(
                    new AccessRelationalDatabaseRequest(++_requestCorrelationId,
                        _client.Client.LocalEndPoint), out var isChained);

            if (isChained &&
                stream.ReadResponse() is PiggyBackSchemaDescResponse response)
            {
                TraceInformation($"\tPBSD: {response.IsolationLevel} @ {response.Schema}");
            }
        }

        public IDrdaStatement CreateStatement(
            String sqlStatement) => new DrdaImmediateStatement(this, sqlStatement);

        internal NetworkStream GetStream() => _client.GetStream();

        internal UInt16 GetNextRequestCorrelationId() => unchecked(++_requestCorrelationId);

        internal UInt16 GetNextPackageSerialNumber() => unchecked(++_packageSerialNumber);
    }
}
