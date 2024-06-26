/* Author:  Leonardo Trevisan Silio
 * Date:    23/06/2023
 */
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Orkestra.Extensions.VSCode;

/// <summary>
/// A Grammar Contribute for a VSCode Extension
/// </summary>
public class GrammarContribute(LanguageInfo info) : VSCodeContribute
{

    public override VSCodeContributeType Type 
        => VSCodeContributeType.Grammars;

    public override string Declaration => 
        $$"""
        {
            "language": "{{info.Name}}",
            "scopeName": "source{{info.Extension}}",
            "path" : "./{{info.Name}}.tmLanguage.json"
        }
        """;

    public override string Documentation
    {
        get
        {
            return "";
        }
    }

    public override async Task GenerateFile(string dir)
    {
        const string schema = "https://raw.githubusercontent.com/martinring/tmlanguage/master/tmlanguage.json";
        var sw = new StreamWriter($"{dir}/{info.Name}.tmLanguage.json");

        string nums = "";
        string ids = "";

        var groupKeys = new List<Key>();

        string append(string regex, string exp)
        {
            if (regex is null || regex == string.Empty)
                return exp;
            
            return regex + "|" + exp;
        }

        foreach (var key in info.Keys)
        {
            if (key.IsAuto)
                continue;
            
            if (key.IsIdentity) {
                ids = append(ids, key.Expression);
                continue;
            }
            
            if (!key.IsKeyword && key.Expression.Contains('0')) {
                nums = append(nums, key.Expression);
                continue;
            }

            if (!key.Expression.All(c => c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z'))
                continue;

            groupKeys.Add(key);
        }

        string keywords = "";
        string controls = "";
        string operations = "";
        string definitions = "";

        var start = info.Rules
            .FirstOrDefault(rule => rule.IsStartRule);
        (var brothers, var others) = getContextInfo(start, groupKeys);

        if (brothers.Count > 0)
        {
            keywords = string.Join('|',
                from key in brothers[0].list
                select key.Expression
            );
        }

        if (others.Count > 0)
        {
            keywords += '|' + string.Join('|',
                from key in others
                select key.Expression
            );
        }

        var extraContextBrothers = brothers[1..];
        if (extraContextBrothers.Count == 0)
        {
            await write();
            return;
        }

        var problabyTypes = extraContextBrothers
            .MaxBy(brothers => brothers.list.Count);
        definitions = string.Join('|',
            from key in problabyTypes.list
            select key.Expression
        );

        extraContextBrothers = extraContextBrothers
            .Where(brothers => brothers != problabyTypes)
            .ToList();
        if (extraContextBrothers.Count == 0)
        {
            await write();
            return;
        }
        
        var problabyControl = extraContextBrothers
            .FirstOrDefault(brothers => brothers.list
                .Count(key => 
                    key.Expression.Contains("for") ||
                    key.Expression.Contains("while") ||
                    key.Expression.Contains("if")
                ) > 0
            );
        if (problabyControl.list is not null)
        {
            controls = string.Join('|',
                from key in problabyTypes.list
                select key.Expression
            );
        }

        extraContextBrothers = extraContextBrothers
            .Where(brothers => brothers != problabyControl)
            .ToList();
        if (extraContextBrothers.Count == 0)
        {
            await write();
            return;
        }

        while (extraContextBrothers.Count > 0)
        {
            var bgroup = extraContextBrothers[0];
            extraContextBrothers.RemoveAt(0);

            switch (bgroup.type)
            {
                case 0:
                    keywords += (keywords.Length == 0 ? "" : "|") + 
                        string.Join('|',
                            from key in bgroup.list
                            select key.Expression
                        );
                    break;
                    
                case 1:
                    operations += (operations.Length == 0 ? "" : "|") + 
                        string.Join('|',
                            from key in bgroup.list
                            select key.Expression
                        );
                    break;
                    
                case 2:
                    controls += (controls.Length == 0 ? "" : "|") + 
                        string.Join('|',
                            from key in bgroup.list
                            select key.Expression
                        );
                    break;
            }
        }

        await write();

        async Task write()
        {
            await sw.WriteAsync(
                $$"""
                {
                    "$schema": "{{schema}}",
                    "name": "{{info.Name}}",
                    "patterns": [
                        { "include": "#keywords" },
                        { "include": "#constants" },
                        { "include": "#variables" },
                        { "include": "#entitys" }
                    ],
                    "repository": {
                        "keywords": {
                            "patterns": [
                                {
                                    "name": "keyword.{{info.Name}}",
                                    "match": "\\b({{keywords}})\\b"
                                },
                                {
                                    "name": "keyword.control.{{info.Name}}",
                                    "match": "\\b({{controls}})\\b"
                                }
                            ]
                        },
                        
                        "entitys": {
                            "patterns": [
                                {
                                    "name": "entity.name.function.{{info.Name}}",
                                    "match": "\\b({{operations}})\\b"
                                },
                                {
                                    "name": "entity.name.class.{{info.Name}}",
                                    "match": "\\b({{definitions}})\\b"
                                }
                            ]
                        },

                        "constants": {
                            "patterns": [
                                {
                                    "name": "constant.numeric.{{info.Name}}",
                                    "match": "\\b({{nums}})\\b"
                                }
                            ]
                        },

                        "variables": {
                            "patterns": [
                                {
                                    "name": "variable.parameter.{{info.Name}}",
                                    "match": "\\b({{ids}})\\b"
                                }
                            ]
                        }
                    },
                    "scopeName": "source{{info.Extension}}"
                }
                """
            );

            sw.Close();
        }
    }

    (List<(List<Key> list, int type)> brothers, List<Key> others) getContextInfo(Rule start, List<Key> allKeys)
    {
        var brothers = new List<(List<Key> list, int type)>();
        var others = new List<Key>();

        var queue = new Queue<Rule>();
        queue.Enqueue(start);

        var set = new HashSet<Rule>();
        var keySet = new HashSet<Key>();

        while (queue.Count > 0)
        {
            var rule = queue.Dequeue();

            if (set.Contains(rule))
                continue;
            set.Add(rule);

            var tokens = rule.SubRules
                .SelectMany(r => r.RuleTokens);
            
            int operationIndex = rule.SubRules
                .Count(r => r.RuleTokens.Skip(1).FirstOrDefault() is Key);
            int controlIndex = rule.SubRules
                .Count(r => r.RuleTokens.FirstOrDefault() is Key);
            int otherIndex = rule.SubRules.Count() - operationIndex - controlIndex;
            int max = int.Max(otherIndex, int.Max(operationIndex, controlIndex));
            int type = 0;
            if (operationIndex == max)
                type = 1;
            if (controlIndex == max)
                type = 2;

            List<Key> keys = [];
            foreach (var token in tokens)
            {
                switch (token)
                {
                    case Key k:
                        if (keySet.Contains(k))
                            continue;
                        if (!allKeys.Contains(k))
                            continue;
                        keySet.Add(k);
                        keys.Add(k);
                        break;
                    
                    case Rule r:
                        queue.Enqueue(r);
                        break;
                }
            }
            brothers.Add((keys, type));            
        }

        foreach (var key in allKeys)
        {
            if (keySet.Contains(key))
                continue;
            
            others.Add(key);
        }

        return (brothers, others);
    }
}