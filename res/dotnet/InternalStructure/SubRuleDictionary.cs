using System.Linq;
using System.Collections.Generic;

namespace Orkestra.InternalStructure;

public class SubRuleDictionary
{
    private List<Rule> RuleList = null;
    private Dictionary<IRuleElement, SubRule[]> dict = new Dictionary<IRuleElement, SubRule[]>();

    public SubRuleDictionary(IEnumerable<Rule> rules)
        => this.RuleList = new List<Rule>(rules);

    private IEnumerable<SubRule> findSubRules(INode node)
    {
        foreach (var rule in RuleList)
        {
            foreach (var subRule in rule.SubRules)
            {
                if (node.Is(subRule.RuleTokens.FirstOrDefault()))
                    yield return subRule;
            }
        }
    }

    public IEnumerable<SubRule> Get(INode node)
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