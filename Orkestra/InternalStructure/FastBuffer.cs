/* Author:  Leonardo Trevisan Silio
 * Date:    04/03/2024
 */
using System;
using System.Buffers;
using System.Collections.Generic;

namespace Orkestra.InternalStructure;

internal class FastBuffer<T> : IDisposable
{
    List<T[]> buffers = new();

    public void Dispose()
        => ReturnAll();

    public T[] Rent(int size)
    {
        // var pool = ArrayPool<T>.Shared;
        // var buffer = pool.Rent(size);
        // buffers.Add(buffer);
        return new T[size];
    }

    public void Return(T[] buffer)
    {
        if (!buffers.Contains(buffer))
            return;
        
        var pool = ArrayPool<T>.Shared;
        buffers.Remove(buffer);
        pool.Return(buffer);
    }

    public void ReturnAll()
    {
        var pool = ArrayPool<T>.Shared;
        foreach (var buffer in buffers)
            pool.Return(buffer);
        buffers.Clear();
    }
}