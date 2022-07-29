namespace Orchestra;

public record Key
{
    private Key(string name, string expression, 
        bool contextual, bool identity, bool keyword, bool auto)
    { 
        this.Name = name;
        this.Expression = expression;
        this.IsContextual = contextual;
        this.IsIdentity = identity;
        this.IsKeyword = keyword;
        this.IsAuto = auto;
    }
    
    public static Key CreateKey(string name, string expression)
        => new Key(name, expression, false, false, false, false);
    
    public static Key CreateKeyword(string name, string expression)
        => new Key(name, expression, false, false, true, false);
    
    public static Key CreateIdentity(string name, string expression)
        => new Key(name, expression, false, true, false, false);
      
    public static Key CreateAutoKeyword(string name)
        => new Key(name, null, false, false, true, true);

    public static Key CreateContextual(string name, string expression)
        => new Key(name, expression, true, false, true, false);

    public string Name { get; private set; }
    public string Expression { get; private set; }
    public bool IsContextual { get; private set; }
    public bool IsIdentity { get; private set; }
    public bool IsKeyword { get; private set; }
    public bool IsAuto { get; private set; }
}