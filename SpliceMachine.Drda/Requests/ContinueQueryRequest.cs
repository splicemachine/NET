﻿using System;

namespace SpliceMachine.Drda
{
    internal sealed class ContinueQueryRequest : IDrdaRequest
    {
        private readonly UInt16 _packageSerialNumber;

        private readonly UInt64 _queryInstanceId;

        public ContinueQueryRequest(
            UInt16 requestCorrelationId,
            UInt16 packageSerialNumber,
            UInt64 queryInstanceId)
        {
            _packageSerialNumber = packageSerialNumber;
            _queryInstanceId = queryInstanceId;
            RequestCorrelationId = requestCorrelationId;
        }
        
        public UInt16 RequestCorrelationId { get; }

        MessageFormat IDrdaRequest.Format => MessageFormat.Request;

        public void CheckResponseType(DrdaResponseBase response)
        {
        }

        CompositeCommand IDrdaRequest.GetCommand() =>
            new CompositeCommand(
                CodePoint.CNTQRY,
                new PackageSerialNumber(_packageSerialNumber),
                new UInt32Parameter(CodePoint.QRYBLKSZ, 0x100000),
                new UInt16Parameter(CodePoint.MAXBLKEXT, 0xFFFF),
                new UInt64Parameter(CodePoint.QRYINSID, _queryInstanceId));
    }

}
