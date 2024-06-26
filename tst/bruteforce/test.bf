define cs as subset of (nat, nat)

given n in nat

if n % 2 is 0 then
    cs contains (n, n / 2)

if n % 2 is 1 then
    cs contains (n, 3 * n + 1)

define csk as subset of (nat, nat, nat)
csk contains (n, 0, n)

given k in nat
given m in nat
given y in nat
if csk contains (n, k, m) and cs contains (m, y) then
    csk contains (n, k + 1, y)

define collatz as subset of nat
if csk contains (n, k, 1) then
    collatz contains n

check if
    for all x in nat
    collatz contains x