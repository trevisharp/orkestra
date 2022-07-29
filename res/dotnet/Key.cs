namespace Orchestra;

public record Key
{
    public Key(string name, string expression)
    {
        this.Name = name;
        this.Expression = expression;
    }

    public Key(string name, string expression, bool keyword)
    {
        Key(name, expression);
        this.IsKeyword = keyword;
    }
    public Key(string name, string expression, bool keyword, bool auto)
    {
        Key(name, expression);
        this.IsKeyword = keyword;
        this.IsAuto = auto;
    }

    public Key(string name, string expression, 
        bool keyword, bool auto, bool contextual, bool identity)
    {
        Key(name, expression, keyword, auto);
        this.IsContextual = contextual;
        this.IsIdentity = identity;
    }

    public string Name { get; init; }
    public string Expression { get; init; }
    public bool IsContextual { get; init; } = false;
    public bool IsIdentity { get; init; } = false;
    public bool IsKeyword { get; init; } = false;
    public bool IsAuto { get; init; } = false;
}