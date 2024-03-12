namespace Orkestra;

using SyntacticAnalysis;

public interface IMatch
{
    bool Is(ISyntaticElement token);
    ISyntaticElement Element { get; }
}