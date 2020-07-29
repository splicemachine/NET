using System;

namespace SpliceMachine.Drda
{
    public interface IDrdaStatement
    {
        IDrdaStatement Prepare();

        Boolean Execute();
    }
}
