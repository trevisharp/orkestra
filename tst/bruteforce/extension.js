const vscode = require('vscode');

function activate(context) {
const provider0 = vscode.languages.registerCompletionItemProvider('bruteforce', {
	
	    provideCompletionItems(document, position) {
	    
	        const comp = new vscode.CompletionItem('nat');
			return [ comp ];
	
	    }
	});
	context.subscriptions.push(provider0);
	const provider1 = vscode.languages.registerCompletionItemProvider('bruteforce', {
	
	    provideCompletionItems(document, position) {
	    
	        const comp = new vscode.CompletionItem('real');
			return [ comp ];
	
	    }
	});
	context.subscriptions.push(provider1);
	const provider2 = vscode.languages.registerCompletionItemProvider('bruteforce', {
	
	    provideCompletionItems(document, position) {
	    
	        const comp = new vscode.CompletionItem('int');
			return [ comp ];
	
	    }
	});
	context.subscriptions.push(provider2);
	const provider3 = vscode.languages.registerCompletionItemProvider('bruteforce', {
	
	    provideCompletionItems(document, position) {
	    
	        const comp = new vscode.CompletionItem('rat');
			return [ comp ];
	
	    }
	});
	context.subscriptions.push(provider3);
	const provider4 = vscode.languages.registerCompletionItemProvider('bruteforce', {
	
	    provideCompletionItems(document, position) {
	    
	        const comp = new vscode.CompletionItem('subset');
			comp.insertText = new vscode.SnippetString('subset of ${1|subset,nat,real,int,rat|} ');
			return [ comp ];
	
	    }
	});
	context.subscriptions.push(provider4);
	const provider5 = vscode.languages.registerCompletionItemProvider('bruteforce', {
	
	    provideCompletionItems(document, position) {
	    
	        const comp = new vscode.CompletionItem('given');
			comp.insertText = new vscode.SnippetString('given ${1:id} in ${2|subset,nat,real,int,rat|} ');
			return [ comp ];
	
	    }
	});
	context.subscriptions.push(provider5);
	const provider6 = vscode.languages.registerCompletionItemProvider('bruteforce', {
	
	    provideCompletionItems(document, position) {
	    
	        const comp = new vscode.CompletionItem('define');
			comp.insertText = new vscode.SnippetString('define ${1:id} as ${2|subset,nat,real,int,rat|} ');
			return [ comp ];
	
	    }
	});
	context.subscriptions.push(provider6);
	const provider8 = vscode.languages.registerCompletionItemProvider('bruteforce', {
	
	    provideCompletionItems(document, position) {
	    
	        const comp = new vscode.CompletionItem('not');
			comp.insertText = new vscode.SnippetString('not ${1||} ');
			return [ comp ];
	
	    }
	});
	context.subscriptions.push(provider8);
	const provider9 = vscode.languages.registerCompletionItemProvider('bruteforce', {
	
	    provideCompletionItems(document, position) {
	    
	        const comp = new vscode.CompletionItem('if');
			comp.insertText = new vscode.SnippetString('if ${1|not|} then ${2||} ');
			return [ comp ];
	
	    }
	});
	context.subscriptions.push(provider9);
	const provider10 = vscode.languages.registerCompletionItemProvider('bruteforce', {
	
	    provideCompletionItems(document, position) {
	    
	        const comp = new vscode.CompletionItem('for');
			comp.insertText = new vscode.SnippetString('for ${1|some,all|} ${2:id} in ${3|subset,nat,real,int,rat|} ');
			return [ comp ];
	
	    }
	});
	context.subscriptions.push(provider10);
	const provider11 = vscode.languages.registerCompletionItemProvider('bruteforce', {
	
	    provideCompletionItems(document, position) {
	    
	        const comp = new vscode.CompletionItem('considering');
			comp.insertText = new vscode.SnippetString('considering ${1:id} ');
			return [ comp ];
	
	    }
	});
	context.subscriptions.push(provider11);
	const provider12 = vscode.languages.registerCompletionItemProvider('bruteforce', {
	
	    provideCompletionItems(document, position) {
	    
	        const comp = new vscode.CompletionItem('some');
			return [ comp ];
	
	    }
	});
	context.subscriptions.push(provider12);
	const provider13 = vscode.languages.registerCompletionItemProvider('bruteforce', {
	
	    provideCompletionItems(document, position) {
	    
	        const comp = new vscode.CompletionItem('all');
			return [ comp ];
	
	    }
	});
	context.subscriptions.push(provider13);
	

}

function deactivate() {}

module.exports = {
    activate,
    deactivate
}