namespace Orkestra;

public record RuleMatch : INode
{
    public RuleMatch(SubRule subRule)
        => this.SubRule = subRule;

    public SubRule SubRule { get; init; }
    public IRuleElement Element => SubRule;

    public bool Is(IRuleElement node)
        => node is Rule rule && rule == this.SubRule.Parent;

    public override string ToString()
        => $"Match: ({SubRule})";
}