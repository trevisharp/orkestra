/* Author:  Leonardo Trevisan Silio
 * Date:    02/05/2024
 */
namespace Orkestra.SyntacticAnalysis;

/// <summary>
/// Represents a Token data output from lexical analisys.
/// </summary>
public record Token(Key Key, string? Value, string File, int Line)
{
    public bool Is(ISyntacticElement token)
        => token is Key key && key == this.Key;

    public override string ToString()
        => $"T:{Key.Name}";
}