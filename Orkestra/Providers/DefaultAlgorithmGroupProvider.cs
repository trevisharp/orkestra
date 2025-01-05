/* Author:  Leonardo Trevisan Silio
 * Date:    18/04/2024
 */
namespace Orkestra.Providers;

using LexicalAnalysis;
using SyntacticAnalysis;
using SyntacticAnalysis.LR1;

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