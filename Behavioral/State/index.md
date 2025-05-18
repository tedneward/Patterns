title=State
date=2022-02-25
type=pattern
tags=pattern, behavioral
status=published
description=(TODO)
~~~~~~

***tl;dr*** Patterns, 20 Years Later: State ...

<!--more-->

## Problem

## Context

## Solution

Some questions arise out of this:

## Implementations

## Consequences
A State tends to lead to several consequences:


## Variations
A couple of different takes on the State include:

* *Balking Object.* This is often seen in concurrency scenarios, where an object will not execute certain methods against it (either blocking or returning an error/exception) while the object is in a "not good" state, and then returning viable results when it is in the acceptable state. Many "future" objects (as in objects that are placeholder for results once they are calculated) fall into this category.

