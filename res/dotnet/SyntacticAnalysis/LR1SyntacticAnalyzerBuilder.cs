/* Author:  Leonardo Trevisan Silio
 * Date:    04/11/2023
 */
using System.Collections.Generic;

namespace Orkestra.SyntacticAnalysis;

using System.Linq;
using InternalStructure;

public class LR1SyntacticAnalyzerBuilder : ISyntacticAnalyzerBuilder
{
    private List<Rule> rules = new();

    public Rule StartRule { get; set; }
    public IEnumerable<Rule> Rules => this.rules;

    public void Add(Rule rule)
        => this.rules.Add(rule);

    public void Load(IEnumerable<Key> keys)
    {
        int index = 0;
        var dict = new Dictionary<IRuleElement, int>();
        foreach (var rule in this.rules)
            dict.Add(rule, ++index);
        
        foreach (var key in keys)
            dict.Add(key, ++index);
        
        var sb = this.Rules
            .SelectMany(r => r.SubRules)
            .ToArray();
        
        foreach (var rule in sb)
        {
            Verbose.Info($"""
                {rule.Parent.Name} -> {
                    string.Join(' ', rule.RuleTokens
                        .Select(t => t.KeyName)
                    )
                }
            """);
        }
    }

    public ISyntacticAnalyzer Build()
    {
        throw new System.NotImplementedException();
    }

    public bool LoadCache()
    {
        return false;
    }

    public void SaveCache()
    {
        throw new System.NotImplementedException();
    }
}