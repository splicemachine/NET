using System;

namespace SpliceMachine.Drda
{
    public sealed class CommandCheckResponse : DrdaResponseBase
    {
        internal CommandCheckResponse(
            ResponseMessage response)
            : base(response)
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

        internal override Boolean Accept(
            DrdaStatementVisitor visitor) => visitor.Visit(this);
    }
}
