/* Author:  Leonardo Trevisan Silio
 * Date:    27/06/2023
 */
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orkestra.Extensions.VSCode;

public class SnippetContribute(LanguageInfo info) : VSCodeContribute
{
    public override VSCodeContributeType Type => VSCodeContributeType.Snippets;

    public override string Declaration =>
        $$"""
        {
            "language": "{{info.Name}}",
            "path": "./{{info.Name}}-snippets.json"
        }
        """;

    public override string Documentation => 
        """

        """;

    public override async Task GenerateFile(string dir)
    {
        var sw = new StreamWriter($"{dir}/{info.Name}-snippets.json");
        
        bool first = true;
        foreach (var fst in getFirstSet())
        {
            var header = fst.RuleTokens.FirstOrDefault() as Key;
            if (header is null)
                continue;
            var headerExp = header.Expression;
            var normalForm = fst.GetNormalForm();

            await sw.WriteAsync(
                $$"""
                {{(first ? "{" : ",")}}
                    "{{headerExp}}" : {
                        "prefix": "{{headerExp}}",
                        "body": [
                            "{{normalForm}}"
                        ],
                        "descrition": "{{headerExp}} snippet."
                    }
                """
            );
            first = false;
        }

        await sw.WriteAsync("}");
        sw.Close();
    }

    IEnumerable<SubRule> getFirstSet()
    {
        var queue = new Queue<SubRule>();
        var hash = new HashSet<SubRule>();
        var parentHash = new HashSet<Rule>();

        var first = info.Rules
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
                    foreach (var sb in ruleToken.SubRules)
                        queue.Enqueue(sb);
            }
        }
    }
}