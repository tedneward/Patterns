title=Closure-based State: F#
date=2016-04-07
type=pattern
tags=structural patterns, patterns, fsharp
status=published
description=A Closure-based State implementation in F#.
~~~~~~

Being an object/functional hybrid language, F# offers the opportunity to support either a traditional object-field approach to encapsulating state, as well as the more "functional" style of  [Closure-basedState](ClosureBasedState.html). As such, developers will generally prefer to store state in object/class fields, but there can still be situation where encapsulating the state away from the object as a whole can be preferable.

However, F# has some interesting language restrictions around enclosed bound variables; specifically, because F# wants to assume that all bound values are immutable by default, F# will require a slight amendment to the enclosed local state, marking it as a "reference" (using the `ref` keyword), before it can be used inside of a returned function:

````fsharp
let operation =
    let state = ref 100
    fun adjust ->
        state := (!state) + adjust
````

Thanks to F#'s "last expression in an expression block is the assumed return value" feature, making use of this feels absolutely trivial; the anonymous function receiving the single parameter `adjust` is assigned to the local value `operation`, meaning that now `operation` will be operating on the referenced `state` value, but without any sort of opportunity for any other operation or library to modify `state`.

When working with objects, this gets trickier, since F#, like its kin C# and Visual Basic, is a strongly-typed language, and as such, wants clients to have compile-time awareness of the object types they are invoking. Thus, the typical idiomatic usage for F# here will be to use Closure-based State to implement an anonymous implementation of an interface, and have the outer function declared to return an instance of such. This is trivial to do with object expressions:

````fsharp
type IInterface =
    interface
        abstract Operation: (int) -> int
    end
    
let instance =
    let state = ref 100
    { new IInterface with
        member x.Operation(adjust) = 
            state := (!state) + adjust
            !state
    }

let result = instance.Operation(100)
printfn "%A" result
````

Alternatively, F# can return an instance of a [Dynamic Object](DynamicObject.html); this is somewhat contradictory to the spirit of the F# language, since it is strongly-typed by nature, but circumstances will sometimes suggest it as useful, depending (as always) on the context.
