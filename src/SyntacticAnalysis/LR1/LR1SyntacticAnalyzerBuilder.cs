/* Author:  Leonardo Trevisan Silio
 * Date:    23/04/2024
 */
using System;
using System.Linq;
using System.Collections.Generic;

namespace Orkestra.SyntacticAnalysis.LR1;

using Caches;

/// <summary>
/// A builder for a LR(1) analyzer.
/// </summary>
public class LR1SyntacticAnalyzerBuilder : ISyntacticAnalyzerBuilder
{
    private List<Rule> rules = new();
    private LR1ItemSet set;
    private int[] table;
    private int rows;
    private int rowSize;

    public Rule StartRule { get; set; }
    public IEnumerable<Rule> Rules => this.rules;

    public void Add(Rule rule)
        => this.rules.Add(rule);

    public ISyntacticAnalyzer Build()
    {
        LR1SyntacticAnalyzer analyzer = new(
            this.rowSize,
            this.table,
            set.ElementMap,
            set.IndexMap
        );
        return analyzer;
    }

    public bool LoadCache()
    {
        return false;
    }

    public void SaveCache()
    {
        
    }

    public void Load(IEnumerable<Key> keys)
    {
        this.set = new LR1ItemSet(
            keys.ToArray(),
            rules.ToArray(),
            StartRule
        );
        List<int[]> gotoTable = new List<int[]>();

        var goal = set.GetGoal();
        var eof = set.GetEOF();
        var initEl = set.CreateLookAheadItem(goal, eof);

        List<int> s0 = closure([ initEl ], set);
        List<List<int>> states = [ s0 ];
        List<int> initStateHashes = [ getHash([ initEl ]) ];
        List<int> stateHashes = [ getHash(s0) ];
        Dictionary<int, int> hashStateId = new Dictionary<int, int>();
        Queue<List<int>> queue = new Queue<List<int>>();
        queue.Enqueue(s0);

        var elementCount = set.GetElementsLength();
        while (queue.Count > 0)
        {
            var crr = queue.Dequeue();
            var stateRow = new int[elementCount];
            for (int i = 0; i < stateRow.Length; i++)
                stateRow[i] = set.GetEmpty();
            gotoTable.Add(stateRow);

            // find possible transite states
            for (int el = 0; el < elementCount; el++)
            {
                // generate state that transite by element el
                var newState = goTo(crr, el);
                if (newState.Count == 0)
                    continue;
                int stateId = gotoTable.Count + queue.Count;
                
                // test if the new state will be generate
                // other already generated state
                var hash = getHash(newState);
                if (initStateHashes.Contains(hash))
                {
                    stateRow[el] = hashStateId[hash];
                    continue;
                }
                initStateHashes.Add(hash);
                hashStateId.Add(hash, stateId);
                
                // get closure of state
                closure(newState, set);

                // test if the new state already exists
                hash = getHash(newState);
                if (stateHashes.Contains(hash))
                {
                    stateRow[el] = hashStateId[hash];
                    continue;
                }
                stateHashes.Add(hash);
                if (!hashStateId.ContainsKey(hash))
                    hashStateId.Add(hash, stateId);
                
                // save new state
                states.Add(newState);
                queue.Enqueue(newState);
                stateRow[el] = stateId;
            }
        }
        
        const int accept = 1 << 28;
        const int shift = 1 << 29;
        const int reduce = 1 << 30;
        const int sizeParam = 1 << 16;

        ISyntacticElement[] elements = set
            .GetElements()
            .ToArray();
        
        this.rows = states.Count;
        this.rowSize = elements.Length;
        this.table = new int[rows * rowSize];

        int stateIndex = 0;
        foreach (var state in states)
        {
            var gotoRow = gotoTable[stateIndex];
            int stateIndexOf = stateIndex * rowSize;
            
            foreach (var laItem in state)
            {
                var pureItem = set.GetPureItem(laItem);
                var rule = set.GetRule(pureItem);
                var ruleSize = set.GetRuleSize(pureItem);
                var lookAhead = set.GetLookAhead(laItem);
                var crrElement = set.GetCurrentElement(pureItem);
                var gotoValue = 
                    crrElement == set.GetEmpty() ?
                    set.GetEmpty() :
                    gotoRow[crrElement];

                if (gotoValue != set.GetEmpty() && !set.IsRule(crrElement))
                    table[stateIndexOf + crrElement] = shift | gotoValue;
                else if (set.IsGoal(pureItem) && crrElement == set.GetEmpty())
                    table[stateIndexOf] = accept;
                else if (crrElement == set.GetEmpty())
                    table[stateIndexOf + lookAhead] = reduce
                        | rule | (ruleSize * sizeParam);
            }
            
            for (int j = 0; j < rowSize; j++)
            {
                if (elements[j] is not Rule)
                    continue;
                
                var value = gotoRow[j];
                if (value == 0)
                    continue;
                
                table[stateIndexOf + j] = value;
            }

            stateIndex++;
        }
    }

    private void show(List<int[]> gotoTable)
    {
        var elementCount = set.GetElementsLength();
        Console.WriteLine();
        Console.Write("State\t|");
        for (int i = 0; i < elementCount; i++)
            Console.Write(
                set.GetElementString(i) + "\t|"
            );
        Console.WriteLine();
        int id = 0;
        foreach (var row in gotoTable)
        {
            Console.Write($"{id++}\t|");
            foreach (var gt in row)
                Console.Write($"{gt}\t|");
            Console.WriteLine();
        }
    }

    private void show(List<int> list)
    {
        Console.WriteLine("{ " +
            list
                .Select(set.GetLookAheadItemString)
                .Aggregate("", (acc, crr) => $"{acc}, {crr}")
            + " }"
        );
    }

    private void show(int[] table, ISyntacticElement[] elements)
    {
        Console.Write("#\t|");
        for (int i = 0; i < elements.Length; i++)
        {
            if (elements[i] is null)
                Console.Write("EOF\t|");
            else Console.Write(elements[i].Name + "\t|");
        }
        Console.WriteLine();
        
        int stateLen = elements.Length;
        int states = table.Length / stateLen;
        for (int i = 0; i < states; i++)
        {
            Console.Write($"{i}\t|");
            for (int j = 0; j < elements.Length; j++)
            {
                var value = table[stateLen * i + j];
                var op = (value / (1 << 28)) switch
                {
                    4 => "r ",
                    2 => "s ",
                    1 => "a ",
                    _ => ""
                };

                Console.Write($"{op}{value % (1 << 28)}\t|"); 
            }
            Console.WriteLine();
        }
    }

    private int getHash(List<int> list)
    {
        int hash = 0,
            last = 1,
            secondToLast = 1,
            pow = 1;
        
        foreach (var element in list)
        {
            var mod = element * last * secondToLast % 1024;
            hash += mod * pow;

            pow *= 31;
            secondToLast = last;
            last = element;
        }

        return hash;
    }

    private List<int> closure(List<int> state, LR1ItemSet set)
    {
        var queue = new Queue<int>();
        foreach (var item in state)
            queue.Enqueue(item);

        while (queue.Count > 0)
        {
            var laItem = queue.Dequeue();
            var item = set.GetPureItem(laItem);
            var lookAhead = set.GetLookAhead(laItem);
            var element = set.GetCurrentElement(item);
            if (!set.IsRule(element))
                continue;
            
            var next = set.GetNextElement(item);
            var prodcutions = set.GetPureElementsByRule(element);
            var firstSet = 
                next == set.GetEmpty() ?
                set.GetFirstSet(lookAhead) :
                set.GetFirstSet(next);
            foreach (var production in prodcutions)
            {
                foreach (var fstLA in firstSet)
                {
                    var newlaItem = set.CreateLookAheadItem(production, fstLA);
                    if (state.Contains(newlaItem))
                        continue;
                    
                    state.Add(newlaItem);
                    queue.Enqueue(newlaItem);
                }
            }
        }

        state.Sort();
        return state;
    }

    private List<int> goTo(List<int> state, int el)
    {
        List<int> newState = new List<int>();
        foreach (var laItem in state)
        {
            var item = set.GetPureItem(laItem);
            var lookAhead = set.GetLookAhead(laItem);
            var element = set.GetCurrentElement(item);
            if (element != el)
                continue;
            
            var movedItem = set.GetMovedItem(item);
            var newlaItem = set.CreateLookAheadItem(movedItem, lookAhead);
            newState.Add(newlaItem);
        }
        return newState;
    }
}