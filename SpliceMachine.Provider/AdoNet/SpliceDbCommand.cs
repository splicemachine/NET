using System;
using System.Data.Common;
using Simba.ADO.Net;

namespace SpliceMachine.Provider
{
    public sealed class SpliceDbCommand : SCommand
    {
        public override Object Clone()
        {
            var command = new SpliceDbCommand();
            command.CloneFrom(this);
            return command;
        }

        protected override DbParameter CreateDbParameter() => 
            new SpliceDbParameter();
    }
}
