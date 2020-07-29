﻿using System;

namespace SpliceMachine.Drda
{
    public sealed class RelationalDatabaseUpdateResponse : DrdaResponseBase
    {
        internal RelationalDatabaseUpdateResponse(
            ResponseMessage response)
            : base(
                response.RequestCorrelationId,
                response.IsChained)
        {
            foreach (var parameter in response.Command)
            {
                switch (parameter)
                {
                    case UInt16Parameter para when para.CodePoint == CodePoint.SRVCOD:
                        SeverityCode = para.Value;
                        break;
                }
            }
        }

        public UInt16 SeverityCode { get; }
    }
}