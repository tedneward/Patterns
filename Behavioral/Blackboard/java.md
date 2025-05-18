title=Blackboard: Java
date=2016-03-08
type=pattern
tags=pattern implementation, behavioral, java
status=published
description=A Blackboard implementation in Java.
~~~~~~

Building a Blackboard at the design pattern level faintly resembles a [Chain of Responsibility](../ChainOfResponsibility/) in that the knowledge sources form a chain to which the current state of the solution is offered for examination. Several key differences emerge, however, most notably that where a Chain of Responsibility has no consideration or concern about the order of execution of handlers in the chain, a Blackboard specifically wants to allow each processor component to examine when it chooses to run. This is often captured in a controller component that decides who gets to run next, but it can be helpful to defer that decision-making to the individual components for easier update/modification.

<!--more-->

