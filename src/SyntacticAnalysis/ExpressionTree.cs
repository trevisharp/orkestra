/* Author:  Leonardo Trevisan Silio
 * Date:    04/11/2023
 */
using System.Text;
using System.Collections.Generic;

namespace Orkestra.SyntacticAnalysis;

/// <summary>
/// A tree o syntax match.
/// </summary>
public class ExpressionTree(
    ExpressionMatch match,
    List<ExpressionTree> children
)
{
    public readonly ExpressionMatch Match = match;
    public readonly List<ExpressionTree> Children = children;

    public override string ToString()
    {
        var sb = new StringBuilder();
        char[] tabData = [ '|', '\t', '|', '\t', '|', '\t', '|', '\t'];
        toString(this, 0);
        return sb.ToString();

        void toString(
            ExpressionTree node,
            int tabulation)
        {
            int i = 0;
            while (i + 8 < tabulation) {
                sb.Append(tabData, 0, 8);
                i += 8;
            }
            sb.Append(tabData, 0, tabulation - i);
            
            sb.AppendLine(node.Match.Element.Name);
            tabulation += 2;

            foreach (var child in this.Children)
                toString(child, tabulation);
        }
    }
}