using System;
using System.ComponentModel;
using System.Data.Common;
using Simba.DotNetDSI;

namespace SpliceMachine.Provider
{
    public sealed class SpliceDbConnectionStringBuilder : DbConnectionStringBuilder
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public SpliceDbConnectionStringBuilder()
            : base()
        {
            ; // Do nothing.
        }

        #endregion // Constructors

        #region Properties

        // TODO(ADO)  #04: Create the properties for the connection string keys.

        [Category("Connection Settings")]
        [DisplayName("HOST")]
        [RefreshProperties(RefreshProperties.All)]
        public string Host
        {
            get
            {
                object outValue;
                if (this.TryGetValue("HOST", out outValue))
                {
                    return outValue as string;
                }

                return "";
            }
            set
            {
                this["HOST"] = value;
            }
        }

        [Category("Connection Settings")]
        [DisplayName("PORT")]
        [RefreshProperties(RefreshProperties.All)]
        public string Port
        {
            get
            {
                object outValue;
                if (this.TryGetValue("PORT", out outValue))
                {
                    return outValue as string;
                }

                return "";
            }
            set
            {
                this["PORT"] = value;
            }
        }

        [Category("Connection Settings")]
        [DisplayName("UID")]
        [RefreshProperties(RefreshProperties.All)]
        public string UserId
        {
            get
            {
                object outValue;
                if (this.TryGetValue("UID", out outValue))
                {
                    return outValue as string;
                }

                return "";
            }
            set
            {
                this["UID"] = value;
            }
        }

        [Category("Connection Settings")]
        [DisplayName("PWD")]
        [RefreshProperties(RefreshProperties.All)]
        public string Password
        {
            get
            {
                object outValue;
                if (this.TryGetValue("PWD", out outValue))
                {
                    return outValue as string;
                }

                return "";
            }
            set
            {
                this["PWD"] = value;
            }
        }



        #endregion // Properties
    }
}
