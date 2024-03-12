using System.Linq;
using System.Collections.Generic;

using static System.Console;

namespace Orkestra.SyntacticAnalysis.InternalStructure;

using LexicalAnalysis;

public class SyntacticStateGraph
{
    public StackLinkedList TokenList { get; private set; }
    public List<Rule> RuleList { get; private set; }
    public AttemptDictionary Dictionary { get; private set; }

    public SyntacticStateGraph(IEnumerable<IMatch> tokens, IEnumerable<Rule> rules)
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

    private ExpressionTree depthFirstSearch()
    {
        var stack = new Stack<ReductionState>();
        stack.Push(createState());

        while (stack.Count > 0)
        {
            if (TokenList.Count() == 3)
                break;
            
            var crrState = stack.Pop();
            var nexts = computeNext(crrState)
                .ToArray();

            if (nexts.Length == 0)
            {
                tryReverse(crrState.ReverseParameter);
                continue;
            }
            
            foreach (var next in nexts)
                stack.Push(next);
        }

        var header = TokenList.FirstOrDefault();
        var main = header.Next;
        
        ExpressionTree tree = new ExpressionTree();
        tree.Root = main.Value;
        return tree;
    }

    //TODO: correct this without use Disconnect to improve
    //      the performance
    private IMatch buildTree(StackLinkedListNode node)
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

    private ReductionState createState(
        StackLinkedListNode reverse = null)
    {
        var header = TokenList.FirstOrDefault();
        var first = header.Next;
        return createState(first, reverse);
    }

    private ReductionState createState(
        StackLinkedListNode initial, 
        StackLinkedListNode reverse = null)
    {
        var attempts = Dictionary.GetAttempts(initial.Value);
        List<IEnumerator<ISyntaticElement>> firstItList = attempts
            .Select(sb => sb.RuleTokens.GetEnumerator())
            .ToList();
        var state = new ReductionState(
            initial, initial, attempts, firstItList, reverse);
        return state;
    }

    private ReductionState nextState(
        ReductionState state, 
        StackLinkedListNode newCurrent, 
        List<IEnumerator<ISyntaticElement>> newIterators,
         List<SubRule> newAttempts)
    {
        return new ReductionState(
            state.InitialNode,
            newCurrent.Next, 
            newAttempts, 
            newIterators,
            state.ReverseParameter
        );
    }

    private List<ReductionState> computeNext(ReductionState state)
    {
        List<ReductionState> result = new List<ReductionState>();

        var initial = state.InitialNode;
        var current = state.CurrentNode;
        var attempts = new List<SubRule>(state.Attempts);
        var its = state.Iterators;
        var reverse = state.ReverseParameter;

        while (attempts.Count > 0)
        {
            for (int i = 0; i < attempts.Count; i++)
            {
                var it = its[i];
                if (it.MoveNext())
                {
                    var node = current.Value;
                    if (node != null && node.Is(it.Current))
                        continue;
                    
                    its.RemoveAt(i);
                    attempts.RemoveAt(i);
                    i--;
                    continue;
                }

                RuleMatch match = new RuleMatch(attempts[i]);
                StackLinkedListNode newNode = new StackLinkedListNode();
                newNode.Value = match;
                attempts.RemoveAt(i);

                if (attempts.Count > 0)
                    result.Add(nextState(state, current, its, attempts));
                else if (attempts.Count == 0 && initial.Next.Value != null)
                    result.Add(createState(initial.Next, reverse));

                //TODO: Remove this to use treeBuilder
                var start = initial;
                var end = current;
                while (start != end)
                {
                    match.Children.Add(start.Value);
                    start = start.Next;
                }

                initial.Previous.Connect(newNode);
                newNode.Connect(end);

                result.Add(createState(newNode));
                return result;
            }
            current = current.Next;
        }
        return result;
    }

    private void tryReverse(StackLinkedListNode reverseNode)
    {
        if (reverseNode == null)
        {
            throw new System.NotImplementedException();
        }
        reverseNode.Disconnect();
    }
}