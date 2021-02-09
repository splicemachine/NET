using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using Simba.DotNetDSI;
using Simba.DotNetDSI.DataEngine;
using SpliceMachine.Drda;

namespace SpliceMachine.Provider.DataEngine
{
    public class SpliceDataResult : DSISimpleResultSet
    {

        #region Fields

        /// <summary>
        /// The table columns.
        /// </summary>
        private IList<IColumn> m_Columns = new List<IColumn>();

        /// <summary>
        /// The table data.
        /// </summary>
        private IDrdaStatement _drdaStatement;

        #endregion // Fields

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="log">The logger to use for logging.</param>
        public SpliceDataResult(ILogger log, IDrdaStatement drdaStatement) : base(log)
        {
            this._drdaStatement = drdaStatement;
            LogUtilities.LogFunctionEntrance(Log, log);
            InitializeColumns();
        }

        #endregion // Constructor

        #region Properties

        /// <summary>
        /// Retrieve the columns that represent the data provided in the result set. Even if there 
        /// are no rows in the result set, the columns should still be accurate. 
        /// 
        /// The position in the returned list should match the position in the result set. 
        /// The first column should be found at position 0, the second at 1, etc...
        /// </summary>
        public override IList<IColumn> SelectColumns
        {
            get { return m_Columns; }
        }

        #endregion // Properties

        #region Methods

        /// <summary>
        /// Fills in out_data with the data for a given column in the current row.
        /// </summary>
        /// <param name="column">The column to retrieve data from, 0 based.</param>
        /// <param name="offset">The number of bytes in the data to skip before copying.</param>
        /// <param name="maxSize">The maximum number of bytes of data to return.</param>
        /// <param name="out_data">The data to be returned.</param>
        /// <returns>True if there is more data in the column; false otherwise.</returns>
        public override bool GetData(
            int column,
            long offset,
            long maxSize,
            out object out_data)
        {
            LogUtilities.LogFunctionEntrance(Log, column, offset, maxSize, "out_data");
            out_data = _drdaStatement.GetColumnValue(column);
            return false;

        }

        /// <summary>
        /// Closes the result set's internal cursor. After a call to this method, no
        /// more calls will be made to MoveToNextRow() and GetData().
        /// </summary>
        protected override void DoCloseCursor()
        {
            LogUtilities.LogFunctionEntrance(Log);
        }

        /// <summary>
        /// Indicates that the cursor should be moved to the next row.
        /// </summary>
        /// <returns>True if there are more rows; false otherwise.</returns>
        protected override bool MoveToNextRow()
        {
            LogUtilities.LogFunctionEntrance(Log);
            return _drdaStatement.Fetch();
        }

        /// <summary>
        /// Initialize the column metadata for the result set.
        /// </summary>
        private void InitializeColumns()
        {
            LogUtilities.LogFunctionEntrance(Log);
            for (int i = 0; i < _drdaStatement.Columns; i++)
            {
                DSIColumn column = new DSIColumn(TypeMetadataFactory.CreateTypeMetadata(SqlType.WVarChar));
                column.IsNullable = Nullability.Nullable;
                //column.Catalog = UltraLight.UL_CATALOG;
                column.Schema = _drdaStatement.GetSchemaName(i);
                //column.TableName = UltraLight.UL_TABLE;
                column.Name = _drdaStatement.GetColumnName(i);
                column.Label = _drdaStatement.GetColumnLabel(i);
                column.Size = _drdaStatement.GetColumnSize(i);
                m_Columns.Add(column);
            }            
        }

        #endregion // Methods

    }
}
