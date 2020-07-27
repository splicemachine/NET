using System;
using System.Collections.Generic;
using System.Text;

namespace SpliceMachine.Drda
{
    internal sealed class DescAreaRowDescResponse : DrdaResponseBase
    {
        private readonly DescAreaGroupDescriptor _descAreaGroupDescriptor;

        internal DescAreaRowDescResponse(
            ResponseMessage response)
            : base(
                response.RequestCorrelationId,
                response.IsChained)
        {
            Console.WriteLine($"RCID: {RequestCorrelationId}, CP: {response.Command.CodePoint}");

            _descAreaGroupDescriptor = response.Command as DescAreaGroupDescriptor;
        }

        public Int32 SqlCode => _descAreaGroupDescriptor.SqlCode;

        public IReadOnlyList<DrdaColumn> Columns => _descAreaGroupDescriptor.Columns;
    }
}
