/* Author:  Leonardo Trevisan Silio
 * Date:    05/01/2025
 */
using System.Collections;
using System.Collections.Generic;

namespace Orkestra;

/// <summary>
/// Represents a set of rules with same name.
/// </summary>
public class Rule : ISyntacticElement, IEnumerable<SubRule>
{
    readonly List<SubRule> subRules = [];

    public string? Name { get; set; }

    public IEnumerable<SubRule> SubRules => subRules;

    public override string ToString()
        => $"R:{Name ?? "unnamed"}";

    public void Add(SubRule subRule)
    {
        subRules.Add(subRule);
        subRule.Parent = this;
    }

    public void AddSubRules(params SubRule[] subRules)
    {
        this.subRules.AddRange(subRules);

        foreach (var subRule in subRules)
            subRule.Parent = this;
    }
    
    public static Rule CreateRule(string? name, params SubRule[] subRules)
    {
        Rule rule = [];
        rule.Name = name;
        rule.AddSubRules(subRules);
        return rule;
    }
    
    public static Rule CreateRule(params SubRule[] subRules)
        => CreateRule(null, subRules);

    public IEnumerator<SubRule> GetEnumerator()
        => subRules.GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator()
        => subRules.GetEnumerator();

    public static IntermediaryRuleOption operator |(Rule r1, Rule r2)
        => new IntermediaryRuleOption() | r1 | r2;
}