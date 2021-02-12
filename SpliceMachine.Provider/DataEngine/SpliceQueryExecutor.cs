using System;
using System.Collections.Generic;
using Simba.DotNetDSI;
using Simba.DotNetDSI.DataEngine;
using SpliceMachine.Drda;
using SpliceMachine.Provider.DataEngine;

namespace SpliceMachine.Provider
{
    internal sealed class SpliceQueryExecutor : IQueryExecutor
    {
        private readonly ILogger _log;
        private readonly IDrdaStatement _drdaStatement;
        private readonly DrdaConnection _drdaConnection;

        public SpliceQueryExecutor(
            ILogger log, IDrdaStatement drdaStatement, DrdaConnection drdaConnection)
        {
            // TODO(ADO)  #09: Implement a QueryExecutor.
            this._drdaStatement = drdaStatement;
            this._drdaConnection = drdaConnection;
            LogUtilities.LogFunctionEntrance(log, log);
            _log = log;
            Results = new List<IResult>();
            // Create the prepared results.            
            Results.Add(new SpliceDataResult(log, _drdaStatement));
            //TODO: Provide the count result based on type of query.
            // TODO(ADO)  #10: Provide parameter information.
            ParameterMetadata = new List<ParameterMetadata>();
            for (int i = 0; i < drdaStatement.Parameters; i++)
            {
                ParameterMetadata.Add(new Simba.DotNetDSI.DataEngine.ParameterMetadata(i+1, ParameterDirection.Input, new TypeMetadata(SqlType.VarChar, SqlType.VarChar.ToString(), 0, 0, 0), false));
            }
        }

        public void Dispose()
        {
            // TODO: olegra - implement proper disposing
        }

        public void CancelExecute()
        {
            LogUtilities.LogFunctionEntrance(_log);
            // TODO: olgra - implement cancellation support            
        }

        /// <summary>
        /// Clears any state that might have been set by CancelExecute().
        /// </summary>
        public void ClearCancel()
        {
            LogUtilities.LogFunctionEntrance(_log);
            // TODO: olgra - implement cancellation support
        }

        /// <summary>
        /// Clears any parameter data that has been pushed down using PushParamData(). The 
        /// ULQueryExecutor may be re-used for execution following this call.
        /// </summary>
        public void ClearPushedParamData()
        {
            LogUtilities.LogFunctionEntrance(_log);
        }

        public void Execute(
            ExecutionContexts contexts, 
            IWarningListener warningListener)
        {
            for (int i = 0; i < contexts.Count; i++)
            {
                for (int j = 0; j < contexts[i].Inputs.Count; j++)
                {
                    _drdaStatement.SetParameterValue(j, contexts[i].Inputs[j].Data);
                }
            }
            // TODO(ADO)  #11: Implement Query Execution.
            _drdaStatement.Execute();
            //TODO: Commit the transaction at known scope
            if (_drdaStatement.RowsUpdated>0)
            {
                Results.Add(new SpliceRowCountResult(_drdaStatement.RowsUpdated));
            }
            LogUtilities.LogFunctionEntrance(_log, contexts, warningListener);
            
            // The contexts argument provides access to the parameters that were not pushed.
            // Statement execution is a 3 step process:
            //      1. Serialize all input parameters into a form that can be consumed by the data 
            //         source. If your data source does not support parameter streaming for pushed
            //         parameters, then you will need to re-assemble them from your parameter cache.
            //         See PushParamData.
            //      2. Send the Execute() message.
            //      3. Retrieve all output parameters from the server and update the contexts with
            //         their contents.
            
            // No action needs to be taken here since the results are static and encapsulated in
            // ULPersonTable and DSISimpleRowCountResult.
        }

        /// <summary>
        /// Informs the ULQueryExecutor that all parameter values which will be pushed have been 
        /// pushed prior to query execution. After the next Execute() call has finished, this pushed 
        /// parameter data may be cleared from memory, even if the Execute() call results in an 
        /// exception being thrown.
        /// 
        /// The first subsequent call to PushParamData() should behave as if the executor has a 
        /// clear cache of pushed parameter values.
        /// </summary>
        public void FinalizePushedParamData()
        {
            LogUtilities.LogFunctionEntrance(_log);
        }

        /// <summary>
        /// Pushes part of an input parameter value down before execution. This value
        /// should be stored for use later during execution.
        /// 
        /// This method may only be called once for any parameter set/parameter
        /// combination (a "parameter cell") where the parameter has a
        /// non-character/binary data type.
        /// 
        /// For parameters with character or binary data types, this method may be
        /// called multiple times for the same parameter set/parameter combination.
        /// The multiple parts should be concatenated together in order to get the
        /// complete value. For character data, the byte array passed down for one
        /// chunk may NOT necessarily be a complete UTF-8 string representation.
        /// There may be bytes provided in the previous or subsequent chunk to
        /// complete codepoints at the start and/or end.
        /// 
        /// The metadata passed in should be taken notice of because it may not match
        /// metadata supplied by a possible call to GetMetadataForParameters(), as
        /// the ODBC consumer is able to change parameter metadata themselves.
        /// </summary>
        /// <param name="parameterSet">The parameter set the pushed value belongs to.</param>
        /// <param name="value">The pushed parameter value, including metadata for identification.</param>
        public void PushParamData(
            Int32 parameterSet, 
            ParameterInputValue value)
        {
            LogUtilities.LogFunctionEntrance(_log, parameterSet, value);
        }

        public IList<ParameterMetadata> ParameterMetadata { get; }

        public IList<IResult> Results { get; }
    }
}
