using System;

namespace SpliceMachine.Drda
{
    [Flags]
    internal enum MessageFormat : byte
    {
        Request = 0x01,
        Response = 0x02,
        DataObject = 0x03,
        Correlated = 0x10,
        Continued = 0x20,
        Chained = 0x40
    }
}
