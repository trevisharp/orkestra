using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Orkestra;

public class LexicalAnalyzer
{
    public List<Key> Keys { get; private set; } = new List<Key>();
    public void Add(Key key) => this.Keys.Add(key);

    public IEnumerable<Token> Parse(string text)
    {
        yield break;
    }
}