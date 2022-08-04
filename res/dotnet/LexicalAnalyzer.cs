using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Orkestra;

public class LexicalAnalyzer
{
    public List<Key> Keys { get; private set; } = new List<Key>();
    public void Add(Key key) => this.Keys.Add(key);

    public IEnumerable<Token> Parse(string text)
    {
        Regex regex = null;
        int tokenIndex = 0;
        int i = 0;
        int keyIndex = 0;
        int minToken = int.MaxValue;
        Match crrMatch = null;
        Key crrKey = null;
        List<Key> keys = new List<Key>(this.Keys);
        List<Match> matches = new List<Match>();
        
        while (keys.Count > 0)
        {
            var key = keys[keyIndex];
            regex = new Regex(key.Expression);
            Match match;

            if (keyIndex < matches.Count)
            {
                match = matches[keyIndex]
                    .NextMatch();
            }
            else
            {
                match = regex.Match(text, i);
                matches.Add(match);
            }
            
            if (!match.Success)
            {
                keys.RemoveAt(keyIndex);
                matches.RemoveAt(keyIndex);
                continue;
            }

            if (match.Index < minToken)
            {
                minToken = match.Index;
                crrMatch = match;
                crrKey = key;
            }

            keyIndex++;
            if (keyIndex < keys.Count)
                continue;
            
            if (crrMatch == null)
                yield break;
            
            Token token = new Token(
                crrKey,
                crrMatch.Value,
                tokenIndex
            );
            yield return token;

            tokenIndex++;
            crrKey = null;
            crrMatch = null;
            keyIndex = 0;
            i = match.Index + match.Value.Length;
        }
    }
}