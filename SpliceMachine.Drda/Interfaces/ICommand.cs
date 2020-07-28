using System.Collections.Generic;

namespace SpliceMachine.Drda
{
    internal interface ICommand : IEnumerable<IDrdaMessage>
    {
        CodePoint CodePoint { get; }
    }
}
