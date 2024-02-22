/* Author:  Leonardo Trevisan Silio
 * Date:    22/02/2024
 */
namespace Orkestra.SyntacticAnalysis;

/// <summary>
/// Represents a Token data output from lexical analisys.
/// </summary>
public record Token(Key Key, string Value, int Index) : INode
{
    public IRuleElement Element => Key;

    public bool Is(IRuleElement token)
        => token is Key key && key == this.Key;

    public override string ToString()
        => $"T:{Key.Name}";
}