using System.Text;

namespace Orkestra;

public class ExpressionTree
{
    public INode Root { get; set; }

    public override string ToString()
        => toString(Root, string.Empty);

    private string toString(INode node, string tabulation)
    {
        if (node is Token token)
            return tabulation + token.Key.Name + "\n";
        else if (node is RuleMatch match)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(tabulation + match.SubRule.Parent.Name + "\n");
            tabulation += "|\t";
            foreach (var child in match.Children)
                sb.Append(toString(child, tabulation));
            return sb.ToString();
        }
        return "";
    }
}