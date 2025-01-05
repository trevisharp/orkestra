define cs as subset of (nat, nat) :: Função de Collatz

given n in nat

if n % 2 is 0 then
    cs contains (n, n / 2)

if n % 2 is 1 then
    cs contains (n, 3 * n + 1)

define csk as subset of (nat, nat, nat) :: Função de Collatz aplicado k vezes
csk contains (n, 0, n)

given k in nat
given m in nat
given z in nat
if csk contains (n, k, m) and cs contains (m, z) then
    csk contains (n, k + 1, z)

check if 
    for all n in nat
    for some k in nat
    csk contains (n, k, 1)

:: future usefull contento to expand this project:
:: https://medium.com/dataseries/quick-start-to-vscode-plug-ins-diagnostic-information-b218823f6bdf
:: comp.command = { command: 'editor.action.triggerSuggest' }
:: https://vshaxe.github.io/vscode-extern/vscode/DiagnosticSeverity.html