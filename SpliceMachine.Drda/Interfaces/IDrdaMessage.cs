using System;

namespace SpliceMachine.Drda
{
    internal interface IDrdaMessage
    {
        Int32 GetSize();

        void Write(
            DrdaStreamWriter writer);
    }
}
