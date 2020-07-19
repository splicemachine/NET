using System;
using System.Linq;
using System.Text;

namespace SpliceMachine.Drda
{
    public sealed class AccessRelationalDatabaseResponse : DrdaResponseBase
    {
        internal AccessRelationalDatabaseResponse(
            ResponseMessage response)
            : base(response.RequestCorrelationId)
        {
            Console.WriteLine($"RCID: {RequestCorrelationId}, CP: 0x{response.Command.CodePoint:X4}");

            foreach (var parameter in response.Command.Parameters.OfType<BytesParameter>())
            {
                switch (parameter.CodePoint)
                {
                    case CodePoint.PRDID:
                    case CodePoint.TYPDEFNAM:
                        Console.WriteLine("\tCP: 0x{0:X4} = '{1}'",
                            parameter.CodePoint, Encoding.UTF8.GetString(parameter.Value));
                        break;
                }
            }
        }
    }
}
