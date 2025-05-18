title=Value Object
date=2022-03-09
type=pattern
tags=pattern, structural
status=published
description=Some objects represent values and have no intrinsic sense of identity or uniqueness. As such, these objects must present different semantics around their use.
~~~~~~

***tl;dr*** A Value Object ...

<!--more-->

## Problem
"Mutable objects are the bedrock of object-oriented design and programming. The ability to dynamically instantiate objects, change their value, retrieve their state, and share them among multiple users for communication allows the development of efficient software with a close relationship to real word models. ... Yet, another bedrock of object-orientation is encapsulation (see section 2.2.2 on page 32). Encapsulated objects provide us with the peace of mind that every change to an object is controlled by the object itself. Obviously, an inconsistent or unexpected change of object state can be caused by the object itself only. Unfortunately, this is not quite true. ... [However], aliasing with unwanted effects is the standard behavior."

## Context

## Solution
"One way to control such introduced side-effects is to disallow the application of operations that mutate object state. Hence, it appears useful to introduce an immutable interface."

Some questions arise out of this:

## Implementations

## Consequences
Value Objects tend to lead to several consequences:


## Variations
A couple of different takes on Value Object include:

### Data Transfer Object (DTO)
DTOs are essentially Value Objects designed for transmission across network links; they are, in essence, bundles of data, typically intended as part of a query or query response, and quite often have no domain or business behavior defined. In languages which support tuples natively, these are most often a tuple implementation, although sometimes there may be some optionality semantics that are hard to capture with a tuple.
