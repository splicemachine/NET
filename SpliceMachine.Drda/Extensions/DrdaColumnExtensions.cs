using System;
using System.Globalization;

namespace SpliceMachine.Drda
{
    using static ColumnType;

    internal static class DrdaColumnExtensions
    {
        public static Object ReadColumnValue(
            this DrdaColumn column,
            in DrdaStreamReader reader)
        {
            var type = column.TripletType;
            var baseType = type & (~Nullable);

            if (type != baseType)
            {
                var x = reader.ReadUInt8();
                switch (x)
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
                    reader.ReadString(column.TripletDataSize).Substring(0,27),
                    "yyyy-MM-dd-hh.mm.ss.fffffff", CultureInfo.InvariantCulture),

                DECIMAL => reader.ReadDecimal(column.Precision, column.Scale),

                BOOLEAN => reader.ReadUInt8() != 0x00,
                LOBBYTES => reader.ReadBytes((uint)column.Length),
                LOBCMIXED => reader.ReadString((int)column.Length),
                // TODO: olegra - add support for BLOB and UDT

                // TODO: olegra - just eat the unknown type as ODBC do?
                _ => throw new InvalidOperationException($"Unknown type: 0x{type:X}")
            };
        }

        public static void WriteColumnValue(
            this DrdaColumn column,
            in DrdaStreamWriter writer)
        {
            var length = (UInt16)column.Length;
            var type = column.Db2Type.AsDrdaType(ref length);
            var baseType = type & (~Nullable);

            if (type != baseType)
            {
                writer.WriteUInt8(column.Value is null ? (Byte)0xFF : (Byte)0x00);
            }

            switch (baseType)
            {
                case CHAR: // Maybe we need another approach here
                case LONG:
                case VARMIX:
                case VARCHAR:
                case LONGMIX:
                    writer.WriteVarString(Convert.ToString(column.Value));
                    break;

                case SMALL:
                    writer.WriteUInt16(Convert.ToUInt16(column.Value));
                    break;

                case INTEGER:
                    writer.WriteUInt32(Convert.ToUInt32(column.Value));
                    break;

                case INTEGER8:
                    writer.WriteUInt64(Convert.ToUInt64(column.Value));
                    break;

                case FLOAT4:
                    writer.WriteUInt32(
                          BitConverter.ToUInt32(BitConverter.GetBytes(Convert.ToSingle(column.Value)),0));
                    break;

                case FLOAT8:
                    writer.WriteUInt64(
                        BitConverter.ToUInt64(BitConverter.GetBytes(Convert.ToDouble(column.Value)),0));
                    break;

                case DATE:
                    writer.WriteString(Convert.ToDateTime(column.Value)
                        .ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), length);
                    break;

                case TIME:
                    writer.WriteString(Convert.ToDateTime(column.Value)
                        .ToString("hh:mm:ss", CultureInfo.InvariantCulture), length);
                    break;

                case TIMESTAMP:
                    writer.WriteString(Convert.ToDateTime(column.Value)
                        .ToString("yyyy-MM-dd-hh.mm.ss.fffffff", CultureInfo.InvariantCulture)+"00", length);
                    break;

                case DECIMAL:
                    writer.WriteDecimal(Convert.ToDecimal(column.Value),
                        (length >> 8) & 0xFF, length & 0xF);
                    break;

                case BOOLEAN:
                    writer.WriteUInt8(Convert.ToBoolean(column.Value) ? (Byte)0xFF : (Byte)0x00);
                    break;
                case LOBBYTES:
                case LONGVARBYTE:
                case NLONGVARBYTE:
                    var byteArray = (byte[])column.Value;
                    writer.WriteUInt16((UInt16)byteArray.Length);
                    writer.WriteBytes(byteArray);
                    break;
                case LOBCMIXED:
                    writer.WriteVarString((string)column.Value);
                    break;
                // TODO: olegra - add support for BLOB and UDT

                // TODO: olegra - just eat the unknown type as ODBC do?
                default:
                    throw new InvalidOperationException($"Unknown type: 0x{type:X}");
            }
        }

        public static Int32 GetValueSize(
            this DrdaColumn column)
        {
            var length = (UInt16)column.Length;
            var type = column.Db2Type.AsDrdaType(ref length);
            var baseType = type & (~Nullable);

            var nullFlag = (baseType != type ? sizeof(Byte) : 0);
            return nullFlag + baseType switch
            {
                DATE => length,
                TIME => length,
                TIMESTAMP => length,
                SMALL => sizeof(UInt16),
                BOOLEAN => sizeof(Byte),
                FLOAT8 => sizeof(Double),
                FLOAT4 => sizeof(Single),
                INTEGER => sizeof(UInt32),
                INTEGER8 => sizeof(UInt64),
                LOBBYTES => sizeof(UInt32),
                LOBCMIXED => sizeof(UInt32),
                CHAR => sizeof(UInt16) + Convert.ToString(column.Value).Length, // Maybe we need another approach here
                LONG => sizeof(UInt16) + Convert.ToString(column.Value).Length,
                VARMIX => sizeof(UInt16) + Convert.ToString(column.Value).Length,
                VARCHAR => sizeof(UInt16) + Convert.ToString(column.Value).Length,
                LONGMIX => sizeof(UInt16) + Convert.ToString(column.Value).Length,
                DECIMAL => ((length >> 8) & 0x0FF) / 2 + 1,
                LONGVARBYTE => sizeof(UInt32),
                NLONGVARBYTE => sizeof(UInt32),
                _ => throw new InvalidOperationException(
                    $"Unknown DRDA type 0x{type:X}")
            };
        }
    }
}
