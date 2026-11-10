namespace Orkestra;

/// <summary>
/// A range of characters in a <see cref="SourceText"/>, given as a start offset and a length.
/// </summary>
public readonly record struct TextSpan(int Start, int Length)
{
    /// <summary>The offset just past the last character of the span.</summary>
    public int End => Start + Length;

    /// <summary>Creates a span covering <paramref name="start"/> up to (but not including) <paramref name="end"/>.</summary>
    public static TextSpan FromBounds(int start, int end) => new(start, end - start);

    /// <summary>Creates the smallest span covering both <paramref name="first"/> and <paramref name="last"/>.</summary>
    public static TextSpan Cover(TextSpan first, TextSpan last) => FromBounds(first.Start, last.End);

    /// <inheritdoc/>
    public override string ToString() => $"[{Start}..{End})";
}
