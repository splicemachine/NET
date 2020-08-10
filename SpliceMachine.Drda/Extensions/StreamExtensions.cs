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
            var response = new ResponseMessage(
                new DrdaStreamReader(stream.AsReadonlyMemoryStream()));

            TraceInformation($"RCID: {response.RequestCorrelationId}, CP: {response.Command.CodePoint}");

            return response.Command.CodePoint switch
            {
                PBSD => new PiggyBackSchemaDescResponse(response),
                QRYDSC => new QueryAnswerSetDescMessage(response),
                QRYDTA => new QueryAnswerSetDataMessage(response),
                EXTDTA => new QueryAnswerSetExtraMessage(response),
                SQLCARD => new CommAreaRowDescMessage(response),
                SQLDARD => new DescAreaRowDescMessage(response),
                EXSATRM => new ExchangeServerAttributesResponse(response),
                ACCSECRM => new AccessSecurityDataResponse(response),
                SECCHKRM => new SecurityCheckResponse(response),
                ACCRDBRM => new AccessRelationalDatabaseResponse(response),
                CMDCHKRM => new CommandCheckResponse(response),
                SQLERRRM => new SqlErrorResponse(response),
                RDBUPDRM => new RelationalDatabaseUpdateResponse(response),
                SYNTAXRM => new SyntaxResponse(response),
                RSLSETRM => new RelationalDatabaseResultSetResponse(response),
                SQLRSLRD => new SqlResultSetDataMessage(response),
                OPNQRYRM => new OpenQueryCompleteResponse(response),
                SQLCINRD => new SqlResultSetColumnsMessage(response),
                ENDUOWRM => new EndUnitOfWorkResponse(response),
                _ => throw new InvalidOperationException()
            };
        }

        private static Stream AsReadonlyMemoryStream(
            this Stream stream)
        {
            var reader = new DrdaStreamReader(stream);

            var length = reader.ReadUInt16();
            var normalizedLength = (length & 0x7FFF) - sizeof(UInt16);

            var message = new MemoryStream(length & 0x7FFF);
            var writer = new DrdaStreamWriter(message);
            writer.WriteUInt16(length);

            while (true)
            {
                var buffer = new Byte[normalizedLength];
                stream.Read(buffer, 0, buffer.Length);
                message.Write(buffer, 0, buffer.Length);

                if ((length & 0x8000) != 0x8000)
                {
                    break;
                }

                length = reader.ReadUInt16();
                normalizedLength = (length & 0x7FFF) - sizeof(UInt16);
                message.Capacity += normalizedLength;
            }

            message.Seek(0, SeekOrigin.Begin);
            return message;
        }
    }
}
