using System;
using System.IO;

namespace SpliceMachine.Drda
{
    public static class StreamExtensions
    {
        public static void WriteDrdaMessage<TRequest>(
            this Stream stream,
            TRequest request)
            where TRequest : DrdaRequestBase =>
            new RequestMessage(
                    request.RequestCorrelationId,
                    request.GetCommand())
                .Write(new DrdaStreamWriter(stream));

        public static DrdaResponseBase ReadDrdaMessage(
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
