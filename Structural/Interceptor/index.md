title=Interceptor
date=2016-03-25
type=pattern
tags=pattern, structural
status=published
description=Provide additional functionality to an existing component by "intercepting" a request to the component and either replacing or supplementing the component's behavior.
~~~~~~

*tl;dr* Interceptors were popular particularly in distributed systems (CORBA, EJB, DCOM) providing transactional capabilities: an interceptor would begin a transaction, allow the component to carry out its work (usually involving some work against a database), and then commit, abort, or roll back the transaction based on the results of the component's behavior. This concept generalizes well across a variety of different systems, however, and was captured as a pattern in POSA2.

<!--more-->

In addition to being useful around distributed system middleware, we see interceptors show up in a number of areas, including some of the more crude "aspect-oriented" implementations that used proxies or code-generation (either compile-time or run-time) to put behavior before, around, or after a component method or field. Additionally, component containers (such as a Java Servlet engine from the Java Enterprise Edition specifications) often provide interceptor functionality (such as "filters" in the Java Servlet specification). Lastly, at the operating system level, it's often been a useful exercise to "intercept" calls to a particular dynamically-loaded library (DLL or libso or dylib) and replace the loaded function with an Interceptor that then decorates/wraps the original call with new behavior.

## Problem

Often, behavior is common yet independent of a particular family of objects, and therefore is difficult to capture using traditional object-oriented means. We often look to frameworks and libraries and containers to provide those sorts of cross-cutting behaviors as a result, but frameworks/libraries/containers cannot anticipate all of the services/behavior that they will be expected to offer clients. In many cases, that behavior might also be outside the realm of the framework/library/container's area of responsibility, and so capturing it inside that framework or library or container would be a semantic violation of encapsulation.

## Context

* *The additional behavior is common.* If it's common enough to merit consideration under the DRY principle, then it's common enough to want to capture in some kind of reusable form. One-off behavior is better written inside the component itself.

* *The behavior is not specific to a particular domain family.* 

* *A framework should allow integration of additional services without requiring modifications to its core architecture.*

* *The integration of application-specific services into a framework should not affect existing framework components, nor should it require changes to the design or implementation of existing applications that use the framework.*

* *Applications using a framework may need to monitor and control its behavior.* 

## Solution

Allow clients to extend a framework/library/container transparently by registering 'out-of-band' services or behavior via predefined "hook points", then let the framework/library/container trigger these services automatically when certain events occur.

In detail: for a designated set of events processed by a framework, specify and expose an interceptor callback interface. This callback interface can be either a traditional O-O interface (so that interest parties can use inheritance to gain type-safety and -compatibility) or it can be a named first-class function reference. Applications can derive concrete interceptors from this interface, or provide [Strategy](../../behavioral/Strategy/)-based/first-class-function objects, to implement out-of-band services that process occurrences of these events in an application-specific manner. Provide a dispatcher for each interceptor that allows applications to register their concrete interceptors with the framework. When the designated events occur, the framework notifies the appropriate dispatchers to invoke the callbacks of the registered concrete interceptors.

Then, to create an interceptor, create a block of code that can be "hooked" into place (sometimes silently, replacing the original component's method, such that callers to the original component's method are now invoking the Interceptor). The Interceptor can (and should) have access to the parameters passed in to the method call, but generally also pass those parameters (possibly modified) on to the original component's method for processing, and take the return value from that processing and hand it back (possibly modified) to the original caller.

## Implementations

## Consequences

* *Behavior can be dynamically determined.* Interceptors are often (though not always, depending on the tool and/or the need) installed at runtime, thus allowing for runtime configuration based on conditions that may not be known at compile-time or deploy-time, or even entirely based on user preferences or choices.

* *Performance.* Each interceptor adds additional processing (which often means one or more additional method call blocks) to the cost of carrying out the component's original behavior; too many such interceptors, and the performance cost of calling the component's behavior becomes buried in the surrounding "noise" of the additional behavior.

* *Clarity.* The ability to define the additional behavior in an Interceptor is both blessing and curse--the additional behavior is captured in one place, but it can sometimes be counterintuitive for additional behavior to be executing that isn't obvious when looking at the code for the original component, particularly if the Interceptor is installed at runtime. For example, when looking at a servlet, developers may be expecting to find authorization checks within the servlet code, and upon not finding them (and not knowing the authorization checks are actually happening within a filter that executes prior and after the servlet processes the request), conclude that they never happen.

## Relationships

Interceptors will often be passed or have access to a [Context Object](../ContextObject) as part of the call for support, in which can be found the parameters used to invoke the original component to be available, as well as any additional supporting data or functionality. Context objects can also allow a concrete interceptor to introspect and control certain aspects of the framework's internal state and behavior in response to events.

If the Interceptor is a "silent" replacement of the original method, it obeys the same call signature (explicit or implicit) of the orignal maethod, and thus provides a common footprint for multiple Interceptors to "stack", one after the other, each providing some specific behavior to a variety of different components that otherwise have nothing to do with each other, thus forming a chain like a [Chain of Responsibility](../../behavioral/ChainOfResponsibility/), but where the Chain generally looks for the first handler and aborts the remainder of the chain, interceptors are usually all guaranteed a chance to run and provide the behavior desired.

If the Interceptor is a "silent" replacement of the original, Interceptors are often modeled using [Proxy](../Proxy/), but the use of a proxy object is not required--only that the original component's behavior is adjusted. Proxies can often hold some interesting state of use only to the interceptor, but that can also be held via [Closure-Based State](../ClosureBasedState/) in situations where the original object must be maintained in place.

[Dynamic Objects](../DynamicObject) are particularly easy to use in conjunction with Interceptors, as the meta-object replacement of the object's behavior with the Interceptor is usually a trivial operation--however, the interceptor must be careful to retain a reference to the original behavior if the original behavior is to remain accessible to the Interceptor.

If all of an object's methods are valid targets for an Interceptor, and the additional behavior is known at compile-time (or at least at the time of instantiation), a [Decorator](../Decorator) may in fact be a more effective and/or efficient means of behavioral supplementation.

Interceptors are often made out of a [Continuation Chain](../../behavioral/ContinuationChain/), where the Interceptor is passed an explicit function parameter as the next step in the chain. This provides flexibility for the Interceptor to choose not to pass the call onwards, essentially using the Interceptor as more of a [Chain of Responsibility](../../behavioral/ChainOfResponsibility/), but this also runs the risk of a poorly-written Interceptor not invoking the next step in the chain as it is expected to, and thereby breaking expectations.

## Variations


