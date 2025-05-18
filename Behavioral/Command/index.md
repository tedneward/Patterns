title=Command
date=2022-02-25
type=pattern
tags=pattern, behavioral
status=published
description=(TODO)
~~~~~~

***tl;dr*** Patterns, 20 Years Later: Command ...

<!--more-->

## Problem

## Context

## Solution

Some questions arise out of this:

## Implementations

## Consequences
Command tends to lead to several consequences:


## Variations
A couple of different takes on the Command include:

* *Unit of Work*: Frequently, particularly in data-persistence systems, we have a desire to put explicit "boundaries" around several engagements to the database. Were these entirely database-specific, they might be bundled into a transaction, but to open and hold a transaction from a middleware (or client-side) component is undesirable. Thus, we "bundle up" the work to be done into a single conceptual entity, referred to as a Unit of Work.


