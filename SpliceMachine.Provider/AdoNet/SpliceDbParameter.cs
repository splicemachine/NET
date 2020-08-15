using System;
using Simba.ADO.Net;

namespace SpliceMachine.Provider
{
    public sealed class SpliceDbParameter : SParameter
    {
        public override Object Clone()
        {
            var parameter = new SpliceDbParameter();
            parameter.CloneFrom(this);
            return parameter;
        }
    }
}
