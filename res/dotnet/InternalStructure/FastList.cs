using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Orkestra.InternalStructure;

internal class FastList<T> : IEnumerable<T>
{
    private LinkedList<FastListNode<T>> list = new();
    private int count = 0;

    internal FastList()
    {
        list.AddLast(new FastListNode<T>());
    }

    private FastListNode<T> Last => list.Last.Value;

    internal IEnumerable<LinkedListNode<FastListNode<T>>> Nodes
    {
        get
        {
            var crr = list.First;
            if (crr == null)
                yield break;
            yield return crr;
            
            while (crr != list.Last)
            {
                crr = crr.Next;
                yield return crr;
            }
        }
    }

    internal void Add(T value)
    {
        count++;

        if (Last.Add(value))
            return;
        
        list.AddLast(new FastListNode<T>());
    }

    internal void AddRange(T[] values)
    {
        int added = Last.AddRange(values);
        while (added < values.Length)
        {
            list.AddLast(new FastListNode<T>());
            added += Last.AddRange(values);
        }
        count += added;
    }
    
    internal void AddRange(IEnumerable<T> values)
        => this.AddRange(values.ToArray());
    
    internal void AddNode(T[] values)
        => this.list.AddLast(new FastListNode<T>(values));

    internal void Replace(LinkedListNode<FastListNode<T>> node, T[] values)
        => node.Value.Replace(values);
    
    internal void Append(LinkedListNode<FastListNode<T>> node, T[] values)
        => list.AddAfter(node, new FastListNode<T>(values));

    internal void Prepend(LinkedListNode<FastListNode<T>> node, T[] values)
        => list.AddBefore(node, new FastListNode<T>(values));
    
    public IEnumerator<T> GetEnumerator()
    {
        foreach (var node in list)
        {
            foreach (var value in node)
                yield return value;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}