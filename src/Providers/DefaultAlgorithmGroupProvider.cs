/* Author:  Leonardo Trevisan Silio
 * Date:    04/11/2023
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

    public ISyntacticAnalyzerBuilder ProvideSyntacticAnalyzerBuilder()
        => new LR1SyntacticAnalyzerBuilder();
}