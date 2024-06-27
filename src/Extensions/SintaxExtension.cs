/* Author:  Leonardo Trevisan Silio
 * Date:    27/06/2023
 */
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Data;

namespace Orkestra.Extensions;

public static class SintaxExtension
{
    public static string GetNormalForm(this Key key, ref int index)
    {
        if (key.IsKeyword)
            return key.Expression;
        
        if (key.Expression is [ '{' or '}' or '[' or ']' ])
            return key.Expression + "\n";
        
        index++;
        return $"${{{index}:{key.Name}}}";
    }

    public static string GetNormalForm(this SubRule rule)
    {
        int index = 0;
        return rule.GetNormalForm(ref index);
    }

    public static string GetNormalForm(this SubRule rule, ref int index)
    {
        var sb = new StringBuilder();

        foreach (var token in rule.RuleTokens)
        {
            if (token is Key key)
            {
                sb.Append(key.GetNormalForm(ref index));
                sb.Append(" ");
                continue;
            }

            if (token is Rule rul)
            {
                if (rul.SubRules.All(r => r.RuleTokens.Count() == 1))
                {
                    
                }

                var srul = rul.SubRules
                    .FirstOrDefault();
                if (srul is null)
                    continue;

                sb.Append(srul.GetNormalForm(ref index));
                continue;
            }
        }

        return sb.ToString().Trim();
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
}