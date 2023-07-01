/* Author:  Leonardo Trevisan Silio
 * Date:    30/06/2023
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
    /// Define the initial rule of derivation, so the root of
    /// expression tree.
    /// </summary>
    Rule StartRule { get; set; }

    /// <summary>
    /// Add a rule to be used by syntactical algorithm.
    /// </summary>
    void Add(Rule rule);

    /// <summary>
    /// Parse a collection of tokens in a expression tree.
    /// </summary>
    ExpressionTree Parse(IEnumerable<Token> tokens);
}