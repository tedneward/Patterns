title=Proxy
date=2022-02-25
type=pattern
tags=pattern, structural
status=published
description=Provide a surrogate or placeholder for another object to control access to it.
~~~~~~

***tl;dr*** The Proxy pattern ...

<!--more-->

## Problem

## Context

## Solution

Some questions arise out of this:

## Implementations

## Consequences
A Proxy tends to lead to several consequences:

## Relationships

If the Proxy wraps another object entirely (such as the Java examples in the Variations section below), it might be seen as acting more like a [Decorator](../Decorator/) than a Proxy, depending on whether the behavior of the underlying object is being modified.

## Variations
A couple of different takes on the Proxy include:

### Forwarder-Receiver

This was published in *Pattern-Oriented Software Architecture*, Vol 1.

Often called "stubs and skeletons" (CORBA) or "proxies and stubs" (DCOM), distributed system RPC toolkits often code-generated Proxy instances at compile-time, which could then be called (client-side) or subclassed to provide implementation (server-side). In this manner, the generated client-side Proxy is acting as a distributed form of [Flyweight](../Flyweight/), often to the point that a given Proxy instance would in fact notify the server of its existence in order to keep the server-side instance alive longer via reference-counts.

### Access Control

Java used the Proxy pattern for authorization and encryption with the creation of two separate object types that would "wrap" a Java Serializable object and add additional layers of functionality; `java.security.SignedObject`, which allowed for creating authentic runtime objects whose integrity cannot be compromised without being detected, and `java.security.GuardedObject`, which encapsulates a target object and a `Guard` object, such that access to the target object is possible only if the guard allows it, and a `javax.crypto.SealedObject, which can encapsulate the original object in serialized format (i.e., a "deep copy") and encrypt its serialized contents using a cryptographic algorithm.

