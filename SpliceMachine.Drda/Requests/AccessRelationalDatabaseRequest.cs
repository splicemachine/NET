using System;
using System.Net;
using System.Text;

namespace SpliceMachine.Drda
{
    public sealed class AccessRelationalDatabaseRequest
        : DrdaRequestBase<AccessRelationalDatabaseResponse>, IDrdaRequest
    {
        private readonly String _correlationToken;

        public AccessRelationalDatabaseRequest(
            UInt16 requestCorrelationId,
            EndPoint endPoint)
            : base(
                requestCorrelationId) =>
            _correlationToken = endPoint.GetCorrelationToken();

        CompositeCommand IDrdaRequest.GetCommand() =>
            new CompositeCommand(
                CodePoint.ACCRDB,
                Encoding.UTF8.GetParameter(CodePoint.RDBNAM, WellKnownStrings.DatabaseName),
                new UInt16Parameter(CodePoint.RDBACCCL, 0x2407), // SQLAM
                Encoding.UTF8.GetParameter(CodePoint.PRDID, "SNC10090"), // 
                Encoding.UTF8.GetParameter(CodePoint.TYPDEFNAM, "QTDSQLASC"),

                // TODO: olegra - place for enabling/disbling Snappy compression support
                Encoding.UTF8.GetParameter(CodePoint.PRDDTA, "Splice ODBC Driver"),

                Encoding.ASCII.GetParameter(CodePoint.CRRTKN, _correlationToken),
                new CompositeCommand(
                    CodePoint.TYPDEFOVR,
                    new UInt16Parameter(CodePoint.CCSIDSBC, 1208), // CCSID_1208
                    new UInt16Parameter(CodePoint.CCSIDDBC, 1208), // CCSID_1208
                    new UInt16Parameter(CodePoint.CCSIDMBC, 1208)) // CCSID_1208
            );
    }
}
