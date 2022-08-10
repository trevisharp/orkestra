using System.Linq;
using System.Collections.Generic;

namespace Orkestra;

using InternalStructure;

public class SyntacticAnalyzer
{
    public Rule StartRule { get; set; }

    public List<Rule> Rules { get; private set; } = new List<Rule>();
    public void Add(Rule rule) => this.Rules.Add(rule);

    public ExpressionTree Parse(IEnumerable<Token> tokens)
    {
        ExpressionTree tree = new ExpressionTree();
        ReduceBuffer buffer = new ReduceBuffer(tokens);

        foreach (var initial in buffer)
        {
            
        }

        tree.Root = buffer.First().Value;
        return tree;
    }
}