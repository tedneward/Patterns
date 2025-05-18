title=Observer
date=2022-02-25
type=pattern
tags=pattern, behavioral, architectural
status=published
description=(TODO)
~~~~~~

***tl;dr*** Patterns, 20 Years Later: Observer ...

<!--more-->

## Problem

## Context

## Solution

Some questions arise out of this:

## Implementations

## Consequences
A Observer tends to lead to several consequences:


## Variations
A couple of different takes on the Observer include:

### Event Bus
When messages are being sent to a single message queue or topic with multiple registered handler/processors, we refer to that as an Event Bus. If the first interested handler/processor on the bus pulls the message off the bus, it is a [Chain of Responsibility](../ChainOfResponsibility/); if the messages continue down the bus so that all interested handler/processors get a chance to see it, it is more of an Observer.

