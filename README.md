# Patterns
A repository containing the text and example code of design patterns. The goals of this enterprise are admittedly audacious: to not only re-visit the original Gang-of-Four patterns, bringing them "up to speed" with modern languages and idioms, but also to incorporate concepts from functional (and object/functional hybrid) languages.

Note that most of these are "design patterns", meaning they are at the level of software implementation design, not "architectural" patterns (such as what we might find from Martin Fowler's "Patterns of Enterprise Application Architecture"). That said, architectural patterns are often made up of, or inspired by, "design patterns", and I find it helpful to have a category of architectural patterns described here as well.

This roughly follows the same format as the original Gang-of-Four patterns catalog, but with a few changes.

* *The original patterns are not duplicated in their entirety*. This is deliberate, in that I wish to avoid any copyright infringement and I want to encourage those who have not read the original book to procure a copy and do so. ***No assertion of copyright of any pattern language text is intended or implied.***

* *The patterns are cast into a "simpler" form.* Rather than the long form the GOF used, I have chosen to try and "simplify" the patterns by following a Problem/Solution/Context/Consequences format, with particular emphasis on Context and Consequences. A few Variations on the patterns have emerged over time, so I try to capture those, as well.

* *Some additional patterns are described here.* In seeking to bring the patterns up to "modern times" and languages, I have chosen to add a few more patterns that I think we have discovered along the way. These, in particular, should be treated with some level of skepticism and suspicion, as they have not been properly workshopped. Additionally, I will be going back through much of my patterns library and looking for additional patterns that seem to fit the rough category the GOF book occupied, and add them here (again, with some level of modernizing, if necessary).

This catalog is a continual work-in-progress; as more patterns are added to the catalog, their links will become active. Readers are encouraged to comment liberally, provide alternative implementations and/or suggest different language-specfic idioms, and/or participate in whatever fashion feels meaningful.

## [Behavioral patterns](behavioral/)
Patterns which describe the runtime relationship between one entity and another, and the flow of control between them. 

## [Creational patterns](creational/)
Patterns which specifically deal with the creation of objects/entities in the code.

## [Structural patterns](structural/)
Patterns which describe the structural (usually compile-time-related, in langauges which are compiled) relationship between one entity and another.

## [Concurrency patterns](concurrent/)
Patterns which describe how to execute operations in parallel and safeguard them from the various dangers that arise from doing so.

## [Pattern compositions](compositional/)
I believe that some patterns are, in fact, combinations/compositions of other patterns, and so I want to take a stab at capturing and analyzing them. (I think a number of Fowler's PEAA and the POSA books are made up of some other patterns, arranged in a particular way but interesting and useful nonetheless.)

## [Architectural patterns](architectural/)
A number of patterns "widen" well, operating either at the class/design level or at a larger scope (such as a distributed system). I'm personally not entirely sure of the parameters around an architectural pattern, or if an architectural pattern is a design pattern, particularly when I can see some architectural patterns being reasonable design patterns and vice versa, but I'll capture them and refactor later as inspiration/illumination strikes.

## [Bibliography](Bibliography)
A bibliographic reference to all/many/most of the patterns in the catalog, in case anybody wants to get the original texts.

# Additional pattern language(s)
I'd like to capture all of these together into a larger pattern language/fabric.

* Pattern-Oriented Software Architecture, vol 1

    * [Whole-Part](structural/Whole-Part/) (structural): aggregations of components that together form a semantic unit
    * <del>Master-Slave</del> *(combine this with Leader-Followers, from POSA2)* (behavioral): a master component distributes work to identical slave components and computers a final result from the results these slaves return.
    * <del>Proxy</del>
    * <del>Command Processor</del> *(definitely GOF-Command)*
    * View Handler *(sounds like a Chain of Responsibility/Observer hybrid)*
    * Counted Pointer (structural) *(this is an idiom for C++, but the idea of objects "carrying" additional information about themselves to provide additional functionality feels too commmon to be just a variant on Proxy)*
    * <del>Forwarder-Receiver</del> *(aka proxy/stub from DCOM or CORBA stubs/skeletons; definitely a Proxy variant)*
    * <del>Client-Dispatcher-Server</del>: provides location transparency by means of a name service and hides the details of the establishment of the communication connection between clients and servers *(seems like a combination of a Registry and Proxy/Forwarder-Receiver)*
    * Publisher-Subscriber *(variant of Chain of Responsibility? variant of Pipes-and-filters?)*
    * [Layers](structural/Layers/): structure applications that can be decomposed into groups of subtasks in which each group of subtasks is at a particular level of abstraction; *(structural?)*
    * [Pipes and filters](behavioral/PipesAndFilters/): a structure for systems that process a stream of data *(structural?)*
    * [Blackboard](behavioral/Blackboard/): useful for problems for which no deterministic solution strategies are known *(behavioral)*
    * Broker *(feels like a combination of multiple patterns)*
    * Model-View-Controller: divides an interactive application into three components: core functionality, representation, and control, with a change-propagation mechanism to ensure consistency between the three parts *(definitely feels like GOF-Observer/Chain-of-Responsibility hybrid)*
    * Presentation-Abstraction-Control: defines a structure for interactive software systems in the form of a hierarchy of cooperating agents, each of which is responsible for a specific aspect of the application's functionality, principally built out of three components (presentation of information, abstraction, and control). *(this is different from MVC even though it's similar)*
    * [Microkernel](structural/Microkernel/): separates a minimal functional core from extended functionality and customer-specific parts
    * <del>Reflection</del>: changing structure and behavior of software systems dynamically, supporting the modification of fundamental aspects, such as type structures and function call mechanisms. *(This is a DynamicObject, unless its provided by the underlying language/platform directly)*

* Pattern-Oriented Software Architecture, vol 2 (Patterns for Concurrent and Networked Objects)

    * [Wrapper Facade](structural/WrapperFacade) (structural)
    * [Component Configuration](creational/ComponentConfiguration) (creational)
    * [Interceptor](structural/Interceptor/) (structural)
    * [Extension Interface](structural/ExtensionInterface/) (structural)
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
    * Leader/Followers
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

## Implementations
I have notes around the different pattern implementations (and the languages in which I am choosing to do them) [here](./PatternImplementations). However, the implementations will always appear in a separate page from the pattern itself, owing to the fact that (a) I want to explore several languages around each pattern, and it would make each pattern page extremely long to have them all in one place, but also (b) patterns can and should be language-independent, and therefore it makes sense to me to split them apart.

