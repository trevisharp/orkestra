namespace Orchestra;

public record Key
{
    public Key(string name, string expression)
    {
        this.Name = name;
        this.Expression = expression;
    }

    public Key(string name, string expression, bool contextual, bool identity)
    {
        Key(name, expression);
        this.IsContextual = contextual;
        this.IsIdentity = identity;
    }

    public string Name { get; init; }
    public string Expression { get; init; }
    public bool IsContextual { get; init; } = false;
    public bool IsIdentity { get; init; } = false;
}