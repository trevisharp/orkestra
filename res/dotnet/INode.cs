using System.Collections.Generic;

namespace Orkestra;

public interface INode
{
    bool Is(IRuleElement token);
    IRuleElement Element { get; }
}