using System;
using System.Linq;

namespace SpliceMachine.Drda
{
    public sealed class CommandCheckResponse : DrdaResponseBase
    {
        internal CommandCheckResponse(
            ResponseMessage response)
            : base(
                response.RequestCorrelationId,
                response.IsChained)
        {
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
