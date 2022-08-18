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
        WriteLine("In TryReduce:");

        StackLinkedListNode updatedInitial = state.InitialNode;
        StackLinkedListNode updatedCurrent = state.CurrentNode;
        int updatedCurrentIndex = state.CurrentNodeIndex;
        List<SubRule> updatedAttempts = new List<SubRule>(state.Attempts);
        List<IEnumerator<IRuleElement>> itList = updatedAttempts
            .Select(sb => sb.RuleTokens
                .Skip(updatedCurrentIndex).GetEnumerator())
            .ToList();
        ReductionState updatedState = null;
        
        while (updatedAttempts.Count > 0)
        {
            WriteLine($"\tIn Attempt {updatedAttempts.Count}:");
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

                    #if DEBUG
                    var crr = TokenList.FirstOrDefault();
                    Write("\t\tNew Buffer:\n\t\t\t");
                    while (true)
                    {
                        Write($"{crr.Value?.ToString() ?? "null"},  ");
                        if (!crr.HasNext)
                            break;
                        crr = crr.Next;
                    }
                    WriteLine();
                    #endif
        
                    updatedState = new ReductionState(
                        updatedInitial, 
                        updatedCurrent, 
                        updatedCurrentIndex, 
                        updatedAttempts,
                        null
                    );

                    return (updatedState, newNode);
                }

                bool subMatchCondition = updatedCurrent.Value != null && updatedCurrent.Value.Is(itList[i].Current);

                #if DEBUG
                WriteLine($"\t\tAttempt {updatedCurrent.Value} is {itList[i].Current} = {subMatchCondition}");
                #endif

                if (subMatchCondition)
                {
                    updatedCurrent = updatedCurrent.Next;
                    continue;
                }
                
                WriteLine("\t\tRemoving Attempt...");
                itList.RemoveAt(i);
                updatedAttempts.RemoveAt(i);
                i--;
            }
        }
        
        updatedState = new ReductionState(
            updatedInitial, 
            updatedCurrent, 
            updatedCurrentIndex, 
            updatedAttempts,
            null
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
            WriteLine($"Stack Size: {stack.Count}");
            WriteLine($"Buffer Size: {TokenList.Count()}");

            var crrState = stack.Pop();

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
                    var updatedState = new ReductionState(next, next, 0, nextAttempts, null);
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