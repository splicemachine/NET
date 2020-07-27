using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SpliceMachine.Drda
{
    public sealed class DrdaConnection : IDisposable
    {
        private readonly TcpClient _client = new TcpClient(AddressFamily.InterNetwork);

        private readonly DrdaConnectionOptions _options;

        private Int32 _requestCorrelationId;

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
                Console.WriteLine($"\tPBSD: {response.IsolationLevel} @ {response.Schema}");
            }
        }

        public Boolean ExecuteImmediateSql(
            String sqlStatement)
        {
            var requestCorrelationId = ++_requestCorrelationId;
            var packageSerialNumber = ++_packageSerialNumber;

            var stream = _client.GetStream();
            stream
                .SendRequest(
                    new ExecuteImmediateSqlRequest(requestCorrelationId, packageSerialNumber))
                .SendRequest(
                    new SqlStatementRequest(requestCorrelationId, sqlStatement));

            DrdaResponseBase message;
            do
            {
                message = stream.ReadResponse();
                switch (message)
                {
                    case CommandCheckResponse response:
                        Console.WriteLine($"\tCMDCHKRM: {response.SeverityCode}");
                        break;

                    case SqlErrorResponse response:
                        Console.WriteLine($"\tSQLERRRM: {response.SeverityCode}");
                        break;

                    case RelationalDatabaseUpdateResponse response:
                        Console.WriteLine($"\tRDBUPDRM: {response.SeverityCode}");
                        break;

                    case CommAreaRowDescResponse response:
                        Console.WriteLine(
                            $"\tSQLCARD: '{String.Join(" / ", response.SqlMessages)}' [{response.RowsUpdated}]");
                        break;

                    case PiggyBackSchemaDescResponse response:
                        Console.WriteLine($"\tPBSD: {response.IsolationLevel} @ {response.Schema}");
                        break;

                    default:
                        return false;
                }
            }
            // ReSharper disable once ConstantConditionalAccessQualifier
            while (message?.IsChained ?? false);

            return true;
        }

        public Boolean ExecutePreparedSql(
            String sqlStatement)
        {
            List<DrdaColumn> columns = new List<DrdaColumn>();

            var requestCorrelationId = ++_requestCorrelationId;
            var packageSerialNumber = ++_packageSerialNumber;

            var stream = _client.GetStream();
            stream
                .SendRequest(
                    new PrepareSqlStatement(requestCorrelationId, packageSerialNumber))
                .SendRequest(
                    new SqlStatementRequest(requestCorrelationId, sqlStatement));

            DrdaResponseBase message;
            do
            {
                message = stream.ReadResponse();
                switch (message)
                {
                    case CommandCheckResponse response:
                        Console.WriteLine($"\tCMDCHKRM: {response.SeverityCode}");
                        break;

                    case SqlErrorResponse response:
                        Console.WriteLine($"\tSQLERRRM: {response.SeverityCode}");
                        break;

                    case CommAreaRowDescResponse response:
                        Console.WriteLine(
                            $"\tSQLCARD: '{String.Join(" / ", response.SqlMessages)}' [{response.RowsUpdated}]");
                        break;

                    case DescAreaRowDescResponse response:
                        foreach (var column in response.Columns)
                        {
                            Console.WriteLine(
                                $"\tColumn: {column.BaseName}.{column.Name}");
                        }

                        columns.AddRange(response.Columns);
                        break;

                    default:
                        return false;
                }
            }
            // ReSharper disable once ConstantConditionalAccessQualifier
            while (message?.IsChained ?? false);

            // TODO: olegra - add parameters description request/response processing

            return true;
        }
    }
}
