using System;
using System.IO;
using System.Collections.Generic;

namespace Orkestra;

/// <summary>
/// The full text of one source file. Lexers read it as a <see cref="ReadOnlySpan{T}"/>,
/// so nothing is copied; the line table used for error positions is built only on first use.
/// </summary>
public sealed class SourceText
{
    int[]? lineStarts;

    /// <summary>Creates a source text from a string already in memory.</summary>
    public SourceText(string content, string path = "")
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(path);
        Content = content;
        Path = path;
    }

    /// <summary>The file the text came from, or an empty string for in-memory text.</summary>
    public string Path { get; }

    /// <summary>The whole text.</summary>
    public string Content { get; }

    /// <summary>The number of characters in the text.</summary>
    public int Length => Content.Length;

    /// <summary>The whole text as a span, for lexers.</summary>
    public ReadOnlySpan<char> Span => Content.AsSpan();

    /// <summary>The characters covered by <paramref name="span"/>, without copying.</summary>
    public ReadOnlySpan<char> Slice(TextSpan span) => Content.AsSpan(span.Start, span.Length);

    /// <summary>The characters covered by <paramref name="span"/> as a new string.</summary>
    public string ToString(TextSpan span) => Content.Substring(span.Start, span.Length);

    /// <inheritdoc/>
    public override string ToString() => Content;

    /// <summary>Reads a whole file.</summary>
    public static SourceText FromFile(string path)
        => new(File.ReadAllText(path), path);

    /// <summary>Reads a whole file asynchronously.</summary>
    public static async System.Threading.Tasks.Task<SourceText> FromFileAsync(
        string path, System.Threading.CancellationToken cancellationToken = default)
        => new(await File.ReadAllTextAsync(path, cancellationToken).ConfigureAwait(false), path);

    /// <summary>The number of lines in the text. An empty text has one line.</summary>
    public int LineCount => LineStarts.Length;

    /// <summary>
    /// The zero-based line and column of <paramref name="offset"/>.
    /// Lines end at '\n'; a '\r' before it counts as the last column of its line.
    /// </summary>
    public LinePosition GetLinePosition(int offset)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(offset);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(offset, Length);

        var starts = LineStarts;
        int line = Array.BinarySearch(starts, offset);
        if (line < 0)
            line = ~line - 1;
        return new LinePosition(line, offset - starts[line]);
    }

    /// <summary>The span of line <paramref name="line"/>, without its line break.</summary>
    public TextSpan GetLineSpan(int line)
    {
        var starts = LineStarts;
        ArgumentOutOfRangeException.ThrowIfNegative(line);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(line, starts.Length);

        int start = starts[line];
        int end = line + 1 < starts.Length ? starts[line + 1] - 1 : Length;
        if (end > start && Content[end - 1] == '\r')
            end--;
        return TextSpan.FromBounds(start, end);
    }

    int[] LineStarts => lineStarts ??= ComputeLineStarts(Content);

    static int[] ComputeLineStarts(string content)
    {
        var starts = new List<int> { 0 };
        var span = content.AsSpan();
        int offset = 0;
        int index;
        while ((index = span[offset..].IndexOf('\n')) >= 0)
        {
            offset += index + 1;
            starts.Add(offset);
        }
        return [.. starts];
    }
}
