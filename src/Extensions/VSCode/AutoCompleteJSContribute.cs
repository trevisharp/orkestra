/* Author:  Leonardo Trevisan Silio
 * Date:    01/07/2023
 */
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.Frozen;

namespace Orkestra.Extensions.VSCode;

/// <summary>
/// A Contribute to a VSCode Extension with AutoComplete mechanics.
/// </summary>
public class AutoCompleteJSContribute(LanguageInfo language) : JSContribute
{
    public override string JSCode => buildJs();

    string buildJs()
    {
        var sb = new StringBuilder();
        var headers = language.GetHeaders();
        var keyHash = new HashSet<string>();
        var simpleSet = language.Keys.Where(
            key => key.IsSimple(language.Rules)
        );

        int providerIndex = 0;
        foreach (var header in headers)
            process(header, keyHash, sb, providerIndex++);
        
        var missingKeys = 
            from key in language.Keys
            where !keyHash.Contains(key.Name)
            where key.IsCompletableKey()
            where simpleSet.Contains(key)
            select key;
        foreach (var key in missingKeys)
        {
            keyHash.Add(key.Name);
            sb.Append(getSimpleAutoComplete(key, providerIndex++));
        }

        generateNextCompletations(sb, providerIndex);
        
        return sb.ToString();

        return 
            """
            const provider = vscode.languages.registerCompletionItemProvider('bruteforce', {
                provideCompletionItems(document, position) {
                    
                    const comp = new vscode.CompletionItem('check');
                    comp.insertText = new vscode.SnippetString('check if\n\t${1|for|}');
                    comp.command = { command: 'editor.action.triggerSuggest' }
                    // comp.commitCharacters = [' '];

                    return [
                        comp
                    ]
                }
            })

            const provider2 = vscode.languages.registerCompletionItemProvider('bruteforce', {
                provideCompletionItems(document, position) {

                    const linePrefix = document.lineAt(position).text.slice(0, position.character);
                    if (!linePrefix.endsWith('for ')) {
                        return undefined;
                    }

                    const some = new vscode.CompletionItem('some');
                    some.insertText = new vscode.SnippetString('some $1 in ${2|nat,real,rat,int|}');

                    const all = new vscode.CompletionItem('all');
                    some.insertText = new vscode.SnippetString('all $1 in ${2|nat,real,rat,int|}');

                    return [some, all];
                }
            }, ' ');

            context.subscriptions.push(provider);
            context.subscriptions.push(provider2);
            """;
    }

    void generateNextCompletations(StringBuilder sb, int index)
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
            sb.AppendLine(registerCompletionItemProvider(provider, 
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

    void process(
        IGrouping<string, SubRule> group,
        HashSet<string> keyHash, 
        StringBuilder sb, 
        int index)
    {
        var code = getCode(group, index);
        if (code is null)
            return;
            
        keyHash.Add(group.Key);
        sb.AppendLine(code);
    }

    string registerCompletionItemProvider(string providerName, string code, char trigger = char.MinValue)
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

    string getCode(
        IGrouping<string, SubRule> group,
        int index)
    {
        if (group.All(g => g.Count() == 1))
            return getSimpleAutoComplete(group, index);
        
        if (group.Count() == 1)
            return getComplexUnique(group, index);

        return getComplexMin(group, index);
    }

    string getSimpleAutoComplete(
        IGrouping<string, SubRule> group,
        int index)
    {
        var subRule = group.FirstOrDefault();
        if (subRule is null)
            return null;
        
        var header = subRule.FirstOrDefault() as Key;
        if (header is null)
            return null;

        return registerCompletionItemProvider($"provider{index}",
            $$"""
            const comp = new vscode.CompletionItem('{{header.Expression}}', vscode.CompletionItemKind.Keyword);
            comp.commitCharacters = [' '];
            return [ comp ];
            """
        );
    }

    string getSimpleAutoComplete(Key key, int index)
    {
        return registerCompletionItemProvider($"provider{index}",
            $$"""
            const comp = new vscode.CompletionItem('{{key.Expression}}', vscode.CompletionItemKind.Keyword);
            comp.commitCharacters = [' '];
            return [ comp ];
            """
        );
    }

    string getComplexUnique(
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
            registerCompletionItemProvider(baseProviderName,
            $$"""
            const comp = new vscode.CompletionItem('{{item}}', vscode.CompletionItemKind.Keyword);
            comp.insertText = new vscode.SnippetString('{{snippet}}');
            comp.commitCharacters = [' '];
            return [ comp ];
            """
        );
    }

    string getComplexMin(
        IGrouping<string, SubRule> group,
        int index)
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
            registerCompletionItemProvider(baseProviderName,
            $$"""
            const comp = new vscode.CompletionItem('{{item}}', vscode.CompletionItemKind.Keyword);
            comp.insertText = new vscode.SnippetString('{{snippet}}');
            comp.commitCharacters = [' '];
            return [ comp ];
            """
        );
    }
}