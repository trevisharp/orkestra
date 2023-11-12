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
            .Prepend(new int[] { 0, dict[StartRule], 1})
            .ToList();
        
        List<List<int>> states = new();
        extend(new() { 0 });

        foreach (var rule in states[0].Select(x => prods[x]))
        {
            Verbose.Info($"{rule[0]} -> {string.Join(' ', rule.Skip(1))}");
        }

        void closure(List<int> state)
        {
            List<int> searchedRules = new();
            foreach (var index in state)
            {
                var production = prods[index];
                var rule = production[0];
                if (searchedRules.Contains(rule))
                    continue;
                
                searchedRules.Add(rule);
            }

            for (int i = 0; i < state.Count; i++)
            {
                var production = prods[state[i]];
                var dot = production[^1];
                var crr = production[dot];
                if (crr >= rulesEnd)
                    continue;

                if (searchedRules.Contains(crr))
                    continue;
                searchedRules.Add(crr);

                for (int j = 0; j < prods.Count; j++)
                {
                    var prod = prods[j];
                    if (prod[^1] > 1)
                        break;

                    if (prod[0] != crr)
                        continue;
                    
                    state.Add(j);
                }
            }
        }
    
        void extend(List<int> state)
        {
            closure(state);
            states.Add(state);

            List<int> expansionList = new();
            foreach (var index in state)
            {
                var production = prods[index];
                var dot = production[^1];
                var expansion = production[dot];
                if (expansionList.Contains(expansion))
                    continue;
                
                expansionList.Add(expansion);
            }

            foreach (var expansion in expansionList)
                expand(state, expansion);
        }

        void expand(List<int> state, int expansion)
        {
            List<int> newState = new();
            foreach (var index in state)
            {
                var pro = prods[index];
                var dot = pro[^1];
                var exp = pro[dot];
                if (exp != expansion)
                    continue;
                
                var newRule = (int[])pro.Clone();
                newRule[^1]++;
                prods.Add(newRule);
                // TODO: Treat item repetition
                newState.Add(prods.Count - 1);
            }
            // TODO: Treat state repetition
            extend(newState);
            states.Add(newState);
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