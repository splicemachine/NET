﻿using System;

namespace SpliceMachine.Drda
{
    // TODO: olegra - split 'simple' and 'prepared' statements interfaces

    public interface IDrdaStatement
    {
        IDrdaStatement Prepare();

        Boolean Execute();

        Boolean Fetch();

        Int32 Columns { get; }

        String GetColumnName(Int32 index);

        Object GetColumnValue(Int32 index);

        void SetParameterValue(Int32 index, Object value);
    }
}
