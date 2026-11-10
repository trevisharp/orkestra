namespace Orkestra;

/// <summary>
/// A zero-based line and column in a <see cref="SourceText"/>.
/// <see cref="ToString"/> prints them one-based, as editors show them.
/// </summary>
public readonly record struct LinePosition(int Line, int Column)
{
    /// <inheritdoc/>
    public override string ToString() => $"{Line + 1},{Column + 1}";
}
