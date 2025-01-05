/* Author:  Leonardo Trevisan Silio
 * Date:    04/11/2023
 */
using System.Collections.Generic;

namespace Orkestra.SyntacticAnalysis;

/// <summary>
/// Base interface pro all syntactic analyzer algorithms.
/// The algorithm can transform a collection of tokens
/// in a expression tree using a set of rules.
/// </summary>
public interface ISyntacticAnalyzer
{
    /// <summary>
    /// Parse a collection of tokens in a expression tree.
    /// </summary>
    ExpressionTree Parse(IEnumerable<Token> tokens);
}