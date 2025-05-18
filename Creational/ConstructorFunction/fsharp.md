title=Constructor Function: F#
date=2022-03-16
type=pattern
tags=pattern implementation, creational, fsharp
status=published
description=A Constructor Function implementation in F#.
~~~~~~
"You are creating a Function as Object using a Closure. How do you create instances of the object? [M]ake a function that returns your Function as Object. Give the function an Intention Revealing Name (Beck) such as make-object."

Really, not any different from the other languages: a function binding that returns a function, with the passed-in "balance" captured as a reference (see the earlier pattern element discussion for why it's a ref) inside the outer function scope, and used from the inner function scope.

```
let constructorFunction = fun () ->
    let makeAccount =
        fun bal ->
            let balance = ref bal
            fun amt ->
                if amt <= !balance then
                    balance := (!balance) - amt
                    !balance
                else
                    raise (Exception("Insufficient funds"))                
            
    Console.WriteLine "=========> Constructor Function"
    let acctForEugene = makeAccount 100
    printfn "%d" (acctForEugene 20)
```

