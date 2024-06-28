/* Author:  Leonardo Trevisan Silio
 * Date:    28/06/2023
 */
using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;

namespace Orkestra.Extensions;

public static class SintaxExtension
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

    public static IEnumerable<SubRule> OrderedDeepSubRules<T>(this IEnumerable<Rule> rules)
    {
        var first = rules.GetFirstRule();
        if (first is null)
            yield break;

        var hash = new HashSet<Rule>();
        var queue = new Queue<Rule>();
        queue.Enqueue(first);
        
        while (queue.Count > 0)
        {
            var rule = queue.Dequeue();
            if (hash.Contains(rule))
                continue;
            hash.Add(rule);

            foreach (var subRule in rule)
                yield return subRule;    
        }
    }

    public static Rule GetFirstRule(this IEnumerable<Rule> rules)
    {
        foreach (var rule in rules)
        {
            if (!rule.IsStartRule)
                continue;
            
            return rule;
        }

        return null;
    }

    const string newBody = "\",\n\"\\t";
    public static string GetNormalForm(this Key key, ref int index)
    {
        if (key.IsKeyword)
            return key.Expression;
        
        // close snippet body, jump line and start another snippet body
        if (key.Expression is [ '{' or '}' or '[' or ']' ])
            return key.Expression + newBody;
        
        if (key.Expression is [ '(' or ')' ])
            return key.Expression;
        
        index++;
        return $"${{{index}:{key.Name}}}";
    }

    public static string GetNormalForm(this Rule rule, ref int index, HashSet<Rule> asoSet = null)
    {
        asoSet ??= [];
        if (asoSet.Contains(rule))
            return null;
        asoSet.Add(rule);

        if (rule.SubRules.All(r => r.RuleTokens.Count() == 1))
        {
            var exps = rule.SubRules
                .Select(x => x.RuleTokens.FirstOrDefault())
                .Select(x => x as Key)
                .Select(x => x.Expression);
            index++;
            return $"${{{index}|{string.Join(',', exps)}|}}";
        }
        
        foreach (var subrule in rule.SubRules)
        {
            var oldIndex = index;
            var normalForm = subrule.GetNormalForm(ref index, asoSet);
            if (normalForm is null)
            {
                index = oldIndex;
                continue;
            }
            
            return normalForm;
        }

        index++;
        return $"${{{index}:{rule.Name.ToLower()}}}";
    }

    public static string GetNormalForm(this SubRule rule)
    {
        int index = 0;
        return rule.GetNormalForm(ref index);
    }

    public static string GetNormalForm(this SubRule rule, ref int index, HashSet<Rule> asoSet = null)
    {
        var sb = new StringBuilder();

        foreach (var token in rule.RuleTokens)
        {
            if (sb.Length > 40)
                sb.Append(newBody);

            if (token is Key key)
            {
                sb.Append(key.GetNormalForm(ref index));
                sb.Append(" ");
                continue;
            }

            if (token is Rule rul)
            {
                int oldIndex = index;
                var form = rul.GetNormalForm(ref index, asoSet);
                if (form is null)
                {
                    index = oldIndex;
                    return null;
                }
                sb.Append(form);
                sb.Append(" ");
                continue;
            }
        }

        return sb.ToString()
            .Replace("  ", " ")
            .Trim();
    }

    public static int GetComplexity(this SubRule rule)
    {
        var hash = new HashSet<SubRule>();
        var queue = new Queue<SubRule>();
        queue.Enqueue(rule);

        while (queue.Count > 0)
        {
            var item = queue.Dequeue();
            if (hash.Contains(item))
                continue;
            hash.Add(item);

            var subRules = (
                from tk in item.RuleTokens
                where tk is Rule
                select tk as Rule
            ).SelectMany(r => r.SubRules);
            
            foreach (var sb in subRules)
                queue.Enqueue(sb);
        }

        return hash.Count;
    }

    public static IEnumerable<SubRule> GetFirstSet(this IEnumerable<Rule> rules)
    {
        var queue = new Queue<SubRule>();
        var hash = new HashSet<SubRule>();
        var parentHash = new HashSet<Rule>();

        var first = rules
            .FirstOrDefault(r => r.IsStartRule);
        foreach (var sb in first.SubRules)
            queue.Enqueue(sb);
        
        while (queue.Count > 0)
        {
            var rule = queue.Dequeue();
            if (hash.Contains(rule))
                continue;
            hash.Add(rule);

            var header = rule.RuleTokens
                .FirstOrDefault();
            if (header is null)
                continue;
            
            if (parentHash.Contains(rule.Parent))
                continue;
            
            if (header is Key key && key.IsKeyword)
            {
                parentHash.Add(rule.Parent);
                yield return rule;
                continue;
            }
            
            foreach (var token in rule.RuleTokens)
            {
                if (token is Rule ruleToken)
                {  
                    var subRules = ruleToken.SubRules
                        .OrderByDescending(r => r.RuleTokens.Count());
                    foreach (var sb in subRules)
                        queue.Enqueue(sb);
                }
            }
        }
    }
}