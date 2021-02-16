using System;
using System.Collections.Generic;
using System.Text;

namespace SpliceMachine.Drda.Helpers
{
    public class SpliceException : Exception
    {
        public SpliceException(string message) : base(message)
        {

        }
    }
}
