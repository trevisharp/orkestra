/* Author:  Leonardo Trevisan Silio
 * Date:    10/02/2025
 */
namespace Orkestra.Expressions;

/// <summary>
/// Represents a node in a rule expression.
/// </summary>
public abstract class ExpressionNode(ExpressionType type)
{
    public readonly ExpressionType ExpressionType = type;

    public Rule ToRule()
    {
        throw new System.NotImplementedException();
    }
}