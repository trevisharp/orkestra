/* Author:  Leonardo Trevisan Silio
 * Date:    28/06/2024
 */
using System.Collections.Generic;

namespace Orkestra;

/// <summary>
/// Intermediary object to improve expressiveness.
/// Do not use out of the "rule1 | rule2 | ke1 | ..." context.
/// </summary>
public class IntermediaryRuleOption
{
    private List<SubRule> subRules = new List<SubRule>();

    public static implicit operator Rule(IntermediaryRuleOption option)
        => Rule.CreateRule(option?.subRules?.ToArray() ?? []);
    
    public static IntermediaryRuleOption operator |(IntermediaryRuleOption ro1, IntermediaryRuleOption ro2)
    {
        ro1.subRules.AddRange(ro2.subRules);
        return ro1;
    }
    
    public static IntermediaryRuleOption operator |(IntermediaryRuleOption ro, Key k)
    {
        ro.subRules.Add(new SubRule(k));
        return ro;
    }
    
    public static IntermediaryRuleOption operator |(Key k, IntermediaryRuleOption ro)
    {
        ro.subRules.Insert(0, new SubRule(k));
        return ro;
    }
    
    public static IntermediaryRuleOption operator |(IntermediaryRuleOption ro, Rule r)
    {
        ro.subRules.Add(new SubRule(r));
        return ro;
    }
    
    public static IntermediaryRuleOption operator |(Rule r, IntermediaryRuleOption ro)
    {
        ro.subRules.Insert(0, new SubRule(r));
        return ro;
    }
}