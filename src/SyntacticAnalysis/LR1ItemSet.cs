/* Author:  Leonardo Trevisan Silio
 * Date:    04/03/2024
 */
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
    /// The first set of elements.
    /// </summary>
    Dictionary<int, List<int>> firstSet;

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
        this.firstSet = new(indexSize + 1);
        firstSet[-1] = [-1];
        
        int index = 0;
        this.elementMap = new (indexSize);
        foreach (var key in keys)
        {
            var keyIndex = ++index;
            elementMap.Add(key, keyIndex);
            indexMap.Add(keyIndex, key);
            firstSet.Add(keyIndex, [keyIndex]);
        }
        this.keyLastIndex = index;

        foreach (var rule in rules)
        {
            var ruleIndex = ++index;
            elementMap.Add(rule, ruleIndex);
            indexMap.Add(ruleIndex, rule);
            firstSet.Add(ruleIndex, []);
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
                itemRules.Add(itemIndex);
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
        this.lookAheadMap = new();
        this.revlookAheadMap = new();

        bool hasChange = true;
        while (hasChange)
        {
            hasChange = false;
            foreach (var rule in rules)
            {
                var ruleId = elementMap[rule];
                var set = firstSet[ruleId];
                foreach (var sub in rule.SubRules)
                {
                    var fst = sub.RuleTokens.First();
                    var fstId = elementMap[fst];
                    var otherSet = firstSet[fstId];
                    foreach (var el in otherSet)
                    {
                        if (set.Contains(el))
                            continue;
                        
                        set.Add(el);
                        hasChange = true;
                    }
                }
            }
        }

        // foreach (var pair in firstSet)
        // {
        //     System.Console.Write(
        //         indexMap[pair.Key].KeyName
        //     );
        //     System.Console.Write(" = {");
        //     foreach (var item in pair.Value)
        //     {
        //         System.Console.Write(indexMap[item].KeyName);
        //         System.Console.Write(", ");
        //     }
        //     System.Console.WriteLine("}");
        // }
    }

    /// <summary>
    /// Get goal item ID.
    /// </summary>
    public int GetGoal()
        => 0;
    
    /// <summary>
    /// Get EOF lookAhead ID.
    /// </summary>
    public int GetEOF()
        => -1;
    
    /// <summary>
    /// Make a item with lookahead
    /// </summary>
    public int CreateLookAheadItem(int itemId, int lookAheadId)
    {
        var element = (itemId, lookAheadId);
        if (revlookAheadMap.ContainsKey(element))
            return revlookAheadMap[element];
        
        int key = ++nextLookAheadMap;
        lookAheadMap.Add(key, element);
        revlookAheadMap.Add(element, key);
        return key;
    }
    
    /// <summary>
    /// Get the id of a item with lookahead.
    /// </summary>
    public int GetPureItem(int laItemId)
    {
        var laItem = lookAheadMap[laItemId];
        return laItem.item;
    }

    /// <summary>
    /// Get the id of a lookahead.
    /// </summary>
    public int GetLookAhead(int laItemId)
    {
        var laItem = lookAheadMap[laItemId];
        return laItem.lookAhead;
    }

    /// <summary>
    /// Get the id of element in current position of a item. 
    /// </summary>
    public int GetCurrentElement(int itemId)
    {
        var item = itemMap[itemId];
        var crrElement = item[1] + 2;
        if (crrElement >= item.Length)
            return -1;
        return item[crrElement];
    }

    /// <summary>
    /// Get the id of next element in current position of a item. 
    /// </summary>
    public int GetNextElement(int itemId)
    {
        var item = itemMap[itemId];
        var crrElement = item[1] + 2;
        var nxtElement = crrElement + 1;
        if (nxtElement >= item.Length)
            return -1;
        return item[nxtElement];
    }

    /// <summary>
    /// Get if a element is a rule.
    /// </summary>
    public bool IsRule(int elementId)
        => elementId > keyLastIndex;

    /// <summary>
    /// Get all pure items that expando from a rule.
    /// </summary>
    public List<int> GetPureElementsByRule(int ruleId)
        => ruleItemMap[ruleId];

    /// <summary>
    /// Get the first set items from a rule.
    /// </summary>
    public List<int> GetFirstSet(int elementId)
        => firstSet[elementId];
}