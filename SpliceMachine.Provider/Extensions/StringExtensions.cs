using Simba.DotNetDSI.DataEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpliceMachine.Provider.Extensions
{
    internal static class StringExtensions
    {
        public static SqlType GetSqlType(this string dbType)
        {
            switch (dbType)
            {
                case "DATE"         : return SqlType.Type_Date;
                case "NDATE"        : return SqlType.Type_Date;
                case "TIME"         : return SqlType.VarChar;
                case "NTIME"        : return SqlType.VarChar;
                case "TIMESTAMP"    : return SqlType.Type_Timestamp;
                case "NTIMESTAMP"   : return SqlType.Type_Timestamp;
                case "BLOB"         : return SqlType.VarBinary;
                case "NBLOB"        : return SqlType.VarBinary;
                case "CLOB"         : return SqlType.VarChar;
                case "NCLOB"        : return SqlType.VarChar;
                case "VARCHAR"      : return SqlType.VarChar;
                case "NVARCHAR"     : return SqlType.VarChar;
                case "CHAR"         : return SqlType.Char;
                case "NCHAR"        : return SqlType.Char;
                case "LONG"         : return SqlType.BigInt;
                case "NLONG"        : return SqlType.BigInt;
                case "FLOAT"        : return SqlType.Float;
                case "NFLOAT"       : return SqlType.Float;
                case "DECIMAL"      : return SqlType.Decimal;
                case "NDECIMAL"     : return SqlType.Decimal;
                case "BIGINT"       : return SqlType.BigInt;
                case "NBIGINT"      : return SqlType.BigInt;
                case "INTEGER"      : return SqlType.Integer;
                case "NINTEGER"     : return SqlType.Integer;
                case "SMALL"        : return SqlType.SmallInt;
                case "NSMALL"       : return SqlType.SmallInt;
                case "NUMERIC"      : return SqlType.Decimal;
                case "NNUMERIC"     : return SqlType.Decimal;
                case "BOOLEAN"      : return SqlType.Bit;
                case "NBOOLEAN"     : return SqlType.Bit;
                default: throw new NotImplementedException();
            }
        }
    }
}
