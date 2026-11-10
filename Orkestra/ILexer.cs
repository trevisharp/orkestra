namespace Orkestra;

/// <summary>
/// A source of tokens. Generated lexers implement it as a <c>ref struct</c>, and
/// <see cref="LRParser"/> takes it as a generic parameter, so each call is resolved
/// at compile time with no interface dispatch.
/// </summary>
public interface ILexer
{
    /// <summary>
    /// Returns the next token. After the input runs out it returns a
    /// <see cref="Token.EndOfFile"/> token, and keeps returning it on every later call.
    /// </summary>
    Token Next();
}
