title=Factory Method
date=2016-03-31
type=pattern
tags=pattern, creational
status=published
description=Define an interface for creating an object, but let something (usually subclasses, but not always) decide which object type to instantiate.
~~~~~~

*tl;dr* Factory Method is a pattern that is often called by a simpler name hinging on the word "Factory", a la "the Factory pattern" or somesuch. The GOF language actually has two patterns which each could qualify under that moniker, this one and the [Abstract Factory](../AbstractFactory/), depending on the intent and the desired consequences.

<!--more-->

## Problem
We want to define an interface for creating an object, but let something (usually subclasses, but not always) decide which object type to instantiate. Factory Method allows a class to defer instantiation to subclasses.

## Context
*A class can't anticipate the class of objects it must create.* In other words, either the system is deliberately designed to be "open", allowing types that weren't known at the time of the framework's creation to be created (which is the case in most application frameworks, for example), or the system is deliberately drawing an encapsulatory barrier between the class representing the abstract type and the implementations, in order to help facilitate better decoupling. Most systems or platforms that support some notion of "plugins" are in the former category, although often at a binary level of interoperability, rather than a source-language level of interop. (Most Component Object Model (COM) developers will remember CoCreateInstance, for example, which is the classic example of a Factory Method.)

*A class wants its subclasses to specify the objects it creates.* The defining type doesn't know the exact subtype to create, but it wants some level of relationship to itself, so it will ask that the actual construction be done via a subclass. This way, clients can still reference the Factory directly through the base interface/class surface, and don't have to worry about any of the subclass details---all knowledge is funneled through the base interface.

*Classes delegate responsibility to one of several helper subclasses, and you want to localize the knowledge of which helper subclass is the delegate.* But if there are more than one subclass, how do clients know which one to use? This is where the base type can still provide value to the system---it can examine the request in some fashion, determine what the appropriate type of subclass to use, and then delegate all further construction knowledge to the subclass.

## Solution
Create a hierarchy of Product types that vary by inheritance, and a corresponding hierarchy of Creator types that provide a method by which to construct instances of Product (sub)types by deferring the construction logic to the Creator subtype.

## Implementations

* [Java](java.html)
* [Swift](swift.html)

## Consequences

*Factory methods eliminate the need to bind application-specific classes into your code.* Because clients only ever deal with the base Product type interface/surface area, clients can remain entirely ignorant as to the exact nature of the Product with which they are dealing. This is particularly important in "open" systems, in which the complete set of possible Product types is not known, allowing third-parties to participate as part of the system.

*Clients (might) have to subclass the Creator class just to create a particular ConcreteProduct object.* In the classic GOF pattern, subclassing was seen as the principal way to customize the FactoryMethod implementation. This is fine when the client has to subclass the Creator class anyway, but otherwise the client now must deal with another point of evolution. This can be mitigated in some cases through the use of functions, but doing so would lose some of the other benefits of subclassing (such as locality of related features in a subclass).

*Provides hooks for subclasses.* Creating objects inside a class with a factory method is always more flexible than creating an object directly. Factory Method gives subclasses a hook for providing an extended version of an object.

*Connects parallel class hierarchies.* In most of the GOF examples, the factory method is only called by Creators. But this doesn't have to be the case; clients can find factory methods useful, especially in the case of parallel class hierarchies. The GOF show an example of graphical shapes (Figure serves as a base to LineFigure and TextFigure), and each has different "manipulators" for when being manipulated (dragged, resized, etc) in a graphical editor; therefore, Figure defines a CreateManipulator() method, which creates a Manipulator-derived type. LineFigure creates a LineManipulator instance, TextFigure creates a TextManipulator instance, and so on. In this way, the FactoryMethod connects the Figure hierarchy to the Manipulator hierarchy in a fairly natural way.

*Creator may or may not have additional responsibilities.* In the classic GOF hierarchy, Creator has little by way of responsibility beyond that of creating Product instances; however, in several of their examples (including the Figure/ Manipulator example mentioned above), it is implicit that the Creator has more than just creational responsibilities. To some, this will feel like a violation of the "one thing and only one thing" rule around design. Likely this will depend on the degree of complexity inside of the Creator and/or its subclasses.

*Creator may or may not return a default implementation.* If there is a reasonable default for Product subclasses, the Creator base type may return new instances of that, and so long as the default is good enough, thus removes the obligation to subclass from the clients when creating new Creators. Similarly, if Creator can "generalize" the creation process (perhaps using a facility of the language or runtime to be able to instantiate by class name, rather than compiled type, such as how Java uses Class.forName or the CLR uses Assembly.Load), then additional subclasses of Creator may not be necessary.

## Relationships

## Variations
There are several variations on Factory Method.

### Registry
In some scenarios, we want to make use of a [FactoryMethod](FactoryMethod.html) (or [AbstractFactory](AbstractFactory.html)) to return objects out of a bound set of subtypes based on some kind of parameterized request. (Classic examples are the Microsoft Windows use of the Windows Registry for COM object construction, or the JDBC DriverManager to construct JDBC Connection objects.) In this case, the FactoryMethod is "registered" with a (typically [Singleton](../Singleton.html)) Registry, to which all the object requests are deferred, and the Registry is itself deciding which version of the ConcreteCreator to use to construct the ConcreteProduct object requested.

Note that the Registry will actually be a two-step process: first, the decision-making step in which the Registry decides which ConcreteCreator to use, and the second, the actual construction process. In languages that support them, either (or both) of these steps can be standalone functions (if the Creator has no other responsibilities, of course.)

### Constructor Function
If we consider pure-functional (or "functional-first") languages, then this goes by a slightly different name, the [Constructor Function](../ConstructorFunction.html), in which we localize knowledge about which kind of entity to create to a standalone function, rather than to a class or class hierarchy.  However, if there are numerous types of objects that want to hide behind an encapsulatory barrier (a function in a functional language), then the Constructor Function effectively now serves as a "Factory Function", and is essentially a Factory Method but using a function by which to do the decision-making and logic-hiding, rather than a class instance or static method.

The use of a Constructor Function will depend greatly on whether the Creator provides additional behavior beyond just the construction of the Product types; if so, the Constructor Function will likely be a part of a larger class (the Creator), and more in line with the traditional FactoryMethod. Some of the implementations of FactoryMethod shown above demonstrate how to vary the creational method by passing in (and storing) Constructor Functions, allowing for flexible construction without having to subclass.

### Factory Module
For some languages which support explicit module syntax and semantics, and top-level functions, the Factory Method can be buried behind the module interface, rather than an object instance, but providing the same kind of experience. Typically this will be more "large-scale" than the standard Factory Method, in that the module is always expected to be stateless or a [Singleton](../Singleton.html), and as such, generally won't have much in the way of additional responsibilities.

## Relationships
Factory Method base classes are often [Singletons](../Singleton.html), since they maintain no state and usually want to be in a well-known and namespaced name for easy reference.

Factory Method will often produce instances of [Composite](../Composite.html) or [Facade](../Facade.html) for client use. For aggregate types, they will frequently produce [Iterators](../Iterator.html).
