title=Message-Passing Interface
date=2022-03-10
type=pattern
tags=pattern, structural, architectural
status=published
description=Define an interface for a component that takes a message (a name and a variable number of named values) and processes it, either synchronously or asynchronously.
~~~~~~

***tl;dr*** Message-Passing Interfaces are ...

<!--more-->

## Problem

## Context

## Solution

Some questions arise out of this:

## Implementations

## Consequences
A Message-Passing Interface tends to lead to several consequences:


## Variations
A couple of different takes on the Message-Passing Interface include:

### Event-Driven Architecture
Architecturally, when components use messages to signal that particular criteria have been met or that state has changed, we call that an Event-Driven Architecture. 

### Parameter Object
Fowler's Parameter Object (PEAA, ###) suggests taking the parameters to a method and putting them into a single object and passing that object as the sole parameter; if this is generalized across all the methods, and the method name is included as part of the object, we have essentially created a Message-Passing Interface.




