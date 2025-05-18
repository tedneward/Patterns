title=Adapter
date=2022-03-15
type=pattern
tags=pattern, structural, architectural
status=published
description=TODO
~~~~~~

***tl;dr*** Adapters are ...

<!--more-->

## Problem

## Context

## Solution

Some questions arise out of this:

## Implementations

## Consequences
A Adapter tends to lead to several consequences:


## Variations
A couple of different takes on the Adapter include:

### (Architectural) Adapter
We frequently see the Adapter used to connect two disparate distributed systems together, either by assuming the same interface as the target and providing some adaptive behavior, or by acting as a pass-through adapter taking source requests and shifting them to the target's interface and communication style. This is how many legacy systems are adapted to be accessible through HTTP APIs, for example.


