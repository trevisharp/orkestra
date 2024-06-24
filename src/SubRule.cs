/* Author:  Leonardo Trevisan Silio
 * Date:    24/06/2024
 */
using System.Linq;
using System.Collections.Generic;

namespace Orkestra;

/// <summary>
/// A record that represents a sub sintactycal rule. 
/// </summary>
public class SubRule(params ISyntacticElement[] tokens)
{
    private List<ISyntacticElement> ruleTokens = [ ..tokens];
    public IEnumerable<ISyntacticElement> RuleTokens => this.ruleTokens;
    public Rule Parent { get; set; } = null;
    public string Name => Parent.Name + "." + ruleTokens.First().Name;
    public override string ToString()
        => $"sR:{Parent?.Name ?? "null"}";
}