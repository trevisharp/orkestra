/* Author:  Leonardo Trevisan Silio
 * Date:    10/02/2025
 */
using System.Collections.Generic;

namespace Orkestra.Expressions.Nodes;

public class OrExpressionNode(
    ExpressionNode left, 
    ExpressionNode right
    ) : ExpressionNode(ExpressionType.OrOperation)
{
    public readonly ExpressionNode Left = left;
    public readonly ExpressionNode Right = right;

    public override SubRule[] GetSubRules()
    {
        List<SubRule> subRules = [];

        foreach (var subRule in Left.GetSubRules())
            subRules.Add(subRule);

        foreach (var subRule in Right.GetSubRules())
            subRules.Add(subRule);
        
        return [ ..subRules ];
    }
}