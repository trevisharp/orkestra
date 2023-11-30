/* Author:  Leonardo Trevisan Silio
 * Date:    07/11/2023
 */
using System.Collections.Generic;

namespace Orkestra.SyntacticAnalysis;

/// <summary>
/// A builder for a syntactic analyzer.
/// </summary>
public interface ISyntacticAnalyzerBuilder
{
    /// <summary>
    /// Build a Syntactic Analyzer.
    /// </summary>
    ISyntacticAnalyzer Build();

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
    /// Load data from rules to speedup parsing.
    /// </summary>
    void Load(IEnumerable<Key> key);

    /// <summary>
    /// Save cache data for speedup future parsings, if exist.
    /// </summary>
    void SaveCache();

    /// <summary>
    /// Load cache data for speedup future parsings. Returns true if
    /// rules may added to builder and Load function may be called.
    /// </summary>
    bool LoadCache();
}