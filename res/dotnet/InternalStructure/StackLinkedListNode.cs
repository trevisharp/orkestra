using System.Collections.Generic;

namespace Orkestra.InternalStructure;

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

    public INode Value { get; set; }

    public void Connect(StackLinkedListNode node)
    {
        this.NextStack.Push(node);
        node.PreviousStack.Push(this);
    }

    public void Disconnect()
    {
        var node = this.NextStack.Pop();
        node.PreviousStack.Pop();
    }
}