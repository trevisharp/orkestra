namespace Orkestra.Processings.InternalStructure;

internal struct CodeCharacter
{
    internal char Value { get; set; }
    internal int Line { get; set; }
    internal int NextLineIndex { get; set;}
}