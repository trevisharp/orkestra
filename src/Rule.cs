/* Author:  Leonardo Trevisan Silio
 * Date:    11/03/2024
 */
using System.Collections.Generic;

namespace Orkestra;

/// <summary>
/// A record that represents a set of rules with same name.
/// </summary>
/// <value></value>
public record Rule : ISyntacticElement
{
    private List<SubRule> subRules;

    private Rule(string name, bool startRule, SubRule[] subRules)
    {
        this.Name = name;
        this.subRules = [ ..subRules ];
        this.IsStartRule = startRule;

        foreach (var subRule in subRules)
            subRule.Parent = this;
    }
    
    public string Name { get; set; }
    public bool IsStartRule { get; set; }
    public IEnumerable<SubRule> SubRules => subRules;

    public void AddSubRules(params SubRule[] subRules)
    {
        this.subRules.AddRange(subRules);

        foreach (var subRule in subRules)
            subRule.Parent = this;
    }
    
    public static Rule CreateRule(string name, params SubRule[] subRules)
        => new Rule(name, false, subRules);

    public static Rule CreateStartRule(string name, params SubRule[] subRules)
        => new Rule(name, true, subRules);

    public override string ToString()
        => $"R:{Name}";
}