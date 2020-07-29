using System;
using System.Collections.Generic;

namespace SpliceMachine.Drda
{
    internal sealed class SqlResultSetDataMessage : DrdaResponseBase
    {
        private readonly List<DrdaResultSet> _resultSets;

        internal SqlResultSetDataMessage(
            ResponseMessage response)
            : base(response)
        {
            var reader = ((ReaderCommand) response.Command).Reader;

            var number = reader.ReadUInt16();
            _resultSets = new List<DrdaResultSet>(number);

            for (var i = 0; i < number; i++)
            {
                _resultSets.Add(new DrdaResultSet(reader));
            }
        }

        public IReadOnlyList<DrdaResultSet> ResultSets => _resultSets;

        internal override Boolean Accept(
            DrdaStatementVisitor visitor) => visitor.Visit(this);
    }
}
