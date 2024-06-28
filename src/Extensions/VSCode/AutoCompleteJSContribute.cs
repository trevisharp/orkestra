/* Author:  Leonardo Trevisan Silio
 * Date:    28/06/2023
 */
namespace Orkestra.Extensions.VSCode;

/// <summary>
/// A Contribute to a VSCode Extension with AutoComplete mechanics.
/// </summary>
public class AutoCompleteJSContribute(LanguageInfo language) : JSContribute
{
    public override string JSCode => buildJs();

    string buildJs()
    {
        return 
            """
            const provider = vscode.languages.registerCompletionItemProvider('bruteforce', {
                provideCompletionItems(document, position) {
                    
                    const comp = new vscode.CompletionItem('check');
                    comp.insertText = new vscode.SnippetString('check if\n\t${1|for|}');
                    comp.command = { command: 'editor.action.triggerSuggest' }

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
}