# Patterns
A repository containing the text and example code of design patterns. The goals of this enterprise are admittedly audacious: to not only re-visit the original Gang-of-Four patterns, bringing them "up to speed" with modern languages and idioms, but also to incorporate concepts from functional (and object/functional hybrid) languages.

Note that most of these are "design patterns", meaning they are at the level of software implementation design, not "architectural" patterns (such as what we might find from Martin Fowler's "Patterns of Enterprise Application Architecture"). That said, architectural patterns are often made up of, or inspired by, "design patterns", and I find it helpful to have a category of architectural patterns described here as well.

This roughly follows the same format as the original Gang-of-Four patterns catalog, but with a few changes.

* *The original patterns are not duplicated in their entirety*. This is deliberate, in that I wish to avoid any copyright infringement and I want to encourage those who have not read the original book to procure a copy and do so. ***No assertion of copyright of any pattern language text is intended or implied.***

* *The patterns are cast into a "simpler" form.* Rather than the long form the GOF used, I have chosen to try and "simplify" the patterns by following a Problem/Solution/Context/Consequences format, with particular emphasis on Context and Consequences. A few Variations on the patterns have emerged over time, so I try to capture those, as well.

* *Some additional patterns are described here.* In seeking to bring the patterns up to "modern times" and languages, I have chosen to add a few more patterns that I think we have discovered along the way. These, in particular, should be treated with some level of skepticism and suspicion, as they have not been properly workshopped. Additionally, I will be going back through much of my patterns library and looking for additional patterns that seem to fit the rough category the GOF book occupied, and add them here (again, with some level of modernizing, if necessary).

This catalog is a continual work-in-progress; as more patterns are added to the catalog, their links will become active. Readers are encouraged to comment liberally, provide alternative implementations and/or suggest different language-specfic idioms, and/or participate in whatever fashion feels meaningful.

## [Behavioral patterns](Behavioral/)
Patterns which describe the runtime relationship between one entity and another, and the flow of control between them. 

## [Creational patterns](Creational/)
Patterns which specifically deal with the creation of objects/entities in the code.

## [Structural patterns](Structural/)
Patterns which describe the structural (usually compile-time-related, in langauges which are compiled) relationship between one entity and another.

## [Pattern compositions](Compositional/)
I believe that some patterns are, in fact, combinations/compositions of other patterns, and so I want to take a stab at capturing and analyzing them. (I think a number of Fowler's PEAA and the POSA books are made up of some other patterns, arranged in a particular way but interesting and useful nonetheless.)

## [Concurrency patterns](Concurrent/)
Patterns which describe how to execute operations in parallel and safeguard them from the various dangers that arise from doing so.

## [Synchronization patterns](Synchronization/)
Patterns which describe ways to restrict execution in such a way as to prevent two or more independent paths of execution from doing so simultaneously. These could be viewed as idiomatic to a particular platform except that they so frequently get re-implemented at all different levels of scope that it makes sense to call them out as patterns.

## [Architectural patterns](Architectural/)
A number of patterns "widen" well, operating either at the class/design level or at a larger scope (such as a distributed system). I'm personally not entirely sure of the parameters around an architectural pattern, or if an architectural pattern is a design pattern, particularly when I can see some architectural patterns being reasonable design patterns and vice versa, but I'll capture them and refactor later as inspiration/illumination strikes.

# Additional pattern language(s)
I'd like to capture all of these together into a larger pattern language/fabric.

* Pattern-Oriented Software Architecture, vol 1

    * [Whole-Part](Structural/Whole-Part/) (structural): aggregations of components that together form a semantic unit
    * <del>Master-Slave</del> *(combine this with Leader-Followers, from POSA2)* (behavioral): a master component distributes work to identical slave components and computers a final result from the results these slaves return.
    * <del>Proxy</del>
    * <del>Command Processor</del> *(definitely GOF-Command)*
    * View Handler *(sounds like a Chain of Responsibility/Observer hybrid)*
    * Counted Pointer (structural) *(this is an idiom for C++, but the idea of objects "carrying" additional information about themselves to provide additional functionality feels too commmon to be just a variant on Proxy)*
    * <del>Forwarder-Receiver</del> *(aka proxy/stub from DCOM or CORBA stubs/skeletons; definitely a Proxy variant)*
    * <del>Client-Dispatcher-Server</del>: provides location transparency by means of a name service and hides the details of the establishment of the communication connection between clients and servers *(seems like a combination of a Registry and Proxy/Forwarder-Receiver)*
    * Publisher-Subscriber *(variant of Chain of Responsibility? variant of Pipes-and-filters?)*
    * [Layers](Structural/Layers/): structure applications that can be decomposed into groups of subtasks in which each group of subtasks is at a particular level of abstraction; *(structural?)*
    * [Pipes and filters](Behavioral/PipesAndFilters/): a structure for systems that process a stream of data *(structural?)*
    * [Blackboard](Behavioral/Blackboard/): useful for problems for which no deterministic solution strategies are known *(behavioral)*
    * Broker *(feels like a combination of multiple patterns)*
    * Model-View-Controller: divides an interactive application into three components: core functionality, representation, and control, with a change-propagation mechanism to ensure consistency between the three parts *(definitely feels like GOF-Observer/Chain-of-Responsibility hybrid)*
    * Presentation-Abstraction-Control: defines a structure for interactive software systems in the form of a hierarchy of cooperating agents, each of which is responsible for a specific aspect of the application's functionality, principally built out of three components (presentation of information, abstraction, and control). *(this is different from MVC even though it's similar)*
    * [Microkernel](Structural/Microkernel/): separates a minimal functional core from extended functionality and customer-specific parts
    * <del>Reflection</del>: changing structure and behavior of software systems dynamically, supporting the modification of fundamental aspects, such as type structures and function call mechanisms. *(This is a DynamicObject, unless its provided by the underlying language/platform directly)*

* Pattern-Oriented Software Architecture, vol 2 (Patterns for Concurrent and Networked Objects)

    * [Wrapper Facade](Structural/WrapperFacade) (structural)
    * [Component Configuration](Creational/ComponentConfiguration) (creational)
    * [Interceptor](Structural/Interceptor/) (structural)
    * [Extension Interface](Structural/ExtensionInterface/) (structural)
    * Reactor
    * Proactor
    * Asynchronous Completion Token
    * Acceptor-Connector
    * Scoped Locking *(originally presented as a C++ idiom relying on C++'s "RAII" (Resource Acquisition Is Initialization) behavior around determinstic constructors and destructors; not sure if it generalizes well beyond that)*
    * Strategized Locking
    * Thread-Safe Interface
    * <del>Double-Checked Locking Optimization</del> *(this has been proven over and over again to be a naive optimization given insufficient memory model guarantees to prevent out-of-order execution)*
    * Active Object
    * Monitor Object
    * Half-Sync/Half-Async
    * [Leader/Followers](Structural/Leader-Followers/)
    * <del>Thread-Specific Storage</del> *(really, this is a thread-specific [Context Object](structural/ContextObject))*

    The first four are categorized there as "Service Access and Configuration"; the next four, "Event Handling". "Sychronization" covers Scoped Locking, Strategized Locking, Thread-Safe Interface and Double-Checked, and "Concurrency" captures the remaining five.

* [Envoy](../blog/2012/envoy-in-scala-javascript-and-more) This is a set of patterns around how to accomplish various functional ideas. The author originally demonstrated all of his examples in Scheme; a while back [I blogged](../blog/2012/envoy-in-scala-javascript-and-more) about how to implement the patterns in a few other languages. I fully intend to examine each of these and think about where they fit in the above, or, if not, what the new category should be.

    * [Function as Object](behavioral/Strategy/) *(almost certainly a synonym for Strategy in its simplest form, or vice versa, depending on how we want to look at it.)*
    * [Closure](structural/ClosureBasedState/) (I'm calling this "Closure-Based State")
    * [Constructor Function](creational/ConstructorFunction)
    * Method Selector *(A method selector is essentially the ability to identify and pass around an identifier to a method directly--a method pointer, function reference, or delegate, depending on your choice of platform--and inoke it. Many languages have this built in, but there's some nuance to the pattern here that deserves examination, as well.)*
    * [Message-Passing Interface](structural/MessagePassingInterface/)
    * Generic Function
    * Delegation
    * Private Method *(This is probably an idiom for functional languages, not a full pattern, per se; it is built-in syntax for most object-oriented languages, and many languages that support nested-scope functions/procedures can do something similar.)*

* Kuhne's ["Functional Pattern System for Object-Oriented Design"](http://homepages.ecs.vuw.ac.nz/~tk/fps/): Thomas Kuhne wrote his thesis (the above title) on patterns of functional style in OO systems, and his patterns would seem to have direct bearing on this effort. (I was fortunate enough to see an early draft of the work back in the late 90's, and his hand-signed copy of the printed thesis is one of my book treasures.) Again, I'll look for ways to incorporate them into the larger collection here.
  
    * Function Object *(like the Function as Object from Envoy, I think this is a Strategy or something similar to it)*
    * Lazy Object
    * Value Object
    * Transfold
    * Void Value
    * Translator
  
* Monads/monoids. These staples of the functional community seem, to me, to be patterns, but with a bit more rigor implied to them. "Arrows" may be in a similar category.

* Data Access Patterns (DAP): This was published in the early 2000s, during the peak of the patterns wave, and captured a lot of the common thinking around data access (particularly relational data access) at the time. Many of these patterns, I think are compositional in nature, but a few feel a little novel and/or bring a different perspective, and are worth capture.

    * Data Accessor
    * Active Domain Object
    * Object/Relational Map
    * Layers
    * Resource Decorator
    * Resource Pool
    * Resource Timer
    * Resource Descriptor
    * Retryer
    * Selection Factory
    * Domain Object Factory
    * Update Factory
    * Domain Object Assembler
    * Paging Iterator
    * Cache Accessor
    * Demand Cache
    * Primed Cache
    * Cache Search Sequence
    * Cache Collector
    * Cache Replicator
    * Cache Statistics
    * Transaction
    * Optimistic Lock
    * Pessimistic Lock
    * Compensating Transaction

I also plan to go back through some of my patterns books (such as the "Pattern Languages of Program Design" books that were published in the late 90's/early 00's) and cherry-pick some that seem to fit in the above categorization scheme.

In other words, this is going to be a *long* work in progress.

## Implementation Goals
The "implementation" or "examples" section of any pattern discussion holds several goals (and a few "anti-goals"):

* ***Be able to put a concrete-ish example in front of people seeking such.*** It's hard to understand exactly how the pattern is supposed to work without pictures or code. I am not great with graphical tools, so for me it's easier to use code to provide that demonstration.
* ***NOT to expect "ready-made code" for reuse.*** Patterns are not drop-in building blocks that can save you time and energy when doing your own implementation. These examples are here to demonstrate a few techniques around implementation, but attempts to re-use the code directly will probably always meet with failure at some level.
* ***Demonstrate how to do certain things idiomatically within a particular language.*** Each language brings with it a particular idiomatic style or feature set that may shape how one might use a particular pattern. I am not an expert with all of these languages below, but part of this exercise (for me) is to learn (and document) how to exercise the idioms of a particular language using the pattern as a scaffold upon which to hang it.
* ***Provide a mechanism by which to concretely compare and contrast one pattern against another.*** Is a Strategy really all that close to a Command? Having concrete examples of each allows for a certain amount of comparison-and-contrast, and hopefully sparks some good discussion around when to use each.
* ***NOT to suggest that one language is "better" than another.*** Any such qualitative judgment around one language over another is entirely in the eyes of the beholder; no such judgement is intended from me, and any attempt to use this exercise as a means to judge one language more harshly than another will quickly earn this author's scorn. Different languages chose to do things in different ways for very good reasons; if you cannot explain the reasons, then you have no business offering up the judgement.

### Languages
In general, there's a long list of languages I will use to define some example implementations of the patterns in the catalog. Note that while this isn't an "ordered" list, meaning I will probably do implementations in a seemingly-random order, the hope is that when this is all said and done, the list of pattern implementations will range across the following:

#### C++
*An imperative, strongly-typed object-oriented language for native platforms.* This was one of the original laguages used for examples in the GOF book, and it would seem highly useful to revisit them now, particularly given the changes that C++ has seen since 1995.

Specifically, I will be looking for opportunities to incorporate C++11 and C++14 features into the pattern implementations, particularly templates (and the STL), exceptions, code blocks, and more. I will try to remain aware of the idiomatic approach to some of these, but since some are still freshly-minted, there may be no established idioms upon which to draw.

#### Swift
*An imperative, strongly-typed object-oriented language for the LLVM (iOS/Mac OS X) platform.* For the most part, Swift is pretty straightforward as a GOF pattern implementation language, since it more or less finds itself in the same space as its kin, C++, Java and C#. The example implementations were written starting after Swift 2.2 was released; Swift 3.0 is on the horizon, but no clear definition (as of this writing) as to what that feature set will/won't turn out to include.

Note that Swift has no "abstract class" or "abstract method" in the Swift 2.2 language, so as a result, where the code may require some form of method-definition-but-deliberately-without-an-implementation, we will use Swift's `preconditionFailure` method instead.

#### Kotlin
*An imperative, strongly-typed object-oriented language for the JVM platform.* You can't really mention Swift without also covering Kotlin; where Swift is the default language for doing iOS development, Google has made Kotlin the same for Android. And honestly, Kotlin is quickly becoming my go-to choice for doing things on the JVM, even over Java itself.

#### C# #
*An imperative, strongly-typed object-oriented language for the CLR platform.*

#### F# #
*An object/functional, strongly-typed language for the CLR platform.*

#### Java
*An imperative, strongly-typed object-oriented language for the JVM platform.* Java follows C++ in the object-oriented tradition, and as such is pretty closely relatable to the GOF patterns, with a few modifications. Java lacks a number of the syntactic features of the C++ family tree, but the gap is smaller now (as of 1.8) than it was fifteen years ago. For the most part, since lambdas and function literals are still very new in the Java ecosystem, pattern implementations will show both "with and without" scenarios, at least until such point as doing so gets either (a) tiresome, or (b) less necessary (owing to greater proliferation of lambdas/function literals through the Java ecosystem).

#### Scala
*An object/functional, strongly-typed language for the JVM platform.*

#### JavaScript
*An imperative, weakly-typed object-oriented interpreted language.*

JavaScript runs in browsers and on servers (using NodeJS), and will probably continue to extend its reach to a variety of other places as time goes on. JavaScript has been described in some quarters as being a functional language, but given that it lacks key critical functional features (partial application of functions, immutability by default), I do not consider it as such. I will also include implementations of ECMAScript6- and 7-oriented code, where it seems appropriate or important.

#### Python
*An imperative, weakly-typed object-oriented interpreted language.* Python is becoming ridiculously popular for a large number of reasons, and despite my earlier misgivings about the use of significant whitespace to denote scope blocks, after having toyed with it for a while, I'm starting to come around to the idea that maaaaaybe it's not so bad. It has some functional concepts like many of its peers, but again, it's not really a functional language because it lacks some of the critical things (immutability by default, partial evaluation, and so on) that make functional languages powerful. Useful, all the same, but not a functional language.

#### Ruby
*An imperative, weakly-typed object-oriented interpreted language.* Ruby is, in many ways, akin to JavaScript in its approach and feature list. It has just enough functional-like features (function literals, for example) that it can mimic some functional features, but like JavaScript, it cannot be called a "functional language" akin to Haskell or ML.

### Other languages
I also plan to explore some other languages, sometimes as an intellectual exercise (and as a way to practice writing code in those languages), sometimes because I think the language is going to gain more traction, and sometimes just because I'm intrigued. That list includes, but is not limited to:

#### Functional languages
* *[Yeti](http://research.tedneward.com/languages/jvm/yeti): an ML clone; a functional, strongly-typed for the JVM platform.*
* *[Haskell](http://research.tedneward.com/languages/haskell): one of the original pure functional languages; compiles to native platforms.* My Haskell is not great, however, so this will definitely be a learning exercise for me.
* *[Frege](http://research.tedneward.com/languages/jvm/frege): a Haskell clone; a functional, strongly-typed language for the JVM platform.* Quite frankly, if I'm going to learn a Haskell, I'll probably do it in Frege instead, since that runs on a platform I often care deeply about. Two birds, one stone.
* *[Elixir](http://research.tedneward.com/languages/elixir): a functional, weakly-typed language for the Erlang platform.* Like some others on this list, Elixir has some features that allow it to take advantage of some of these patterns.
* *[Erlang](http://research.tedneward.com/languages/erlang): a functional, strongly-typed language for the Erlang platform.*

#### Lisps
* *[Clojure](http://research.tedneward.com/languages/lisp/clojure): a Lisp; a weakly-typed functional language with a few object-interoperability features for the JVM platform.* My Clojure Fu is not as strong as I would like it to be, so I will periodically attempt a pattern in Clojure just to experiment and see how well/poorly I can implement said pattern.
* *[Scheme](http://research.tedneward.com/languages/lisp/scheme): one of the original Lisps; a functional, weakly-typed interpreted language.*

#### Dynamically-typed languages
* *[Lua](http://research.tedneward.com/languages/lua): an imperative, untyped, object-ish interpreted language.* Lua is a scripting language with only a very loose notion of objects (no inheritance to speak of, no class types to speak of), whose great claim to fame is that it is ridiculously easy to embed inside of a native application. (This is why Lua shows up so frequently inside of game engines.) This represents a pretty strong challenge to the patterns: can a language that lacks one of the core features the Gang-of-Four relied on---inheritance---still implement some of these patterns?
* *[Objective-C](http://research.tedneward.com/languages/objc): a weakly-typed object-oriented language for native platforms.* ObjC is more or less on its way "out" as the language of choice for Mac OSX/iOS, but I may periodically tinker with it as an implementation language, just for fun.

And of course I reserve the right to add a few more languages to the mix, if they're interesting. Because, in a lot of ways, while I hope that readers get a lot out of this, this whole exercise is more for me than anybody else.
