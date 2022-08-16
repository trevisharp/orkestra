using System.Linq;
using System.Collections.Generic;

namespace Orkestra.InternalStructure;

public class SyntacticStateGraph
{
    public StackLinkedList TokenList { get; private set; }
    public List<Rule> RuleList { get; private set; }

    private Dictionary<IRuleElement, SubRule[]> dict = new Dictionary<IRuleElement, SubRule[]>();

    public SyntacticStateGraph(IEnumerable<INode> tokens, IEnumerable<Rule> rules)
    {
        this.TokenList = new StackLinkedList(tokens);
        this.RuleList = new List<Rule>(rules);
    }

    private SubRule[] getStartRules(INode node, Dictionary<IRuleElement, SubRule[]> dict)
    {
        // foreach (var key in dict.Keys)
        // {
        //     if (node.Is(key))
        //         return dict[key];
        // }

        var list = new List<SubRule>();
        foreach (var rule in RuleList)
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
        // dict.Add(node.Element, subRules);

        return subRules;
    }

    private StackLinkedListNode searchMatches(StackLinkedListNode initial, SubRule[] rules)
    {
        if (rules.Length == 0)
            return null;

        foreach (var rule in rules)
        {
            var newNode = testMatch(initial, rule);

            if (newNode == null)
                continue;
            return newNode;
        }

        return null;
    }

    private StackLinkedListNode testMatch(StackLinkedListNode initial, SubRule rule)
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
        StackLinkedListNode newNode = new StackLinkedListNode();
        newNode.Value = match;

        initial.Previous.Connect(newNode);
        newNode.Connect(it);

        return newNode;
    }

    private StackLinkedListNode discover()
    {
        StackLinkedListNode discovered = null;
        
        foreach (var initial in TokenList)
        {
            if (initial.Value == null)
                continue;
            
            var startRules = getStartRules(initial.Value, dict);
            discovered = searchMatches(initial, startRules);
            if (discovered != null)
                break;
        }

        return discovered;
    }

    private StackLinkedList depthFirstSearch()
    {
        Stack<object> stack = new Stack<object>();

        throw new System.NotImplementedException();
    }
}