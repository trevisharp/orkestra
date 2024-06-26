define collatzfunc as subset of (nat, nat)

given n in nat
given k in nat

collatzfunc contains (n, n)

if collatzfunc contains (k, n) and n % 2 is 0 then
    collatzfunc contains (k, n / 2)

if collatzfunc contains (k, n) and n % 2 is 1 then
    collatzfunc contains (k, 3 * n + 1)

define collatz as subset of nat
if collatzfunc contains (n, 1) then
    collatz contains n

check if
    for all x in nat
    collatz contains x