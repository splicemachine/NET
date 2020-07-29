using System;

namespace SpliceMachine.Drda
{
    internal static class SeverityCodes
    {
        public const UInt16 Info = 0;

        public const UInt16 Warning = 4;

        public const UInt16 Error = 8;

        public const UInt16 Severe = 16;

        public const UInt16 AccountDamage = 32;

        public const UInt16 PrimaryDamage = 64;

        public const UInt16 SessionDamage = 128;
    }
}
