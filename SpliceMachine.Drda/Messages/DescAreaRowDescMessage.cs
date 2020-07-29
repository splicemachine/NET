using System;
using System.Collections.Generic;

namespace SpliceMachine.Drda
{
    internal sealed class DescAreaRowDescMessage : DrdaResponseBase
    {
        private readonly CommAreaGroupDescriptor _commAreaGroupDescriptor;

        private readonly List<DrdaColumn> _columns;

        internal DescAreaRowDescMessage(
            ResponseMessage response)
            : base(response)
        {
            var reader = ((ReaderCommand) response.Command).Reader;
            _commAreaGroupDescriptor = new CommAreaGroupDescriptor(reader);

            if (reader.ReadUInt8() != 0xFF)
            {
                var holdable = reader.ReadUInt16();
                var returnable = reader.ReadUInt16();
                var scrollable = reader.ReadUInt16();
                var sensitive = reader.ReadUInt16();
                var code = reader.ReadUInt16();
                var keyType = reader.ReadUInt16();

                var rdbName = reader.ReadVarString();
                var schema = reader.ReadVcmVcs();
            }

            var number = reader.ReadUInt16();
            _columns = new List<DrdaColumn>(number);

            for (var i = 0; i < number; i++)
            {
                _columns.Add(new DrdaColumn(reader));
            }
        }

        public Int32 SqlCode => _commAreaGroupDescriptor.SqlCode;

        public IReadOnlyList<DrdaColumn> Columns => _columns;

        internal override Boolean Accept(
            DrdaStatementVisitor visitor) => visitor.Visit(this);
    }
}
