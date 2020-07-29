using System;

namespace SpliceMachine.Drda
{
    internal sealed class RelationalDatabaseResultSetResponse : DrdaResponseBase
    {
        internal RelationalDatabaseResultSetResponse(
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
