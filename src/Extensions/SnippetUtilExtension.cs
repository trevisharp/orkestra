/* Author:  Leonardo Trevisan Silio
 * Date:    01/07/2023
 */
using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Orkestra.Extensions;

public static class SnippetUtilExtension
{
    /// <summary>
    /// Returns the dictionary of all Subrules of a language that start with a keyword.
    /// </summary>
    public static IEnumerable<IGrouping<string, SubRule>> GetHeaders(this LanguageInfo language)
    {
        if (language is null)
            return [];
        
        var rules = language.Rules;
        var keyGroups = 
            from subRule in rules.SelectMany(r => r)
            where hasKeywordHeader(subRule)
            group subRule by subRule.FirstOrDefault().Name;
        
        return keyGroups;
    }

    /// <summary>
    /// Convert SubRule to a Visual Studio Code Snippet
    /// </summary>
    public static string GetVSSnippetForm(this SubRule rule)
    {
        string snippet = "";
        int snippetIndex = 0;

        foreach (var element in rule)
        {
            if (element is Key key)
            {
                snippet += getSnippetParam(key, ref snippetIndex);
                snippet += " ";
                continue;
            }

            if (element is Rule subRule)
            {
                snippet += getSnippetParam(subRule, ref snippetIndex);
                snippet += " ";
                continue;
            }
        }

        return snippet.Trim();
    }

    /// <summary>
    /// Get the Visual Studio Code Snippet Parameter form:
    /// ${number_of_parameter:default_value|option1,option2,option3|}
    /// </summary>
    public static string GetVSSnippetParameter(this Key key)
    {
        int snippetIndex = 0;
        return getSnippetParam(key, ref snippetIndex);
    }

    public static string GetVSSnippetParameter(this IEnumerable<Key> keys)
    {
        var completableKeys = 
            from key in keys
            where IsCompletableKey(key)
            select key.Expression;
        var snippet = string.Join(',', completableKeys);
        return $"${{1|{snippet}|}}";
    }

    /// <summary>
    /// Get the Visual Studio Code Snippet Parameter form:
    /// ${number_of_parameter:default_value|option1,option2,option3|}
    /// </summary>
    public static string GetVSSnippetParameter(this Rule rule)
    {
        int snippetIndex = 0;
        return getSnippetParam(rule, ref snippetIndex);
    }
    
    public static IEnumerable<Key> GetHeaders(this IEnumerable<ISyntacticElement> elements)
        => elements?
            .SelectMany(el => el switch
            {
                Key key => [ key ],
                Rule rule => getHeaders(rule),
                _ => []
            }) ?? [];

    /// <summary>
    /// Return if the key is a non-variable alphanumeric keyword.
    /// </summary>
    public static bool IsCompletableKey(this Key key)
    {
        if (key.IsIdentity)
            return false;
        
        if (!key.IsKeyword)
            return false;
        
        var regex = new Regex("[A-Za-z0-9]+");
        var match = regex.Match(key.Expression);
        return match.Length == key.Expression.Length;
    }

    /// <summary>
    /// Return if a key is not the header of a complex rule.
    /// </summary>
    public static bool IsSimple(this Key key, IEnumerable<Rule> rules)
    {
        foreach (var rule in rules.SelectMany(r => r))
        {
            if (rule.FirstOrDefault() != key)
                continue;
            
            if (rule.Count() == 1)
                continue;
            
            if (!rule.Any(t => t is Rule))
                continue;
            
            return false;
        }

        return true;
    }

    static bool hasKeywordHeader(SubRule rule)
    {
        if (rule?.FirstOrDefault() is not Key key)
            return false;

        if (!key.IsKeyword)
            return false;
        
        return true;
    }

    static string getSnippetParam(Key key, ref int snippetIndex)
    {
        if (key.IsIdentity)
            return $"${{{++snippetIndex}:id}}";

        if (!key.IsKeyword)
            return $"${{{++snippetIndex}:{key?.Name?.ToLower() ?? "value"}}}";
        
        return key.Expression;
    }

    static string getSnippetParam(Rule rule, ref int snippetIndex)
    {
        if (rule is null)
            return null;
        
        var headers = getHeaders(rule);
        int headerCount = headers.Count();

        var completableHeaders = 
            from header in headers
            where IsCompletableKey(header)
            select header.Expression;
        int completableCount = completableHeaders.Count();

        if (completableCount == 0)
            return $"${{{++snippetIndex}:{rule.Name?.ToLower() ?? "value"}}}";

        bool isWeakCompletable = completableCount <= headerCount / 2;
        if (isWeakCompletable)
            return $"${{{++snippetIndex}:{rule.Name?.ToLower() ?? "value"}}}";

        var snippet = string.Join(',', completableHeaders);
        return $"${{{++snippetIndex}|{snippet}|}}";
    }

    static IEnumerable<Key> getHeaders(Rule rule)
    {
        var queue = new Queue<Rule>();
        var hash = new HashSet<Rule>();
        queue.Enqueue(rule);

        while (queue.Count() > 0)
        {
            var crr = queue.Dequeue();
            if (hash.Contains(crr))
                continue;
            hash.Add(crr);

            foreach (var sr in crr)
            {
                var header = sr.FirstOrDefault();
                if (header is null)
                    continue;
                
                if (header is Key key)
                    yield return key;
                
                if (header is Rule inner)
                    queue.Enqueue(inner);
            }
        }
    }
}