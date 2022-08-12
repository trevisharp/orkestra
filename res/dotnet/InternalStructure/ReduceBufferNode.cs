using System.Collections.Generic;

namespace Orkestra.InternalStructure;

public class ReduceBufferNode
{
    public ReduceBufferNode()
    {
        this.NextStack = new Stack<ReduceBufferNode>();
        this.PreviousStack = new Stack<ReduceBufferNode>();
    }

    public Stack<ReduceBufferNode> NextStack { get; set; }
    public ReduceBufferNode Next => HasNext ? NextStack.Peek() : null;
    public bool HasNext => NextStack.Count > 0;

    public Stack<ReduceBufferNode> PreviousStack { get; set; }
    public ReduceBufferNode Previous => PreviousStack.Peek();

    public INode Value { get; set; }

    public void Connect(ReduceBufferNode node)
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