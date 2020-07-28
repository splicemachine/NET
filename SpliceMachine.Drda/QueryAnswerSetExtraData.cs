using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SpliceMachine.Drda
{
    internal class QueryAnswerSetExtraData : IDrdaMessage, ICommand
    {
        private const Int32 BaseSize = sizeof(UInt16) + sizeof(UInt16);

        private readonly Int32 _size;

        [SuppressMessage("ReSharper", "UnusedVariable")]
        public QueryAnswerSetExtraData(
            DrdaStreamReader reader,
            Int32 size)
        {
            _size = size - BaseSize;

            reader.ReadBytes(_size);
        }

        public Int32 GetSize() => _size;

        public void Write(DrdaStreamWriter writer)
        {
            throw new NotImplementedException();
        }

        public CodePoint CodePoint => CodePoint.EXTDTA;

        public IEnumerator<IDrdaMessage> GetEnumerator()
        {
            yield return this;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
