using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SpliceMachine.Drda
{
    internal sealed class SqlResultSetColumnInfo : IDrdaMessage, ICommand
    {
        private readonly List<DrdaColumn> _columns;

        private readonly Int32 _size;

        [SuppressMessage("ReSharper", "UnusedVariable")]
        public SqlResultSetColumnInfo(
            DrdaStreamReader reader,
            Int32 size)
        {
            _size = size;

            // SQLDHROW INDICATOR
            reader.ReadUInt8();

            //SQLDHOLD
            reader.ReadUInt16();
            //SQLDRETURN
            reader.ReadUInt16();
            //SQLDSCROLL
            reader.ReadUInt16();
            //SQLDSENSITIVE
            reader.ReadUInt16();
            //SQLDFCODE
            reader.ReadUInt16();
            //SQLDKEYTYPE
            reader.ReadUInt16();
            //SQLRDBNAME
            reader.ReadUInt16();
            //SQLDSCHEMA (NULL VCM/VCS string)
            reader.ReadUInt16();
            reader.ReadUInt16();

            var number = reader.ReadUInt16();
            _columns = new List<DrdaColumn>(number);

            for (var i = 0; i < number; i++)
            {
                _columns.Add(new DrdaColumn(reader));
            }
        }

        public IReadOnlyList<DrdaColumn> Columns => _columns;

        public Int32 GetSize() => _size;

        public void Write(DrdaStreamWriter writer)
        {
            throw new NotImplementedException();
        }

        public CodePoint CodePoint => CodePoint.SQLCINRD;

        public IEnumerator<IDrdaMessage> GetEnumerator()
        {
            yield return this;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
