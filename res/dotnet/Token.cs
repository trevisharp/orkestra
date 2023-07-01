﻿/* Author:  Leonardo Trevisan Silio
 * Date:    30/06/2023
 */
namespace Orkestra;

using SyntacticAnalysis;

/// <summary>
/// Represents a Token data output from lexical analisys.
/// </summary>
public record Token : INode
{
    public Token(Key key, string value, int index)
    {
        this.Key = key;
        this.Value = value;
        this.Index = index;
    }

    public Key Key { get; init; }
    public IRuleElement Element => Key;
    public string Value { get; init; }
    public int Index { get; init; }

    public bool Is(IRuleElement token)
        => token is Key key && key == this.Key;

    public override string ToString()
        => $"T:{Key.Name}";
}