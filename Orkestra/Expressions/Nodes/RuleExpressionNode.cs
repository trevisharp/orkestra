/* Author:  Leonardo Trevisan Silio
 * Date:    10/02/2025
 */
namespace Orkestra.Expressions.Nodes;

public class RuleExpressionNode(Rule rule) : ExpressionNode(ExpressionType.Rule)
{
    public readonly Rule Rule = rule;

    public override SubRule[] GetSubRules()
        => [ [ Rule ] ];
}