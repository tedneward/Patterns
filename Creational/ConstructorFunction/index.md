title=Constructor Function
date=2016-03-31
type=pattern
tags=pattern, creational
status=published
description=Define an easy interface for creating an object, but defer the actual decision of what object type to instantiate.
~~~~~~

*tl;dr* A Constructor Function is a function designed specifically to construct instances of entities (typically objects, although in languages which do not support objects as native types, this will typically be something that masquerades as an object). It is often seen as a variation on a [Factory Method](FactoryMethod.html), though there is enough variation on the intent that it is worth calling this out as a standalone pattern.

<!--more-->

## Problem
We want to define an easy interface for creating an object, but defer the actual decision of what object type to instantiate. (In this respect, it is very very similar to [Factory Methods](../FactoryMethod) and the main decision point on which to use will vary on the context.)

## Context
*A class can't anticipate the class of objects it must create.* In other words, either the system is deliberately designed to be "open", allowing types that weren't known at the time of the framework's creation to be created (which is the case in most application frameworks, for example), or the system is deliberately drawing an encapsulatory barrier between the class representing the abstract type and the implementations, in order to help facilitate better decoupling. Most systems or platforms that support some notion of "plugins" are in the former category, although often at a binary level of interoperability, rather than a source-language level of interop. (Most Component Object Model (COM) developers will remember CoCreateInstance, for example, which is the classic example of a Factory Method.)

*A class wants the creation to be localized to a function or method.* Specifically, the class wants to avoid establishing the requirement of subclassing upon its clients.

*A class wants the creation to be parameterized.* This parameterization can be done either using function parameters or through generic (parameterized type) parameters to the function (in the languages that support such).

*A class wants to provide a higher-fidelity syntax to describe the object being created.* In many cases, the act of using the standard language facilities ("new" and the constructor parameters) to construct an object can be a little confusing and/or unclear as to exactly what kind of thing is being created. In these cases, a clearer thing to do would be to provide one or more methods or functions that use the name of the function/method to make it more clear what is being created.

## Solution
Define a functional interface that is responsible for the creation of objects. Place the details of the construction inside this functional interface/implementation. Optionally, allow clients to replace this function in some manner during the execution of the program so as to change the type created at runtime.

## Implementations
The [Envoy pattern language](/blog/2012/envoy-in-scala-javascript-and-more), from which this pattern is borrowed, describes several implementations as part of that pattern language. This is also a variant of the "Creator Method" that is described in RP/57).

* [C#](csharp.html)
* [F#)(fsharp.html)
* [Java](java.html)
* [Javascript](javascript.html)
* [Scala](scala.html)
* [Swift](swift.html)
* [Yeti](yeti.html)

## Consequences

*State can be localized elsewhere.* Because a Constructor Function is "just" a function, it can capture state and hold it in a variety of locations before returning the object in question. For example, a Constructor Function can hide the details of a database connection (and the details of the connection itself) inside of the function, using [Closure-Based State](ClosureBasedState.html) to hold on to a reference to the connection throughout the lifetime of the object being returned.

*Construction details can be encapsulated without putting them in the type being constructed.* In some scenarios, an object or family of objects will have a common construction pattern, but the object/object-family are not set up to have a convenience constructor defined on the class. In this case, an extension method (such as what F# or Swift provide) or a global function defined in the client's namespace can create a convenient way to capture all of those construction details in one place for easy modification. This can also be incredibly convenient when seeking to construct objects using many of the Structural patterns (a la [Decorator](Decorator.html), [Facade](Facade.html), and so
on). 

*Construction can potentially fail.* In many languages, the only option for indicating that a constructor has failed (due to invalid input, for example) is to throw an exception, rather than a more "gentle" form of error-reporting. Developers tend to dislike exceptions because of the additional work required to define a guard block and handle the reported exceptions. In those cases where the construction of an object could fail without being a catastrophic error (such as a parser/scanner attempting to parse a String), it can be far more preferable to "fail gently" by simply returning nil or an optional instance of None. A constructor function can capture this easily, even for those types which fail construction with exceptions, by "wrapping" the constructor actions and either returning the new object or returning null/nil/None.

*Destruction details are left up to the object(s) returned.* If there is any cleanup process that needs to happen when this object is collected/goes out of scope, the object itself must implement it. There is no convenient way to define a "destructor function" (so that, for example, a pooled object instance can be returned to the pool).

*Construction can potentially be replaced at runtime.* If the Constructor Function is a name-bound function value, instead of a compile-time bound function, then the implementation can be replaced at runtime without clients being aware of the change. This can make it much easier to "slip in" [Decorators](Decorator.html) or [Proxies](Proxy.html) without the clients being aware of the change.

## Variations
If a Constructor Function needs to belong as part of a larger entity (because the responsibility of creating subtypes is only a part of the overall responsibility), then this is a [Factory Method](FactoryMethod.html).

Constructor Functions may well turn into a [Chain of Responsibility](../../behavioral/ChainOfResponsibility) for constructing different kinds of objects, with individual Constructor Functions being added and removed at runtime, and the decision criteria for construction being passed in as part of the Constructor Function's signature. This "Chain of Constructors" can be a powerful way to allow third parties to participate in the construction process without requiring recompilation, but ordering here will be critical.

A Constructor Function may want to use a [MessagePassingInterface](../../structural/MessagePassingInterface) to generalize the actual construction signature to allow for future criteria to be passed without changing client code.

## Relationships
Constructor Functions will often be producing [Composite](../../structural/Composite) products, if the products themselves are somehow seen as a "tree" structure.

A Constructor Function can make it relatively easy to construct [Decorators](../../structural/Decorator), building the Decorator around the returned object from the decorated's Constructor Function (or other creational operation).

Constructor Functions can be chained using composed functions to form a [Builder](../Builder). One might argue that a Builder invocation is itself a Constructor Function, with the difference being that in a Builder, the client is driving the construction, whereas with a Constructor Function the client seeks to remain ignorant of the construction details.
