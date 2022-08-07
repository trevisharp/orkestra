using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Orkestra;

public class LexicalAnalyzer
{
    public List<Key> Keys { get; private set; } = new List<Key>();
    public void Add(Key key) => this.Keys.Add(key);

    public IEnumerable<Token> Parse(string code)
    {
        int startIndex = 0;
        int keyIndex = 0;
        int minToken = int.MaxValue;
        int tokenCount = 0;
        Match crrMatch = null;
        Key crrKey = null;
        List<Key> keys = new List<Key>(this.Keys);
        
        var matches = getRegexMatchList(keys, code);
        
        while (keys.Count > 0)
        {
            var match = getCurrentMatch(matches[keyIndex], startIndex);
            matches[keyIndex] = match;

            if (!match.Success)
            {
                keys.RemoveAt(keyIndex);
                matches.RemoveAt(keyIndex);

                if (keyIndex >= keys.Count)
                    yield break;

                continue;
            }

            if (match.Index < minToken)
            {
                minToken = match.Index;
                crrMatch = match;
                crrKey = keys[keyIndex];
            }

            keyIndex++;
            if (keyIndex < keys.Count)
                continue;
            
            if (crrMatch == null)
                yield break;
            
            yield return createToken(crrKey, crrMatch, tokenCount++);

            startIndex = crrMatch.Index + crrMatch.Value.Length;
            minToken = int.MaxValue;
            crrMatch = null;
            crrKey = null;
            keyIndex = 0;
        }
    }

    private List<Match> getRegexMatchList(List<Key> keys, string code)
    {
        List<Match> matches = new List<Match>();

        for (int j = 0; j < keys.Count; j++)
        {
            var key = keys[j];
            var regex = new Regex(key.Expression);
            var match = regex.Match(code);
            
            if (!match.Success)
            {
                keys.RemoveAt(j);
                j--;
                continue;
            }
            
            matches.Add(match);
        }

        return matches;
    }

    private Match getCurrentMatch(Match match, int startIndex)
    {
        while (match.Index < startIndex && match.Success)
            match = match.NextMatch();
        return match;
    }

    private Token createToken(Key key, Match match, int count)
    {           
        Token token = new Token(
            key,
            match.Value,
            count
        );
        return token;
    }
}