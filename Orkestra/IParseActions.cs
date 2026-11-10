using System;

namespace Orkestra;

/// <summary>
/// Builds the result of a parse, one step at a time. Symphony generates an implementation
/// that creates the typed syntax tree; it can be a <c>ref struct</c> holding the source span.
/// </summary>
/// <typeparam name="TNode">The type of value kept on the parser stack, usually the base syntax node class.</typeparam>
public interface IParseActions<TNode>
{
    /// <summary>Creates the value for a token the parser just consumed.</summary>
    TNode Shift(in Token token);

    /// <summary>
    /// Creates the value for <paramref name="production"/> from the values of its right side.
    /// <paramref name="children"/> is valid only during the call. Never called for production 0.
    /// </summary>
    TNode Reduce(int production, ReadOnlySpan<TNode> children);
}
