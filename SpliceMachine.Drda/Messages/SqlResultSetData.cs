using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SpliceMachine.Drda
{
    internal sealed class SqlResultSetData : IDrdaMessage, ICommand
    {
        private readonly List<DrdaResultSet> _resultSets;

        private readonly Int32 _size;

        [SuppressMessage("ReSharper", "UnusedVariable")]
        public SqlResultSetData(
            DrdaStreamReader reader,
            Int32 size)
        {
            _size = size;

            var number = reader.ReadUInt16();
            _resultSets = new List<DrdaResultSet>(number);

            for (var i = 0; i < number; i++)
            {
                _resultSets.Add(new DrdaResultSet(reader));
            }
        }

        public IReadOnlyList<DrdaResultSet> ResultSets => _resultSets;

        public Int32 GetSize() => _size;

        public void Write(DrdaStreamWriter writer)
        {
            throw new NotImplementedException();
        }

        public CodePoint CodePoint => CodePoint.SQLRSLRD;

        public IEnumerator<IDrdaMessage> GetEnumerator()
        {
            yield return this;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
