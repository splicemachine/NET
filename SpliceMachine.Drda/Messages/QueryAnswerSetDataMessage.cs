using System;
using System.IO;
using System.Linq;

namespace SpliceMachine.Drda
{
    using static System.Diagnostics.Trace;

    internal sealed class QueryAnswerSetDataMessage : DrdaResponseBase
    {
        private readonly Byte[] _messageBytes;

        internal QueryAnswerSetDataMessage(
            ResponseMessage response)
            : base(response) =>
            _messageBytes = ((ReaderCommand) response.Command).GetMessageBytes();

        internal override Boolean Accept(
            DrdaStatementVisitor visitor) => visitor.Visit(this);

        public void Process(
            QueryContext context)
        {
            using var stream = new MemoryStream(_messageBytes, false);

            var reader = new DrdaStreamReader(stream);
            while (stream.Position < stream.Length &&
                   ProcessSingleRow(reader, context))
            {
                TraceInformation("-------- Next row fetched...");
            }
            TraceWarning($"P: {stream.Position} / L: {stream.Length}");
        }

        private Boolean ProcessSingleRow(
            DrdaStreamReader reader,
            QueryContext context)
        {
            var groupDescriptor = new CommAreaGroupDescriptor(reader);

            reader.ReadUInt8(); // Parent nullable triplet

            if (groupDescriptor.SqlCode == 100 &&
                String.Equals(groupDescriptor.SqlState, "02000"))
            {
                context.HasMoreData = false;
                return false;
            }

            context.Rows.Enqueue(context.Columns
                .Select(reader.ReadColumnValue).ToList());

            return true;
        }
    }
}
