title=Singleton: Scala
date=2016-03-27
type=pattern
tags=pattern implementation, creational, scala
status=published
description=A Singleton implementation in Scala.
~~~~~~

Since Scala has some language syntactic sugar around "statics" that essentially turn all statics into a single object, it basically suggests that Scala has built-in implementation of [Singleton](../Singleton) without much more work to do:

````scala
class Product private(var state : Int) {
  def DoSomething() = {
    state += 1
    System.out.println("I did something for the " + this.state + " time")
  }
}

object Product {
  private val _instance = new Product(0)
  def instance() =
    _instance
}

Product.instance.DoSomething()
Product.instance.DoSomething()
Product.instance.DoSomething()
````

Here, we define a companion object in Scala (the "object" declaration) to hold the static instance of the Product, and return it when requested through the "instance()" definition.

In order to support the consequence that the Singleton should only ever have one instance at a time, we mark the primary constructor as "private", which, since the primary constructor is defined on the class-declaration line and is considered to be class-wide, means that "private" appears before the primary constructor parameters in the very beginning of the class declaration.

Notice that Scala deliberately chooses to syntactically try to "hide" the differences in syntax between property, method, and field; as a result, despite that "instance()" is written using the method-style syntax, Scala will know how to resolve the field/property-style usage in the example.

#### Initialization note
For pedagogical purposes, we choose to demonstrate passing in the initial state for the Singleton, rather than having it initialized within the body of the class. This is simply to demonstrate how the private primary constructor syntax should look in Scala.

#### Lazy initialization
The earlier example uses the JVM's static-initializer facilities to create the Product instance. (For the companion object above, Scala generates an auxiliary class, "Product$", which holds a static initializer that immediately turns around and constructs an instance of "Product$", which in turn immediately constructs the "Product" instance.) If the context demands that this initialization be deferred until the first time the "instance()" is requsted, the instance() definition must construct-on-demand, using traditional logic:

````scala
class Product private(var state : Int) {
  def DoSomething() = {
    state += 1
    System.out.println("I did something for the " + this.state + " time")
  }
}

object Product {
  private var _instance : Product = null
  def instance() = {
    if (_instance == null)
      _instance = new Product(0)
    _instance
  }
}

Product.instance.DoSomething()
Product.instance.DoSomething()
Product.instance.DoSomething()
````

A few syntactic changes are necessary in this implementation:

* The instance field must be explicitly type-declared. This is because Scala's type inference will make the best match it can, and in this case, since it is being initialized to null, it will assume it is of type Null, which then does not have a method "DoSomething()" declared on it.
* The instance method must now be block-enclosed, since it is now more than one expression.
* The instance field must be declared as "var" instead of "val", since by definition we are changing its value after the companion object has been initialized.

It is not clear what practical benefit is to be had from using this approach, given that the type's load will typically be extremely close (in locality terms) to its first usage, except under unusual circumstances.

#### Concurrency
Being a language that runs ont the JVM, Scala has no stronger concurrency guarantees than Java, though it certainly prefers to take a different approach to the design of objects than Java has traditionally embraced (Scala prefers to build immutable objects over mutable ones). However, given that the Singleton often holds state, and that state tends to be immutable (after all, a constant global is often a compile-time constant and therefore no reason to use Singleton at all), it is generally safe to assume that any Singleton in Scala will need to have some kind of concurrency control around it just as any Java or other JVM-based language would.

#### All-Companion implementation
As Joe Ottinger pointed out in comments, Scala's companion object can, in many respects, be considered to be the Singleton, and thus, no separation/distinction between the "Product class" and "Product object" need be made, instead just collapsing everything on to the Product companion object itself:

````scala
object Product {
  private var state : Int = 0
  def DoSomething() = {
    state += 1
    System.out.println("I did something for the " + state + " time")
  }
}

Product.DoSomething()
Product.DoSomething()
Product.DoSomething()
````

Note that although it's possible to capture the Singleton instance into a local variable/value for easier use by relying on Scala's type-inference to discover that "Product" is a companion object:
  
````scala
var prod = Product
prod.DoSomething()
````
  
This implementation introduces a few consequences:

* *Method or property? Class.* By "hiding" the Singleton in the companion object, we do away with the need for the "instance" in the usage of the Singleton; essentially this is Scala's means by which we get back to static methods (a la Java/C++/C#). If the desire of the Singleton was to call out the single instance as an explicit and obvious instance, then this syntax doesn't meet up to that intent.
* *Singleton scope.* By hiding the instance behind "static" methods, we have greater opportunity to constrict or restrict access to the Singleton's scope, such as by making the Singleton thread-scoped (a la the ThreadLocal<T> from the Java runtime). However, if the scope cannot be syntactically "hidden" behind the companion object, then this syntax actually makes things worse, potentially requiring a change in client access code.
* *Enforcing Singleton-ness.* If there is no class, there is no way to create an instance of Product.
* *Refactoring to multi-tons.* Should the system later decide to move away from a Singleton to constructing more than one instance (a la an Object Pool or one of the other Creational patterns), this syntax may require a client code change, depending on semantics.
