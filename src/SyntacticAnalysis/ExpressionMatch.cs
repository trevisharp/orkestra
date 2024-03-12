/* Author:  Leonardo Trevisan Silio
 * Date:    11/03/2024
 */
namespace Orkestra.SyntacticAnalysis;

/// <summary>
/// Represents a match in a expression tree.
/// </summary>
public record ExpressionMatch(
    ISyntacticElement Element,
    object DataMatch
);