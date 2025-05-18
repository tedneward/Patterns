title=Builder: C#
date=2016-05-23
type=pattern
tags=pattern implementation, creational, csharp
status=published
description=A Builder implementation in C#.
~~~~~~

C# has in many ways inherited its relationship with [Builder](Builder.html) from Java, where it was usually called by the more degenerative term "Factory" or "Factory pattern".  (Technically, what Java calls a "Factory pattern" is typically one of Builder,  [Factory Method](FactoryMethod), or [Abstract Factory](AbstractFactory), depending  on what precisely looks to be varied and/or encapsulated.) C#, however, never fell quite as deeply in love with the "Factory pattern" as the Java development crowd did, and as such it wasn't as widely used.

We start with the target Product:

````csharp
class Product
{
    public List<string> Parts = new List<String>();
}
````

Then, Builder suggests that we create an Abstract Creator:

````csharp
abstract class Builder
{
    public abstract void BuildPart ();
    public abstract Product Construct ();
}
````

The Abstract Creator is intended simply as an abstract interface, and as such, might initially seem to be better modeled using a C# interface; however, as the pattern itself notes, if there is level of reusability desired in the construction of parts, then it will be natural to put that reusable functionality into the base Abstract Creator (so that the Concrete Creators will pick it up automatically). C# makes this decision pretty binary---if there's going to be any behavior, it must be an abstract class.

Next, we need a Concrete Creator:

````csharp
class ConcreteBuilder : Builder
{
    private Product product = new Product();
    private int part = 0;

    public override void BuildPart()
    {            
        product.Parts.Add ("Adding part #" + (part++));
    }

    public override Product Construct()
    {
        return product;
    }
}
````

Builder suggests that the construction be deferred to a Director to do the actual assembly of the parts:

````csharp
class Director
{
    private Builder builder = new ConcreteBuilder();
    public Product Construct()
    {
        for (int i = 0; i < 5; i++)
            builder.BuildPart ();

        return builder.Construct ();
    }
}
````

And then, finally, the client instantiates the Director and uses it to construct the Product for use:

````csharp
public static void Main (string[] args)
{
    var director = new Director ();
    var product = director.Construct ();
    product.Parts.ForEach ((part) => Console.WriteLine (part));
}
````

This is pretty straightforward. Note that syntactically, we might prefer using a method named "New" instead of "Construct", since then it will feel more syntactically similar to the traditional C# "new" keyword except for the case. This is a highly aesthetic choice.

### Fluent Builder
In the event that we seek to construct a Fluent API in C# for a Builder, the first decision will be whether to use property syntax or method-call syntax to describe the "steps" in the Fluent API. Generally, properties feel more "readable", particularly to the non-technical crowd, but properties cannot receive parameters. On top of that, it remains a point of high contention to this day whether Fluent APIs are actually going to be exercised by non-programmers, thus making the more "readable" argument of properties somewhat moot.


````csharp
class FluentBuilder
{
    private Product product;
    public FluentBuilder Begin() {
        product = new Product();
        return this;
    }
    public FluentBuilder Engine {
        get 
        {
            product.Parts.Add ("Engine");
            return this;
        }
    }
    public FluentBuilder SteeringWheel
    {
        get 
        {
            product.Parts.Add("Steering Wheel");
            return this;
        }
    }
    public FluentBuilder Tire() {
        product.Parts.Add("Tire");
        return this;
    }
    public Product Build() {
        return product;
    }
}
````

and using it looks like this:

````csharp
var builder = new FluentBuilder ();
product = builder.Begin()
    .Engine
    .SteeringWheel
    .Tire()
    .Tire()
    .Build();
````

Like most Fluent Builders, the C# version relies on the idea of returning the Builder object as part of each construction call, carrying the state of the construction process as-is as state inside the Builder itself, until the Product as requested as part of the final step (`Build`).

#### State- vs Command-based Builders
Note that this state-basde Fluent Builder approach suggests that a Fluent Builder will not be accessed across multiple threads (or other actors); if that becomes necessary, then it may be better to construct a Builder that is fundamentally made up of [Command](Command.html) objects that are waiting to be all executed in order, on the `build` call. That way, the Product isn't "half-baked" during the construction process, and potentially corrupted; the construction chain can be examined and/or modified (concurrently or otherwise) before the actual construction process.

In C#, this can be elegantly modeled using a list of `Func<T>` objects, each one taking in the Product in its current state, performing some operation upon it (continuing the Builder process), and then returning the object-in-process. We can then chain the functions together, and run them in sequence to arrive at the result.

````csharp
class FluentBuilderFns
{
    private List<Func<Product, Product>> steps = 
        new List<Func<Product, Product>>();

    public FluentBuilderFns Begin() {
        steps.Clear ();
        return this;
    }
    public FluentBuilderFns Engine {
        get 
        {
            steps.Add ((product) => {
                product.Parts.Add("Engine");
                return product;
            });
            return this;
        }
    }
    public FluentBuilderFns SteeringWheel
    {
        get 
        {
            steps.Add ((product) => {
                product.Parts.Add("Steering Wheel");
                return product;
            });
            return this;
        }
    }
    public FluentBuilderFns Tire() {
        steps.Add ((product) => {
            product.Parts.Add("Tire");
            return product;
        });
        return this;
    }
    public Product Build() {
        var working = new Product ();
        foreach (var step in steps) {
            working = step (working);
        }
        return working;
    }
}
````

But, as any good functional programmer knows, this is basically a sequence of functions that can be composed. While C# `Func<T>`s lack any direct compositional capabilities, it's relatively easy to create a utility "Compose" function that takes `Func<T>`s and composes them into a new `Func<T>`:

````csharp
static class FnUtils
{
    public static Func<A,C> Compose<A,B,C>(Func<A,B> f1, Func<B, C> f2) 
    {
        return (a) => f2(f1(a));
    }
}
````

From here, it's easy to use that function to compose a string of builder functions together into what is effectively a single [Constructor Function](../ConstructorFunction):

````csharp
class FluentBuilderFns
{
    private Func<Product, Product> fn = null;

    public FluentBuilderFns Begin() {
        fn = (ignored) => new Product ();
        return this;
    }
    public FluentBuilderFns Engine {
        get 
        {
            fn = FnUtils.Compose (fn, (product) => {
                product.Parts.Add("Engine");
                return product;
            });
            return this;
        }
    }
    public FluentBuilderFns SteeringWheel
    {
        get 
        {
            fn = FnUtils.Compose (fn, (product) => {
                product.Parts.Add("Steering Wheel");
                return product;
            });
            return this;
        }
    }
    public FluentBuilderFns Tire() {
        fn = FnUtils.Compose (fn, (product) => {
            product.Parts.Add("Tire");
            return product;
        });
        return this;
    }
    public Product Build() {
        return fn(null);
    }
}
````

Note that the Product parameter in the `Begin` step is ignored, so it's safe to pass in `null` here.

FluentBuilders will sometimes want to take parameters, but thanks to the closure rules of C#, that's easy to capture as part of the construction logic:

````csharp
public FluentBuilderFns Tire(int numberOfTires) {
    fn = FnUtils.Compose(fn, (product) => {
        for (int i=0; i<numberOfTires; i++)
            product.Parts.Add("Tire");
        return product;
    });
    return this;
}
````

The biggest advantage of writing the FluentBuilder this way is that each "chain" of calls is effectively one giant [Constructor Function](../ConstructorFunction). These are now intrinsically thread-safe, so if the Builder wants to return the generated Constructor Function for direct invocation, it can be used from as many threads simultaneously as desired, without any sort of concurrent impact.

### Incremental construction with guards in place

As noted in the text of the pattern, Builder can also make sure that a given object being constructed incrementally is not returned if it would be in an unusable (or potentially unusable) state:

````csharp
class GuardedFluentBuilder
{
    private Product product;
    public FluentBuilder Begin() {
        product = new Product();
        return this;
    }
    public FluentBuilder Engine {
        get 
        {
            product.Parts.Add ("Engine");
            return this;
        }
    }
    public FluentBuilder SteeringWheel
    {
        get 
        {
            product.Parts.Add("Steering Wheel");
            return this;
        }
    }
    public FluentBuilder Tire() 
    {
        product.Parts.Add("Tire");
        return this;
    }
    public Product Build() 
    {
        if (product.Parts.Length < 4)
            throw new Exception ("Product doesn't have enough parts to it yet");

        if (product.Parts.Contains ("Engine") == false)
            throw new Exception ("Product must have an Engine");

        if (product.Parts.Find((str) => str == "Tire").Length < 2)
            throw new Exception ("Product must have at least 2 Tires");

        return product;
    }
}
````

The `Build` method can either throw an exception or return `null`, depending on the particular aesthetics desired.