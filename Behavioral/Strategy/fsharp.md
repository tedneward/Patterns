title=Strategy: F#
date=2022-03-16
type=pattern
tags=pattern implementation, behavioral, fsharp
status=published
description=A Strategy implementation in F#.
~~~~~~

## Variant: Function-as-Object

In pure functional languages, it's actually difficult to keep state and data tied together--in fact, part of the whole point of a functional language is to write functions that operate on data, ideally on lots of different kinds of data. "Therefore, create a function that acts like an object. Such a function carries the data it needs along with the expression that operates on the data. More importantly, an object encapsulates its data, ensuring that only the allowed operations are applied to them." In other words, by writing a function and keeping the data buried inside of it, we achieve the same kind of encapsulation that object-orientation has traditionally reserved for itself as its principal advantage. This is done via a closure, which is the next element in the language.

F#, like most functional/object hybrid languages, also offers the ability to bind functions to values, so this is also pretty straightforward. I choose to just operate against the "global" balance value, rather than do the more functional "pass the balance in" that the previous two use:

```
let functionAsObject = fun () ->
    let balance = ref 0
    let withdraw = 
        fun amt ->
            if amt <= !balance then
                balance := (!balance) - amt
                !balance
            else
                raise (Exception("Insufficient funds"))
    let deposit = 
        fun amt -> 
            balance := (!balance) + amt
            !balance
    let accrueInterest = 
        fun (intRate : float) -> 
            balance := (!balance) + (int (float !balance * intRate))
            !balance
    
    Console.WriteLine "=========> Function as Object"
    printfn "%d" (deposit 200)
    printfn "%d" (withdraw 50)
```

