using System.Linq;
using System.Text;

namespace SpliceMachine.Drda
{
    using static System.Diagnostics.Trace;

    public sealed class AccessRelationalDatabaseResponse : DrdaResponseBase
    {
        internal AccessRelationalDatabaseResponse(
            ResponseMessage response)
            : base(
                response.RequestCorrelationId,
                response.IsChained)
        {
            foreach (var parameter in response.Command.OfType<BytesParameter>())
            {
                switch (parameter.CodePoint)
                {
                    case CodePoint.PRDID:
                    case CodePoint.TYPDEFNAM:
                        TraceInformation("\tCP: {0} = '{1}'",
                            parameter.CodePoint, Encoding.UTF8.GetString(parameter.Value));
                        break;
                }
            }
        }
    }
}
