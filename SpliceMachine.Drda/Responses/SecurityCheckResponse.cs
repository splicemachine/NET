using System.Linq;

namespace SpliceMachine.Drda
{
    public sealed class SecurityCheckResponse : DrdaResponseBase
    {
        internal SecurityCheckResponse(
            ResponseMessage response)
            : base(
                response.RequestCorrelationId,
                response.IsChained)
        {
            foreach (var parameter in response.Command.OfType<BytesParameter>())
            {
                switch (parameter.CodePoint)
                {
                    case CodePoint.RDBNAM:
                        System.Diagnostics.Trace.TraceInformation("\tCP: {0} = '{1}'",
                            parameter.CodePoint, EncodingEbcdic.GetString(parameter.Value));
                        break;
                }
            }
        }
    }
}
