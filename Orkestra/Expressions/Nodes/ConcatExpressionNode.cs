/* Author:  Leonardo Trevisan Silio
 * Date:    10/02/2025
 */
using System.Collections.Generic;

namespace Orkestra.Expressions.Nodes;

public class ConcantExpressionNode(
    ExpressionNode left,
    ExpressionNode right
    ) : ExpressionNode(ExpressionType.ConcatOperation)
{
    public readonly ExpressionNode Left = left;
    public readonly ExpressionNode Right = right;

    public override SubRule[] GetSubRules()
    {
        List<SubRule> subrules = [];

        var leftSubrules = Left.GetSubRules();
        var rightSubrules = Right.GetSubRules();

        foreach (var left in leftSubrules)
        {
            foreach (var right in rightSubrules)
            {
                var sb = new SubRule();

                foreach (var item in left)
                    sb.Add(item);
                
                foreach (var item in right)
                    sb.Add(item);
                
                subrules.Add(sb);
            }
        }

        return [ ..subrules ];
    }
}