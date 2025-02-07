/* Author:  Leonardo Trevisan Silio
 * Date:    25/06/2023
 */
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Orkestra.Extensions.VSCode;

using System.Text;
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

        var start = info.StartRule;
        (var brothers, var headers, var others) = GetContextInfo(start, groupKeys);

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
            var includes = new StringBuilder();
            var repositories = new StringBuilder();
            var patterns = new StringBuilder();

            void addInclude(string include)
            {
                if (includes.Length > 0)
                    includes.Append(",\n");
                
                includes.Append($"\t\t{{ \"include\": \"#{include}\" }}");
            }

            void openRepo(string title)
            {
                if (repositories.Length > 0)
                    repositories.Append(",\n");
                
                repositories.Append(
                    $$"""
                            "{{title}}": {
                                "patterns": [
                    
                    """
                );
            }

            void addRepoPattern(string name, string match)
            {
                if (match is null || match.Length == 0)
                    return;
                
                if (patterns.Length > 0)
                    patterns.Append(",\n");
                
                patterns.Append(
                    $$"""
                                    {
                                        "name": "{{name}}{{info.Extension}}",
                                        "match": "\\b({{match}})\\b"
                                    }
                    """
                );
            }

            void closeRepo()
            {
                repositories.Append($"{patterns}\n\t\t\t]\n\t\t}}");
                patterns.Clear();
            }

            if (keywords.Length > 0 || controls.Length > 0)
            {
                addInclude("keywords");
                openRepo("keywords");
                
                addRepoPattern("keyword", keywords);

                addRepoPattern("keyword.control", controls);

                closeRepo();
            }

            if (operations.Length > 0 || definitions.Length > 0)
            {
                addInclude("entitys");
                openRepo("entitys");

                addRepoPattern("entity.name.function", operations);

                addRepoPattern("entity.name.class", definitions);

                closeRepo();
            }

            if (nums.Length > 0)
            {
                addInclude("constants");
                openRepo("constants");

                addRepoPattern("constant.numeric", operations);

                closeRepo();
            }

            if (ids.Length > 0)
            {
                addInclude("variables");
                openRepo("variables");

                addRepoPattern("variables.parameter", ids);

                closeRepo();
            }

            if (lineComment is not null)
            {
                addInclude("comments");
                openRepo("comments");
                
                patterns.Append(
                    $$"""
                                    {
                                        "name": "comment.line{{info.Extension}}",
                                        "match": "{{lineComment.CommentStarter}}.*$"
                                    }
                    """
                );

                closeRepo();
            }

            await sw.WriteAsync(
                $$"""
                {
                    "$schema": "{{schema}}",
                    "name": "{{info.Name}}",
                    "patterns": [
                {{includes}}
                    ],
                    "repository": {
                {{repositories}}
                    },
                    "scopeName": "source{{info.Extension}}"
                }
                """);

            sw.Close();
        }
    }

    (List<List<Key>> brothers, List<Key> headers, List<Key> others) GetContextInfo(Rule start, List<Key> allKeys)
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