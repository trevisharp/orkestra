/* Author:  Leonardo Trevisan Silio
 * Date:    04/11/2023
 */
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Runtime.InteropServices;

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
        var initEl = set.MakeLookAhead(goal, eof);

        List<int> s0 = [ initEl ];
        var queue = new Queue<int>();
        queue.Enqueue(initEl);

        while (queue.Count > 0)
        {
            var laItem = queue.Dequeue();
            var item = set.GetPureItem(laItem);
            var lookAhead = set.GetLookAhead(laItem);
            var element = set.GetCurrentElement(item);
            if (!set.IsRule(element))
                continue;
            
            var nexts = set.GetPureElementsByRule(element);
            foreach (var next in nexts)
            {
                var newlaItem = set.MakeLookAhead(next, lookAhead);
                if (s0.Contains(newlaItem))
                    continue;
                
                s0.Add(newlaItem);
                queue.Enqueue(newlaItem);
            }
        }

        System.Console.WriteLine(s0.Count);
    }
}