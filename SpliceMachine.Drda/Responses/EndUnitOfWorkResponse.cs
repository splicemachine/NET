using System;

namespace SpliceMachine.Drda
{
    public sealed class EndUnitOfWorkResponse : DrdaResponseBase
    {
        internal EndUnitOfWorkResponse(
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

                    case UInt8Parameter para when  para.CodePoint == CodePoint.UOWDSP:
                        OperationType = para.Value;
                        break;
                }
            }
        }

        public UInt16 SeverityCode { get; }

        public Byte OperationType { get; }

        internal override Boolean Accept(
            DrdaStatementVisitor visitor) => visitor.Visit(this);
    }
}
