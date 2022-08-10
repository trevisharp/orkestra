using System.Collections;
using System.Collections.Generic;

namespace Orkestra.InternalStructure;

public class ReduceBuffer : IEnumerable<ReduceBufferNode>
{
    private ReduceBufferNode root = null;
    public ReduceBuffer(IEnumerable<INode> buffer)
    {
        ReduceBufferNode header = new ReduceBufferNode();
        header.Value = null;
        this.root = header;
        
        ReduceBufferNode last = header;
        foreach (var node in buffer)
        {
            var newnode = new ReduceBufferNode();
            newnode.Value = node;
            last.Connect(newnode);
        }
    }

    public void Reduce(
        ReduceBufferNode initialNode, 
        INode inner,
        ReduceBufferNode finalNode)
    {
        ReduceBufferNode innerNode = new ReduceBufferNode();
        innerNode.Value = inner;
        var previous = initialNode.Previous;
        var next = finalNode.Next;
        previous.Connect(innerNode);
        next.Connect(finalNode);
    }

    public IEnumerator<ReduceBufferNode> GetEnumerator()
    {
        var crr = root;
        while (crr != null)
        {
            yield return crr;
            crr = crr.NextStack.Peek();
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}