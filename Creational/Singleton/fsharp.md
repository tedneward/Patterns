title=Singleton: F#
date=2016-03-29
type=pattern
tags=pattern implementation, creational, fsharp
status=published
description=A Singleton implementation in F#.
~~~~~~

F# being a functional/object hybrid language makes it easy to do the traditional [Singleton](../Singleton) without much more work to do beyond what we see in languages like C++/Java/C#:

````fsharp
type Product private () =
  let mutable state = 0
  
  static let instance = Product()

  static member Instance = instance
  
  member this.DoSomething() = 
    state <- state + 1
    printfn "Doing something for the %i time" state
    ()

Product.Instance.DoSomething()
Product.Instance.DoSomething()
Product.Instance.DoSomething()
````

F# defines a "primary constructor" using the same top line of the type declaration as is occupied by the name, so in order to support the "singleton-ness" of this type, we use the "private" modifier to indicate that nobody else is allowed to construct an instance of Product. Since this is a Singleton, any instance state can be initialized inside the type, rather than through its constructor, particularly since the body of the type is "executed" as part of initialiing the primary constructor.

Note that "Instance" is declared as a "member", which effectively obscures whether this is a property or a field or a method; this is a deliberate choice on the part of the language, and essentially obviates the whole "field/method/property" discussion of the classic Singleton.

#### Initialization note
We choose to use local let bindings for the instance values simply because it is much simpler to use in F# than to pass these through the private primary constructor.

Comments have pointed out that the .NET platform defines a type specifically around the feature of lazy instantiation, the `Lazy<T>` type. As such, if lazy-instantiation semantics are all that is really desired, a `let static Instance : Lazy<T> = ...` binding can often be the easiest way to implement a Singleton. However, should refactoring/encapsulated be needed later (to support more than one, for example), then the field will need to be converted to a property, and there can be no parameters passed to the property (which could hinder evolution). (At the same time, modern refactoring tools can likely handle this scenario without too much difficulty, as long as all the client code is within easy reach.)

#### Module implementation
F# brings forward (from OCaml) the concept of strong modules (a module construct that is recognized and enforced by the language and runtime, instead of just convention), and modules support all forms of value bindings, meaning both fields and functions at the top level of the module. This then makes it realatively easy to write a Singleton as a module instead of as a class, and is perhaps the preferred approach for more complex/larger Singletons. (For example, the module will almost certainly be the better approach for a Singleton that sits in front of a full [Facade](../../structural/Facade), for example.)

Consider:

````fsharp
module Singleton =

  type Product internal () =
    let mutable state = 0

    member this.DoSomething() =
      state <- state + 1
      printfn "Doing something for the %i time" state

  let Instance = Product()
````

(Author's note: A whitespace-sensitive language is sometimes tricky to get right with Markdown, so please pay close attention to the indentation and comment/email/contact me if something in here doesn't render correctly.)

In order to enforce the concept of Singleton-ness more carefully, we should probably mark the Product type itself as internal and implementing a publicly-declared Product interface, so as to allow the maximum flexibility in this approach. This would make it much easier to later replace the Singleton with a variety of different possible options, including multiple instances and/or derived instances, without the client's awareness.

#### Lazy initialization
Like other languages that run on top of the CLR, F# will initialize types when the assembly is loaded into the process space and referenced for the first time; this means that the initialization of Product will occur at (roughly) the same time Instance is first used in most cases. Should some form of lazier initialization be required, the Instance member needs to be fleshed out into a method to do the does-the-instance-exist check, create a new instance, and hand it back. It's not clear what benefit would be derived from doing so, however, in the simple case.

#### All-static implementation
Of course, like most languages that support static methods, F# could simply be rewritten to use all static fields and methods, rather than hand back the singleton object instance upon which we invoke instance member operations. 

#### Scopes
It is important to note that due to the implementation of the CLR, using statics (as above) will not actually  be scoped to the entire CLR/process, but only to the AppDomain that loaded the class. This is discussed in more detail in a variety of different places, including books and papers that I have written, as well as numerous other sources. (However, with the fading favor of the AppDomain, this may be a moot point in a future version of the CLR.)

#### Concurrency
Being a language that runs ont the CLR, F# has no stronger concurrency guarantees than C#, though it certainly prefers to take a different approach to the design of objects than C# has traditionally embraced (F# prefers to build immutable objects over mutable ones). However, given that the Singleton often holds state, and that state tends to be immutable (after all, a constant global is often a compile-time constant and therefore no reason to use Singleton at all), it is generally safe to assume that any Singleton in F# will need to have some kind of concurrency control around it just as any C# or other CLR-based language would.

#### Thread-local scopes
As it turns out, a related concurrency example of Singletons is also an example of "scoping" Singletons differently than traditionally assumed. .NET 4.0 makes available a `ThreadLocal` class that will implicitly store the value, but store separate values per Thread. The docs demonstrate some C#-based usage:

````csharp
using System;
using System.Threading;
using System.Threading.Tasks;

class ThreadLocalDemo
{
  // Demonstrates:
  //      ThreadLocal(T) constructor
  //      ThreadLocal(T).Value
  //      One usage of ThreadLocal(T)
  static void Main()
  {
      // Thread-Local variable that yields a name for a thread
      ThreadLocal<string> ThreadName = new ThreadLocal<string>(() =>
      {
          return "Thread" + Thread.CurrentThread.ManagedThreadId;
      });

      // Action that prints out ThreadName for the current thread
      Action action = () =>
      {
          // If ThreadName.IsValueCreated is true, it means that we are not the
          // first action to run on this thread.
          bool repeat = ThreadName.IsValueCreated;

          Console.WriteLine("ThreadName = {0} {1}", ThreadName.Value, repeat ? "(repeat)" : "");
      };

      // Launch eight of them.  On 4 cores or less, you should see some repeat ThreadNames
      Parallel.Invoke(action, action, action, action, action, action, action, action);

      // Dispose when you are done
      ThreadName.Dispose();
  }
}
````

Each time `action` is invoked, the "thread-specific Singleton" thread name will be returned.

This then makes it much easier for a Singleton implementation to manage a single instance per thread, rather than one instance across all threads.

#### Serialization
Note that the CLR is a platform which supports an opt-in automatic object serialization mechanism, and as such, if the Singleton is marked `[Serializable]`, developers must take additional care to ensure that the Singleton, when deserialized, still remains the only instance. (Joshua Bloch talks about this in Effective Java; I've seen no .NET-specific book discuss this issue in any degree of depth or detail, unfortunately.) 

However, it's fair to ask why a Singleton might need to be serialized in the first place; if it is still determined that the Singleton must be serialized, then the follow-up question must be, "What happens when Singleton A is deserialized into a CLR where Singleton B already exists?" This becomes an important question around any state held in the two Singletons (which by itself, being a contradiction in terms, should be a red flag to the entire conversation), and whether that state should be merged, replaced, or cast aside. Danger, Will Robinson, danger.
