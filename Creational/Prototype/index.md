title=Prototype
date=2022-03-09
type=pattern
tags=pattern, creational
status=published
description=Specify the kinds of objects to create using a prototypical instance, and create new objects by copying this prototype.
~~~~~~

*tl;dr* Prototype has always been one of those patterns that nobody in the strongly-typed OO world really spent much time thinking about, but when Javascript came along and forcibly (re-)introduced people to the concept of prototypical construction, it brought some new life to the idea.

<!--more-->

## Problem

## Context

Use the Prototype pattern when a system should be independent of how its products are created, composed, and represented; and

* when the classes to instantiate are specified at run-time, for example, by dynamic loading; or
* to avoid building a class hierarchy of factories that parallels the class hierarchy of products; or
* when instances of a class can have one of only a few different combinations of state. It may be more convenient to install a corresponding number of prototypes and clone them rather than instantiating the class manually, each time with the appropriate state.

The big question that emerges from Prototype is "how deep"--that is, when we clone a given object, are we cloning just that object, or all of its associated/referenced objects, as well? Put into practical terms, if we clone the Person object, which holds a field for "spouse" and a field for a collection of "children" Person objects, do we clone the Person's spouse and any children objects as well? Are we cloning the entire family, or just the individual Person object? In some cases, if the referenced object is a [Value Object](../../structural/ValueObject/), 

## Solution
A couple of interesting questions emerge when thinking about implementing a Prototype:

* *Where is the creator?* The classic GOF pattern suggests that any instantiated object (that is, any object that is participating in the Prototype construction scheme) can be cloned, but this means that we get a cloned copy of that particular object, when in fact we may want a "pristine" clone to work from. Choosing the latter takes us closer to the [Builder](../Builder/), since now that particular "source" object becomes the source of created objects, but it can be useful to have a small subset of Prototype-generated objects serving as "templates" for constructing other objects of similar form. In any event, it's important to understand whether cloned objects can in turn create clones, or whether that is intended to be scoped to a particular "source".

* 


## Implementations

## Consequences

## Relationships

## Variations


