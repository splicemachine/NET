using System;
using System.Linq;

namespace SpliceMachine.Drda
{
    public sealed class SecurityCheckResponse
        : DrdaResponseBase
    {
        internal SecurityCheckResponse(
            ResponseMessage response)
            : base(response.RequestCorrelationId)
        {
            Console.WriteLine($"RCID: {RequestCorrelationId}, CP: {response.Command.CodePoint}");

            foreach (var parameter in response.Command.Parameters.OfType<BytesParameter>())
            {
                switch (parameter.CodePoint)
                {
                    case CodePoint.RDBNAM:
                        Console.WriteLine("\tCP: {0} = '{1}'",
                            parameter.CodePoint, EncodingEbcdic.GetString(parameter.Value));
                        break;
                }
            }
        }
    }
}
