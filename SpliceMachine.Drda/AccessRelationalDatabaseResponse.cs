using System;
using System.Linq;
using System.Text;

namespace SpliceMachine.Drda
{
    public sealed class AccessRelationalDatabaseResponse
        : DrdaResponseBase
    {
        internal AccessRelationalDatabaseResponse(
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
                    case CodePoint.PRDID:
                    case CodePoint.TYPDEFNAM:
                        Console.WriteLine("\tCP: {0} = '{1}'",
                            parameter.CodePoint, Encoding.UTF8.GetString(parameter.Value));
                        break;
                }
            }
        }
    }
}
