/* Author:  Leonardo Trevisan Silio
 * Date:    04/11/2023
 */
using System.Linq;
using System.Collections.Generic;

namespace Orkestra.SyntacticAnalysis;

using System.Runtime.CompilerServices;
using InternalStructure;
using Orkestra.InternalStructure;

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

    public void Load(IEnumerable<Key> keys)
    {
        int ruleCount = rules.Count();
        int keyCount = keys.Count();
        int indexSize = ruleCount + keyCount;
        Dictionary<int, IRuleElement> indexMap = new (indexSize);
        Dictionary<int, IEnumerable<SubRule>> subRuleMap = new();
        
        int index = 0;
        Dictionary<IRuleElement, int> elementMap = new (indexSize);
        foreach (var key in keys)
        {
            var keyIndex = ++index;
            elementMap.Add(key, keyIndex);
            indexMap.Add(keyIndex, key);
        }
        var keyLastIndex = index;

        foreach (var rule in rules)
        {
            var ruleIndex = ++index;
            elementMap.Add(rule, ruleIndex);
            indexMap.Add(ruleIndex, rule);
            subRuleMap.Add(ruleIndex, rule.SubRules);
        }

        using FastBuffer<int> pool = new();
        Dictionary<int, int[]> itemMap = [];
        Dictionary<int, List<int>> ruleItemMap = new();

        int itemIndex = 1;
        foreach (var rule in rules)
        {
            var ruleIndex = elementMap[rule];
            List<int> itemRules = new();
            foreach (var sb in rule.SubRules)
            {
                var tokens = sb.RuleTokens.ToArray();
                var ruleSize = tokens.Length;
                var item = pool.Rent(ruleSize + 2);
                item[0] = ruleIndex;
                item[1] = 0;
                for (int i = 2; i < item.Length; i++)
                    item[i] = elementMap[tokens[i - 2]];
                itemMap.Add(itemIndex, item);
                itemIndex++;
            }
            ruleItemMap.Add(ruleIndex, itemRules);
        }

        var goal = pool.Rent(3);
        goal[0] = 0; 
        goal[1] = 0;
        goal[2] = elementMap[StartRule];
        itemMap.Add(0, goal);
        ruleItemMap.Add(0, [0]);

        Dictionary<int, int> moveItemMap = new();

        Stack<List<(int item, int ahead)>> stack = new();
        stack.Append([(0, -1)]);
        while(stack.Count > 0)
        {
            var state = stack.Pop();

            
        }
    }
}