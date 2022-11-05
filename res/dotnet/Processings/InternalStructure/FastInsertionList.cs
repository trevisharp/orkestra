using System;
using System.Collections;
using System.Collections.Generic;

namespace Orkestra.Processings.InternalStructure;

internal class FastInsertionList<T> : IList<T>
{
    private int size = 0;
    private int inLast = 0;
    private LinkedList<T[]> list = new LinkedList<T[]>();
    private List<LinkedListNode<T[]>> breakpoints 
        = new List<LinkedListNode<T[]>>();

    public FastInsertionList()
    {
        list.AddLast(new T[256]);
        breakpoints.Add(list.First);
    }

    public int Count => size;

    public bool IsReadOnly => false;

    public IEnumerable<IEnumerable<T>> BreakPoints
    {
        get
        {
            for (int i = 0; i < this.breakpoints.Count; i++)
            {
                yield return getBreakpointIterator(i);
            }
        }
    }
    
    public T this[int i]
    {
        get
        {
            var vec = findNode(ref i).Value;
            return vec[i];
        }
        set
        {
            var vec = findNode(ref i).Value;
            vec[i] = value;
        }
    }

    public void AddRange(T[] vec)
    {
        var last = list.Last.Value;
        int lastCompleteCount = last.Length - inLast;
        Array.Copy(vec, 0, last, inLast, lastCompleteCount);

        for (int i = lastCompleteCount; i < vec.Length; i += 256)
        {
            list.AddLast(new T[256]);
            last = list.Last.Value;

            int valuesToCopy = vec.Length - i > 256 ? 256 : vec.Length - i;
            inLast = valuesToCopy;
            size += valuesToCopy;

            Array.Copy(vec, i, last, 0, valuesToCopy);
        }
    }

    public void Add(T value)
    {
        if (inLast == 256)
        {
            list.AddLast(new T[256]);
            inLast = 0;
        }
        
        list.Last.Value[inLast] = value;
        size++;
        inLast++;
    }

    public IEnumerator<T> GetEnumerator()
    {
        var it = list.First;
        while (it != list.Last)
        {
            for (int i = 0; i < it.Value.Length; i++)
                yield return it.Value[i];
            it = it.Next;
        }
        for (int i = 0; i < inLast; i++)
            yield return it.Value[i];
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public int IndexOf(T item)
    {
        int index = 0;
        foreach (var el in this)
        {
            if (el.Equals(item))
                break;
            index++;
        }
        if (index == this.Count)
            return -1;

        return index;
    }

    public void Insert(int index, T item)
    {
        var node = findNode(ref index);
        insert(node, index, item);
    }

    public void RemoveAt(int index)
    {
        var node = findNode(ref index);
        remove(node, index);
    }

    public void Clear()
    {
        list.Clear();
        list.AddLast(new T[256]);
        inLast = size = 0;
    }

    public bool Contains(T item)
    {
        foreach (var el in this)
            if (el.Equals(item))
                return true;
        return false;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        var it = this.GetEnumerator();
        for (int i = arrayIndex; 
            i < array.Length && it.MoveNext(); i++)
            array[i] = it.Current;
    }

    public bool Remove(T item)
    {
        var node = findNode(out int i, item);

        if (node == null)
            return false;
        
        remove(node, i);
        
        return true;
    }

    public void AddBreakPoint()
    {
        var array = list.Last.Value;
        var newArray = new T[inLast];
        
        list.Last.Value = newArray;
        Array.Copy(array, newArray, inLast);

        inLast = 0;
        list.AddLast(new T[256]);
        breakpoints.Add(list.Last);
    }

    private void remove(LinkedListNode<T[]> node, int index)
    {
        var array = node.Value;

        if (array.Length == 1)
        {
            list.Remove(node);
            return;
        }
        
        var newArray = new T[array.Length - 1];
        for (int i = 0; i < index; i++)
            newArray[i] = array[i];
        for (int i = index + 1; i < array.Length; i++)
        newArray[i - 1] = array[i];

        node.Value = newArray;
    }

    private void insert(LinkedListNode<T[]> node, int index, T item)
    {
        var array = node.Value;
        
        var newArray = new T[array.Length + 1];
        for (int i = 0; i < index; i++)
            newArray[i] = array[i];

        newArray[index] = item;

        for (int i = index + 1; i < array.Length; i++)
        newArray[i + 1] = array[i];

        node.Value = newArray;
    }

    private LinkedListNode<T[]> findNode(ref int index)
    {
        if (size == 0)
            throw new IndexOutOfRangeException();

        while (index < 0)
            index += size;
        var it = list.First;

        while (index > it.Value.Length)
        {
            index -= it.Value.Length;
            it = it?.Next;
        }

        if (it == null)
            throw new IndexOutOfRangeException();
        
        return it;
    }
    
    private LinkedListNode<T[]> findNode(out int index, T item)
    {
        index = 0;
        if (size == 0)
            throw new IndexOutOfRangeException();

        var it = list.First;

        while (it != null)
        {
            for (int i = 0; i < it.Value.Length; i++)
                if (it.Value[i].Equals(item))
                {
                    index += i;
                    return it;
                }
            index += it.Value.Length;
            it = it.Next;
        }
        
        return null;
    }

    private IEnumerable<T> getBreakpointIterator(int bpindex)
    {
        var start = this.breakpoints[bpindex];
        var end = bpindex + 1 < this.breakpoints.Count
            ? this.breakpoints[bpindex + 1]
            : null;

        var it = start;
        while (it != end)
        {
            var arr = it.Value;
            for (int i = 0; i < arr.Length; i++)
                yield return arr[i];
            it = it.Next;
        }
    }
}