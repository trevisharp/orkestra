/* Author:  Leonardo Trevisan Silio
 * Date:    14/11/2023
 */
using System;
using System.Buffers;

namespace Orkestra.SyntacticAnalysis.InternalStructure;

using Orkestra.InternalStructure;

internal class LRItemTree : IDisposable
{
    private int maxValue;
    FastList<LRItemNode> nodes = new();
    readonly record struct LRItemNode(
        int value,
        bool hasLeaf,
        int[] children
    );

    public LRItemTree(int maxValue)
    {
        this.maxValue = maxValue;
        this.nodes.Add(new (-1, false, rent()));
    }

    public void Add(params int[] data)
        => add(data, 0, 0);
    
    public bool Has(params int[] data)
    {
        var crrNode = 0;
        for (int i = 0; i < data.Length; i++)
        {
            var value = data[i];
            var node = nodes[crrNode];
            var next = node.children[value];

            if (next == 0)
                return false;
            crrNode = next;
        }
        return nodes[crrNode].hasLeaf;
    }

    public void Dispose()
    {
        foreach (var node in nodes)
        {
            ArrayPool<int>.Shared
                .Return(node.children);
        }
        nodes.Clear();
    }

    private void add(int[] data, int index, int nodeIndex)
    {
        var value = data[index];
        var node = nodes[nodeIndex];
        var next = node.children[value];
        var leaf = index == data.Length - 1;

        if (next == 0)
        {
            nodes.Add(create(value, leaf));
            next = node.children[value] = nodes.Count - 1;
        }

        if (leaf)
        {
            if (!node.hasLeaf)
                nodes[nodeIndex] = node with { hasLeaf = true };
            return;
        }
        
        add(data, index + 1, next);
    }

    int[] rent()
        => ArrayPool<int>.Shared.Rent(maxValue);
    
    LRItemNode create(int value, bool leaf)
        => new (value, leaf, rent());
}