/* Author:  Leonardo Trevisan Silio
 * Date:    30/06/2023
 */
namespace Orkestra.Providers;

using LexicalAnalysis;
using SyntacticAnalysis;

/// <summary>
/// A default provider for basic implementations of Orkestra framework.
/// </summary>
public class DefaultAlgorithmGroupProvider : IAlgorithmGroupProvider
{
    public ILexicalAnalyzer ProvideLexicalAnalyzer()
        => new DefaultLexicalAnalyzer();

    public ISyntacticAnalyzer ProvideSyntacticAnalyzer()
        => new DFSSyntacticAnalyzer();
}