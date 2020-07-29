﻿using System;

namespace SpliceMachine.Drda
{
    public sealed class QueryAnswerSetDescMessage : DrdaResponseBase
    {
        internal QueryAnswerSetDescMessage(
            ResponseMessage response)
            : base(response)
        {
            // TODO: olegra - implement parsing logic correctly
            ((ReaderCommand) response.Command).ConsumeAllBytes();
        }

        internal override Boolean Accept(
            DrdaStatementVisitor visitor) => visitor.Visit(this);
    }
}
