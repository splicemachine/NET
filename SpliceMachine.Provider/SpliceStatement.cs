using Simba.DotNetDSI;
using Simba.DotNetDSI.DataEngine;

namespace SpliceMachine.Provider
{
    internal sealed class SpliceStatement : DSIStatement
    {
        public SpliceStatement(
            IConnection connection)
            : base(connection) =>
            LogUtilities.LogFunctionEntrance(Connection.Log, connection);

        public override IDataEngine CreateDataEngine()
        {
            LogUtilities.LogFunctionEntrance(Connection.Log);
            return new SpliceDataEngine(this);
        }
    }
}
