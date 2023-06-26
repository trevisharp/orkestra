using System.Collections.Generic;
using Orkestra.LexicalAnalysis;

namespace Orkestra.SyntacticAnalysis;

public class ShiftReduceSyntacticAnalyzer : ISyntacticAnalyzer
{
    public Rule StartRule { get; set; }

    private List<Rule> rules = new List<Rule>();
    public void Add(Rule rule)
        => this.rules.Add(rule);

    public ExpressionTree Parse(IEnumerable<Token> tokens)
    {
        throw new System.NotImplementedException();
    }
}