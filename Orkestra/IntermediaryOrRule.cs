/* Author:  Leonardo Trevisan Silio
 * Date:    28/06/2024
 */
using System.Collections.Generic;

namespace Orkestra;

/// <summary>
/// Intermediary object to improve expressiveness.
/// Do not use out of the "rule1 | rule2 | ke1 | ..." context.
/// </summary>
public class IntermediaryOrRule
{
    readonly List<SubRule> subRules = [];

    public static implicit operator Rule(IntermediaryOrRule intermediary)
        => Rule.CreateRule([ ..intermediary.subRules ]);
    
    public static IntermediaryOrRule operator |(IntermediaryOrRule ro1, IntermediaryOrRule ro2)
    {
        ro1.subRules.AddRange(ro2.subRules);
        return ro1;
    }
    
    public static IntermediaryOrRule operator |(IntermediaryOrRule ro, Key k)
    {
        ro.subRules.Add(new SubRule(k));
        return ro;
    }
    
    public static IntermediaryOrRule operator |(Key k, IntermediaryOrRule ro)
    {
        ro.subRules.Insert(0, new SubRule(k));
        return ro;
    }
    
    public static IntermediaryOrRule operator |(IntermediaryOrRule ro, Rule r)
    {
        ro.subRules.Add(new SubRule(r));
        return ro;
    }
    
    public static IntermediaryOrRule operator |(Rule r, IntermediaryOrRule ro)
    {
        ro.subRules.Insert(0, new SubRule(r));
        return ro;
    }
}