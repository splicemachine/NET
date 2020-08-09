using System.Diagnostics.CodeAnalysis;

namespace SpliceMachine.Drda
{
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal enum Db2Type
    {
        DATE = 0x180,

        NDATE = 0x181,
        
        TIME = 0x184,
        
        NTIME = 0x185,
        
        TIMESTAMP = 0x188,
        
        NTIMESTAMP = 0x189,
        
        BLOB = 0x194,
        
        NBLOB = 0x195,
        
        CLOB = 0x198,
        
        NCLOB = 0x199,
        
        VARCHAR = 0x1C0,
        
        NVARCHAR = 0x1C1,
        
        CHAR = 0x1C4,
        
        NCHAR = 0x1C5,
        
        LONG = 0x1C8,
        
        NLONG = 0x1C9,
        
        FLOAT = 0x1E0,
        
        NFLOAT = 0x1E1,
        
        DECIMAL = 0x1E4,
        
        NDECIMAL = 0x1E5,

        BIGINT = 0x1EC,
        
        NBIGINT = 0x1ED,
        
        INTEGER = 0x1F0,
        
        NINTEGER = 0x1F1,
        
        SMALL = 0x1F4,
        
        NSMALL = 0x1F5,

        NUMERIC = 0x1F8,
        
        NNUMERIC = 0x1F9,

        BOOLEAN = 0x984,
        
        NBOOLEAN = 0x985,
    }
}
