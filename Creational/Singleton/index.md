title=Singleton
date=2016-03-25
type=pattern
tags=pattern, creational
status=published
description=Ensure only one instance of a given type, and provide a global point of access to it.
~~~~~~

*tl;dr* Everybody's favorite (and most despised) pattern, the Singleton. Conceptually, Singleton seems the easiest of the lot to understand and (in most post-1995 languages) the easiest to implement. But everybody hates it because its singleton-y nature means it is a natural target for concurrency problems up the wazoo. (And then there's that whole "Singleton instance vs static methods" debate that goes on.) After all, globals are bad--unless we call them Singletons!

<!--more-->

## Problem
We want to ensure only one instance of a given type, and provide a global point of access to it.

## Context
*"There must be exactly one instance of a class, and it must be accessible to clients from a well-known access point."* For whatever domain reason, we must have exactly one instance of a given type. (Typically, in a class-based language like Java/C#/C++, this means one instance of a given class type.) This instance should be accessible from anywhere in the codebase, but because we generally want to retain some level of encapsulation over this instance, there should be some level of indirection or access control to the instance, so that we have the ability to change/evolve/refactor.

*When the sole instance should be extensible by subclassing, and clients should be able to use an extended instance without modifying their code.* One of the interesting subtexts around Singleton, as mentioned above, is that whole "instance vs statics" debate: When does a Singleton make sense, as opposed to just a set of statics (methods/properties/fields)? If there is some need/desire to subclass the Singleton type, then the discussion becomes much clearer (although there are a few languages in which static methods can be "overridden" in derived classes, which brings the discussion back again.)

## Solution
Provide some global/static access entity to obtain that single instance.

Some interesting questions come up when implementing a Singleton:

* *Method or property?* Most programming languages have, by this point, created some kind of field-like/method-like hybrid called a "property" that offers the ability to do some level of code execution but retaining field-like syntax to the client. Since C++ had no explicit notion of properties, the original GOF book used a static method; when we get to other languages, however, this isn't quite as clear. Frankly, this feels (most of the time) to be an aesthetic choice. On the one hand, it may look a little weird to some programmers to use "Class.Field.DoSomething()" as a syntax, particularly to us old-hand C++ developers, since we tend to not see field-like references chained together like that very often. On the other hand, in languages that have properties (C#, for example), it's much more common. As a result, it seems like this can remain an implementation choice based mostly around the coding style of the organization doing the implementation.
* *Scope of the singleton.* The original GOF book assumed Singletons would be scoped to the process running the code, since that was mostly the only option popularly available/recognized at the time. However, in the years since, we've seen processes get much bigger (app servers and the like) and much smaller (hello, mobile! hello, containers!), and even to within the process (hello, threads!). As a result, process-wide scope may not always be the best choice. Additionally, as soon as the Singleton moves outside the process to becoming a Distributed Singleton, really bad things start to happen---the Distributed Singleton is essentially hanging a sign on the code that reads, "WANTED: Massive Bottlenecks and Lock Contention, Apply Here!"
* *Concurrency-safety.* Given that the Singleton is a single instance, and given an environment in which multiple paths of execution (generally threads) are happening simultaneously, the Singleton needs to take great care to make sure it is concurrency-safe. This is not trivial, particularly since (depending on the domain issues) the concurrency may need to happen across multiple method calls/property accesses on the Singleton instance (meaning: simply marking all the methods as "synchronized" or "lock"ed will not guarantee safety). Considering that much of the reason that developers like Singleton is its easy ability to hold state, and that state is the principal reason we need concurrency-safety, Singleton usually is a Pandora's Box when being used in a concurrently-executing environment.
* *Class or ...?* Some languages (F# and JavaScript come to mind) have the ability to provide an additional (larger-than-a-class/object) scoping mechanism, which they will often refer to as "modules". As a result, it's often possible to achieve the same benefits of a Singleton by defining the instance at the module level, and still reap the same benefits. (In fact, in some environments---many Javascript interpreters, for example---the global namespace is simply the unnamed module, or a module named "GLOBAL" or something along those lines, and "global fields" and "global methods"/"top-level functions" are simply members of that global module; seen this way, it really becomes unclear how a Singleton is different from a global, save for the controlled-access discussion.) Scala, for example, allows for "companion objects" that appear to be statics, are actually instances, and offer a great deal of the same benefits as a Singleton.
* *Eager or JIT?* When should the Singleton instance come into existence? The first time it is accessed (which means that for those environments that never use it, they never pay the cost for its instantiation and initialization), or as eagerly as possible (reasonably speaking)? Classic GOF implementations used a method and created the instance on demand (the first time it was accessed). When C++ developers started to wrestle with concurrent-execution environments, they discovered that it was (easily) possible for two threads to be accessing the Singleton's access method for the first time, leading to a potential double-initialization scenario (with the last one being stored and the rest being silently thrown away), which was obviously a violation of "only one". This led to the "Double-Checked Locking Idiom", which then turned out to be a false sense of security unless it was accompanied by thread-safe locking. Later, Java implementations used a static-initializer (which is fired as soon as the class is loaded into the runtime) to initialize the Singleton, so it was guaranteed to be available as soon as the first client sought to use it. Since static initializers were (mostly, but only mostly) guaranteed to be thread-safe, this "eager" initialization seemed preferable.
* *Enforcing Singleton-ness.* In some cases, the Singleton-ness is something we wish to enforce entirely---not only should one be created for us ahead of time, but we want to make sure that nobody can ever create one themselves. In this case, typically the constructor/initializer for the type or object should be somehow made unavailable to clients, usually by way of marking constructors (or construction methods) as inaccessible ("private").

Some of these are explored in the implementation examples.

Note that some solutions seek to have singleton-ness imposed externally, by wrapping an object instance into a mechanism that "transforms" the object instance into a Singleton. This approach usually cannot enforce Singleton-ness on its own (somebody had to create one, so that means others are able to create one), except in some languages where the object's definition doesn't need to be known in order to use it (a la an interface/implementation split in Java or C#, or using a dynamically-resolved language like Ruby or Javascript). These provide an easy way to put an object into a "known location" (making it more easily accessible from wider contexts---or from any context, as the desire may be), but lacks the ability to enforce the Singleton-ness that Singleton usually stresses.

## Implementations

* [Swift](./swift.html)
* [Java](./java.html)
* [Scala](./scala.html)
* [JavaScript](./javascript.html)
* [C#](./csharp.html)
* [F#](./fsharp.html)

## Consequences
Singleton yields the following:

* *Controlled access to sole instance.* By encapsulating access to it, it's easier to replace it or refactor it.
* *Reduced name space.* "Singleton" is just "global" hiding behind another name. One of the explicit goals (in 1995) was to be able to have the necessary scope-wide state, but without accidentally clashing over names in that global namespace. Languages which support explicit namespacing (Java, C#, C++, Swift, yeah pretty much all of them) mean that we can have this benefit even without doing anything more than moving the global variable into one of those namespacing mechanisms.
* *Permits refinement of operations and representation.* The GOF book originally explained this point as "The  Singleton class may be subclassed, and it's easy to configure an application with an instance of this extended  class. You can configure the application with an instance of the class you need at run-time." The most obvious example of this (that I can think of) comes from the old C++/Windows toolkits, Microsoft's MFC and Borland's OWL; each had a Singleton "application" object instance of a class that had to derive from a base class coming from the framework (CApp in MFC's case, TApplication in Borland's). Borland then made use of a more explicit Singleton implementation, while Microsoft expected the developer to instantiate the CApp-derivative as a global object (which, since this needed to link against the MFC code that used it, had to be called by a specific name). 
* *Permits a variable number of instances.* This is where things get interesting, and the Singleton becomes alone no longer---because of the access control mechanism (whatever that happens to be), it becomes trivial to refactor the code from handing out a single instance to handing out a new instance each time to handing out up to a finite number of instances. As a matter of fact, this suggests that most object pools (such as a database connection pool) is a variant of Singleton.
* *More flexible than class operations.* The GOF wrote that statics "make it hard to change a design to allow more than one instance of a class. Moreover, static member functions in C++ are never virtual, so subclasses can't override them polymorphically." While that is true of C++ (and most languages like it, a la Java and C#), this actually does not always hold true, particularly for languages which engage in some syntactic slight-of-hand (such as how Scala can create "utility objects").

## Variations
A Singleton that takes a String or some other key as a parameter in its instance-accessing method is often called a Registry; generally the context there changes from "ensure only one instance of this object" to "ensure only one instance of this object, as well as any other object accessible via a key lookup". (If the Registry gets too large or too complicated, it usually turns into a database.)

Many systems took the Singleton and stretched it across multiple processes in a distributed system. This resulting Distributed Singleton usually caused nothing but pain and suffering to the system once it started to scale up (and often long before then). To be avoided at almost any cost. (There may be a very few edge cases where a Distributed Singleton is useful, but the bar should be very high when thinking about using it.)





