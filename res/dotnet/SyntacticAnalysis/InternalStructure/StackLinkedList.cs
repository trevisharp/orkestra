using System.Collections;
using System.Collections.Generic;

namespace Orkestra.SyntacticAnalysis.InternalStructure;

using LexicalAnalysis;

public class StackLinkedList : IEnumerable<StackLinkedListNode>
{
    private StackLinkedListNode root = null;
    public StackLinkedList(IEnumerable<INode> buffer)
    {
        StackLinkedListNode header = new StackLinkedListNode();
        header.Value = null;
        this.root = header;
        
        StackLinkedListNode last = header;
        foreach (var node in buffer)
        {
            var newnode = new StackLinkedListNode();
            newnode.Value = node;
            last.Connect(newnode);
            last = newnode;
        }
        
        StackLinkedListNode footer = new StackLinkedListNode();
        footer.Value = null;
        last.Connect(footer);
    }

    public void Reduce(
        StackLinkedListNode initialNode, 
        INode inner,
        StackLinkedListNode finalNode)
    {
        StackLinkedListNode innerNode = new StackLinkedListNode();
        innerNode.Value = inner;
        var previous = initialNode.Previous;
        var next = finalNode.Next;
        previous.Connect(innerNode);
        next.Connect(finalNode);
    }

    public IEnumerator<StackLinkedListNode> GetEnumerator()
    {
        var crr = root;
        while (crr != null)
        {
            yield return crr;
            crr = crr.Next;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}