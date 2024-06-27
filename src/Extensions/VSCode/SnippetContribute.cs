using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        foreach (var sb in getFirstSet())
        {
            Verbose.Warning(sb);
        }

        await sw.WriteAsync(
            $$"""
            {
                "Define": {
                    "prefix": "define",
                    "body": [
                        "define ${1:setname} as ${2:subset of nat}",
                    ],
                    "description": "Define Snippet."
                }
            }
            """
        );

        sw.Close();
    }

    List<SubRule> getFirstSet()
    {
        var list = new List<SubRule>();
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
                list.Add(rule);
                continue;
            }
            
            foreach (var token in rule.RuleTokens)
            {
                if (token is Rule ruleToken)
                    foreach (var sb in ruleToken.SubRules)
                        queue.Enqueue(sb);
            }
        }

        return list;
    }
}