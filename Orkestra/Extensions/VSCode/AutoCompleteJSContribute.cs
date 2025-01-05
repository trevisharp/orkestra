/* Author:  Leonardo Trevisan Silio
 * Date:    03/07/2023
 */
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Orkestra.Extensions.VSCode;

/// <summary>
/// A Contribute to a VSCode Extension with AutoComplete mechanics.
/// </summary>
public class AutoCompleteJSContribute(LanguageInfo language) : JSContribute
{
    public override string JSCode => BuildJs();

    string BuildJs()
    {
        var sb = new StringBuilder();
        var headers = language.GetHeaders();
        var keyHash = new HashSet<string>();
        var simpleSet = language.Keys.Where(
            key => key.IsSimple(language.Rules)
        );

        int providerIndex = 0;
        foreach (var header in headers)
            Process(header, keyHash, sb, providerIndex++);
        
        var missingKeys = 
            from key in language.Keys
            where !keyHash.Contains(key.Name)
            where key.IsCompletableKey()
            where simpleSet.Contains(key)
            select key;
        foreach (var key in missingKeys)
        {
            keyHash.Add(key.Name);
            sb.Append(GetSimpleAutoComplete(key, providerIndex++));
        }

        GenerateNextCompletations(sb, providerIndex);
        
        return sb.ToString();
    }

    void GenerateNextCompletations(StringBuilder sb, int index)
    {
        var completableKeys =
            from key in language.Keys
            where key.IsCompletableKey()
            select key;
        var rules = language.Rules
            .SelectMany(subrule => subrule);
        
        var next = new Dictionary<Key, IEnumerable<ISyntacticElement>>();
        var nextElements = new List<ISyntacticElement>();
        foreach (var key in completableKeys)
        {
            foreach (var rule in rules)
            {
                var nexts = 
                    from pair in rule.Zip(rule.Skip(1))
                    where pair.First == key
                    select pair.Second;
                var hasElements = nexts.Any();
                if (!hasElements)
                    continue;
                nextElements.AddRange(nexts);
            }

            if (nextElements.Count == 0)
                continue;
            
            next.Add(key, nextElements.Distinct());
            nextElements = [];
        }

        foreach (var pair in next)
        {
            var list = pair.Value.ToList();
            var option = 
                list switch
                {
                    [ Key key ] => key.GetVSSnippetParameter(),
                    [ Rule rule ] => rule.GetVSSnippetParameter(),
                    { Count: > 1 } => 
                        list.All(el => el is Key) ?
                            list.Select(el => el as Key).GetVSSnippetParameter() :
                            list.GetHeaders().GetVSSnippetParameter(),
                    _ => null
                };
            if (option is null)
                continue;

            var provider = $"provider{++index}";
            var item = pair.Value.FirstOrDefault()?.Name?.ToLower() ?? "value";
            sb.AppendLine(RegisterCompletionItemProvider(provider, 
                $$"""
                    const linePrefix = document.lineAt(position).text.slice(0, position.character);
                    if (!linePrefix.endsWith('{{pair.Key.Expression}} ')) {
                        return undefined;
                    }

                    const comp = new vscode.CompletionItem('{{item}}');
                    comp.insertText = new vscode.SnippetString('{{option}}');
                    return [ comp ];
                """, ' '
            ));
        }
    }

    void Process(
        IGrouping<string, SubRule> group,
        HashSet<string> keyHash, 
        StringBuilder sb, 
        int index)
    {
        var code = GetCode(group, index);
        if (code is null)
            return;
            
        keyHash.Add(group.Key);
        sb.AppendLine(code);
    }

    string RegisterCompletionItemProvider(string providerName, string code, char trigger = char.MinValue)
    {
        var triggerData = trigger == char.MinValue ? "" : $", '{trigger}'";
        return
            $$"""
            const {{providerName}} = vscode.languages.registerCompletionItemProvider('{{language.Name}}', {

                provideCompletionItems(document, position) {
                
                    {{code.Replace("\n", "\n\t\t")}}

                }
            }{{triggerData}});
            context.subscriptions.push({{providerName}});
            """;
    }

    string? GetCode(
        IGrouping<string, SubRule> group,
        int index)
    {
        if (group.All(g => g.Count() == 1))
            return GetSimpleAutoComplete(group, index);
        
        if (group.Count() == 1)
            return GetComplexUnique(group, index);

        return GetComplexMin(group, index);
    }

    string? GetSimpleAutoComplete(
        IGrouping<string, SubRule> group,
        int index)
    {
        var subRule = group.FirstOrDefault();
        if (subRule is null)
            return null;

        if (subRule.FirstOrDefault() is not Key header)
            return null;

        return RegisterCompletionItemProvider($"provider{index}",
            $$"""
            const comp = new vscode.CompletionItem('{{header.Expression}}', vscode.CompletionItemKind.Keyword);
            comp.commitCharacters = [' '];
            return [ comp ];
            """
        );
    }

    string GetSimpleAutoComplete(Key key, int index)
    {
        return RegisterCompletionItemProvider($"provider{index}",
            $$"""
            const comp = new vscode.CompletionItem('{{key.Expression}}', vscode.CompletionItemKind.Keyword);
            comp.commitCharacters = [' '];
            return [ comp ];
            """
        );
    }

    string? GetComplexUnique(
        IGrouping<string, SubRule> group,
        int index)
    {
        var rule = group.FirstOrDefault();
        if (rule is null)
            return null;

        var header = rule.FirstOrDefault() as Key;
        if (header is null)
            return null;

        string item = header.Expression;
        string snippet = rule.GetVSSnippetForm();
        string baseProviderName = $"provider{index}";

        return 
            RegisterCompletionItemProvider(baseProviderName,
            $$"""
            const comp = new vscode.CompletionItem('{{item}}', vscode.CompletionItemKind.Keyword);
            comp.insertText = new vscode.SnippetString('{{snippet}}');
            comp.commitCharacters = [' '];
            return [ comp ];
            """
        );
    }

    string? GetComplexMin(IGrouping<string, SubRule> group, int index)
    {
        var biggesst = group.MinBy(g => g.Count());
        if (biggesst is null)
            return null;

        var header = biggesst.FirstOrDefault() as Key;
        if (header is null)
            return null;

        string item = header.Expression;
        string snippet = biggesst.GetVSSnippetForm();
        string baseProviderName = $"provider{index}";

        return 
            RegisterCompletionItemProvider(baseProviderName,
            $$"""
            const comp = new vscode.CompletionItem('{{item}}', vscode.CompletionItemKind.Keyword);
            comp.insertText = new vscode.SnippetString('{{snippet}}');
            comp.commitCharacters = [' '];
            return [ comp ];
            """
        );
    }
}