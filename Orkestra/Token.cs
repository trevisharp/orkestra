namespace Orkestra;

/// <summary>
/// One token produced by a lexer: a kind and the span of text it covers.
/// The token's text is read back from the <see cref="SourceText"/>, so tokens never allocate.
/// </summary>
/// <remarks>
/// Kinds are numbers assigned by the generated lexer. Two are reserved:
/// <see cref="EndOfFile"/> and <see cref="Error"/>. Generated kinds start at <see cref="FirstUserKind"/>.
/// </remarks>
public readonly record struct Token(int Kind, TextSpan Span)
{
    /// <summary>Kind of the token returned at the end of the input.</summary>
    public const int EndOfFile = 0;

    /// <summary>Kind of a token covering characters that no lexer rule matches.</summary>
    public const int Error = 1;

    /// <summary>The first kind a generated lexer may assign.</summary>
    public const int FirstUserKind = 2;

    /// <summary>True for the end-of-file token.</summary>
    public bool IsEndOfFile => Kind == EndOfFile;
}
