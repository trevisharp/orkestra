/* Author:  Leonardo Trevisan Silio
 * Date:    03/07/2023
 */
using System.Linq;
using System.Collections.Generic;

namespace Orkestra;

/// <summary>
/// A record that represents a sub sintactycal rule. 
/// </summary>
public record SubRule : ISyntaticElement
{
    private List<ISyntaticElement> ruleTokens;
    private SubRule(ISyntaticElement[] tokens)
    {
        ruleTokens = [ ..tokens];
    }

    public IEnumerable<ISyntaticElement> RuleTokens => this.ruleTokens;
    public Rule Parent { get; set; } = null;

    public string KeyName => Parent.Name + "." + ruleTokens.First().KeyName;

    public static SubRule Create(params ISyntaticElement[] tokens)
        => new SubRule(tokens);

    public override string ToString()
        => $"sR:{Parent?.Name ?? "null"}";
}