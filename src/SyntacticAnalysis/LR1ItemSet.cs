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

    /// <summary>
    /// The map to item from another item moving the
    /// current character one position.
    /// </summary>
    Dictionary<int, int> moveItemMap;

    /// <summary>
    /// The pair item with a lookAhead.
    /// </summary>
    Dictionary<int, (int item, int lookAhead)> lookAheadMap;

    /// <summary>
    /// next lookahed map index.
    /// </summary>
    int nextLookAheadMap = 0;

    /// <summary>
    /// The reverse of lookAheadMap.
    /// </summary>
    Dictionary<(int item, int lookAhead), int> revlookAheadMap;

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
                var itemSize = tokens.Length + 2;
                var item = pool.Rent(itemSize);
                item[0] = ruleIndex;
                item[1] = 0;
                for (int i = 2; i < itemSize; i++)
                {
                    var token = tokens[i - 2];
                    var tokenId = elementMap[token];
                    item[i] = tokenId;
                }
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

        this.moveItemMap = new();
        this.lookAheadMap = new ();
        this.revlookAheadMap = new ();
    }

    public int GetGoal()
        => 0;
    
    public int GetEOF()
        => -1;
    
    public int MakeLookAhead(int itemId, int lookAheadId)
    {
        var element = (itemId, lookAheadId);
        if (revlookAheadMap.ContainsKey(element))
            return revlookAheadMap[element];
        
        int key = ++nextLookAheadMap;
        lookAheadMap.Add(key, element);
        revlookAheadMap.Add(element, key);
        return key;
    }

    public int GetPureItem(int laItemId)
    {
        var laItem = lookAheadMap[laItemId];
        return laItem.item;
    }

    public int GetCurrentElement(int itemId)
    {
        var item = itemMap[itemId];
        var crrElement = item[1] + 2;
        if (crrElement >= item.Length)
            return -1;
        return item[crrElement];
    }

    public bool IsRule(int elementId)
        => elementId > keyLastIndex;

    public List<int> GetPureElementsByRule(int ruleId)
        => ruleItemMap[ruleId];
}