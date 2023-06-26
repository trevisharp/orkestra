using System.Collections.Generic;

namespace Orkestra.SyntacticAnalysis;

using LexicalAnalysis;

public interface ISyntacticAnalyzer
{
    Rule StartRule { get; }
    void Add(Rule rule);
    ExpressionTree Parse(IEnumerable<Token> tokens);
}