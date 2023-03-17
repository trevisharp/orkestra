using System;
using System.Collections;
using System.Collections.Generic;

namespace Orkestra.InternalStructure;

internal class FastListNode<T> : IEnumerable<T>
{
    private int size;
    private int maxSize;
    private T[] arr;

    internal FastListNode()
    {
        this.maxSize = 1024;
        this.arr = new T[maxSize];
        this.size = 0;
    }

    internal FastListNode(T[] values)
    {
        this.maxSize = values.Length;
        this.arr = new T[maxSize];
        this.size = maxSize;
    }

    internal int Length => size;
    internal T this[int i]
    {
        get => this.arr[i];
        set => this.arr[i] = value;
    }

    internal bool Add(T value)
    {
        if (size > maxSize - 1)
            return false;
        
        arr[size] = value;
        size++;

        return true;
    }

    internal int AddRange(T[] array, int offset = 0)
    {
        int validPositions = maxSize - size;
        int validItens = array.Length - offset;
        int addedValues = validPositions > validItens ?
            validItens : validPositions;

        Array.Copy(array, offset, arr, size, addedValues);
        size += addedValues;

        return addedValues;
    }

    internal void Replace(T[] array)
    {
        int newSize = array.Length;

        this.size = newSize;
        this.maxSize = newSize;
        this.arr = new T[newSize];

        Array.Copy(array, this.arr, newSize);
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < size; i++)
            yield return this.arr[i];
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}