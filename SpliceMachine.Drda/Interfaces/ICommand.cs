using System.Collections.Generic;

namespace SpliceMachine.Drda
{
    internal interface ICommand : IDrdaMessage, IEnumerable<IDrdaMessage>
    {
        CodePoint CodePoint { get; }
    }
}
