using System;

namespace SpliceMachine.Drda
{
    internal interface IDrdaMessage
    {
        UInt32 GetSize();

        void Write(
            DrdaStreamWriter writer);
    }
}
