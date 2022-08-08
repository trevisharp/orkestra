using System.Collections.Generic;

namespace Orkestra;

public class SubRule : IRuleElement
{
    private List<IRuleElement> ruleTokens;
    private SubRule(IRuleElement[] tokens)
    {
        ruleTokens = new List<IRuleElement>();
        ruleTokens.AddRange(tokens);
    }

    public IEnumerable<IRuleElement> RuleTokens => this.ruleTokens;
    public Rule Parent { get; set; } = null;

    public static SubRule Create(params IRuleElement[] tokens)
        => new SubRule(tokens);
}