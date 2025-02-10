/* Author:  Leonardo Trevisan Silio
 * Date:    10/02/2025
 */
namespace Orkestra.Expressions;

using Nodes;

/// <summary>
/// Represents a node in a rule expression.
/// </summary>
public abstract class ExpressionNode(ExpressionType type)
{
    public readonly ExpressionType ExpressionType = type;

    public Rule ToRule()
        => Rule.CreateRule(GetSubRules());

    public abstract SubRule[] GetSubRules();

    public static implicit operator Rule(ExpressionNode node)
        => node.ToRule();

    public static implicit operator ExpressionNode(Key key)
        => new KeyExpressionNode(key);
    
    public static implicit operator ExpressionNode(Rule rule)
        => new RuleExpressionNode(rule);

    public static ExpressionNode operator |(ExpressionNode left, ExpressionNode right)
        => new OrExpressionNode(left, right);
    
    public static ExpressionNode operator +(ExpressionNode left, ExpressionNode right)
        => new ConcantExpressionNode(left, right);
}