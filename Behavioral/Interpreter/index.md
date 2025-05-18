title=Interpreter
date=2022-02-25
type=pattern
tags=pattern, behavioral
status=published
description=(TODO)
~~~~~~

***tl;dr*** Patterns, 20 Years Later: Interpreter ...

<!--more-->

## Problem

## Context

## Solution

Some questions arise out of this:

## Implementations

## Relationships

Given how often an Interpreter uses a parse tree/AST (abstract syntax tree) as its source format, there is a reasonable argument that an Interpreter is simply a [Visitor](../Visitor/) across an AST while maintaining state. Were the only executable format a tree, that would make sense, but because an Interpreter can execute a variety of different things--such as an intermediate bytecode format, for example--it makes sense that these two remain entirely independent patterns. That said, anyone building an Interpreter will likely want (perhaps need) to be conversant with Visitor.

## Consequences
An Interpreter tends to lead to several consequences:


## Variations


