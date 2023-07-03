/* Author:  Leonardo Trevisan Silio
 * Date:    03/07/2023
 */
using System.Collections.Generic;

namespace Orkestra.SyntacticAnalysis;

/// <summary>
/// A syntactic analyzer using CYK algorithm only for Chomsky nromal form.
/// </summary>
public class ChomskyCYKSyntacticAnalyzer : ISyntacticAnalyzer
{
    public Rule StartRule { get; set; }

    public List<Rule> Rules { get; private set; } = new List<Rule>();
    public void Add(Rule rule) => this.Rules.Add(rule);

    public ExpressionTree Parse(IEnumerable<Token> tokens)
    {
        var x = reduct(tokens);

        return null;
    }

    private IEnumerable<INode> reduct(IEnumerable<INode> elements)
    {
        yield break;
    }
}