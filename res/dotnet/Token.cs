namespace Orchestra;

public record Token
{
    public string Value { get; init; }
    public int Index { get; init; }
}
