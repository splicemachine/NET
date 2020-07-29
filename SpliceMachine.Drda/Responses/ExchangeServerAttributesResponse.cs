using System;

namespace SpliceMachine.Drda
{
    using static System.Diagnostics.Trace;

    public sealed class ExchangeServerAttributesResponse : DrdaResponseBase
    {
        internal ExchangeServerAttributesResponse(
            ResponseMessage response)
            : base(response)
        {
            foreach (var parameter in response.Command)
            {
                switch (parameter)
                {
                    case BytesParameter para when para.CodePoint == CodePoint.MGRLVLLS:
                        break;

                    case BytesParameter para when para.CodePoint == CodePoint.EXTNAM:
                        ExternalName = EncodingEbcdic.GetString(para.Value);
                        break;
                }
            }
        }

        public String ExternalName { get; }
    }
}
