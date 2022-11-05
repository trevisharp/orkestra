namespace Orkestra.Processings.InternalStructure;

using LexicalAnalysis;

internal struct CodeUnity
{
    internal char Value { get; set; }
    internal int SourceLine { get; set; }
    internal Token TokenUnity { get; set; }
}