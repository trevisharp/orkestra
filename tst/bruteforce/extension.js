const vscode = require('vscode');

function activate(context) {
            
    let disposable = vscode.commands.registerCommand('bruteforce.helloWorld', function () {
        vscode.window.showInformationMessage('Hello World from Orkestra!');
    });

    context.subscriptions.push(disposable);

    const provider = vscode.languages.registerCompletionItemProvider('bruteforce', {
		provideCompletionItems(document, position, token, context) {
            const forCompletetion = new vscode.CompletionItem('for');
            
            const completionWithSnippet = new vscode.CompletionItem('check');
            completionWithSnippet.insertText = new vscode.SnippetString('check if ${1|for all,for some|}!');
            completionWithSnippet.documentation = new vscode.MarkdownString("Inserir 'Goodbye' seguido de 'World' ou 'Universe'");

            return [
                forCompletetion,
                completionWithSnippet
            ]
        }
    })

    context.subscriptions.push(provider);
}

function deactivate() {}

module.exports = {
    activate,
    deactivate
}