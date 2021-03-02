using System;

namespace SpliceMachine.Drda
{
    using static Db2Type;

    internal static class Db2TypeExtensions
    {
        public static ColumnType AsDrdaType(
            this Db2Type db2Type,
            ref UInt16 length)
        {
            return db2Type switch
            {
                DATE => ColumnType.DATE,
                TIME => ColumnType.TIME,
                LONG => ColumnType.LONG,
                NLONG => ColumnType.NLONG,
                NTIME => ColumnType.NTIME,
                NDATE => ColumnType.NDATE,
                SMALL => ColumnType.SMALL,
                CHAR => ColumnType.VARMIX,
                NCHAR => ColumnType.NVARMIX,
                NSMALL => ColumnType.NSMALL,
                DECIMAL => ColumnType.DECIMAL,
                NUMERIC => ColumnType.DECIMAL,
                INTEGER => ColumnType.INTEGER,
                BIGINT => ColumnType.INTEGER8,
                VARCHAR => ColumnType.VARCHAR,
                BOOLEAN => ColumnType.BOOLEAN,
                NDECIMAL => ColumnType.NDECIMAL,
                NNUMERIC => ColumnType.NDECIMAL,
                NINTEGER => ColumnType.NINTEGER,
                NBIGINT => ColumnType.NINTEGER8,
                NVARCHAR => ColumnType.NVARCHAR,
                NBOOLEAN => ColumnType.NBOOLEAN,
                TIMESTAMP => ColumnType.TIMESTAMP,
                NTIMESTAMP => ColumnType.NTIMESTAMP,
                BLOB => ColumnType.LONGVARBYTE,
               // NBLOB => AdjustLength(ColumnType.NLOBBYTES, ref length),
                NBLOB => ColumnType.NLONGVARBYTE,
                CLOB => AdjustLength(ColumnType.LOBCMIXED, ref length),
                NCLOB => AdjustLength(ColumnType.NLOBCMIXED, ref length),
                FLOAT => length == 4 ? ColumnType.FLOAT4 : ColumnType.FLOAT8,
                NFLOAT => length == 4 ? ColumnType.NFLOAT4 : ColumnType.NFLOAT8,
                _ => throw new InvalidOperationException($"Unknown DB2 SQL type 0x{db2Type:X}")
            };
        }

        // ReSharper disable once RedundantAssignment
        private static ColumnType AdjustLength(ColumnType columnType, ref UInt16 length)
        {
            length = 0x8004;
            return columnType;
        }
    }
}
