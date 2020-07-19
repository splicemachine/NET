using System;
using System.Linq;

namespace SpliceMachine.Drda
{
    public sealed class ExchangeServerAttributesResponse : DrdaResponseBase
    {
        internal ExchangeServerAttributesResponse(
            ResponseMessage response)
            : base(response.RequestCorrelationId)
        {
            Console.WriteLine($"RCID: {RequestCorrelationId}, CP: 0x{response.Command.CodePoint:X4}");

            foreach (var parameter in response.Command.Parameters.OfType<BytesParameter>())
            {
                switch (parameter.CodePoint)
                {
                    case CodePoint.MGRLVLLS:
                        break;

                    default:
                        Console.WriteLine("\tCP: 0x{0:X4} = '{1}'",
                            parameter.CodePoint, EncodingEbcdic.GetString(parameter.Value));
                        break;
                }
            }
        }
    }
}
