/* Author:  Leonardo Trevisan Silio
 * Date:    10/02/2025
 */
namespace Orkestra.Expressions.Nodes;

public class OptionalExpressionNode(ExpressionNode expression) 
    : ExpressionNode(ExpressionType.OptionalOperation)
{
    public readonly ExpressionNode Expression = expression;

    public override SubRule[] GetSubRules()
        => [ ..Expression.GetSubRules(), [] ];
}