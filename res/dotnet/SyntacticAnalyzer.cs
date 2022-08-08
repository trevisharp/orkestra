using System.Linq;
using System.Collections.Generic;

namespace Orkestra;

public class SyntacticAnalyzer
{
    public Rule StartRule { get; set; }

    public List<Rule> Rules { get; private set; } = new List<Rule>();
    public void Add(Rule rule) => this.Rules.Add(rule);

    public ExpressionTree Parse(IEnumerable<Token> tokens)
    {
        ExpressionTree tree = new ExpressionTree();

        List<INode> buffer = new List<INode>(tokens);
        int index = 0;
        
        return tree;
    }

    private bool matchRule(List<INode> buffer, int start)
    {
        foreach (var rule in Rules)
        {
            foreach (var subRule in rule.SubRules)
            {
                bool success = matchRule(buffer, start, subRule);
                if (success)
                {
                    matchRule(buffer, start);
                    return true;
                }
            }
        }
        return false;
    }

    private bool matchRule(List<INode> buffer, int start, SubRule attempt)
    {
        foreach (var ruleToken in attempt.RuleTokens)
        {
            if (!buffer[start].Is(ruleToken))
                return false;
        }
        
        var len = attempt.RuleTokens.Count();
        RuleMatch match = new RuleMatch(
            attempt,
            buffer.Skip(start).Take(len).ToArray()
        );
        buffer.RemoveRange(start, len);
        buffer.Insert(start, match);

        return true;
    }
}