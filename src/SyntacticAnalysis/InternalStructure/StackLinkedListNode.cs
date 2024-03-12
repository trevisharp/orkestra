using System.Collections.Generic;

namespace Orkestra.SyntacticAnalysis.InternalStructure;

using LexicalAnalysis;

public class StackLinkedListNode
{
    public StackLinkedListNode()
    {
        this.NextStack = new Stack<StackLinkedListNode>();
        this.PreviousStack = new Stack<StackLinkedListNode>();
    }

    public Stack<StackLinkedListNode> NextStack { get; set; }
    public StackLinkedListNode Next => HasNext ? NextStack.Peek() : null;
    public bool HasNext => NextStack.Count > 0;

    public Stack<StackLinkedListNode> PreviousStack { get; set; }
    public StackLinkedListNode Previous => PreviousStack.Peek();

    public IMatch Value { get; set; }

    public void Connect(StackLinkedListNode node)
    {
        this.NextStack.Push(node);
        node.PreviousStack.Push(this);
    }

    public void Disconnect()
    {
        var next = this.NextStack.Pop();
        next.PreviousStack.Pop();

        var previous = this.PreviousStack.Pop();
        previous.NextStack.Pop();
    }
}