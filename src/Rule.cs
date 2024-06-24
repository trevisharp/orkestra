/* Author:  Leonardo Trevisan Silio
 * Date:    24/06/2024
 */
using System.Collections.Generic;

namespace Orkestra;

/// <summary>
/// A record that represents a set of rules with same name.
/// </summary>
public class Rule(string name, bool startRule, params SubRule[] subRules) : ISyntacticElement
{
    private List<SubRule> subRules = [ ..subRules ];

    public string Name { get; set; } = name;
    public bool IsStartRule { get; set; } = startRule;
    public IEnumerable<SubRule> SubRules => subRules;

    public override string ToString()
        => $"R:{Name ?? "unnamed"}";

    public void AddSubRules(params SubRule[] subRules)
    {
        this.subRules.AddRange(subRules);

        foreach (var subRule in subRules)
            subRule.Parent = this;
    }
    
    public void AddSubRules(params List<ISyntacticElement>[] subRules)
    {
        foreach (var subRule in subRules)
        {
            var sb = SubRule.Create(
                subRule.ToArray()
            );
            sb.Parent = this;
            this.subRules.Add(sb);
        }
    }
    
    public static Rule CreateRule(string name, params SubRule[] subRules)
        => new Rule(name, false, subRules);

    public static Rule CreateStartRule(string name, params SubRule[] subRules)
        => new Rule(name, true, subRules);
    
    public static Rule CreateRule(params SubRule[] subRules)
        => new Rule(null, false, subRules);

    public static Rule CreateStartRule(params SubRule[] subRules)
        => new Rule(null, true, subRules);
}