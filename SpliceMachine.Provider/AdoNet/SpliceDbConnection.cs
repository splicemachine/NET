using System;
using System.Collections.Generic;
using System.Data.Common;
using Simba.ADO.Net;
using Simba.DotNetDSI;

namespace SpliceMachine.Provider
{
    public sealed class SpliceDbConnection : SConnection
    {
        public SpliceDbConnection()
        {
        }

        public SpliceDbConnection(
            String connectionString) =>
            ConnectionString = connectionString;

        protected override DbProviderFactory DbProviderFactory =>
            SpliceDbFactory.Instance;

        protected override DbCommand CreateDbCommand() =>
            new SpliceDbCommand
            {
                Connection = this
            };

        public override Object Clone()
        {
            var connection = new SpliceDbConnection();
            connection.CloneFrom(this);
            return connection;
        }

        // TODO(ADO)  #02: Construct the IDriver instance.

        protected override IDriver CreateDSIDriverInstance(
            Dictionary<String, Object> connectionSettings) =>
            new SpliceDriver();

        // TODO(ADO)  #14: Set the branding of the registry key to read configuration from.

        /// <summary>
        /// Returns branding of the registry keys to load configuration from.
        /// </summary>
        /// <returns>The branding section for the registry.</returns>
        protected override string GetConfigurationBranding() => 
            @"SpliceMachine\AdoNetProvider";
    }
}
