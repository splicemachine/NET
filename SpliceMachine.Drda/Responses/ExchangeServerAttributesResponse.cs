using System;
using System.Linq;

namespace SpliceMachine.Drda
{
    public sealed class ExchangeServerAttributesResponse
        : DrdaResponseBase
    {
        internal ExchangeServerAttributesResponse(
            ResponseMessage response)
            : base(
                response.RequestCorrelationId,
                response.IsChained)
        {
            Console.WriteLine($"RCID: {RequestCorrelationId}, CP: {response.Command.CodePoint}");

            foreach (var parameter in response.Command.OfType<BytesParameter>())
            {
                switch (parameter.CodePoint)
                {
                    case CodePoint.MGRLVLLS:
                        break;

                    default:
                        Console.WriteLine("\tCP: {0} = '{1}'",
                            parameter.CodePoint, EncodingEbcdic.GetString(parameter.Value));
                        break;
                }
            }
        }
    }
}
