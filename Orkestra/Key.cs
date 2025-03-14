/* Author:  Leonardo Trevisan Silio
 * Date:    05/03/2025
 */
using System.Linq;

namespace Orkestra;

using Expressions;

/// <summary>
/// Represents a Key for syntactical analisys.
/// </summary>
public class Key(
    string? name, string? expression, 
    bool identity, bool keyword, bool auto) : ISyntacticElement
{
    public string? Name { get; set; } = name;
    public string? Expression { get; init; } = expression;
    public bool IsIdentity { get; init; } = identity;
    public bool IsKeyword { get; init; } = keyword;
    public bool IsAuto { get; init; } = auto;

    public override string ToString()
        => $"K:{Name ?? "unnamed"}";
    
    public static Key CreateKey(string name, string expression)
        => new(name, expression, false, false, false);
    
    public static Key CreateKeyword(string name, string expression)
        => new(name, expression, false, true, false);
    
    public static Key CreateIdentity(string name, string expression)
        => new(name, expression, true, false, false);
      
    public static Key CreateAutoKeyword(string name)
        => new(name, null, false, true, true);
    
    public static Key CreateKey(string expression)
        => new(null, expression, false, false, false);
    
    public static Key CreateKeyword(string expression)
        => new(null, expression, false, true, false);
    
    public static Key CreateIdentity(string expression)
        => new(null, expression, true, false, false);
    
    public static Key CreateAutoKeyword()
        => new(null, null, false, true, true);
    
    public static implicit operator Key(string expression)
    {
        bool isKeyword = expression.All(char.IsAsciiLetter);
        return new(null, expression, false, isKeyword, false);
    }
    
    public static ExpressionNode operator |(Key key1, Key key2)
        => (ExpressionNode)key1 | (ExpressionNode)key2;

    public static ExpressionNode operator +(Key key1, Key key2)
        => (ExpressionNode)key1 + (ExpressionNode)key2;

    public static ExpressionNode operator ~(Key key)
        => ~(ExpressionNode)key;
    
    public static Rule operator +(Key key)
    {
        Rule repeatRule = [ [ key ] ];
        repeatRule.Add([ key, repeatRule ]);
        return repeatRule;
    }
}