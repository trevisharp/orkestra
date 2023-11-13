/* Author:  Leonardo Trevisan Silio
 * Date:    13/11/2023
 */
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using State = System.Collections.Generic.List<int>;

namespace Orkestra.SyntacticAnalysis.InternalStructure;

using Orkestra.InternalStructure;

internal class LRItemMap
{
    const int ruleSizeof = 0;
    const int dotSizeof = 1;
    const int closureStartSizeof = 2;
    const int closureLengthSizeof = 3;
    const int moveSizeof = 4;
    const int tokenCountSizeof = 5;
    const int tokensSizeof = 6;

    public LRItemMap(
        List<Rule> rules,
        List<Key> keys,
        Rule startRule
    )
    {
        int ruleCount = rules.Count;
        int keyCount = keys.Count;
        elementMap = new Dictionary<IRuleElement, int>(
            ruleCount + keyCount
        );

        int index = 0;
        foreach (var rule in rules)
            this.elementMap.Add(rule, ++index);
        
        this.rulesLastIndex = index;
        foreach (var key in keys)
            this.elementMap.Add(key, ++index);
        
        var subRules = rules
            .SelectMany(r => r.SubRules);
        
        this.itemBuffer = new FastList<int>();
        foreach (var r in subRules)
        {
            // rule
            itemBuffer.Add(elementMap[r.Parent]);
            
            // dot
            itemBuffer.Add(0);

            // closure start and len
            itemBuffer.Add(-1);
            itemBuffer.Add(-1);

            // move
            itemBuffer.Add(-1);
            
            // tokens
            itemBuffer.Add(r.RuleTokens.Count());
            foreach (var token in r.RuleTokens)
                itemBuffer.Add(elementMap[token]);
        }

        // Add Start Rule
        itemBuffer.Add(0);
        itemBuffer.Add(0);
        itemBuffer.Add(-1);
        itemBuffer.Add(-1);
        itemBuffer.Add(-1);
        itemBuffer.Add(1);
        itemBuffer.Add(elementMap[startRule]);
    }

    internal void Closure(int index)
    {
        
    }

    FastList<int> itemBuffer;
    Dictionary<IRuleElement, int> elementMap;
    int rulesLastIndex;
}