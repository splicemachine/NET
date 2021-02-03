using System;
using System.Collections.Generic;
using Simba.DotNetDSI;
using Simba.DotNetDSI.DataEngine;
using SpliceMachine.Drda;

namespace SpliceMachine.Provider
{
    internal sealed class SpliceDataEngine : DSIDataEngine
    {
        DrdaConnection _drdaConnection;
        public SpliceDataEngine(
            IStatement statement, DrdaConnection drdaConnection)
            : base(statement)
        {
            this._drdaConnection = drdaConnection;
            LogUtilities.LogFunctionEntrance(Statement.Connection.Log, statement);
        }

        public override IMetadataSource MakeNewMetadataSource(
            MetadataSourceID metadataID,
            IDictionary<MetadataSourceColumnTag, String> restrictions,
            String escapeChar,
            String identifierQuoteChar,
            Boolean filterAsIdentifier)
        {
            // TODO(ADO)  #07: Create and return your Metadata Sources.

            LogUtilities.LogFunctionEntrance(
                Log,
                metadataID,
                restrictions,
                escapeChar,
                identifierQuoteChar,
                filterAsIdentifier);

            // At the very least, ODBC conforming applications will require the following metadata 
            // sources:
            //
            //  Tables
            //      List of all tables defined in the data source.
            //
            //  CatalogOnly
            //      List of all catalogs defined in the data source.
            //
            //  SchemaOnly
            //      List of all schemas defined in the data source.
            //
            //  TableTypeOnly
            //      List of all table types (TABLE,VIEW,SYSTEM) defined within the data source.
            //
            //  Columns
            //      List of all columns defined across all tables in the data source.
            //
            //  TypeInfo
            //      List of the supported types by the data source.
            //
            //  In some cases applications may provide values to restrict the metadata sources.
            //  These restrictions are stored within restrictions and can be used to restrict
            //  the number of rows that are returned from the metadata source.

            // Columns, Tables, CatalogOnly, SchemaOnly, TableTypeOnly, TypeInfo.
            switch (metadataID)
            {
                case MetadataSourceID.Tables:
                    return new DSIEmptyMetadataSource(); // ULTablesMetadataSource(Log);

                case MetadataSourceID.CatalogOnly:
                    return new DSIEmptyMetadataSource(); // ULCatalogOnlyMetadataSource(Log);

                case MetadataSourceID.SchemaOnly:
                    return new DSIEmptyMetadataSource(); // ULSchemaOnlyMetadataSource(Log);

                case MetadataSourceID.TableTypeOnly:
                    return new DSITableTypeOnlyMetadataSource(Log);

                case MetadataSourceID.Columns:
                    return new DSIEmptyMetadataSource(); // ULColumnsMetadataSource(Log);

                case MetadataSourceID.TypeInfo:
                    return new DSIEmptyMetadataSource(); // ULTypeInfoMetadataSource(Log);

                default:
                    return new DSIEmptyMetadataSource();
            }
        }

        public override IQueryExecutor Prepare(
            String sqlQuery)
        {
            // TODO(ADO)  #08: Prepare a query.
            LogUtilities.LogFunctionEntrance(Log, sqlQuery);

            // This is the point where you will send the request to your SQL-enabled data source for
            // query preparation. You will need to provide your own implementation of IQueryExecutor
            // which should wrap your statement context to the prepared query.
            //
            // Query preparation is really a 3 part process and is described as follows:
            //      1. Generate and send the request to your data source for query preparation.
            //      2. Handle the response and for each statement in the query retrieve its column 
            //         metadata information prior to query execution.  You will need to derive from 
            //         DSISimpleResultSet to create your representation of a result set.  
            //         See ULPersonTable.
            //      3. Create an instance of IQueryExector seeding it with the results of the query.
            //         See ULQueryExecutor.

            // Determine if doing a SELECT or DML/DDL via very, very simple parsing.
            // Example of how to throw a parsing error.
            if (sqlQuery.IndexOf("CALL", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                throw ExceptionBuilder.CreateException(
                    string.Format(Simba.DotNetDSI.Properties.Resources.INVALID_QUERY, sqlQuery));
            }
            if (sqlQuery.StartsWith("INSERT", StringComparison.OrdinalIgnoreCase) || sqlQuery.StartsWith("DELETE", StringComparison.OrdinalIgnoreCase) || sqlQuery.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            {
                var drdaStatement = _drdaConnection.CreateStatement(sqlQuery).Prepare();
                return new SpliceQueryExecutor(Log, drdaStatement,_drdaConnection);
            }
            else
            {
                var drdaStatement = _drdaConnection.CreateStatement(sqlQuery);
                return new SpliceQueryExecutor(Log, drdaStatement,_drdaConnection);
            }
        }
    }
}
