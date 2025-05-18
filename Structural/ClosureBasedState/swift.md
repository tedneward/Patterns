title=Closure-based State: Swift
date=2016-04-09
type=pattern
tags=pattern implementation, structural, swift
status=published
description=A Closure-based State implementation in Swift.
~~~~~~

Implementing [Closure-based State](../ClosureBasedState) in Swift is not difficult; the language supports function literals as first-class citizens, and the compiler is intelligent enough to know how to "close over" variables referenced in the enclosing scope, so a simple implementation looks pretty straightforward:

````swift
let operation = ({ () -> ((Int) -> Int) in
    var state = 100
    let inner = { (adjust: Int) -> Int in
        state = state + adjust
        return state
    }
    return inner
})()

let result = operation(100)
print(result)
````

#### Objects
Like other strongly-typed object-oriented langauges, Swift will have a bit trickier time doing this for object types, since the interface needs to be known by clients ahead of time. Swift does have the concept of a protocol, which serves much like an interface does in C# or Java (or a pure-abstract class does in C++), and when combined with the fact that Swift allows nested classes within the scope of a function, it seems highly likely that Swift will allow the localized definition of a class that implements a protocol and (given the above) uses closed-over variables for state:

````swift
// This code will fail to compile!
protocol Interface {
    func Operation(adjust: Int) -> Int;
}
let maker = { () -> Interface in
    var state = 100
    class Implementation : Interface {
        func Operation(adjust: Int) -> Int {
            state = state + adjust
            return state
        }
    }
    
    return Implementation()
}
````

Unfortunately, Swift has explicitly forbidden this, with an error message that really couldn't be any more clear: "Class declaration cannot close over value 'state' defined in outer scope." (This is as of Swift 2.2 at this writing.) Attempts to close over reference-type instances defined in the outer scope fail with the same error message, so it appears that Swift explicitly forbids a class-oriented implementation of this pattern.


