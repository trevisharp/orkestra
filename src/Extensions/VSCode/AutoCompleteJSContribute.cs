/* Author:  Leonardo Trevisan Silio
 * Date:    28/06/2023
 */
using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Orkestra.SyntacticAnalysis;

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

        int providerIndex = 0;
        foreach (var header in headers)
            process(header, sb, providerIndex++);
        
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

    void process(IGrouping<string, SubRule> group, StringBuilder sb, int index)
    {
        Verbose.Success(group.Key);
        Verbose.NewLine();
        Verbose.StartGroup();
        foreach (var item in group)
        {
            foreach (var el in item)
                Verbose.InlineContent(el.Name);
            Verbose.NewLine();
        }
        Verbose.EndGroup();
        Verbose.NewLine();

        var code = getCode(group, index);
        if (code is null)
            return;
        
        sb.AppendLine(code);
    }

    string getCode(IGrouping<string, SubRule> group, int index)
    {
        if (group.All(g => g.Count() == 1))
            return getSimpleAutoComplete(group, index);
        
        if (group.Count() == 1)
            return getComplexUnique(group, index);

        return null;
    }

    string getSimpleAutoComplete(IGrouping<string, SubRule> group, int index)
    {
        var subRule = group.FirstOrDefault();
        if (subRule is null)
            return string.Empty;
        
        var header = subRule.FirstOrDefault() as Key;
        if (header is null)
            return string.Empty;

        return registerCompletionItemProvider(index,
            $$"""
            const comp = new vscode.CompletionItem('{{header.Expression}}');

            return [ comp ];
            """
        );
    }

    string getComplexUnique(IGrouping<string, SubRule> group, int index)
    {
        var rule = group.FirstOrDefault();
        if (rule is null)
            return string.Empty;

        return
            $$"""
                
            """;
    }

    string registerCompletionItemProvider(int index, string code)
    {
        return
            $$"""
            const provider{{index}} = vscode.languages.registerCompletionItemProvider('{{language.Name}}', {

                provideCompletionItems(document, position) {
                
            {{code.Replace("\n", "\n\t\t")}}

                }
            });
            context.subscriptions.push(provider{{index}});
            """;
    }
}