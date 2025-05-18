title=Builder: Java
date=2016-05-23
type=pattern
tags=pattern implementation, creational, java
status=published
description=A Builder implementation in Java.
~~~~~~

Java has long had a relationship with [Builder](../Builder), usually calling it by the more degenerative term "Factory" or "Factory pattern". (Technically, what Java calls a "Factory pattern" is typically one of Builder, [Factory Method](../FactoryMethod), or [Abstract Factory](../AbstractFactory), depending on what precisely looks to be varied and/or encapsulated.)

We start with the target Product:

````java
public class Product {
    public List<String> parts = new ArrayList<String>();
}
````

Then, Builder suggests that we create an Abstract Creator:

````java
public abstract class Builder {
    public abstract void buildPart();

    public abstract Product result();
}
````

The Abstract Creator is intended simply as an abstract interface, and as such, might initially seem to be better modeled using a Java interface; however, as the pattern itself notes, if there is level of reusability desired in the construction of parts, then it will be natural to put that reusable functionality into the base Abstract Creator (so that the Concrete Creators will pick it up automatically). Java makes this decision pretty binary---if there's going to be any behavior, it must be an abstract class.

Next, we need a Concrete Creator:

````java
public class ConcreteBuilder extends Builder {
    @Override
    public void buildPart() {
        product.parts.add("Adding part #" + (part++) + "\n");
    }

    @Override
    public Product result() {
        return product;
    }

    private int part = 0;
    private Product product = new Product();
}
````

Builder suggests that the construction be deferred to a Director to do the actual assembly of the parts:

````java
public class Director {
    public Product construct() {
        for (int i = 0; i < 5; i++)
            builder.buildPart();
        
        return builder.result();
    }

    private Builder builder = new ConcreteBuilder();
}
````

And then, finally, the client instantiates the Director and uses it to construct the Product for use:

````java
Director director = new Director();
Product product = director.construct();
System.out.println(product.parts);
````

This is pretty straightforward. Note that syntactically, we might prefer using a method named "New" instead of "construct", since then it will feel more syntactically similar to the traditional Java "new" keyword except for the case. This is a highly aesthetic choice.

### Fluent Builder
In the event that we seek to construct a Fluent API in Java for a Builder, we find that the Fluent API is pretty straightforward, owing to the fact that Java lacks any sort of property syntax, forcing everything to be a method call:

````java
public class FluentBuilder {
    private Product product;
    public FluentBuilder begin() {
        product = new Product();
        return this;
    }
    public FluentBuilder engine() {
        product.parts.add("Engine");
        return this;
    }
    public FluentBuilder steeringWheel() {
        product.parts.add("Steering Wheel");
        return this;
    }
    public FluentBuilder tire() {
        product.parts.add("Tire");
        return this;
    }
    public Product build() {
        return product;
    }
}
````

and using it looks like this:

````java
FluentBuilder vehicleBuilder = new FluentBuilder();
Product motorcyle = vehicleBuilder.begin()
  .engine()
  .steeringWheel()
  .tire()
  .tire()
  .build();
System.out.println(motorcycle.parts);
````

Like most Fluent Builders, the Java version relies on the idea of returning the Builder object as part of each construction call, carrying the state of the construction process as-is as state inside the Builder itself, until the Product as requested as part of the final step (`build`).

#### State- vs Command-based Builders
Note that this state-basde Fluent Builder approach suggests that a Fluent Builder will not be accessed across multiple threads (or other actors); if that becomes necessary, then it may be better to construct a Builder that is fundamentally made up of [Command](../Command) objects that are waiting to be all executed in order, on the `build` call. That way, the Product isn't "half-baked" during the construction process, and potentially corrupted; the construction chain can be examined and/or modified (concurrently or otherwise) before the actual construction process.

In Java, this can be elegantly modeled using a list of Function objects, each one taking in the Product in its current state, performing some operation upon it (continuing the Builder process), and then returning the object-in-process. We can then chain the functions together, and run them in sequence to arrive at the result.

````java
public class FluentBuilderFunctions {

    private List<Function<Product, Product>> steps;

    public FluentBuilderFunctions begin() {
        steps = new ArrayList<Function<Product, Product>>();

        steps.add((ignored) -> {
           return new Product();
        });

        return this;
    }

    public FluentBuilderFunctions engine() {
        steps.add((product) -> {
            product.parts.add("Engine");
            return product;
        });
        return this;
    }

    public FluentBuilderFunctions steeringWheel() {
        steps.add((product) -> {
            product.parts.add("Steering Wheel");
            return product;
        });
        return this;
    }
    public FluentBuilderFunctions tire() {
        steps.add((product) -> {
            product.parts.add("Tire");
            return product;
        });
        return this;
    }

    public Product end() {

        Product working = null;
        for (Function<Product, Product> fn : steps) {
            working = fn.apply(working);
        }

        return working;
    }
}
````

But, as any good functional programmer knows, this is basically a sequence of functions that can be composed, and Java8 Function<> objects support doing that pretty easily:

````java
public class FluentBuilderFunctions {
    private Function<Product, Product> fn;

    public FluentBuilderFunctions begin() {
        fn = (ignored) -> { return new Product(); };

        return this;
    }

    public FluentBuilderFunctions engine() {
        fn = fn.andThen((product) -> {
            product.parts.add("Engine");
            return product;
        });
        return this;
    }

    public FluentBuilderFunctions steeringWheel() {
        fn = fn.andThen((product) -> {
            product.parts.add("Steering Wheel");
            return product;
        });
        return this;
    }
    public FluentBuilderFunctions tire() {
        fn = fn.andThen((product) -> {
            product.parts.add("Tire");
            return product;
        });
        return this;
    }

    public Product end() {
        return fn.apply(null);
    }
}
````

Note that the Product parameter in the `begin` step is ignored, so it's safe to pass in `null` here.

FluentBuilders will sometimes want to take parameters, but thanks to the closure and "effectively final" rules of Java8, that's easy to capture as part of the construction logic:

````java
    public FluentBuilderFunctions tire(int numberOfTires) {
        fn = fn.andThen((product) -> {
            for (int i=0; i<numberOfTires; i++)
                product.parts.add("Tire");
            return product;
        });
        return this;
    }
````

The biggest advantage of writing the FluentBuilder this way is that each "chain" of calls is effectively one giant [Constructor Function](../ConstructorFunction). These are now intrinsically thread-safe, so if the Builder wants to return the generated Constructor Function for direct invocation, it can be used from as many threads simultaneously as desired, without any sort of concurrent impact.

### Incremental construction with guards in place

As noted in the text of the pattern, Builder can also make sure that a given object being constructed incrementally is not returned if it would be in an unusable (or potentially unusable) state:

````java
public class GuardedFluentBuilder {
    private Product product;
    public FluentBuilder begin() {
        product = new Product();
        return this;
    }
    public FluentBuilder engine() {
        product.parts.add("Engine");
        return this;
    }
    public FluentBuilder steeringWheel() {
        product.parts.add("Steering Wheel");
        return this;
    }
    public FluentBuilder tire() {
        product.parts.add("Tire");
        return this;
    }
    public Product build() {
        if (product.parts.size() < 4)
            throw new Exception("Product must have at least 4 parts");
            
        if (product.parts.indexOf("Engine") < 0)
            throw new Exception("Product must have an Engine");
        
        return product;
    }
}
````

The `end` method can either throw an exception or return `null`, depending on the particular aesthetics desired.
