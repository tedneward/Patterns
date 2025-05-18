title=Closure-Based State
date=2016-04-02
type=pattern
tags=pattern, structural
status=published
description=Use the scoping mechanisms of closures to provide a means for storing state outside of, but still available to, an object.
~~~~~~

*tl;dr* With the prevalance of libraries and tools that can peer past access controls (such as Reflection on the JVM and CLR, or the Mirrors facility in Swift), trying to encapsulate private details away from prying eyes can be increasingly difficult. Other languages lack access controls entirely, or the concept of objects. In any of these languages that offer closures, we can hold state within a closure yet outside of the object, rendering it almost entirely inaccessible to outside parties.

<!--more-->

## Problem
We want to preserve "information hiding" and avoid exposing details of how and where internal state is being held.

## Context
*Reflection libraries can bypass "private" and other access controls.* This is a good thing in a number of ways, but Reflection libraries also offer opportunities to completely undo the benefit of access controls.

*The language has no concept of object.* In a traditional functional language, it's actually difficult to keep state and data tied together. In fact, part of the whole point of a functional language is to write functions that operate on data, ideally on lots of different kinds of data. In such languages, then, when seeking to keep state and operations together, we have to get a little tricky to obtain the traditional things that an object-oriented language provides for free. 

*The language offers a closure facility around function literals.* Meaning, the language knows how to "close over" local variables being referenced within scope of the function's definition, yet outside of the function definition itself. (Almost all languages developed since 2000 offer such capability, and those that didn't have it before, such as C++, Java, and others, have already or are being modified to include it.)

## Solution
Use the closure facilities of the language to hold state referenced from within the function, thus providing the functionality of a private field without requiring or using that language facility. 

Practically speaking, in languages that support closures, use an "outer" function to provide the space holding the closure, with an "inner" function providing the functionality desired, and immediately invoke the "outer" function so that the actual return value is the "inner" function; this will result in a function who now holds a reference to the local variables declared in the "outer" function even though that stack frame no longer exists.

Note that there is no reason that a given system couldn't use both Closure-based State and the traditional private fields within a class, so as to have those fields hidden via "private" yet still accessible via Reflection (and thus to tools that make use of Reflection, a la object-relational libraries or serialization systems), yet also have certain data entirely inaccessible to Reflection systems.

## Implementations
The [Envoy pattern language](../blog/2012/envoy-in-scala-javascript-and-more.html), from which this pattern is borrowed, describes several implementations as part of that pattern language.

* [C#](csharp.html)
* [F#](fsharp.html)
* [Java](java.html)
* [JavaScript](javascript.html)
* [Swift](swift.html)

## Consequences
*Invisible references.* The closure is an "invisible reference" from the object, and the closure, along with any additional variables referenced from that enclosing scope. That means that potentially the Closure-based State data will keep more objects alive for longer than intuition suggests, which can create potential memory pressure on the program if it grows out of hand. (This has been called the "dangling reference" problem in Java Swing applications, for example, and has been known to show up in other systems that use "event handlers", a la a [Chain of Responsibility](../../behavioral/ChainOfResponsibility/), that accidentally contain references to enclosing objects and are never released.)

## Variations
In languages that do not support closures natively (which are becoming far apart and few between), this almost always turns into a [Strategy](../../behavioral/Strategy/) implementation.

Additionally, [Strategy](../../behavioral/Strategy/) implementations can hold data as fields in the Strategy object itself, which essentially provides much of the same kind of functionality as Closure-based State, but without any of the concerns around Reflection being addressed at all.

Closure-based State can also be used at the module level for those languages which support modules as first-class concepts, a la F# or Javascript for the same reasons. (In point of fact, many languages which support modules do so by modeling them internally as a singleton object implicitly held at the global scope and always referenced or considered "in scope". Javascript does this, for example, and can be easily seen by iterating across the members of "this" at the top level of a script.) 

## Relationships
Closure-based State can fairly easily adapt to use the [State](../../behavioral/State/) pattern to adjust its internal state and operations. Operations would be "trampolining" into functions defined in the closure, rather than on the object itself.

