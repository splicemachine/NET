﻿using System;
using System.IO;

namespace SpliceMachine.Drda
{
    using static System.Diagnostics.Trace;

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

            switch (response.Command.CodePoint)
            {
                case CodePoint.EXSATRM:
                    return new ExchangeServerAttributesResponse(response);

                case CodePoint.ACCSECRM:
                    return new AccessSecurityDataResponse(response);

                case CodePoint.SECCHKRM:
                    return new SecurityCheckResponse(response);

                case CodePoint.ACCRDBRM:
                    return new AccessRelationalDatabaseResponse(response);

                case CodePoint.CMDCHKRM:
                    return new CommandCheckResponse(response);

                case CodePoint.SQLERRRM:
                    return new SqlErrorResponse(response);

                case CodePoint.RDBUPDRM:
                    return new RelationalDatabaseUpdateResponse(response);

                case CodePoint.PBSD:
                    return new PiggyBackSchemaDescResponse(response);

                case CodePoint.SQLCARD:
                    return new CommAreaRowDescResponse(response);

                case CodePoint.SQLDARD:
                    return new DescAreaRowDescResponse(response);

                case CodePoint.SYNTAXRM:
                    return new SyntaxResponse(response);

                case CodePoint.RSLSETRM:
                    return new RelationalDatabaseResultSetResponse(response);

                case CodePoint.SQLRSLRD:
                    return new SqlResultSetDataResponse(response);

                case CodePoint.OPNQRYRM:
                    return new OpenQueryCompleteResponse(response);

                case CodePoint.SQLCINRD:
                    return new SqlResultSetColumnInfoResponse(response);

                case CodePoint.QRYDSC:
                    return new QueryAnswerSetDescResponse(response);

                case CodePoint.QRYDTA:
                    return new QueryAnswerSetDataResponse(response);

                case CodePoint.EXTDTA:
                    return new QueryAnswerSetExtraDataResponse(response);

                case CodePoint.ENDUOWRM:
                    return new EndUnitOfWorkResponse(response);

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}