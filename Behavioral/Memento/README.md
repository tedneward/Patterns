title=Memento
date=2022-02-25
type=pattern
tags=pattern, behavioral
status=published
description=Without violating encapsulation, capture and externalize an object's internal state so that the object can be restored to this state later.
~~~~~~

***tl;dr*** Memento was one of the more interesting patterns in that the need for it almost entirely disappears when we move to virtual-machine-based platforms. By definition, a VM has full and intimate knowledge of a given object, and as such, the VM could often provide facilities to help memorialize the internal state of an object without violating encapsulation. Java Object Serialization and .NET Serialization are just two examples of this, particularly since .NET Serialization provided facilities for serialization into a variety of different formats (binary or SOAP/XML). (Ironically, these facilities have since fallen into disrepute for a variety of different reasons.)

<!--more-->

## Problem

## Context

## Solution

Some questions arise out of this:

## Implementations

## Consequences
A Memento tends to lead to several consequences:

## Relationships

Memento is often 

## Variations
A couple of different takes on Memento include:


