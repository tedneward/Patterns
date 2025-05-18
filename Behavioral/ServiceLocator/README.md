title=Service Locator
date=2024-08-10
type=pattern
tags=pattern, behavioral
status=published
description=(TODO)
~~~~~~

***tl;dr*** Patterns, 20 Years Later: A Service Locator is used to encapsulate the process(es) involved in obtaining a service via a strong abstraction layer. This pattern uses a central registry, which on request returns the information necessary to perform a certain task. 


<!--more-->

## Problem

## Context

## Solution

Some questions arise out of this:

## Implementations

## Consequences
A Service Locator tends to lead to several consequences:

* *Obscurity of dependencies.* Because the client never sees the implementation directly, users of the functionality on the other side of the Service Locator aren't aware of what's necessary (in terms of runtime deployment artifacts) to have complete functionality available.

* *Obscurity of locality.* Similarly, if the service obtained is a remote service, the fact that it appears to be local can lead clients into a false sense of understanding where remote boundaries are, which can lead to accidental subscription to the Fallacies of Distributed Computing.

## Variations
A couple of different takes on the Service Locator include:


