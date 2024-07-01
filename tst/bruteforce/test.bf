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
    for all x in nat
    for some y in nat
    csk contains (x, y, 1)

given id in ${2|subset|nat|real|int|rat|} 
:: https://code.visualstudio.com/docs/editor/userdefinedsnippets