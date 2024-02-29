/* Author:  Leonardo Trevisan Silio
 * Date:    27/02/2024
 */
using System.Collections.Generic;

namespace Orkestra.LexicalAnalysis;

using Processings;
using SyntacticAnalysis;

/// <summary>
/// Base interface pro all lexical analyzer algorithms
/// </summary>
public interface ILexicalAnalyzer
{
    /// <summary>
    /// Get all keys used by the algorithm
    /// </summary>
    IEnumerable<Key> Keys { get; }

    /// <summary>
    /// Load Keys to execute the lexical analyzer algorithm.
    /// </summary>
    void AddKeys(IEnumerable<Key> keys);

    /// <summary>
    /// Parse a Text class into a collection of Tokens
    /// </summary>
    IEnumerable<Token> Parse(Text text);
}