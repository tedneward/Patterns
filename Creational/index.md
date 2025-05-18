title=Creational Patterns Index
date=2016-04-10
type=patternsindex
tags=patterns
status=published
description=Creational Patterns, 20 years later
patternfilter=creational
~~~~~~
***tl;dr*** Creational patterns specifically deal with the creation of objects/entities in the code. They abstract the instantiation process (meaning that most of the time, they provide an abstraction layer above the use of the raw language facilities to construct an object). They help make a system independent of how its objects are created, composed, and represented.

The Gang of Four wrote:

> Creational patterns become important as systems evolve to depend more on object composition than class inheritance. As that happens, emphasis shifts away from hard-coding a fixed set of behaviors toward defining a smaller set of fundamental behaviors that can be composed into any number of more complex ones. Thus creating objects with particular behaviors requires more than simply instantiating a class.

Initially, they described two recurring themes in these patterns:

1. "They encapsulate knowledge about which concrete classes the system uses."
2. "They hide how instances of these classes are created and put together." 

These two concepts combine to allow the client system to remain ignorant of the details---all the client knows is the interface of the created objects, giving the system a great deal of flexibility in its implementation.

It's important to note that creational patterns show how to make designs more flexible, not necessarily smaller or simpler. In particular, they will make it easy to change the classes that define the components.

### Segregation
Initially, the Gang of Four "split" the creational patterns into "class" and "object", based on the idea that class made use of inheritance to vary the class being created, whereas object delegated instantiation to another object. In the world 20 years later where (a) inheritance is no longer the huge force that it was back in 1995, and (b) functional and dynamic languages don't stress inheritance anywhere nearly as strongly as an object-oriented language would, it's not clear whether this distinction is still meritable.

In that ontology, [Factory Method](../FactoryMethod.html) provides a way to use subclassing to provide the variance around which type is being constructed, making it a "class" Creational, whereas [Abstract Factory](../AbstractFactory.html), [Builder](../Builder.html), and [Prototype](../Prototype.html) were deemed "object" Creational, in that they deferred the actual act of creation to other objects, namely a "factory object" of some form:

* "Abstract Factory has the factory object producing objects of several classes."
* "Builder has the factory object building a complex product incrementally using a correspondingly complex protocol."
* "Prototype has the factory object building a product by copying a prototype object. In this case, the factory object and the prototype are the same object, because the prototype is responsible for returning the product."
 
In this view, a [Constructor Function](../ConstructorFunction.html) can be seen in the "object" Creational form, since it fails to use inheritance to provide that variance. In fact, it arguably stands very close to Builder here, but as a factory function (or functions) rather than a factory object.
