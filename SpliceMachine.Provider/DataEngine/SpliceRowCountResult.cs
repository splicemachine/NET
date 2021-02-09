
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using Simba.DotNetDSI;
using Simba.DotNetDSI.DataEngine;
using SpliceMachine.Drda;

namespace SpliceMachine.Provider.DataEngine
{
    public class SpliceRowCountResult : DSIRowCountResult
    {

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="log">The logger to use for logging.</param>
        public SpliceRowCountResult(long rowCount) : base(rowCount)
        {
        }

        #endregion // Constructor

    }
}
