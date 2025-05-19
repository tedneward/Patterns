# Command
Command ...

## Problem

## Context

## Solution

Some questions arise out of this:

## Implementations
In the implementation sections below, we take a page from the GOF book and create a "text editor" (really just a simulacrum) that consists of a `TextEditor` that in turn holds a `TextField` that holds a simple string. We can issue commands (here through an API, but in the GOF book via a keyboard or menu selection) to modify the text, using `Command` objects, and each object retains enough state to not only carry out the command, but to "undo" and "redo" each operation.

* [Swift](swift/)

## Consequences
Command tends to lead to several consequences:


## Variations
A couple of different takes on the Command include:

* **Unit of Work**: Frequently, particularly in data-persistence systems, we have a desire to put explicit "boundaries" around several engagements to the database. Were these entirely database-specific, they might be bundled into a transaction, but to open and hold a transaction from a middleware (or client-side) component is undesirable. Thus, we "bundle up" the work to be done into a single conceptual entity, referred to as a Unit of Work.

* **Transaction Script** (PEAA xxx): Fowler's Transaction Script pattern is, at its heart, a procedural series of actions to take, usually under transaction scope. In this scenario, the Command often carries with it a great deal more context to it, 

