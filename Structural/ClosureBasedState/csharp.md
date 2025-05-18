title=Closure-based State: C#
date=2016-04-02
type=pattern
tags=pattern implementation, structural, csharp
status=published
description=A Closure-based State implementation in C#.
~~~~~~

While normally we don't think of C# as a functional language, its support for closures (necessary in order to implement LINQ) means that C# can make use of [Closure-based State](../ClosureBasedState) with  very little in the way of extra-linguistic magic. However, the syntax isn't always pretty, and it can get downright ugly in a number of scenarios.

First, let's start with a basic example of holding closure-based state using just a function; no classes, nothing tricky (yet):

````csharp
public class App
{
  public static void Main()
  {
    Func<int, int> productOperation = ((Func<Func<int,int>>)( () => {
      var state = 100;
      Func<int, int> result = delegate(int adjust)
      {
        state += adjust;
        return state;
      };
      return result;
    }))();
    
    Console.WriteLine("Result = {0}", productOperation(100)); // prints 200
  }
}
````

OK, so I lied, and things got tricky fast; that's because C# is a strongly-typed language, for starters,  and doesn't like the idea of a function literal being assigned to "var" (it very explicitly says you can't  do that), so we need to manually type the various instances in use. The inner Func/delegate is the actual  method that we want to invoke, and the outer one (which is immediately invoked after definition) is simply  there to hold the closure-based state.

#### Objects
Should the closure-based state want to be part of an object, C# makes it trickier; the interface for the object definition must appear in a sufficiently wide enough scope that clients can see it, yet the actual implementation has to be deferred until it can be defined inside the closure. In many respects, the easiest way to do this (since C# does not support the concept of types defined inline in a method, a la anonymous inner classes in Java) is to make this a [Dynamic Object](../DynamicObject):

````csharp
public class Product
{
  private Func<int> stateGetter;
  private Action<int> stateSetter;
  private Func<int,int> operation;
  
  private Product(Func<int> sg, Action<int> ss, Func<int,int> op) {
    stateGetter = sg;
    stateSetter = ss;
    operation = op;
  }
  
  public int State
  {
    get { return stateGetter(); }
    set { stateSetter(value); }
  }
  
  public int Operation(int adjust) { return operation(adjust); }
  
  public static Product New()
  {
    var state = 100;

    Func<int> getter = () => { return state; };
    Action<int> setter = (int v) => { state = v; };
    Func<int,int> operation = (int adj) => { state += adj; return state; }; 

    return new Product(getter, setter, operation);
  }
}

public class App
{
  public static void Main()
  {
    Product p = Product.New();
    Console.WriteLine("Result = {0}", p.Operation(100));
  }
}
```` 

The "State" property here is simply to demonstrate how a property could also access the closure-based state; it would actually be defeating much of the purpose of Closure-based State if the state were directly accessible (and modifiable) via the property (in most cases).

Note that the use of Closure-based State will almost always require some form of [Constructor Function](../ConstructorFunction) to create the object, since a closure by definition is function/method-local variable state "closed over" by a function literal. In the above, the Constructor Function is the static "New" method.

Some may prefer to avoid the trampoline through the constructor for the Product, by the way:

````csharp
public class Product
{
  private Func<int> stateGetter;
  private Action<int> stateSetter;
  private Func<int,int> operation;
  
  public int State
  {
    get { return stateGetter(); }
    set { stateSetter(value); }
  }
  
  public int Operation(int adjust) { return operation(adjust); }
  
  public static Product New()
  {
    var state = 100;
    
    var result = new Product();
    result.stateGetter = () => { return state; };
    result.stateSetter = (int v) => { state = v; };
    result.operation = (int adj) => { state += adj; return state; }; 
    return result;
  }
}
````

#### Fully-dynamic Closure-based State
C# can also make use of fully dynamic objects using dynamic here:

````csharp
public class DynProduct
{
  public static dynamic New()
  {
    var state = 100;
    
    dynamic result = new ExpandoObject();
    result.State = ((Func<int>) (() => { return state; }));
    result.Operation = (Func<int,int>) ((int adjust) => { state += adjust; return state; });
    return result;
  }
}

public class App
{
  public static void Main()
  {
    dynamic d = DynProduct.New();
    Console.WriteLine("Result = {0}", d.Operation(100));
  }
}
````

The casts are necessary; the compiler simply will not accept the code without them.

This approach eliminates the need to define the object interface, since that will be implicitly done via the "dynamic" facilities of the C# language. The drawback, of course, is that the type will no longer have a type-checked interface that the compiler can verify for correctness. This will allow for a certain degree of flexibility in that interface over time, but will also potentially yield errors that could have been caught at compile-time. See the [C# Dynamic Object implementation](../DynamicObject/csharp.html) for more details on the pros/cons of dynamic objects in C#.

#### Enumerators
The code produced by a C# enumerator (the "yield return" keyword) can be thought of as a form of Closure-based State, in that the underlying implementation of the enumerable values essentially "closes over" the collection of values described by the "yield return" statement:

````csharp
public class App
{
  public static IEnumerable<string> Names() 
  {
    yield return "Ted";
    yield return "Charlotte";
    yield return "Michael";
    yield return "Matthew";
  }
  public static void Main()
  {
    foreach (var n in Names())
      Console.WriteLine(n);
  }
}
````

(Note that this is actually a better example of an [Iterator](../Iterator), since the code generated actually produces an IEnumerable<T> object, and returns that, which internally maintains a potentially very sophisticated state machine to track the iteration path.)

However, this example is only useful for pedagogical purposes, since the usage is constrained to a fairly narrow use case (producing a stream of values for consumption by IEnumerable<T>-consuming clients). It cannot be generalized beyond this iteration-based usage.



