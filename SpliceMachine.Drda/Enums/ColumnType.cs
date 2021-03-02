using System;
using System.Diagnostics.CodeAnalysis;

namespace SpliceMachine.Drda
{
    [Flags]
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal enum ColumnType : byte
    {
        Nullable = 0x01,

        LONGVARBYTE = 0x2A,

        NLONGVARBYTE = 0x2B,

        CHAR = 0x30,

        NCHAR = 0x31,

        VARCHAR = 0x32,

        NVARCHAR = 0x33,

        VARMIX = 0x3E,

        NVARMIX = 0x3F,

        LONG = 0x34,

        NLONG = 0x35,

        LONGMIX = 0x40,

        NLONGMIX = 0x41,

        INTEGER = 0x02,

        NINTEGER = 0x03,

        BOOLEAN = 0xBE,

        NBOOLEAN = 0xBF,

        SMALL = 0x04,

        NSMALL = 0x05,

        DATE = 0x20,

        NDATE = 0x21,

        INTEGER8 = 0x16,

        NINTEGER8 = 0x17,

        FLOAT8 = 0x0A,
        
        NFLOAT8 = 0x0B,
        
        FLOAT4 = 0x0C,
        
        NFLOAT4 = 0x0D,

        TIME = 0x22,

        NTIME = 0x23,

        TIMESTAMP = 0x24,

        NTIMESTAMP = 0x25,

        DECIMAL = 0x0E,

        NDECIMAL = 0x0F,

        LOBBYTES = 0xC8,

        NLOBBYTES = 0xC9,

        LOBCMIXED = 0xCE,

        NLOBCMIXED = 0xCF,
    }
}
