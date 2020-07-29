using System;

namespace SpliceMachine.Drda
{
    internal sealed class QueryAnswerSetDataMessage : DrdaResponseBase
    {
        internal QueryAnswerSetDataMessage(
            ResponseMessage response)
            : base(response)
        {
            // TODO: olegra - implement parsing logic correctly
            ((ReaderCommand) response.Command).ConsumeAllBytes();
        }

        public Boolean HasMoreData => false; // TODO: olegra - implement it correctly

        internal override Boolean Accept(
            DrdaStatementVisitor visitor) => visitor.Visit(this);
    }
}
