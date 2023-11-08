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
        var dict = new Dictionary<IRuleElement, int>();

        int index = 0;
        foreach (var rule in rules)
            dict.Add(rule, ++index);
        
        int rulesEnd = index;
        foreach (var key in keys)
            dict.Add(key, ++index);
        
        var sb = this.Rules
            .SelectMany(r => r.SubRules);
        
        const int extraData = 2;
        var prods = sb
            .Select(r => {
                int left = dict[r.Parent];
                int size = r.RuleTokens.Count();
                var data = new int[size + extraData];

                data[0] = left;
                int i = 1;
                foreach (var right in r.RuleTokens)
                    data[i++] = dict[right];
                data[i] = 1;

                return data;
            })
            .Prepend(new int[] { 0, dict[StartRule], 0})
            .ToArray();
        
        List<List<int>> states = new();

        List<int> state = new();
        state.Add(0);
        closure(state);

        foreach (var rule in state.Select(x => prods[x]))
        {
            Verbose.Info($"{rule[0]} -> {string.Join(' ', rule.Skip(1))}");
        }

        void closure(List<int> state)
        {
            for (int i = 0; i < state.Count; i++)
            {
                var production = prods[state[i]];
                var dot = production[^1];
                var crr = production[dot];

                for (int j = 0; j < prods.Length; j++)
                {
                    var prod = prods[j];
                    if (prod[0] != crr)
                        continue;
                    
                    state.Add(j);
                }
            }
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