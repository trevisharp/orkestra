using System.Collections.Generic;

namespace Orkestra.SyntacticAnalysis;

using LexicalAnalysis;

public record RuleMatch : INode
{
    public RuleMatch(SubRule subRule)
        => this.SubRule = subRule;

    public SubRule SubRule { get; init; }
    public IRuleElement Element => SubRule;
    public List<INode> Children { get; set; } = new List<INode>();

    public bool Is(IRuleElement node)
        => node is Rule rule && rule == this.SubRule.Parent;

    public override string ToString()
        => $"M:({SubRule})";
}