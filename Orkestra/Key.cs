/* Author:  Leonardo Trevisan Silio
 * Date:    28/06/2024
 */
using System.Linq;

namespace Orkestra;

/// <summary>
/// Represents a Key for syntactical analisys.
/// </summary>
public class Key(
    string? name, string? expression, bool contextual, 
    bool identity, bool keyword, bool auto) : ISyntacticElement
{
    public string? Name { get; set; } = name;
    public string? Expression { get; init; } = expression;
    public bool IsContextual { get; init; } = contextual;
    public bool IsIdentity { get; init; } = identity;
    public bool IsKeyword { get; init; } = keyword;
    public bool IsAuto { get; init; } = auto;

    public override string ToString()
        => $"K:{Name ?? "unnamed"}";
    
    public static Key CreateKey(string name, string expression)
        => new(name, expression, false, false, false, false);
    
    public static Key CreateKeyword(string name, string expression)
        => new(name, expression, false, false, true, false);
    
    public static Key CreateIdentity(string name, string expression)
        => new(name, expression, false, true, false, false);
      
    public static Key CreateAutoKeyword(string name)
        => new(name, null, false, false, true, true);

    public static Key CreateContextual(string name, string expression)
        => new(name, expression, true, false, true, false);
    
    public static Key CreateKey(string expression)
        => new(null, expression, false, false, false, false);
    
    public static Key CreateKeyword(string expression)
        => new(null, expression, false, false, true, false);
    
    public static Key CreateIdentity(string expression)
        => new(null, expression, false, true, false, false);
    
    public static Key CreateAutoKeyword()
        => new(null, null, false, false, true, true);

    public static Key CreateContextual(string expression)
        => new(null, expression, true, false, true, false);
    
    public static implicit operator Key(string expression)
    {
        bool isKeyword = expression.All(char.IsAsciiLetter);
        return new(null, expression, false, false, isKeyword, false);
    }

    public static implicit operator Rule(Key key)
        => [ [ key] ];

    public static Rule operator +(Key key, Key rule)
        => [ [ key, rule ] ];
    
    public static Rule operator +(Key key, Rule rule)
        => [ [ key, rule ] ];
    
    public static Rule operator +(Key key, IntermediaryOrRule irule)
        => [ [ key, (Rule)irule ] ];

    public static IntermediaryOrRule operator |(Key k1, Key k2)
        => new IntermediaryOrRule() | k1 | k2;

    public static IntermediaryOrRule operator |(Rule r, Key k)
        => new IntermediaryOrRule() | r | k;

    public static IntermediaryOrRule operator |(Key k, Rule r)
        => new IntermediaryOrRule() | k | r;
}