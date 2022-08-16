using System.Collections.Generic;

#if DEBUG
using static System.Console;
#endif

namespace Orkestra;

using InternalStructure;

public class SyntacticAnalyzer
{
    public Rule StartRule { get; set; }

    public List<Rule> Rules { get; private set; } = new List<Rule>();
    public void Add(Rule rule) => this.Rules.Add(rule);

    public ExpressionTree Parse(IEnumerable<Token> tokens)
    {
        SyntacticStateGraph graph = new SyntacticStateGraph(tokens, this.Rules);

        throw new System.NotImplementedException();
    }
}