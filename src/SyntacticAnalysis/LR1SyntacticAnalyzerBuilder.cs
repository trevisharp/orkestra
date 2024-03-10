/* Author:  Leonardo Trevisan Silio
 * Date:    04/03/2024
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

        var goal = set.GetGoal();
        var eof = set.GetEOF();
        var initEl = set.CreateLookAheadItem(goal, eof);

        List<int> s0 = closure([ initEl ], set);
        List<List<int>> states = [ s0 ];
        List<int> stateHashes = [ getHash([ initEl ]) ];
        Queue<List<int>> queue = new Queue<List<int>>();
        queue.Enqueue(s0);

        while (queue.Count > 0)
        {
            var crr = queue.Dequeue();
            var elements = set.GetElementsLength();

            for (int el = 0; el < elements; el++)
            {
                var newState = goTo(crr, el);
                if (newState.Count == 0)
                    continue;
                
                var newHash = getHash(newState);
                if (stateHashes.Contains(newHash))
                    continue;
                stateHashes.Add(newHash);
                
                closure(newState, set);
                states.Add(newState);
                queue.Enqueue(newState);
            }
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

        return state;
    }

    private List<int> goTo(List<int> state, int el)
    {
        List<int> newState = new List<int>();
        foreach (var laItem in state)
        {
            var item = set.GetPureItem(laItem);
            var lookAhead = set.GetLookAhead(laItem);
            var element = set.GetNextElement(item);
            if (element != el)
                continue;
            
            var movedItem = set.GetMovedItem(item);
            var newlaItem = set.CreateLookAheadItem(movedItem, lookAhead);
            newState.Add(newlaItem);
        }
        return state;
    }
}