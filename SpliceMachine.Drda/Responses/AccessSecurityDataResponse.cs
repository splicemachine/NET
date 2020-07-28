using System.Linq;

namespace SpliceMachine.Drda
{
    using static System.Diagnostics.Trace;

    public sealed class AccessSecurityDataResponse : DrdaResponseBase
    {
        internal AccessSecurityDataResponse(
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
                        TraceInformation("\tCP: {0} = '{1}'",
                            parameter.CodePoint, EncodingEbcdic.GetString(parameter.Value));
                        break;
                }
            }
        }
    }
}
