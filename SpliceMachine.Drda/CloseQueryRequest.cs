using System;

namespace SpliceMachine.Drda
{
    internal sealed class CloseQueryRequest : IDrdaRequest
    {
        private readonly UInt16 _packageSerialNumber;

        private readonly UInt64 _queryInstanceId;

        public CloseQueryRequest(
            Int32 requestCorrelationId,
            UInt16 packageSerialNumber,
            UInt64 queryInstanceId)
        {
            _packageSerialNumber = packageSerialNumber;
            _queryInstanceId = queryInstanceId;
            RequestCorrelationId = requestCorrelationId;
        }
        
        public Int32 RequestCorrelationId { get; }

        MessageFormat IDrdaRequest.Format => MessageFormat.Request;

        public void CheckResponseType(DrdaResponseBase response)
        {
        }

        CompositeParameter IDrdaRequest.GetCommand() =>
            new CompositeParameter(
                CodePoint.CLSQRY,
                new PackageSerialNumber(_packageSerialNumber),
                new UInt64Parameter(CodePoint.QRYINSID, _queryInstanceId));
    }
}
