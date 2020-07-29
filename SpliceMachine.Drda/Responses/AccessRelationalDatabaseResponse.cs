using System;
using System.Text;

namespace SpliceMachine.Drda
{
    public sealed class AccessRelationalDatabaseResponse : DrdaResponseBase
    {
        internal AccessRelationalDatabaseResponse(
            ResponseMessage response)
            : base(
                response.RequestCorrelationId,
                response.IsChained)
        {
            foreach (var parameter in response.Command)
            {
                switch (parameter)
                {
                    case BytesParameter para when para.CodePoint == CodePoint.PRDID:
                        ProductId = Encoding.UTF8.GetString(para.Value);
                        break;

                    case BytesParameter para when para.CodePoint == CodePoint.TYPDEFNAM:
                        TypeDefinitionName = Encoding.UTF8.GetString(para.Value);
                        break;
                }
            }
        }

        public String ProductId { get; }

        public String TypeDefinitionName { get; }
    }
}
