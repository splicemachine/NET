﻿using System;
using System.Collections.Generic;

namespace SpliceMachine.Drda
{
    internal sealed class DescAreaRowDescResponse : DrdaResponseBase
    {
        private readonly DescAreaGroupDescriptor _descAreaGroupDescriptor;

        internal DescAreaRowDescResponse(
            ResponseMessage response)
            : base(response)
        {
            _descAreaGroupDescriptor = response.Command as DescAreaGroupDescriptor;
        }

        public Int32 SqlCode => _descAreaGroupDescriptor.SqlCode;

        public IReadOnlyList<DrdaColumn> Columns => _descAreaGroupDescriptor.Columns;

        internal override Boolean Accept(
            DrdaStatementVisitor visitor) => visitor.Visit(this);
    }
}
