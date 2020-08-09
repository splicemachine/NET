using System;
using System.Collections.Generic;

namespace SpliceMachine.Drda
{
    public sealed class CommAreaRowDescMessage : DrdaResponseBase
    {
        private static readonly Char[] MessagesSeparator = { (Char)0x14 };

        private readonly CommAreaGroupDescriptor _commAreaGroupDescriptor;

        private readonly String[] _messages;

        internal CommAreaRowDescMessage(
            ResponseMessage response)
            : base(response)
        {
            var reader = ((ReaderCommand) response.Command).Reader;
            _commAreaGroupDescriptor = new CommAreaGroupDescriptor(reader);

            _messages = _commAreaGroupDescriptor?.SqlMessage
                .Split(MessagesSeparator, StringSplitOptions.RemoveEmptyEntries);
        }

        public UInt32 SqlCode => _commAreaGroupDescriptor.SqlCode;

        public String SqlState => _commAreaGroupDescriptor.SqlState;

        public UInt32 RowsUpdated => _commAreaGroupDescriptor.RowsUpdated;

        public IReadOnlyCollection<String> SqlMessages => _messages;

        internal override Boolean Accept(
            DrdaStatementVisitor visitor) => visitor.Visit(this);
    }
}
