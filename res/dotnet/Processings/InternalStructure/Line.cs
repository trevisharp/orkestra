using Orkestra.LexicalAnalysis;

namespace Orkestra.Processings.InternalStructure;

public struct Line
{
    public int Number { get; set; }
    public string Code { get; set; }
    public bool EndLine { get; set; }
    public Token Token { get; set; }
}