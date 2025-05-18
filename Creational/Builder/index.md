title=Builder
date=2016-07-02
type=pattern
tags=pattern, creational
status=published
description=Separate the construction of a complex object from its representation so that the same construction process can create different representations.
~~~~~~

*tl;dr* The Builder pattern has enjoyed some success within the O-O community, particularly among the crowd that sees it as a way to build fluent APIs (APIs which read, more or less, like a natural language, a la English). Builder has a few tricks up its sleeve beyond just fluent APIs, however.

<!--more-->

## Problem
Separate the construction of a complex object from its representation so that the same construction process can create different representations.

## Context
*The object is made up of a collection of parts.* Builder isn't necessary unless there is a non-trivial construction process (meaning not a simple constructor call). In the Gang-of-Four book, they assumed that the Builder would often be building a [Composite](Composite.html), but frankly, this is a limited view; it's trivial to imagine a Builder constructing a [Facade](Facade.html) (such as a compiler whose optimization or code generation steps will vary) or [Interpreter](Interpreter.html).

*Create the complex object independent of the parts that make up the object and how they're assembled.* In other words, the Builder will be used to construct not just a single object, but a family of objects that work together to achieve some useful purpose, but we don't want the client to have to worry about the details of doing the actual assembly (or what parts are being assembled).

*The construction process must allow different representations for the object that's constructed.* The complex object's internals (state and behavior) will need to vary according to different circumstances or needs.

*The client wants to defer the creation to an intermediary, a Director.* This is sometimes waived under certain circumstances, particularly when the desire is to create a Fluent API, enabling easy direct client construction.

*The product created should never be used until its construction is complete, but the steps required for construction need to take place incrementally.* A constructor is a pass/fail
operation, meaning once the constructor returns, the object should be in a ready state and available for immediate use---or else it should throw an exception (or signal some other kind of failure) and not be available at all.

## Solution
In the class Gang-of-Four solution, define a Director object that constructs a ConcreteBuilder, and use that to build the Product (the complex object needing to be constructed). Each ConcreteBuilder inherits from  the same AbstractBuilder, which provides the uniform interface for building the individual parts, but the ConcreteBuilder defines the exact collection of parts, the order in which they are constructed, and so on.

There's some interesting questions that emerge from the implementation of a Builder:

* *Class or function?* The family of ConcreteBuilders don't necessarily have to be related via an inheritance link---any sort of link that follows a natural interface (so as to hide the details of which ConcreteBuilder are in use) would suffice. This could be done via a set of curried functions.
* *Reusable part construction?* There may be certain part construction steps or details that are repeated across ConcreteBuilders, in which case we want to be able to reuse those steps or details (in order to avoid repeating them across ConcreteBuilders, in case they should ever need to change, for example). Both curried functions and an abstract base class can provide that kind of reusability, while still allow for some variation in hte details.
* *Parameterization?* While the Gang-of-Four assumed that the ConcreteBuilder would be the sole source of knowledge for the construction of the Product, some scenarios will suggest or dictate that some form of parameterization will be necessary or desirable.
* *Client-direct or through the Director?* In some cases, the intermediary Director is not desirable; the client may want or need to have some control over the construction process. In these cases, the interface/signature may need or want to be more client-friendly; in these cases, the Builder will be a Fluent API (description elsewhere), and will defer some or all of its construction details to the Director or the client directly.

Additionally, with Builder, a multi-step configuration/construction pass can allow clients to incrementally construct the object without worry about using the object before it is completely ready (so long as the Builder refuses to pass back the product if doing so would leave the product in an incomplete or partially-constructed state).

## Implementations

* [C#](Builder-CSharp.html)
* [Swift](Builder-Swift.html)
* [Java](Builder-Java.html)

## Consequences
Builder brings with it several consequences of note.

* *"Lets you vary a product's internal representation."* Builders can construct different Products, whose internal representation (state/behavior) needs to be different based on certain criteria.
  
* *"It isolates code for construction and representation."* Or it can, if desired.

* *"It gives you finer control over the construction process.* Unlike creational patterns that construct products in one shot, the Builder pattern constructs the product step by step under the director's control. Only when the product is finished does the director retrieve it from the builder. Hence the Builder interface reflects the process of constructing the product more than other creational patterns. This gives you finer control over the construction process and consequently the internal structure of the resulting product."

## Relationships

A Builder is often used to create [Composite](../../structural/Composite/)s.

## Variations
There are a couple of patterns that frequently end up connected to, or similar to, Builder:

* A Builder frequently turns into a [Constructor Function](ConstructorFunction.html) or vice versa, when the details of construction need or want to be hidden behind a more encapsulated barrier. The Constructor Function can either hide, or eliminate the repetition of, the construction process of building similar Builder instances.

* The ConcreteBuilder itself can be seen as a form of [Strategy](Strategy.html) specific to the construction of a Product. 
