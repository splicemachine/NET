using System;

namespace SpliceMachine.Drda
{
    internal sealed class QueryAnswerSetExtraMessage : DrdaResponseBase
    {
        internal QueryAnswerSetExtraMessage(
            ResponseMessage response)
            : base(response)
        {
            // TODO: olegra - implement parsing logic correctly
            ((ReaderCommand) response.Command).GetMessageBytes();
        }

        internal override Boolean Accept(
            DrdaStatementVisitor visitor) => visitor.Visit(this);
    }
}
