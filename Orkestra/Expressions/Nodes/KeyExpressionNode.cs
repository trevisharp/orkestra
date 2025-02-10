/* Author:  Leonardo Trevisan Silio
 * Date:    10/02/2025
 */
namespace Orkestra.Expressions.Nodes;

public class KeyExpressionNode(Key key) : ExpressionNode(ExpressionType.Key)
{
    public readonly Key Key = key;

    public override SubRule[] GetSubRules()
        => [ [ Key] ];
}