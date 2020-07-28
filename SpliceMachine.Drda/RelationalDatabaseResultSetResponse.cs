using System;
using System.Linq;

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
            Console.WriteLine($"RCID: {RequestCorrelationId}, CP: {response.Command.CodePoint}");

            foreach (var parameter in response.Command.OfType<UInt16Parameter>())
            {
                switch (parameter.CodePoint)
                {
                    case CodePoint.SRVCOD:
                        SeverityCode = parameter.Value;
                        break;
                }
            }
        }

        public UInt16 SeverityCode { get; }
    }
}
