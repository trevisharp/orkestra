namespace Orkestra;

public record RuleMatch : INode
{
    public RuleMatch(SubRule subRule, INode[] nodes)
    {
        this.SubRule = subRule;
        this.Children = nodes;
    }

    public SubRule SubRule { get; init; }
    public INode[] Children { get; private set; }

    public bool Is(IRuleElement node)
        => node is SubRule subRule && subRule == this.SubRule;
}