using System.Collections.Generic;

namespace Orkestra;

public class Rule : IRuleToken
{
    private List<SubRule> subRules;

    private Rule(string name, SubRule[] subRules)
    {
        this.Name = name;
        this.subRules = new List<SubRule>();
        this.subRules.AddRange(subRules);
    }
    
    public string Name { get; set; }
    public IEnumerable<SubRule> SubRules => subRules;

    public static Rule Create(string name, params SubRule[] subRules)
        => new Rule(name, subRules);
}