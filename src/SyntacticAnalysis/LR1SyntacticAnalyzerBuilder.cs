/* Author:  Leonardo Trevisan Silio
 * Date:    04/11/2023
 */
using System.Linq;
using System.Collections.Generic;

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
        var el = set.MakeLookAhead(goal, eof);

        List<int> s0 = [ el ];
        var queue = new Queue<int>();
        queue.Enqueue(el);

        while (queue.Count > 0)
        {
            var laItem = queue.Dequeue();
            var item = set.GetPureItem(laItem);
            var element = set.GetCurrentElement(item);
            if (!set.IsRule(element))
                continue;
            
            
        }
    }
}