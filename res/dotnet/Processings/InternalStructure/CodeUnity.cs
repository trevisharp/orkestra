namespace Orkestra.Processings.InternalStructure;

using LexicalAnalysis;

internal struct CodeUnity
{
    internal char? Value { get; set; }
    internal int SourceLine { get; set; }
    internal Token TokenUnity { get; set; }

    public static bool operator ==(CodeUnity c1, CodeUnity c2) =>
        c1.SourceLine == c2.SourceLine &&
        c1.TokenUnity == c2.TokenUnity &&
        c1.Value == c2.Value;

    public static bool operator !=(CodeUnity c1, CodeUnity c2) =>
        c1.SourceLine != c2.SourceLine ||
        c1.TokenUnity != c2.TokenUnity ||
        c1.Value != c2.Value;

}