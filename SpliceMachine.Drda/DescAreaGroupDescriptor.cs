using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SpliceMachine.Drda
{
    internal sealed class DescAreaGroupDescriptor : IDrdaMessage, ICommand
    {
        private readonly CommAreaGroupDescriptor _commAreaGroupDescriptor;

        private readonly List<DrdaColumn> _columns;

        private readonly Int32 _size;

        [SuppressMessage("ReSharper", "UnusedVariable")]
        public DescAreaGroupDescriptor(
            DrdaStreamReader reader,
            Int32 size)
        {
            _commAreaGroupDescriptor =
                new CommAreaGroupDescriptor(reader, size);

            _size = size;

            if (reader.ReadUInt8() != 0xFF)
            {
                var holdable = reader.ReadUInt16();
                var returnable = reader.ReadUInt16();
                var scrollable = reader.ReadUInt16();
                var sensitive = reader.ReadUInt16();
                var code = reader.ReadUInt16();
                var keyType = reader.ReadUInt16();

                var rdbName = reader.ReadVarString();
                var schema = reader.ReadVcmVcs();
            }

            var number = reader.ReadUInt16();
            _columns = new List<DrdaColumn>(number);

            for (var i = 0; i < number; i++)
            {
                _columns.Add(new DrdaColumn(reader));
            }
        }

        public Int32 GetSize() => _size;

        public void Write(DrdaStreamWriter writer)
        {
            throw new NotImplementedException();
        }

        public CodePoint CodePoint => CodePoint.SQLDARD;

        public IReadOnlyList<DrdaColumn> Columns => _columns;

        public Int32 SqlCode => _commAreaGroupDescriptor.SqlCode;

        public IEnumerator<IDrdaMessage> GetEnumerator()
        {
            yield return _commAreaGroupDescriptor;
            yield return this;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}