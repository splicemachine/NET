using System;

namespace SpliceMachine.Drda
{
    public sealed class DrdaConnectionOptions
    {
        public String HostName { get; set; }

        public Int32 Port { get; set; }

        public String UserName { get; set; }

        public String Password { get; set; }
    }
}
