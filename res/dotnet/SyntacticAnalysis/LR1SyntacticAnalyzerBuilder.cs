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

    public void Load()
    {
        var sb = this.Rules
            .SelectMany(r => r.SubRules)
            .ToArray();
        
        
    }

    public ISyntacticAnalyzer Build()
    {
        throw new System.NotImplementedException();
    }

    public bool LoadCache()
    {
        throw new System.NotImplementedException();
    }

    public void SaveCache()
    {
        throw new System.NotImplementedException();
    }
}