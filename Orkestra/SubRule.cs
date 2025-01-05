/* Author:  Leonardo Trevisan Silio
 * Date:    05/01/2025
 */
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Orkestra;

/// <summary>
/// Represents a sub sintactycal rule. 
/// </summary>
public class SubRule(params ISyntacticElement[] tokens) : IEnumerable<ISyntacticElement>
{
    readonly ISyntacticElement[] ruleTokens = tokens;

    /// <summary>
    /// Get tokens for this rule.
    /// </summary>
    public IEnumerable<ISyntacticElement> RuleTokens => ruleTokens;

    /// <summary>
    /// Get the parent Rule for this subrule.
    /// </summary>
    public Rule? Parent { get; set; } = null;

    /// <summary>
    /// Get the name of the subrule.
    /// </summary>
    public string Name => (Parent?.Name ?? "NoParent") + "." + ruleTokens.First().Name;

    public IEnumerator<ISyntacticElement> GetEnumerator()
    {
        foreach (var token in ruleTokens)
            yield return token;
    }
    
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public override string ToString()
        => $"sR:{Parent?.Name ?? "null"}";
}