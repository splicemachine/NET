using Simba.DotNetDSI;

namespace SpliceMachine.Provider
{
    internal sealed class SpliceEnvironment : DSIEnvironment
    {
        public SpliceEnvironment(
            IDriver driver)
            : base(driver) =>
            LogUtilities.LogFunctionEntrance(Driver.Log, driver);

        public override IConnection CreateConnection()
        {
            LogUtilities.LogFunctionEntrance(Driver.Log);
            return new SpliceConnection(this);
        }
    }
}
