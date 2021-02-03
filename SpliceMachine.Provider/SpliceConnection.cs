using System;
using System.Collections.Generic;
using Simba.DotNetDSI;
using SpliceMachine.Drda;

namespace SpliceMachine.Provider
{
    internal sealed class SpliceConnection : DSIConnection
    {
        private DrdaConnection _drdaConnection;

        public SpliceConnection(
            IEnvironment environment)
            : base(environment)
        {
            LogUtilities.LogFunctionEntrance(Log, environment);
            SetConnectionProperties();
        }

        public override void Dispose()
        {
            base.Dispose();
            _drdaConnection?.Dispose();
        }

        public override void Connect(
            Dictionary<String, Object> connectionSettings)
        {
            LogUtilities.LogFunctionEntrance(Log, connectionSettings);
            Utilities.NullCheck("connectionSettings", connectionSettings);

            _drdaConnection = new DrdaConnection(new DrdaConnectionOptions
            {
                Port = Convert.ToInt32(GetRequiredSetting("PORT", connectionSettings)),
                HostName = Convert.ToString(GetRequiredSetting("HOST", connectionSettings)),
                UserName = Convert.ToString(GetRequiredSetting("UID", connectionSettings)),
                Password = Convert.ToString(GetRequiredSetting("PWD", connectionSettings))
            });

            _drdaConnection.ConnectAsync().Wait();
        }

        public override void Disconnect()
        {
            LogUtilities.LogFunctionEntrance(Log);
            _drdaConnection?.DisconnectAsync().Wait();
        }

        public override Dictionary<String, ConnectionSetting> UpdateConnectionSettings(
            Dictionary<String, Object> requestSettings)
        {
            // TODO(ADO)  #05: Check connection settings.

            LogUtilities.LogFunctionEntrance(Log, requestSettings);
            Utilities.NullCheck("requestSettings", requestSettings);

            var responseSettings = new Dictionary<String, ConnectionSetting>();

            VerifyRequiredSetting("HOST", requestSettings, responseSettings);
            VerifyRequiredSetting("PORT", requestSettings, responseSettings);
            VerifyRequiredSetting("UID", requestSettings, responseSettings);
            VerifyRequiredSetting("PWD", requestSettings, responseSettings);

            return responseSettings;
        }

        protected override void DoReset()
        {
            LogUtilities.LogFunctionEntrance(Log);
            SetConnectionProperties();
        }

        public override IStatement CreateStatement()
        {
            LogUtilities.LogFunctionEntrance(Log);
            return new SpliceStatement(this,_drdaConnection);
        }

        private void SetConnectionProperties()
        {
            SetProperty(ConnectionPropertyKey.DSI_CONN_CURRENT_CATALOG, string.Empty);
            SetProperty(ConnectionPropertyKey.DSI_CONN_SERVER_NAME, string.Empty);
            SetProperty(ConnectionPropertyKey.DSI_CONN_USER_NAME, string.Empty);
        }
    }
}
