using System;
using System.Collections.Generic;

namespace SpliceMachine.Drda
{
    public sealed class CommAreaRowDescResponse
        : DrdaResponseBase
    {
        private static readonly Char[] MessagesSeparator = { (Char)0x14 };

        private readonly CommAreaGroupDescResponse _commAreaGroupDescResponse;

        private readonly String[] _messages;

        internal CommAreaRowDescResponse(
            ResponseMessage response)
            : base(
                response.RequestCorrelationId,
                response.IsChained)
        {
            Console.WriteLine($"RCID: {RequestCorrelationId}, CP: {response.Command.CodePoint}");

            _commAreaGroupDescResponse = response.Command as CommAreaGroupDescResponse;
            _messages = _commAreaGroupDescResponse?.SqlMessage
                .Split(MessagesSeparator, StringSplitOptions.RemoveEmptyEntries);
        }

        public Int32 SqlCode => _commAreaGroupDescResponse.SqlCode;

        public String SqlState => _commAreaGroupDescResponse.SqlState;

        public Int32 RowsUpdated => _commAreaGroupDescResponse.RowsUpdated;

        public IReadOnlyCollection<String> SqlMessages => _messages;
    }
}
