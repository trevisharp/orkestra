using System.Linq;
using System.Collections.Generic;

namespace Orkestra.SyntacticAnalysis.InternalStructure;

using LexicalAnalysis;

public class AttemptDictionary
{
    private List<Rule> RuleList = null;
    private Dictionary<ISyntaticElement, SubRule[]> dict = new Dictionary<ISyntaticElement, SubRule[]>();

    public AttemptDictionary(IEnumerable<Rule> rules)
        => this.RuleList = new List<Rule>(rules);

    private IEnumerable<SubRule> findSubRules(IMatch node)
    {
        if (node == null)
            yield break;
        
        foreach (var rule in RuleList)
        {
            foreach (var subRule in rule.SubRules)
            {
                if (node.Is(subRule.RuleTokens.FirstOrDefault()))
                    yield return subRule;
            }
        }
    }

    public IEnumerable<SubRule> GetAttempts(IMatch node)
    {
        // foreach (var key in dict.Keys)
        // {
        //     if (node.Is(key))
        //         return dict[key];
        // }
        
        var subRules = findSubRules(node)
            .OrderByDescending(r => r.RuleTokens.Count())
            .ToArray();
        
        // dict.Add(node.Element, subRules);

        return subRules;
    }
}