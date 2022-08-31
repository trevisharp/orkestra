namespace Orkestra.LexicalAnalysis;

using SyntacticAnalysis;

public interface INode
{
    bool Is(IRuleElement token);
    IRuleElement Element { get; }
}