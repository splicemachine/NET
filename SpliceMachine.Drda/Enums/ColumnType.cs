using System;
using System.Diagnostics.CodeAnalysis;

namespace SpliceMachine.Drda
{
    [Flags]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal enum ColumnType : byte
    {
        Nullable = 0x01,

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

        NBOOLEAN = 0xBF
    }
}
