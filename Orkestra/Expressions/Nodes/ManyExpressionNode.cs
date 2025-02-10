/* Author:  Leonardo Trevisan Silio
 * Date:    10/02/2025
 */
namespace Orkestra.Expressions.Nodes;

public class ManyExpressionNode(ExpressionNode node)
    : ExpressionNode(ExpressionType.ManyOperation)
{
    public readonly ExpressionNode Node = node;
}