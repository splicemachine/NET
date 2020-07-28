﻿using System;

namespace SpliceMachine.Drda
{
    internal sealed class OpenQueryCompleteResponse : DrdaResponseBase
    {
        public OpenQueryCompleteResponse(
            ResponseMessage response)
            : base(
                response.RequestCorrelationId,
                response.IsChained)
        {
            Console.WriteLine($"RCID: {RequestCorrelationId}, CP: {response.Command.CodePoint}");

            foreach (var parameter in response.Command)
            {
                switch (parameter)
                {
                    case UInt16Parameter para when para.CodePoint == CodePoint.SRVCOD:
                        SeverityCode = para.Value;
                        break;

                    case UInt64Parameter para when para.CodePoint == CodePoint.QRYINSID:
                        QueryInstanceId = para.Value;
                        break;
                }
            }
        }

        public UInt16 SeverityCode { get; }

        public UInt64 QueryInstanceId { get; }
    }
}
