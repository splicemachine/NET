using System.Diagnostics.CodeAnalysis;

namespace SpliceMachine.Drda
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    internal enum CodePoint : ushort
    {
        TYPDEFOVR = 0x0035,

        EXCSAT = 0x1041,

        EXSATRM = 0x1443,
        
        ACCSEC = 0x106D,

        ACCSECRM = 0x14AC,
        
        SECCHK = 0x106E,

        SECCHKRM = 0x1219,

        ACCRDB = 0x2001,

        ACCRDBRM = 0x2201,
            
        EXTNAM = 0x115E,

        RDBNAM = 0x2110,

        SECMEC = 0x11A2,

        PRDID = 0x112E,

        TYPDEFNAM = 0x002F,

        MGRLVLLS = 0x1404,

        PRDDTA = 0x2104,

        CRRTKN = 0x2135,

        SRVCLSNM = 0x1147,

        SRVNAM = 0x116D,

        SRVRLSLV = 0x115A,

        PASSWORD = 0x11A1,

        USRID = 0x11A0,

        RDBACCCL = 0x210F,

        CCSIDSBC = 0x119C,

        CCSIDDBC = 0x119D,

        CCSIDMBC = 0x119E,

        EXCSQLIMM = 0x200A,

        RDBCMTOK = 0x2105,

        PKGNAMCSN = 0x2113,

        SQLSTT = 0x2414,

        CMDCHKRM = 0x1254,

        SRVCOD = 0x1149,

        SQLERRRM = 0x2213,

        RDBUPDRM = 0x2218,

        PBSD = 0xC000,

        PBSD_ISO = 0xC001,

        PBSD_SCHEMA = 0xC002,

        SQLCARD = 0x2408,

        SYNTAXRM = 0x124C,

        SYNERRCD = 0x114A
    }
}
