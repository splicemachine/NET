using System;
using System.IO;

namespace SpliceMachine.Drda
{
    using static System.Diagnostics.Trace;
    using static CodePoint;

    internal static class StreamExtensions
    {
        public static Stream SendRequest<TRequest>(
            this Stream stream, 
            TRequest request)
            where TRequest : IDrdaRequest
        {
            new RequestMessage(
                    request.RequestCorrelationId,
                    request.GetCommand(),
                    request.Format)
                .Write(new DrdaStreamWriter(stream));

            return stream;
        }

        public static Stream RequestResponseSequence<TRequest>(
            this Stream stream, 
            TRequest request,
            out Boolean isChained)
            where TRequest : IDrdaRequest
        {
            new RequestMessage(
                    request.RequestCorrelationId,
                    request.GetCommand(),
                    request.Format)
                .Write(new DrdaStreamWriter(stream));

            var response = stream.ReadResponse();
            request.CheckResponseType(response);

            isChained = response.IsChained;
            return stream;
        }

        public static DrdaResponseBase ReadResponse(
            this Stream stream)
        {
            var response = new ResponseMessage(new DrdaStreamReader(stream));

            TraceInformation($"RCID: {response.RequestCorrelationId}, CP: {response.Command.CodePoint}");

            return response.Command.CodePoint switch
            {
                PBSD => new PiggyBackSchemaDescResponse(response),
                QRYDSC => new QueryAnswerSetDescResponse(response),
                QRYDTA => new QueryAnswerSetDataResponse(response),
                EXTDTA => new QueryAnswerSetExtraDataResponse(response),
                SQLCARD => new CommAreaRowDescResponse(response),
                SQLDARD => new DescAreaRowDescResponse(response),
                EXSATRM => new ExchangeServerAttributesResponse(response),
                ACCSECRM => new AccessSecurityDataResponse(response),
                SECCHKRM => new SecurityCheckResponse(response),
                ACCRDBRM => new AccessRelationalDatabaseResponse(response),
                CMDCHKRM => new CommandCheckResponse(response),
                SQLERRRM => new SqlErrorResponse(response),
                RDBUPDRM => new RelationalDatabaseUpdateResponse(response),
                SYNTAXRM => new SyntaxResponse(response),
                RSLSETRM => new RelationalDatabaseResultSetResponse(response),
                SQLRSLRD => new SqlResultSetDataResponse(response),
                OPNQRYRM => new OpenQueryCompleteResponse(response),
                SQLCINRD => new SqlResultSetColumnInfoResponse(response),
                ENDUOWRM => new EndUnitOfWorkResponse(response),
                _ => throw new InvalidOperationException()
            };
        }
    }
}
