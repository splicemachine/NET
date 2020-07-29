using System;
using System.Collections.Generic;

namespace SpliceMachine.Drda
{
    internal sealed class SqlResultSetColumnsMessage : DrdaResponseBase
    {
        private readonly List<DrdaColumn> _columns;

        internal SqlResultSetColumnsMessage(
            ResponseMessage response)
            : base(response)
        {
            var reader = ((ReaderCommand) response.Command).Reader;

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

        internal override Boolean Accept(
            DrdaStatementVisitor visitor) => visitor.Visit(this);
    }
}
