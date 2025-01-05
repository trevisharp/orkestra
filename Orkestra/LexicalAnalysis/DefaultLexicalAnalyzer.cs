/* Author:  Leonardo Trevisan Silio
 * Date:    03/07/2024
 */
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Orkestra.LexicalAnalysis;

using Processings;
using SyntacticAnalysis;

/// <summary>
/// Default Lexical Analyzer based in System.Text.RegularExpressions.
/// </summary>
public class DefaultLexicalAnalyzer : ILexicalAnalyzer
{
    readonly List<Key> keys = [];
    public IEnumerable<Key> Keys => this.keys;

    public void AddKeys(IEnumerable<Key> keys)
        => this.keys.AddRange(keys);

    public IEnumerable<Token> Parse(Text text)
    {
        var sources = text.ToSources();
        foreach (var source in sources)
        {
            if (source is Token tk)
                yield return tk;
            else if (source is Line line)
            {
                foreach (var token in Parse(line.Text, text.SourceFile, line.Number))
                    yield return token;
            }
        }
    }

    IEnumerable<Token> Parse(string code, string file, int line)
    {
        int startIndex = 0;
        int keyIndex = 0;
        int minToken = int.MaxValue;
        Match crrMatch = null!;
        Key crrKey = null!;
        List<Key> keys = new List<Key>(this.keys);
        
        var matches = GetRegexMatchList(keys, code);
        
        while (keys.Count > 0)
        {
            if (keyIndex == keys.Count)
            {       
                if (crrMatch == null)
                    yield break;
                
                yield return CreateToken(crrKey!, crrMatch, file, line);

                startIndex = crrMatch.Index + crrMatch.Value.Length;
                minToken = int.MaxValue;
                crrMatch = null!;
                crrKey = null!;
                keyIndex = 0;
            }

            var match = GetCurrentMatch(matches[keyIndex], startIndex);
            matches[keyIndex] = match;

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
                crrKey = keys[keyIndex];
            }

            keyIndex++;
        }
    }

    static List<Match> GetRegexMatchList(List<Key> keys, string code)
    {
        List<Match> matches = [];

        for (int j = 0; j < keys.Count; j++)
        {
            var key = keys[j];

            if (key.IsAuto)
            {
                keys.RemoveAt(j);
                j--;
                continue;
            }

            var regex = new Regex(key.Expression!);
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

    static Match GetCurrentMatch(Match match, int startIndex)
    {
        while (match.Index < startIndex && match.Success)
            match = match.NextMatch();
        return match;
    }

    static Token CreateToken(Key key, Match match, string file, int line)
    {           
        Token token = new Token(
            key,
            match.Value,
            file,
            line
        );
        return token;
    }
}