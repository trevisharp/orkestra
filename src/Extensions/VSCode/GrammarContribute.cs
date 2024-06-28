/* Author:  Leonardo Trevisan Silio
 * Date:    25/06/2023
 */
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Orkestra.Extensions.VSCode;

using Processings.Implementations;

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

        var lineComment = info.Processings
            .FirstOrDefault(p => p is LineCommentProcessing)
            as LineCommentProcessing;

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
        (var brothers, var headers, var others) = getContextInfo(start, groupKeys);

        keywords = string.Join('|',
            from key in headers
            select key.Expression
        );

        if (others.Count > 0)
        {
            keywords += '|' + string.Join('|',
                from key in others
                select key.Expression
            );
        }
        
        if (brothers.Count == 0)
        {
            await write();
            return;
        }

        var problabyTypes = brothers
            .MaxBy(brothers => brothers.Count);
        definitions = string.Join('|',
            from key in problabyTypes
            select key.Expression
        );

        brothers = brothers
            .Where(brothers => brothers != problabyTypes)
            .ToList();
        if (brothers.Count == 0)
        {
            await write();
            return;
        }
        
        var problabyControl = brothers
            .FirstOrDefault(brothers => brothers
                .Any(key =>
                    key.Expression.Contains("for") ||
                    key.Expression.Contains("while") ||
                    key.Expression.Contains("if")
                )
            );
        if (problabyControl is not null)
        {
            controls = string.Join('|',
                from key in problabyTypes
                select key.Expression
            );
        }

        brothers = brothers
            .Where(brothers => brothers != problabyControl)
            .ToList();
        if (brothers.Count == 0)
        {
            await write();
            return;
        }

        int groupId = 1;
        while (brothers.Count > 0)
        {
            var bgroup = brothers[0];
            brothers.RemoveAt(0);

            switch (groupId % 3)
            {
                case 0:
                    keywords += (keywords.Length == 0 ? "" : "|") + 
                        string.Join('|',
                            from key in bgroup
                            select key.Expression
                        );
                    break;
                    
                case 1:
                    operations += (operations.Length == 0 ? "" : "|") + 
                        string.Join('|',
                            from key in bgroup
                            select key.Expression
                        );
                    break;
                    
                case 2:
                    controls += (controls.Length == 0 ? "" : "|") + 
                        string.Join('|',
                            from key in bgroup
                            select key.Expression
                        );
                    break;
            }
            groupId++;
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
                        { "include": "#entitys" },
                        { "include": "#constants" },
                        { "include": "#variables" }{{(
                            lineComment is null ? string.Empty :
                            ",\n\t\t{ \"include\": \"#comments\" }"
                        )}}
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
                        }{{(
                            lineComment is null ? string.Empty :
                            $$"""
                            ,

                                    "comments": {
                                        "patterns": [
                                            {
                                                "name": "comment.line.{{info.Name}}",
                                                "match": "{{lineComment.CommentStarter}}.*$"
                                            }
                                        ]
                                    }
                            """
                        )}}
                    },
                    "scopeName": "source{{info.Extension}}"
                }
                """
            );

            sw.Close();
        }
    }

    (List<List<Key>> brothers, List<Key> headers, List<Key> others) getContextInfo(Rule start, List<Key> allKeys)
    {
        var brothers = new List<List<Key>>();
        var headers = new List<Key>();
        var others = new List<Key>();

        var queue = new Queue<Rule>();
        var headerQueue = new Queue<bool>();
        queue.Enqueue(start);
        headerQueue.Enqueue(true);

        var set = new HashSet<Rule>();
        var keySet = new HashSet<Key>();

        while (queue.Count > 0)
        {
            var rule = queue.Dequeue();
            var canHeader = headerQueue.Dequeue();

            if (set.Contains(rule))
                continue;
            set.Add(rule);

            var tokens = rule.SubRules
                .SelectMany(r => r.RuleTokens);

            List<Key> keys = [];
            List<Rule> rules = [];
            foreach (var token in tokens)
            {
                if (token is Key k)
                {
                    if (keySet.Contains(k))
                        continue;
                    
                    if (!allKeys.Contains(k))
                        continue;
                    
                    keySet.Add(k);

                    if (canHeader)
                    {
                        headers.Add(k);
                        canHeader = false;
                        continue;
                    }
                    
                    keys.Add(k);
                    continue;
                }

                if (token is Rule r)
                    rules.Add(r);
            }

            foreach (var r in rules)
            {
                queue.Enqueue(r);
                headerQueue.Enqueue(canHeader);
            }

            if (keys.Count < 2)
            {
                others.AddRange(keys);
                continue;
            }
            brothers.Add(keys);            
        }

        foreach (var key in allKeys)
        {
            if (keySet.Contains(key))
                continue;
            
            others.Add(key);
        }

        return (brothers, headers, others);
    }
}