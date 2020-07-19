using System;
using System.IO;

namespace SpliceMachine.Drda
{
    internal static class StreamExtensions
    {
        public static Stream RequestResponseSequence<TRequest>(
            this Stream stream, 
            TRequest request)
            where TRequest : IDrdaRequest
        {
            new RequestMessage(
                    request.RequestCorrelationId,
                    request.GetCommand())
                .Write(new DrdaStreamWriter(stream));

            request.CheckResponseType(stream.ReadMessage());

            return stream;
        }

        private static DrdaResponseBase ReadMessage(
            this Stream stream)
        {
            var response = new ResponseMessage(new DrdaStreamReader(stream));

            switch (response.Command.CodePoint)
            {
                case CodePoint.EXSATRM:
                    return new ExchangeServerAttributesResponse(response);

                case CodePoint.ACCSECRM:
                    return new AccessSecurityDataResponse(response);

                case CodePoint.SECCHKRM:
                    return  new SecurityCheckResponse(response);

                case CodePoint.ACCRDBRM:
                    return new AccessRelationalDatabaseResponse(response);

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
