﻿using System;
using System.Collections.Generic;

namespace SpliceMachine.Drda
{
    internal sealed class QueryContext
    {
        private IReadOnlyList<Object> _currentRow;

        public QueryContext(
            DrdaConnection connection,
            Boolean hasParameters)
        {
            HasParameters = hasParameters;
            PackageSerialNumber = connection.GetNextPackageSerialNumber();
        }

        public Boolean HasParameters { get; }

        public UInt16 SeverityCode { get; set; }

        public Boolean HasMoreData { get; set; }

        public UInt16 PackageSerialNumber { get; }

        public UInt64? QueryInstanceId { get; set; }

        public ColumnMode ColumnMode { get; set; }

        public List<DrdaColumn> Columns { get; } = new List<DrdaColumn>();

        public List<DrdaColumn> Parameters { get; } = new List<DrdaColumn>();

        public Queue<IReadOnlyList<Object>> Rows { get; } = new Queue<IReadOnlyList<Object>>();

        public Boolean IsQueryOpened => QueryInstanceId.HasValue;

        public Object this[Int32 index] => _currentRow[index];

        public Boolean Fetch()
        {
            if (Rows.Count == 0)
            {
                return false;
            }

            _currentRow = Rows.Dequeue();
            return true;
        }
    }
}
