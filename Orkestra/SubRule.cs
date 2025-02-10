/* Author:  Leonardo Trevisan Silio
 * Date:    05/01/2024
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
    readonly List<ISyntacticElement> ruleTokens = [ ..tokens ];

    /// <summary>
    /// Get tokens for this rule.
    /// </summary>
    public IEnumerable<ISyntacticElement> RuleTokens => ruleTokens;

    /// <summary>
    /// Add a new sub toke in this sub rule.
    /// </summary>
    public void Add(ISyntacticElement element)
        => ruleTokens.Add(element);

    public IEnumerator<ISyntacticElement> GetEnumerator()
    {
        foreach (var token in ruleTokens)
            yield return token;
    }
    
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}