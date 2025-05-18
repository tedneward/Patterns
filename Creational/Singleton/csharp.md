title=Singleton: C#
date=2016-03-31
type=pattern
tags=pattern implementation, creational, csharp
status=published
description=A Singleton implementation in C#.
~~~~~~

Like most object-oriented typed languages, C# makes it pretty straightforward to code up a  classic [Singleton](../Singleton):

````c#
public class Product
{
  public static Product Instance {
    get; private set;
  }
  
  static Product()
  {
    Instance = new Product(0);
  }

  private Product(int s) {
    state = s;
  }
  private int state;

  public void DoSomething() {
    System.Console.WriteLine("I did something for the {0} time", ++state);
  }
}

public class App
{
  public static void Main() {
    Product.Instance.DoSomething();
    Product.Instance.DoSomething();
    Product.Instance.DoSomething();
  }
}
````

Like most of its kin, C# Singletons are eagerly-initialized, in that the Singleton is initialized and ready as soon as the class is loaded, but like most dynamically-loaded runtimes (the JVM and the CLR are both such creatures), the class won't be loaded until it is explicitly referenced somehow. (Accessing the static method is one such way to force the class-load.)

C# will usually use a static property, since the concept of the singleton instance object superficially seems closer to a property than a method, and the property syntax permits the kind of encapsulatory barrier that Singleton looks to put into place. If, however, the Singleton ever goes to a Registry or some other Singleton variant, the property will no longer work syntactically, and clients would have to change their code; if it is reasonable to assume that the Singleton may later become one of the variants and would require a parameter to be passed, it's better to utilize a method from the beginning.

#### Implementation note
Note two implementation details: one, the constructor is marked private, so as to enforce the "singleton-ness" of the instance and prevent any casual client from instantiating one outside of the class. Secondly, the Product type declares a type constructor to do the actual construction, so as to demonstrate that this could be more sophisticated code should that be required; whether that code is done inside the type constructor or the instance constructor or the Instance accessor method of the property will likely depend on aesthetic values, though if the construction/initialization process can generate exceptions, it is probably preferable to do so from within the Instance accessor (get), so that the exception can be caught and handled in client code if/when necessary. (Exceptions thrown from type constructors will generally end up going to /dev/null, and represent extremely hard-to-find bugs in the program.)

Comments have pointed out that the .NET platform defines a type specifically around the feature of lazy instantiation, the `Lazy<T>` type. As such, if lazy-instantiation semantics are all that is really desired, a `readonly static Lazy<T> Instance = ...` field can often be the easiest way to implement a Singleton. However, should refactoring/encapsulated be needed later (to support more than one, for example), then the field will need to be converted to a property, and there can be no parameters passed to the property (which could hinder evolution). (At the same time, modern refactoring tools can likely handle this scenario without too much difficulty, as long as all the client code is within easy reach.)

#### Scopes
It is important to note that due to the implementation of the CLR, using statics (as above) will not actually be scoped to the entire CLR/process, but only to the AppDomain that loaded the class. This is discussed in more detail in a variety of different places, including books and papers that I have written, as well as numerous other sources. (However, it should be noted that the use of AppDomains in .NET code seems to be falling out of favor, and may disappear entirely at some future point, so this may not be as big a concern as it sounds.)

#### Serialization
Note that C# is a language which supports an opt-in automatic object serialization mechanism, and as such, if the Singleton is marked `[Serializable]`, developers must take additional care to ensure that the Singleton, when deserialized, still remains the only instance. Joshua Bloch talks about this in Effective Java. (There is no book I have found that discusses .NET's object serialization mechanism in anywhere near the same amount of depth as Bloch does in Effective Java; as a result, I would recommend .NET developers have a look at the book, and then spend a little time figuring out how that translates to the .NET equivalents.) However, it's fair to ask why a Singleton might need to be serialized in the first place; if it is still determined that the Singleton must be serialized, then the follow-up question must be, "What happens when Singleton A is deserialized into a CLR where Singleton B already exists?" This becomes an important question around any state held in the two Singletons (which by itself, being a contradiction in terms, should be a red flag to the entire conversation), and whether that state should be merged, replaced, or cast aside. Danger Will Robinson, danger.

#### Enums-as-Singletons
Having perhaps looked at the [Java](../Singleton-Java) "Enums-as-Singletons" implementation, it's fair to ask whether C# supports a similar kind of implementation, given how the languages are so similar. As it turns out, even though C# lacks some of the features of the Java enum (namely, that we cannot introduce additional state or methods to the enum), we can make use of C#'s extension methods facility to accomplish (more or less) the same thing:

````c#
public enum ProductEnum
{
  INSTANCE
}
public static class ProductEnumExtensions
{
  private static int state = 0;
  public static void DoSomething(this ProductEnum it) {
    System.Console.WriteLine("I did something for the {0} time", ++state);
  }
}
````

In this particular case, this works due to the fact that the extension class holds the Singleton-related state, and therefore the extension methods are able to access that state. Such state is static, however, and not held within an instance of an object, which not only brings up the whole statics-vs-instance argument all over again, but also restricts some of the Singleton's flexibility, particularly if refactoring to "multi-ton-ness" in a later iteration of the code. However, if the Singleton is serialiable (see above), then the enum serializes and deserializes correctly, and ensures that only one such instance is in scope at any time. (The static state, however, would not come with the serialization, which somewhat defeats the purpose.)

#### Adding in Singleton-ness post-facto
In some cases, an existing type that was not originally designed to be a Singleton can be made into such using generics in the C# language, like so:

````c#
// THis code was written by a third-party and cannot be changed
//
public class Product2
{
  public Product2() { }
  
  public int State { get; set; };

  public void DoSomething() {
    System.Console.WriteLine("I did something for the {0} time", ++State);
  }
}
// This code is added to make Product a Singleton
public class Singleton<T> where T : new()
{
  private static T instance = new T();
  public static T Instance {
    get { return instance; }
  }
}
````

Because generic types are reified inside the CLR when instantiated, this means that separate static storage is maintained for each Singleton<T> referenced; Singleton<Foo> is a separate static storage space than Singleton<Bar>. This is what allows the instance to be stored in the generic class as a simple static (as opposed to something more exotic).

However, this approach lacks many of the consequences of the Singleton pattern: it cannot guarantee uniqueness (since anyone can instantiate a Product2, thanks to its public constructor), and it cannot actually initialize the object to a known state (since the constructor must be a parameterless constructor, thanks to the "new()" constraint on the generic, without which the compiler would not allow us to instantiate the parameterized type). In effect, this implementation only restricts the namespace of the instance to the well-known name "Singleton<Name>", and thus provides a well-known place by which to find the desired "single" instance.

#### Concurrent access
The CLR being a multi-threaded platform, it should usually be assumed that the Singleton can and will be accessed from multiple threads (whether that was originally intended or not!). As a result, some level of concurrency-safety needs to be implemented on the instance; one such (potentially naive) way of doing so is to manage the relevant methods by using "lock", though the preferable approach is to hide the concurrency-safety details a little bit more deeply.

#### Thread-scoped Singleton
As it turns out, a related concurrency example of Singletons is also an example of "scoping" Singletons differently than traditionally assumed. C# makes available a `ThreadLocal` class that will implicitly store the value, but store separate values per Thread. See the MSDN documentation for relevant usage.
