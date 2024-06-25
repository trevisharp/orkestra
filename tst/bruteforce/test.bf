given n in nat

define even as subset of nat
if n % 2 is 0 then
    even contains n

// um código bem bonito
define odd as subset of nat
if n % 2 is 1 then
    odd contains n

check if
    for all x in even
        odd contains x + 1