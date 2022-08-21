using System.Linq;
using System.Collections.Generic;

using static System.Console;

namespace Orkestra.InternalStructure;

public class SyntacticStateGraph
{
    public StackLinkedList TokenList { get; private set; }
    public List<Rule> RuleList { get; private set; }
    public AttemptDictionary Dictionary { get; private set; }

    public SyntacticStateGraph(IEnumerable<INode> tokens, IEnumerable<Rule> rules)
    {
        this.TokenList = new StackLinkedList(tokens);
        this.RuleList = new List<Rule>(rules);
        this.Dictionary = new AttemptDictionary(rules);
    }

    public ExpressionTree DepthFirstSearch()
    {
        var result = depthFirstSearch();
        
        return result;
    }

    private (ReductionState state, StackLinkedListNode match) tryReduce(ReductionState state)
    {

        StackLinkedListNode updatedInitial = state.InitialNode;
        StackLinkedListNode updatedCurrent = state.CurrentNode;
        var updatedReverseParameter = state.ReverseParameter;
        int updatedCurrentIndex = state.CurrentNodeIndex;
        List<SubRule> updatedAttempts = new List<SubRule>(state.Attempts);
        List<IEnumerator<IRuleElement>> itList = updatedAttempts
            .Select(sb => sb.RuleTokens
                .Skip(updatedCurrentIndex).GetEnumerator())
            .ToList();
        ReductionState updatedState = null;

        while (updatedAttempts.Count > 0)
        {
            for (int i = 0; i < updatedAttempts.Count; i++)
            {
                if (!itList[i].MoveNext()) // Match
                {
                    WriteLine("\t\tMatch!");
                    updatedCurrentIndex++;

                    var start = updatedInitial.Previous;
                    var end = updatedCurrent;

                    RuleMatch match = new RuleMatch(updatedAttempts[i]);
                    StackLinkedListNode newNode = new StackLinkedListNode();
                    newNode.Value = match;

                    //TODO: Remove this to use treeBuilder
                    var it = updatedInitial;
                    while (it != end)
                    {
                        match.Children.Add(it.Value);
                        it = it.Next;
                    }

                    start.Connect(newNode);
                    newNode.Connect(end);
        
                    updatedState = new ReductionState(
                        updatedInitial, 
                        updatedCurrent, 
                        updatedCurrentIndex, 
                        updatedAttempts,
                        updatedReverseParameter
                    );

                    return (updatedState, newNode);
                }

                bool subMatchCondition = updatedCurrent.Value != null && updatedCurrent.Value.Is(itList[i].Current);
                WriteLine($"\t\t{i}: {updatedCurrent.Value} is {itList[i].Current} = {subMatchCondition}");
                if (subMatchCondition)
                    continue;
                
                itList.RemoveAt(i);
                updatedAttempts.RemoveAt(i);
                i--;
            }
            updatedCurrent = updatedCurrent.Next;
        }
        WriteLine("\t\tFail!");
        
        updatedState = new ReductionState(
            updatedInitial,
            updatedCurrent,
            updatedCurrentIndex,
            updatedAttempts,
            updatedReverseParameter
        );
        return (updatedState, null);
    }

    private ExpressionTree depthFirstSearch()
    {
        var stack = new Stack<ReductionState>();

        var header = TokenList.FirstOrDefault();
        var first = header.Next;
        var attempts = Dictionary.GetAttempts(first.Value);
        var state = new ReductionState(first, first, 0, attempts, null);
        stack.Push(state);

        while (stack.Count > 0)
        {
            var crrState = stack.Pop();

            #if DEBUG
            WriteLine();
            WriteLine($"Stack Size: {stack.Count}");
            var crr = TokenList.FirstOrDefault();
            Write("Buffer:\n\t");
            while (true)
            {
                if (crrState.CurrentNode.Value == crr.Value)
                    Write($"[{crr.Value?.ToString() ?? "null"}],  ");
                else Write($"{crr.Value?.ToString() ?? "null"},  ");
                if (!crr.HasNext)
                    break;
                crr = crr.Next;
            }
            WriteLine();
            WriteLine($"\tindex = {crrState.CurrentNodeIndex}, attempts = {crrState.Attempts.Count()}, reverse = {crrState.ReverseParameter?.Value.ToString() ?? "null"}");
            ReadKey(true);
            #endif


            if (TokenList.Count() == 3)
            {
                break;
            }

            var result = tryReduce(crrState);
            crrState = result.state;
            var match = result.match;
            
            if (match == null)
            {
                if (!crrState.InitialNode.HasNext)
                {
                    var reverseParameter = crrState.ReverseParameter;
                    if (reverseParameter == null)
                    {
                        throw new System.NotImplementedException();
                    }
                    reverseParameter.Disconnect();
                }
                else
                {
                    var next = crrState.InitialNode.Next;
                    var nextAttempts = Dictionary.GetAttempts(next.Value);
                    var updatedState = new ReductionState(next, next, 0, nextAttempts, crrState.ReverseParameter);
                    stack.Push(updatedState);
                }
            }
            else
            {
                stack.Push(crrState);

                header = TokenList.FirstOrDefault();
                first = header.Next;
                attempts = Dictionary.GetAttempts(first.Value);
                var newState = new ReductionState(first, first, 0, attempts, match);
                stack.Push(newState);
            }
        }

        header = TokenList.FirstOrDefault();
        var main = header.Next;
        
        ExpressionTree tree = new ExpressionTree();
        tree.Root = main.Value;
        return tree;
    }

    //TODO: correct this without use Disconnect to improve
    //      the performance
    private INode buildTree(StackLinkedListNode node)
    {
        if (node == null)
            return null;
        if (node.Value is RuleMatch match)
        {
            WriteLine($"Match ({match}) Finded!");
            var start = node.Previous;
            var end = node.Next;
            node.Disconnect();

            var it = start.Next;
            while (it != end)
            {
                var value = buildTree(it);
                match.Children.Add(value);
                it = it.Next;
                WriteLine($"{it.Value} != {end.Value} = {it != end}");
            }

            WriteLine($"Match: {match}");
            return match;
        }
        else if (node.Value is Token token)
        {
            WriteLine($"Token: {token}");

            return token;
        }
        return null;
    }
}