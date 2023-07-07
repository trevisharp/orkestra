/* Author:  Leonardo Trevisan Silio
 * Date:    03/07/2023
 */
using System.Linq;
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

    public ExpressionTree Parse(IEnumerable<Token> tokenCollection)
    {
        var tokens = tokenCollection.ToArray();
        var rules = Rules.ToArray();

        int n = tokens.Length;
        int r = rules.Length;

        bool[] P = new bool[n * n * r];
        List<int>[] B = new List<int>[n * n * r];

        for (int i = 0; i < n; i++)
        {
            var token = tokens[i];
            for (int j = 0; j < r; j++)
            {
                var rule = rules[j];

                foreach (var sbr in rule.SubRules)
                {
                    var ruleTokens = sbr.RuleTokens;
                    if (ruleTokens.Count() != 1)
                        continue;
                    
                    var ruleToken = ruleTokens.FirstOrDefault();
                    if (ruleToken.KeyName != token.Key.Name)
                        continue;

                    P[1 + i * n + j * n * r] = true;
                }
            }
        }

        return null;
    }
}