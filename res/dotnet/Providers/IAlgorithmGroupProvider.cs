/* Author:  Leonardo Trevisan Silio
 * Date:    04/11/2023
 */
namespace Orkestra.Providers;

using LexicalAnalysis;
using SyntacticAnalysis;

/// <summary>
/// A base provider for algorithms to make a code parser
/// </summary>
public interface IAlgorithmGroupProvider
{
    /// <summary>
    /// Provide a lexical analyzer.
    /// </summary>
    ILexicalAnalyzer ProvideLexicalAnalyzer();

    /// <summary>
    /// Provide a syntactical analyzer.
    /// </summary>
    ISyntacticAnalyzerBuilder ProvideSyntacticAnalyzerBuilder();
}