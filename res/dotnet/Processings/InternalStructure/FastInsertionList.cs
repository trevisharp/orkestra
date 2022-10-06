using System;
using System.Collections.Generic;

namespace Orkestra.Processings.InternalStructure;

public class FastInsertionList<T>
{
    private int size = 0;
    private int inLast = 0;
    private LinkedList<T[]> list = new LinkedList<T[]>();

    public FastInsertionList()
    {
        list.AddLast(new T[256]);
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
}