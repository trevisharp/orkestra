/* Author:  Leonardo Trevisan Silio
 * Date:    10/02/2025
 */
namespace Orkestra.Expressions.Nodes;

public class OrExpressionNode(
    ExpressionNode left, 
    ExpressionNode right
    ) : ExpressionNode(ExpressionType.OrOperation)
{
    public readonly ExpressionNode Left = left;
    public readonly ExpressionNode Right = right;
}