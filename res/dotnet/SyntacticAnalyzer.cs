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
        Dictionary<IRuleElement, SubRule[]> dict = new Dictionary<IRuleElement, SubRule[]>();

        foreach (var initial in buffer)
        {
            var startRules = getStartRules(initial.Value, dict);
            searchMatches(initial, startRules);
        }

        tree.Root = buffer.First().Value;
        return tree;
    }

    private SubRule[] getStartRules(INode node, Dictionary<IRuleElement, SubRule[]> dict)
    {
        foreach (var key in dict.Keys)
        {
            if (node.Is(key))
                return dict[key];
        }

        var list = new List<SubRule>();
        foreach (var rule in Rules)
        {
            foreach (var subRule in rule.SubRules)
            {
                if (node.Is(subRule.RuleTokens.FirstOrDefault()))
                    list.Add(subRule);
            }
        }
        
        var subRules = list
            .OrderByDescending(r => r.RuleTokens.Count())
            .ToArray();
        dict.Add(node.Element, subRules);

        return subRules;
    }

    private bool searchMatches(ReduceBufferNode initial, SubRule[] rules)
    {
        if (rules.Length == 0)
            return false;

        bool hasMatches = false;
        foreach (var rule in rules)
        {
            var newNode = testMatch(initial, rule);

            if (newNode == null)
                continue;
            hasMatches = true;
        }

        return hasMatches;
    }

    private ReduceBufferNode testMatch(ReduceBufferNode initial, SubRule rule)
    {
        var it = initial;
        var tokensIterator = rule.RuleTokens.GetEnumerator();
        var nodes = new List<INode>();

        while (tokensIterator.MoveNext())
        {   
            if (it == null || it.Value == null)
                return null;
            
            if (!it.Value.Is(tokensIterator.Current))
                return null;
            nodes.Add(it.Value);
            
            it = it.Next;
        }

        RuleMatch match = new RuleMatch(rule, nodes.ToArray());
        ReduceBufferNode newNode = new ReduceBufferNode();
        newNode.Value = match;

        initial.Previous.Connect(newNode);
        newNode.Connect(it);

        return newNode;
    }
}