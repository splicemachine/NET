using System;
using System.Linq;

namespace SpliceMachine.Drda
{
    using static ColumnType;

    internal sealed class QueryAnswerSetDataMessage : DrdaResponseBase
    {
        private readonly DrdaStreamReader _reader;

        internal QueryAnswerSetDataMessage(
            ResponseMessage response)
            : base(response) =>
            _reader = ((ReaderCommand) response.Command).Reader;

        public Boolean HasMoreData => false; // TODO: olegra - implement it correctly

        internal override Boolean Accept(
            DrdaStatementVisitor visitor) => visitor.Visit(this);

        public Boolean ProcessAndFillQueryContextData(
            QueryContext context)
        {
            var groupDescriptor = new CommAreaGroupDescriptor(_reader);

            _reader.ReadUInt8(); // Parent nullable triplet

            if (groupDescriptor.SqlCode == 100 &&
                String.Equals(groupDescriptor.SqlState, "02000"))
            {
                context.HasMoreData = false;
                return false;
            }

            context.Rows.Enqueue(context.Columns
                .Select(GetColumnValue).ToList());

            return true;
        }

        private Object GetColumnValue(DrdaColumn column)
        {
            var type = column.TripletType;
            var baseType = type & (~Nullable);

            if (type != baseType)
            {
                switch (_reader.ReadUInt8())
                {
                    case 0x00:
                        break;

                    case 0xFF:
                        return null;

                    default:
                        throw new InvalidOperationException("Invalid NULL specifier!");
                }
            }

            return baseType switch
            {
                CHAR => _reader.ReadVarString(),
                LONG => _reader.ReadVarString(),
                VARMIX => _reader.ReadVarString(),
                VARCHAR => _reader.ReadVarString(),
                LONGMIX => _reader.ReadVarString(),

                INTEGER => _reader.ReadUInt32(),

                BOOLEAN => _reader.ReadUInt8() != 0,

                _ => throw new InvalidOperationException($"Unknown type: 0x{type:X}")
            };
        }
    }
}
