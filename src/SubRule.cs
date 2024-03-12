/* Author:  Leonardo Trevisan Silio
 * Date:    03/07/2023
 */
using System.Linq;
using System.Collections.Generic;

namespace Orkestra;

/// <summary>
/// A record that represents a sub sintactycal rule. 
/// </summary>
public record SubRule : ISyntacticElement
{
    private List<ISyntacticElement> ruleTokens;
    private SubRule(ISyntacticElement[] tokens)
    {
        ruleTokens = [ ..tokens];
    }

    public IEnumerable<ISyntacticElement> RuleTokens => this.ruleTokens;
    public Rule Parent { get; set; } = null;

    public string Name => Parent.Name + "." + ruleTokens.First().Name;

    public static SubRule Create(params ISyntacticElement[] tokens)
        => new SubRule(tokens);

    public override string ToString()
        => $"sR:{Parent?.Name ?? "null"}";
}