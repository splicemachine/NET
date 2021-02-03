using Simba.DotNetDSI;
using Simba.DotNetDSI.DataEngine;
using SpliceMachine.Drda;

namespace SpliceMachine.Provider
{
    internal sealed class SpliceStatement : DSIStatement
    {
        private DrdaConnection _drdaConnection;
        public SpliceStatement(
            IConnection connection,DrdaConnection drdaConnection)
            : base(connection)
        {
            this._drdaConnection = drdaConnection;
            LogUtilities.LogFunctionEntrance(Connection.Log, connection);
        }

        public override IDataEngine CreateDataEngine()
        {
            LogUtilities.LogFunctionEntrance(Connection.Log);
            return new SpliceDataEngine(this,_drdaConnection);
        }
    }
}
