/* Author:  Leonardo Trevisan Silio
 * Date:    04/11/2023
 */
using System.Linq;
using System.Collections.Generic;

namespace Orkestra.SyntacticAnalysis;

using InternalStructure;

public class LR1SyntacticAnalyzerBuilder : ISyntacticAnalyzerBuilder
{
    private List<Rule> rules = new();

    public Rule StartRule { get; set; }
    public IEnumerable<Rule> Rules => this.rules;

    public void Add(Rule rule)
        => this.rules.Add(rule);

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

    int rulesLastIndex;
    Dictionary<IRuleElement, int> elementMap;
    Dictionary<int, IRuleElement> indexMap;
    Dictionary<int, IEnumerable<SubRule>> subRuleMap;
    public void Load(IEnumerable<Key> keys)
    {
        int ruleCount = rules.Count();
        int keyCount = keys.Count();
        int indexSize = ruleCount + keyCount;
        elementMap = new (indexSize);
        indexMap = new (indexSize);
        subRuleMap = new();

        int index = 0;
        foreach (var rule in rules)
        {
            var ruleIndex = ++index;
            this.elementMap.Add(rule, ruleIndex);
            this.indexMap.Add(ruleIndex, rule);
            this.subRuleMap.Add(ruleIndex, rule.SubRules);
        }
        
        this.rulesLastIndex = index;
        foreach (var key in keys)
        {
            var keyIndex = ++index;
            this.elementMap.Add(key, keyIndex);
            this.indexMap.Add(keyIndex, key);
        }

        
    }
}