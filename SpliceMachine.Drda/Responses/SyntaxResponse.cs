using System;

namespace SpliceMachine.Drda
{
    public sealed class SyntaxResponse : DrdaResponseBase
    {
        internal SyntaxResponse(
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

                    case UInt8Parameter para when para.CodePoint == CodePoint.SYNERRCD:
                        ErrorCode = para.Value;
                        break;
                }
            }
        }

        public UInt16 SeverityCode { get; }

        public Byte ErrorCode { get; }
    }
}
