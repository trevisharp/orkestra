/* Author:  Leonardo Trevisan Silio
 * Date:    01/07/2023
 */
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.Globalization;
using System;
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

    static bool hasKeywordHeader(SubRule rule)
    {
        if (rule?.FirstOrDefault() is not Key key)
            return false;

        if (!key.IsKeyword)
            return false;
        
        return true;
    }

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

        return snippet;
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

    /// <summary>
    /// Get the Visual Studio Code Snippet Parameter form:
    /// ${number_of_parameter:default_value|option1,option2,option3|}
    /// </summary>
    public static string GetVSSnippetParameter(this Rule rule)
    {
        int snippetIndex = 0;
        return getSnippetParam(rule, ref snippetIndex);
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
        var headers = getHeaders(rule);
        var completableHeaders = 
            from header in headers
            where isCompletableKey(header)
            select header.Expression;
        var snippet = string.Join(',', completableHeaders);

        return $"${{{++snippetIndex}|{snippet}|}}";
    }

    static bool isCompletableKey(Key key)
    {
        if (key.IsIdentity)
            return false;
        
        if (!key.IsKeyword)
            return false;
        
        var regex = new Regex("[A-Za-z]+");
        var match = regex.Match(key.Expression);
        return match.Length == key.Expression.Length;
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