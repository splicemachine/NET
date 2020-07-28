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

        SYNERRCD = 0x114A,

        PRPSQLSTT = 0x200D,

        SQLDARD = 0x2411,

        RTNSQLDA = 0x2116,

        TYPSQLDA = 0x2146,

        QRYBLKSZ = 0x2114,

        MAXBLKEXT = 0x2141,

        EXCSQLSTT = 0x200B,

        RSLSETRM = 0x2219,

        SQLRSLRD = 0x240E,

        OPNQRYRM = 0x2205,

        QRYINSID = 0x215B,

        SQLCINRD = 0x240B,

        QRYDSC = 0x241A,

        QRYDTA = 0x241B,

        ENDUOWRM = 0x220C,

        CNTQRY = 0x2206,

        EXTDTA = 0x146C,

        CLSQRY = 0x2005,

        SECCHKCD = 0x11A4,

        QRYPRCTYP = 0x2102,

        SQLCSRHLD = 0x211F,

        QRYATTUPD = 0x2150,

        PKGSNLST = 0x2139
    }
}
