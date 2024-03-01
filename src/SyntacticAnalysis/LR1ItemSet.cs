using System.Linq;
using System.Collections.Generic;

namespace Orkestra.SyntacticAnalysis;

using Orkestra.InternalStructure;

public class LR1ItemSet
{
    /// <summary>
    /// The map to int from key or rule elements.
    /// </summary>
    Dictionary<IRuleElement, int> elementMap;

    /// <summary>
    /// The last index of keys on elementMap. Index higher than keyLastIndex
    /// are rule indexes.
    /// </summary>
    int keyLastIndex;

    /// <summary>
    /// Reverse dicitionary of elementMap.
    /// </summary>
    Dictionary<int, IRuleElement> indexMap;

    /// <summary>
    /// The pool of buffer for items.
    /// </summary>
    FastBuffer<int> pool;

    /// <summary>
    /// The index map from LR1Itens and int identities.
    /// </summary>
    Dictionary<int, int[]> itemMap;

    /// <summary>
    /// The relation between elementMap's rule index and
    /// the collection of indexes in itemMap.
    /// </summary>
    Dictionary<int, List<int>> ruleItemMap;

    public LR1ItemSet(
        Key[] keys,
        Rule[] rules,
        Rule startRule
    )
    {
        int ruleCount = rules.Length;
        int keyCount = keys.Length;
        int indexSize = ruleCount + keyCount;
        this.indexMap = new (indexSize);
        
        int index = 0;
        this.elementMap = new (indexSize);
        foreach (var key in keys)
        {
            var keyIndex = ++index;
            elementMap.Add(key, keyIndex);
            indexMap.Add(keyIndex, key);
        }
        this.keyLastIndex = index;

        foreach (var rule in rules)
        {
            var ruleIndex = ++index;
            elementMap.Add(rule, ruleIndex);
            indexMap.Add(ruleIndex, rule);
        }

        pool = new();
        this.itemMap = [];
        this.ruleItemMap = new();

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
        goal[2] = elementMap[startRule];
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