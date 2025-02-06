/* Author:  Leonardo Trevisan Silio
 * Date:    24/04/2024
 */
using System.Text;
using System.Collections.Generic;

namespace Orkestra.SyntacticAnalysis;

/// <summary>
/// A tree o syntax match.
/// </summary>
public record ExpressionTree(
    ISyntacticElement Element,
    ExpressionTree[] Children,
    object? Data = null
)
{
    public override string ToString()
    {
        var sb = new StringBuilder();
        toString(this, []);
        return sb.ToString();

        void toString(ExpressionTree node, LinkedList<char> tabulation)
        {
            var tail = tabulation.Last?.Value;
            if (tail is not null)
            {
                foreach (var tab in tabulation)
                    sb.Append(tab);
                
                tabulation.RemoveLast();
                tabulation.AddLast(tail == '├' ? '│' : ' ');
            }
            
            sb.AppendLine(node.Element.Name);

            if (node.Children.Length == 0)
                return;

            var last = node.Children.Length - 1;
            for (int i = 0; i < last; i++)
            {
                tabulation.AddLast('├');
                toString(node.Children[i], tabulation);
                tabulation.RemoveLast();
            }

            tabulation.AddLast('└');
            toString(node.Children[last], tabulation);
            tabulation.RemoveLast();
        }
    }
}