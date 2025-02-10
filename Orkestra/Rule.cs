/* Author:  Leonardo Trevisan Silio
 * Date:    10/02/2025
 */
using System.Collections;
using System.Collections.Generic;

namespace Orkestra;

using Expressions;

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

    public static ExpressionNode operator +(Rule rule)
        => +(ExpressionNode)rule;

    public static ExpressionNode operator |(Rule rule1, Rule rule2)
        => (ExpressionNode)rule1 | (ExpressionNode)rule2;

    public static ExpressionNode operator |(Rule rule, Key key)
        => (ExpressionNode)rule | (ExpressionNode)key;

    public static ExpressionNode operator |(Key key, Rule rule)
        => (ExpressionNode)key | (ExpressionNode)rule;
}