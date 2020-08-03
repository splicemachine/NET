using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SpliceMachine.Drda
{
    using static System.Diagnostics.Trace;
    using static ColumnType;

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
                .Select(column => GetColumnValue(reader, column)).ToList());

            return true;
        }

        private static Object GetColumnValue(
            DrdaStreamReader reader,
            DrdaColumn column)
        {
            var type = column.TripletType;
            var baseType = type & (~Nullable);

            if (type != baseType)
            {
                switch (reader.ReadUInt8())
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
                CHAR => reader.ReadString(column.TripletDataSize),

                LONG => reader.ReadVarString(),
                VARMIX => reader.ReadVarString(),
                VARCHAR => reader.ReadVarString(),
                LONGMIX => reader.ReadVarString(),

                SMALL => reader.ReadUInt16(),
                INTEGER => reader.ReadUInt32(),
                INTEGER8 => reader.ReadUInt64(),

                FLOAT4 => BitConverter.ToSingle(
                    BitConverter.GetBytes(reader.ReadUInt32()), 0),
                FLOAT8 => BitConverter.ToDouble(
                    BitConverter.GetBytes(reader.ReadUInt64()), 0),

                DATE => DateTime.ParseExact(
                    reader.ReadString(column.TripletDataSize), 
                    "yyyy-MM-dd", CultureInfo.InvariantCulture),

                TIME => TimeSpan.ParseExact(
                    reader.ReadString(column.TripletDataSize), 
                    "hh:mm:ss", CultureInfo.InvariantCulture),

                TIMESTAMP => DateTime.ParseExact(
                    reader.ReadString(column.TripletDataSize), 
                    "yyyy-MM-dd-hh.mm.ss.fff", CultureInfo.InvariantCulture),

                DECIMAL => reader.ReadDecimal(column.Precision, column.Scale),

                BOOLEAN => reader.ReadUInt8() != 0,

                // TODO: olegra - add support for BLOB and UDT

                // TODO: olegra - just eat the unknown type as ODBC do?
                _ => throw new InvalidOperationException($"Unknown type: 0x{type:X}")
            };
        }
    }
}
