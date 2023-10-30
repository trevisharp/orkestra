using System.Collections.Generic;

namespace Orkestra.SyntacticAnalysis;

using InternalStructure;

public class LRSyntacticAnalyzer : ISyntacticAnalyzer
{
    public Rule StartRule { get; set; }

    public List<Rule> Rules { get; private set; } = new List<Rule>();
    public void Add(Rule rule) => this.Rules.Add(rule);

    public ExpressionTree Parse(IEnumerable<Token> tokens)
    {
        throw new System.NotImplementedException();
    }
}