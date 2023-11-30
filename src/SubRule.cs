/* Author:  Leonardo Trevisan Silio
 * Date:    03/07/2023
 */
using System.Linq;
using System.Collections.Generic;

namespace Orkestra;

/// <summary>
/// A record that represents a sub sintactycal rule. 
/// </summary>
public record SubRule : IRuleElement
{
    private List<IRuleElement> ruleTokens;
    private SubRule(IRuleElement[] tokens)
    {
        ruleTokens = new List<IRuleElement>();
        ruleTokens.AddRange(tokens);
    }

    public IEnumerable<IRuleElement> RuleTokens => this.ruleTokens;
    public Rule Parent { get; set; } = null;

    public string KeyName => Parent.Name + "." + ruleTokens.First().KeyName;

    public static SubRule Create(params IRuleElement[] tokens)
        => new SubRule(tokens);

    public override string ToString()
        => $"sR:{Parent?.Name ?? "null"}";
}