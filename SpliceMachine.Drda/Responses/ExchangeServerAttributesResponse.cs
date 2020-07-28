using System.Linq;

namespace SpliceMachine.Drda
{
    using static System.Diagnostics.Trace;

    public sealed class ExchangeServerAttributesResponse : DrdaResponseBase
    {
        internal ExchangeServerAttributesResponse(
            ResponseMessage response)
            : base(
                response.RequestCorrelationId,
                response.IsChained)
        {
            foreach (var parameter in response.Command.OfType<BytesParameter>())
            {
                switch (parameter.CodePoint)
                {
                    case CodePoint.MGRLVLLS:
                        break;

                    default:
                        TraceInformation("\tCP: {0} = '{1}'",
                            parameter.CodePoint, EncodingEbcdic.GetString(parameter.Value));
                        break;
                }
            }
        }
    }
}
