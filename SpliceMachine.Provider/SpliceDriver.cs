using System;
using Simba.DotNetDSI;

namespace SpliceMachine.Provider
{
    internal sealed class SpliceDriver : DSIDriver
    {
        public SpliceDriver()
        {
            LogUtilities.LogFunctionEntrance(Log);
            SetDriverPropertyValues();

            // SAMPLE: Adding resource managers here allows you to localize the Simba DotNetDSI and/or ADO.Net components.
            //Simba.DotNetDSI.Properties.Resources.ResourceManager.AddResourceManager(
            //    new System.Resources.ResourceManager("Simba.UltraLight.Properties.SimbaDotNetDSI", GetType().Assembly));
        }

        public override IEnvironment CreateEnvironment()
        {
            LogUtilities.LogFunctionEntrance(Log);
            return new SpliceEnvironment(this);
        }

        // TODO(ADO)  #13: Set the vendor name, which will be prepended to error messages.

        public override String VendorName => "SpliceMachine";

        // TODO(ADO)  #03: Set the driver properties.

        private void SetDriverPropertyValues() => 
            SetProperty(DriverPropertyKey.DSI_DRIVER_DRIVER_NAME, "SpliceMachine");
    }
}
