using System.Collections.Generic;

namespace Orkestra;

public class Rule : IRuleElement
{
    private List<SubRule> subRules;

    private Rule(string name, bool startRule, SubRule[] subRules)
    {
        this.Name = name;
        this.subRules = new List<SubRule>();
        this.subRules.AddRange(subRules);
        this.IsStartRule = startRule;

        foreach (var subRule in subRules)
            subRule.Parent = this;
    }
    
    public string Name { get; set; }
    public bool IsStartRule { get; set; }
    public IEnumerable<SubRule> SubRules => subRules;

    public static Rule CreateRule(string name, params SubRule[] subRules)
        => new Rule(name, false, subRules);

    public static Rule CreateStartRule(string name, params SubRule[] subRules)
        => new Rule(name, true, subRules);
}