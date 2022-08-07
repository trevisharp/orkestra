using System.Collections.Generic;

namespace Orkestra;

public class SubRule : IRuleToken
{
    private List<IRuleToken> ruleTokens;
    private SubRule(IRuleToken[] tokens)
    {
        ruleTokens = new List<IRuleToken>();
        ruleTokens.AddRange(tokens);
    }

    public IEnumerable<IRuleToken> RuleTokens => this.ruleTokens;

    public static SubRule Create(params IRuleToken[] tokens)
        => new SubRule(tokens);
}