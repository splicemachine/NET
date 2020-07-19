using System;
using System.Linq;

namespace SpliceMachine.Drda
{
    public sealed class SecurityCheckResponse : DrdaResponseBase
    {
        internal SecurityCheckResponse(
            ResponseMessage response)
            : base(response.RequestCorrelationId)
        {
            Console.WriteLine($"RCID: {RequestCorrelationId}, CP: 0x{response.Command.CodePoint:X4}");

            foreach (var parameter in response.Command.Parameters.OfType<BytesParameter>())
            {
                switch (parameter.CodePoint)
                {
                    case CodePoint.RDBNAM:
                        Console.WriteLine("\tCP: 0x{0:X4} = '{1}'",
                            parameter.CodePoint, EncodingEbcdic.GetString(parameter.Value));
                        break;
                }
            }
        }
    }
}
