using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SpliceMachine.Drda
{
    internal sealed class CommAreaGroupDescriptor : IDrdaMessage, ICommand
    {
        private readonly Int32 _size;

        [SuppressMessage("ReSharper", "UnusedVariable")]
        public CommAreaGroupDescriptor(
            DrdaStreamReader reader,
            Int32 size)
        {
            _size = size;

            if (reader.ReadUInt8() == 0xFF)
            {
                return;
            }

            SqlCode = reader.ReadUInt32();
            SqlState = reader.ReadString(5);
            var sqlErrProc = reader.ReadString(8);

            if (reader.ReadUInt8() != 0xFF)
            {
                var sqlErr1 = reader.ReadUInt32();
                var sqlErr2 = reader.ReadUInt32();
                RowsUpdated = reader.ReadUInt32();
                var sqlErr4 = reader.ReadUInt32();
                var sqlErr5 = reader.ReadUInt32();
                var sqlErr6 = reader.ReadUInt32();

                var sqlWarn = reader.ReadString(11);
                var rdbName = reader.ReadVarString();

                SqlMessage = reader.ReadVcmVcs();
            }

            if (reader.ReadUInt8() != 0xFF)
            {
                // WORKWORK
            }
        }

        public Int32 SqlCode { get; }

        public String SqlState { get; }

        public Int32 RowsUpdated { get; }

        public String SqlMessage { get; }

        public Int32 GetSize() => _size;

        public void Write(DrdaStreamWriter writer)
        {
            throw new NotImplementedException();
        }

        public CodePoint CodePoint => CodePoint.SQLCARD;

        public IEnumerator<IDrdaMessage> GetEnumerator()
        {
            yield return this;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
