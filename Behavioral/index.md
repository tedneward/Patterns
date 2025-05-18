title=Behavioral Patterns
date=2021-06-08
type=patternsindex
tags=patterns
status=published
description=Behavioral Patterns, 20 years later
patternfilter=behavioral
~~~~~~

***tl;dr*** Behavioral patterns specifically deal with the flow of control and being able to change, modify, capture, or segment that flow of control that contributes to the larger whole. They abstract the flow in some way "above" that of a single method or procedure call, so that the flow can be decoupled, disconnected, or adjusted at runtime in some fashion. They help make a system more independent of how flow is captured, allowing for composition and/or creation without requiring a full rebuild of the system.

<!--more-->

The Gang of Four wrote:

> Behavioral patterns are concerned with algorithms and the assignment of responsibilities between objects. Behavioral patterns describe not just patterns of objects or classes but also the patterns of communication between them. These patterns characterize complex control flow that's difficult to follow at run-time. They shift your focus away from flow of control to let you concentratejust on the way objects are interconnected.

Behavioral patterns feel like one of the hardest ones to classify, because they tend to use structure (inheritance and the like) to help direct and shift the flow of control, and that's sometimes hard to separate from [structural](../structural/) patterns on the surface of things. The key is to focus on the original intent: behavioral patterns' focus is on the flow of control and operations through (typically) a collection of objects (or functions), and the physical structure is there simply to aid and abet in that flow somehow--either by making it easier to chain that flow ([Chain of Responsibility](./ChainOfResponsibility/)), or modify that flow ([Null Object](./NullObject/)), or represent the flow as a discernible object ([Command](./Command/)). It's all about going with the flow.

### Segregation
Initially, the Gang of Four "split" the behavioral patterns into "class" and "object", based on the idea that "class" made use of inheritance to vary the behavior desired, whereas "object" delegated behavior to another object. In the world 20 years later where (a) inheritance is no longer the huge force that it was back in 1995, and (b) functional and dynamic languages don't stress inheritance anywhere nearly as strongly as an object-oriented language would, it's not clear whether this distinction is still meritable.

> Behavioral class patterns use inheritance to distribute behavior between classes. ....  Behavioral object patterns use object composition rather than inheritance. Some describe how a group of peer objects cooperate to perform a task that no single object can carry out by itself. 

For those languages that support inheritance, obviously it continues to be a viable option, but functional languages--in particular currying and partial function application--offers up a number of options that don't require inheritance. Much of the "revisisted" here will be about using functional operations (sometimes in conjunction with object tools, sometimes standlone) to help make it easier to capture and use behavioral patterns. The ability to pass functions as first-class values opens up a lot of options, and in many cases makes the implementation of a particular pattern's intent easier.

> An important issue here is how peer objects know about each other. Peers could maintain explicit references to each other, but that would increase their coupling. In the extreme, every object would know about every other. 

[Template Method](./TemplateMethod/) is an abstract definition of an algorithm; it defines the algorithm step by step, and each step invokes either an abstract operation or a primitive operation. Using inheritance then, a subclass fleshes out the algorithm by defining the abstract operations. If we put some sort of "front end" on the steps to take, we arrive at [Interpreter](./Interpreter/), which represents a grammar as a class hierarchy and implements operations on instances of these classes according to the semantics of the grammar. Interpreters often are formed into an AST (Abstract Syntax Tree) to help insulate grammar from execution, which (loosely) resembles the [Mediator](./Mediator/) pattern, which avoids tight coupling of objects by introducing a mediator object between peers, which provides the indirection needed for loose coupling.

A [Chain of Responsibility](./ChainOfResponsibility/) provides even looser coupling. It lets you send requests to an object implicitly through a chain of candidate objects. Any candidate may fulfill the request depending on run-time conditions. The number of candidates is open-ended, and you can select which candidates participate in the chain at run-time. *(THOUGHT: What about pipes-and-filters? Seems the intent is different from a Chain of Responsibility.)*

The [Observer](./Observer/) pattern defines and maintains a dependency between objects. The classic example of Observer is in Smalltalk Model/View/Controller, where all views of the model are notified whenever the model's state changes; despite the same name, this is a different view of MVC than what the server-side Web frameworks tended to espouse (which was more of a Presentation-Abstraction-Controller, from POSA1), but client-side frameworks like Angular tend to come closer to traditional Observer behavior. Observers combined with a [Message-Passing Interface](../structural/MessagePassingInterface/) are sometimes referred to as "publish-subscribe" systems, though these are also very often used as [Chains of Responsibility](./ChainOfResponsibility/).

Originally more of an architectural pattern, a [Service Locator](./ServiceLocator/) acts as something of a [Mediator](./Mediator/) in that it serves to connect a client to an implementation--in the macro sense, for a client to find a remote service to invoke, in the micro sense it might be more of a lookup to obtain another object or suite-of-objects (a [Strategy](./Strategy/), perhaps, or a [Chain of Responsibility](./ChainOfResponsibility/)) to carry out one or more actions.

Other behavioral object patterns are concerned with encapsulating behavior in an object
and delegating requests to it. 

[Strategy](./Strategy/) encapsulates an algorithm in an object, making it easier to specify and change the algorithm an object uses. [Command](./Command/) encapsulates a request in an object so that it can be passed as a parameter, stored on a history list, or manipulated in other ways. [State](./State/) encapsulates the states of an object so that the object can change its behavior when its internal state object changes without clients needing to know about the change in behavior due to the change in state. [Visitor](./Visitor/) encapsulates behavior that would otherwise be distributed across classes (sometimes known as "double-dispatch"), and [Iterator](./Iterator) abstracts the way you access and traverse objects in an aggregate.
