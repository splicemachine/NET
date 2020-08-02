using System;

namespace SpliceMachine.Drda
{
    public interface IDrdaStatement
    {
        IDrdaStatement Prepare();

        Boolean Execute();

        Boolean Fetch();

        Int32 Columns { get; }

        String GetColumnName(Int32 index);

        Object GetColumnValue(Int32 index);
    }
}
