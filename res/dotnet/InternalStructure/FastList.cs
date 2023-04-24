using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Orkestra.InternalStructure;

internal class FastList<T> : IEnumerable<T>
{
    private const int maxSize = 1024;
    private LinkedList<List<T>> list = new();
    private List<T> Last => list.Last.Value;
    private int count = 0;

    public int Count => count;

    internal FastList()
        => list.AddLast(new List<T>(maxSize));

    public T this[int index]
    {
        get
        {
            var node = list.First;
            while (index > node.Value.Count)
            {
                index -= node.Value.Count;
                node = node.Next;
            }
            return node.Value[index];
        }
        set
        {
            var node = list.First;
            while (index > node.Value.Count)
            {
                index -= node.Value.Count;
                node = node.Next;
            }
            node.Value[index] = value;
        }
    }
    
    private List<T> addNode()
    {
        var newNode = new List<T>();
        list.AddLast(newNode);
        return newNode;
    }

    internal void Add(T value)
    {
        count++;

        if (Last.Count < maxSize)
        {
            Last.Add(value);
            return;
        }
        
        var newNode = addNode();
        newNode.Add(value);
    }

    internal void AddRange(T[] values)
    {
        for (int i = 0; i < values.Length; i++)
            this.Add(values[i]);
    }
    
    internal void AddRange(IEnumerable<T> values)
        => this.AddRange(values.ToArray());
    
    internal void Insert(T value, int index)
    {
        var node = this.list.First;
        while (index > node.Value.Count)
        {
            index -= node.Value.Count;
            node = node.Next;
        }
        var list = node.Value;
        list.Insert(index, value);
    }

    internal void Insert(T[] values, int index)
    {
        var node = this.list.First;
        while (index > node.Value.Count)
        {
            index -= node.Value.Count;
            node = node.Next;
        }
        var list = node.Value;

        List<T> pre = new List<T>();
        List<T> pos = new List<T>();
        for (int i = 0; i < index; i++)
            pre.Add(list[i]);
        for (int i = index; i < list.Count; i++)
            pos.Add(list[i]);
        
        int j = 0;
        while (pre.Count < maxSize && j < values.Length)
            pre.Add(values[j++]);
        node.Value = pre;

        var posNode = new LinkedListNode<List<T>>(pos);
        this.list.AddAfter(node, posNode);

        while (j < values.Length)
        {
            List<T> inner = new List<T>();
            for (int i = 0; i < maxSize; i++)
                inner.Add(values[j++]);
            this.list.AddBefore(posNode, new LinkedListNode<List<T>>(inner));
        }

        this.count += values.Length;
    }

    internal void Remove(int start, int len)
    {
        if (len < 0)
            len = this.count - start - len + 1;

        var node = this.list.First;
        while (start > node.Value.Count)
        {
            start -= node.Value.Count;
            node = node.Next;
        }

        while (len > 0)
        {
            var list = node.Value;
            int removed = list.Count - start;
            if (removed > len)
                removed = len;
            list.RemoveRange(start, removed);
            len -= removed;
            this.count -= removed;

            node = node.Next;
            start = 0;
        }
    }

    internal void Clear()
    {
        this.list.Clear();
        list.AddLast(new List<T>(maxSize));
    }
    
    
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