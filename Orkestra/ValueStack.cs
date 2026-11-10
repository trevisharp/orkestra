using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Orkestra;

/// <summary>A growable stack backed by pooled arrays. Must be passed by reference and disposed.</summary>
internal struct ValueStack<T> : IDisposable
{
    T[] items;
    int count;

    public ValueStack(int capacity)
    {
        items = ArrayPool<T>.Shared.Rent(capacity);
        count = 0;
    }

    public readonly int Count => count;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Push(T item)
    {
        if (count == items.Length)
            Grow();
        items[count++] = item;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T Peek() => items[count - 1];

    /// <summary>The value <paramref name="depth"/> places below the top (0 is the top).</summary>
    public readonly T PeekAt(int depth) => items[count - 1 - depth];

    /// <summary>The top <paramref name="length"/> items, bottom first.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ReadOnlySpan<T> Top(int length) => items.AsSpan(count - length, length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Pop(int length)
    {
        count -= length;
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            items.AsSpan(count, length).Clear();
    }

    /// <summary>Removes items until only <paramref name="newCount"/> remain.</summary>
    public void Truncate(int newCount) => Pop(count - newCount);

    void Grow()
    {
        var bigger = ArrayPool<T>.Shared.Rent(items.Length * 2);
        items.AsSpan(0, count).CopyTo(bigger);
        Return(items);
        items = bigger;
    }

    public void Dispose()
    {
        if (items is null)
            return;
        Return(items);
        items = null!;
        count = 0;
    }

    readonly void Return(T[] array)
        => ArrayPool<T>.Shared.Return(array, RuntimeHelpers.IsReferenceOrContainsReferences<T>());
}
