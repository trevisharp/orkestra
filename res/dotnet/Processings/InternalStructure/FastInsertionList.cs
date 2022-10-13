using System;
using System.Collections;
using System.Collections.Generic;

namespace Orkestra.Processings.InternalStructure;

internal class FastInsertionList<T> : IEnumerable<T>
{
    private int size = 0;
    private int inLast = 0;
    private LinkedList<T[]> list = new LinkedList<T[]>();

    internal int Count => size;

    internal T this[int i]
    {
        get
        {
            var vec = getVector(ref i);
            return vec[i];
        }
        set
        {
            var vec = getVector(ref i);
            vec[i] = value;
        }
    }

    private T[] getVector(ref int index)
    {
        if (size == 0)
            throw new IndexOutOfRangeException();

        while (index < 0)
            index += size;
        var it = list.First;

        while (index > 256)
        {
            index -= 256;
            it = it?.Next;
        }

        if (it == null)
            throw new IndexOutOfRangeException();
        
        return it.Value;
    }

    internal FastInsertionList()
    {
        list.AddLast(new T[256]);
    }

    internal void AddRange(T[] vec)
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

    internal void Add(T value)
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
        }
        for (int i = 0; i < inLast; i++)
            yield return it.Value[i];
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}