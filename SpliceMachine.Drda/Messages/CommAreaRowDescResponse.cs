using System;
using System.Collections.Generic;

namespace SpliceMachine.Drda
{
    public sealed class CommAreaRowDescResponse : DrdaResponseBase
    {
        private static readonly Char[] MessagesSeparator = { (Char)0x14 };

        private readonly CommAreaGroupDescriptor _commAreaGroupDescriptor;

        private readonly String[] _messages;

        internal CommAreaRowDescResponse(
            ResponseMessage response)
            : base(response)
        {
            _commAreaGroupDescriptor = response.Command as CommAreaGroupDescriptor;
            _messages = _commAreaGroupDescriptor?.SqlMessage
                .Split(MessagesSeparator, StringSplitOptions.RemoveEmptyEntries);
        }

        public Int32 SqlCode => _commAreaGroupDescriptor.SqlCode;

        public String SqlState => _commAreaGroupDescriptor.SqlState;

        public Int32 RowsUpdated => _commAreaGroupDescriptor.RowsUpdated;

        public IReadOnlyCollection<String> SqlMessages => _messages;

        internal override Boolean Accept(
            DrdaStatementVisitor visitor) => visitor.Visit(this);
    }
}
