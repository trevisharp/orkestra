/* Author:  Leonardo Trevisan Silio
 * Date:    10/03/2024
 */
using System.Linq;
using System.Collections.Generic;
using System;

namespace Orkestra.SyntacticAnalysis;

/// <summary>
/// A builder for a LR(1) analyzer.
/// </summary>
public class LR1SyntacticAnalyzerBuilder : ISyntacticAnalyzerBuilder
{
    private List<Rule> rules = new();
    private LR1ItemSet set;

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
            gotoTable.Add(stateRow);

            for (int el = 0; el < elementCount; el++)
            {
                var newState = goTo(crr, el);
                if (newState.Count == 0)
                    continue;
                int stateId = gotoTable.Count + queue.Count;
                
                var newHash = getHash(newState);
                if (initStateHashes.Contains(newHash))
                {
                    stateRow[el] = hashStateId[newHash];
                    continue;
                }
                initStateHashes.Add(newHash);
                hashStateId.Add(newHash, stateId);
                
                closure(newState, set);
                newHash = getHash(newState);
                if (stateHashes.Contains(newHash))
                {
                    stateRow[el] = hashStateId[newHash];
                    continue;
                }
                stateHashes.Add(newHash);
                if (!hashStateId.ContainsKey(newHash))
                    hashStateId.Add(newHash, stateId);

                states.Add(newState);
                queue.Enqueue(newState);
                stateRow[el] = stateId;
            }
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
            
            var nexts = set.GetPureElementsByRule(element);
            var nextEl = set.GetNextElement(item);
            foreach (var next in nexts)
            {
                var firstSet = 
                    nextEl == -1 && lookAhead != -1 ?
                    [-1, lookAhead] :
                    set.GetFirstSet(nextEl);
                foreach (var fstLA in firstSet)
                {
                    var newlaItem = set.CreateLookAheadItem(next, fstLA);
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