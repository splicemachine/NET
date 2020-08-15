using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SpliceMachine.Drda
{
    using static System.Diagnostics.Trace;
    using static CodePoint;

    public sealed class DrdaConnection : IDisposable
    {
        private static readonly ISet<CodePoint> AllowedCodePoints =
            new SortedSet<CodePoint>
                { SQLERRRM, SQLCARD, ENDUOWRM };

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

        public async Task DisconnectAsync() => 
            await Task.Yield();

        public IDrdaStatement CreateStatement(
            String sqlStatement) => new DrdaImmediateStatement(this, sqlStatement);

        public Boolean Commit()
        {
            var stream = GetStream();
            var context = new QueryContext(this, false);

            stream.SendRequest(
                new RelationalDatabaseCommitRequest(GetNextRequestCorrelationId()));

            return new DrdaStatementVisitor(AllowedCodePoints, context)
                .ProcessChainedResponses(stream);
        }

        public Boolean Rollback()
        {
            var stream = GetStream();
            var context = new QueryContext(this, false);

            stream.SendRequest(
                new RelationalDatabaseRollbackRequest(GetNextRequestCorrelationId()));

            return new DrdaStatementVisitor(AllowedCodePoints, context)
                .ProcessChainedResponses(stream);
        }

        internal NetworkStream GetStream() => _client.GetStream();

        internal UInt16 GetNextRequestCorrelationId() => unchecked(++_requestCorrelationId);

        internal UInt16 GetNextPackageSerialNumber() => unchecked(++_packageSerialNumber);
    }
}
