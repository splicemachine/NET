using System;
using System.Collections.Generic;

namespace SpliceMachine.Drda
{
    internal sealed class QueryContext
    {
        public QueryContext(
            DrdaConnection connection) =>
            PackageSerialNumber = connection.GetNextPackageSerialNumber();
        
        public UInt16 SeverityCode { get; set; }

        public Boolean HasMoreData { get; set; }

        public UInt16 PackageSerialNumber { get; }

        public UInt64? QueryInstanceId { get; set; }

        public List<DrdaColumn> Columns { get; } = new List<DrdaColumn>();

        public Boolean IsQueryOpened => QueryInstanceId.HasValue;
    }
}