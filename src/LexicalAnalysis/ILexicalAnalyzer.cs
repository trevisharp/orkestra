/* Author:  Leonardo Trevisan Silio
 * Date:    30/06/2023
 */
using System.Collections.Generic;

namespace Orkestra.LexicalAnalysis;

using Processings;

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