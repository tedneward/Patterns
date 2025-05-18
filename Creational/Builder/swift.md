title=Builder: Swift
date=2016-04-11
type=pattern
tags=pattern implementation, creational, swift
status=published
description=A Builder implementation in Swift.
~~~~~~

Swift makes use of the [Builder](Builder.html) (or a variant of it) in a couple of places inside of Mac OS X and iOS, and thus makes it feel somewhat natural to idiomatic Swift.

We start with the target Product:

````swift
class Product {
  var parts : String = ""
}
````

Then, Builder suggests that we create an Abstract Creator:

````swift
class Builder {
  func BuildPart() {
    preconditionFailure("Should never invoke this")
  }
  func GetResult() -> Product {
    preconditionFailure("Should never invoke this")
  }
}
````

The Abstract Creator is intended simply as an abstract interface, and as such, might initially seem to be better modeled using a protocol; however, as the pattern itself notes, if there is level of reusability desired in the construction of parts, then it will be natural to put that reusable functionality into the base Abstract Creator (so that the Concrete Creators will pick it up automatically). That said, the protocol has a significant benefit over a base class, since protocols carry no default implementation; unfortunately, as of this writing (Swift 2.2), Swift lacks a language facility to require a derived class implement a method but without providing a default implementation (the "abstract" keyword found in C# or Java). As a result, we use the Swift built-in `preconditionFailure` to generate a runtime error should this method be invoked. We could leave the implementation body empty, in case we want to establish a habit of always calling `super` from derived methods, but this then runs the risk that a subclass of Builder might not implement one of the `BuildPart` methods. (Remember, Builder assumes that there are multiple part types that want to be constructed, and if there are too many, it will be too easy to forget to implement one if there is no compile-time or run-time check.)

Next, we need a Concrete Creator:

````swift
class ConcreteBuilder : Builder {
  override func BuildPart() {
    product.parts = product.parts + "Adding part #\(part++)\n"
  }
  override func GetResult() -> Product {
    return product
  }
  
  var part = 0
  var product = Product()
}
````

Builder suggests that the construction be deferred to a Director to do the actual assembly of the parts:

````swift
class Director {
  func Construct() -> Product {
    for _ in 1 ... 5 {
      builder.BuildPart()
    }
    
    return builder.GetResult()
  }
  
  let builder = ConcreteBuilder()
}
````

And then, finally, the client instantiates the Director and uses it to construct the Product for use:

````swift
let director = Director()
let product = director.Construct()
print(product.parts)
````

This is pretty straightforward.

#### Fluent Builder
In the event that we seek to construct a Fluent API in Swift for a Builder, the only real trickiness here centers around whether to use a property syntax or a method syntax to do the construction; the rest of the Fluent API is pretty straightforward as well:

````swift
class FluentBuilder {
  var product = Product()
  func Begin() -> FluentBuilder {
    product = Product()
    return self
  }
  func Engine() -> FluentBuilder {
    product.parts = product.parts + "Engine\n"
    return self
  }
  func SteeringWheel() -> FluentBuilder {
    product.parts = product.parts + "SteeringWheel\n"
    return self
  }
  func Tire() -> FluentBuilder {
    product.parts = product.parts + "Tire\n"
    return self
  }
  func Construct() -> Product { return product }
}


let vehicleBuilder = FluentBuilder()
let motorcycle = vehicleBuilder.Begin()
  .Engine()
  .SteeringWheel()
  .Tire()
  .Tire()
  .Construct()
print(motorcycle.parts)
````

Like most Fluent Builders, the Swift version relies on the idea of returning the Builder object as part of each construction call, carrying the state of the construction process as-is as state inside the Builder itself, until the Product as requested as part of the final step (`Construct`).

#### State- vs Command-based Builders
Note that this state-basde Fluent Builder approach suggests that a Fluent Builder will not be accessed across multiple threads (or other actors); if that becomes necessary, then it may be better to construct a Builder that is fundamentally made up of [Command](../../structural/Command) objects that are waiting to be all executed in order, on the `Construct` call. That way, the Product isn't "half-baked" during the construction process, and potentially corrupted; the construction chain can be examined and/or modified (concurrently or otherwise) before the actual construction process.

In Swift, we can actually create a sequence of functions to defer the actual construction work, and then execute them serially when asked to construct the Product:

````swift
class FluentBuilder {
  var steps : [(Product -> Product)] = []

  func Begin() -> FluentBuilder {
    steps.append({ignored in return Product()})
    return self
  }
  func Engine() -> FluentBuilder {
    steps.append({product in
      product.parts = product.parts + "Engine\n"
      return product
    })
    return self
  }
  func SteeringWheel() -> FluentBuilder {
    steps.append({product in
      product.parts = product.parts + "SteeringWheel\n"
      return product
    })
    return self
  }
  func Tire() -> FluentBuilder {
    steps.append({product in
      product.parts = product.parts + "Tire\n"
      return product
    })
    return self
  }
  func Construct() -> Product { 
    var working : Product = Product()
    for step in steps {
      working = step(working)
    }
    return working
  }
}
````

Any reasonable student of functional programming will recognize this as function composition, albeit a pretty crude one. Unfortunately, Swift lacks any sort of built-in function composition facilities, but it's trivial to construct a generalized `compose` function that takes two functions (each of which take a T and return a T) and turn them into a third function that composes the two:

````swift
func compose<A, B, C>(f1: (A -> B), _ f2: (B -> C)) -> A -> C {
  return { a in f2(f1(a)) }
}

class FluentBuilder {
  var fn : (Product -> Product) = 
    { ignored in return Product() }

  func Begin() -> FluentBuilder {
    fn = { ignored in return Product() }
    return self
  }
  func Engine() -> FluentBuilder {
    fn = compose(fn, { product in
      product.parts = product.parts + "Engine\n"
      return product
    })
    return self
  }
  func SteeringWheel() -> FluentBuilder {
    fn = compose(fn, { product in
      product.parts = product.parts + "SteeringWheel\n"
      return product
    })
    return self
  }
  func Tire() -> FluentBuilder {
    fn = compose(fn, { product in
      product.parts = product.parts + "Tire\n"
      return product
    })
    return self
  }
  func Construct() -> Product { 
    return fn(Product())
  }
}
````

Technically, the initial `fn` value is unnecessary, since it will be replaced on the first call to `Begin`, and for that matter, the first function established in the chain (also in `Begin`) is also unnecesary, since we can pass a newly-constructed Product into the chain when invoking the function in `Construct`.

Swift lacks any standard function-composition library, but this is likely something that will be corrected in a future revision of Swift, and/or through the use of a third-party contribution to the community.

### Incremental construction with guards in place

As noted in the text of the pattern, Builder can also make sure that a given object being constructed incrementally is not returned if it would be in an unusable (or potentially unusable) state:

````swift
class GuardedFluentBuilder {
  var product = Product()
  func Begin() -> FluentBuilder {
    product = Product()
    return self
  }
  func Engine() -> FluentBuilder {
    product.parts = product.parts + "Engine\n"
    return self
  }
  func SteeringWheel() -> FluentBuilder {
    product.parts = product.parts + "SteeringWheel\n"
    return self
  }
  func Tire() -> FluentBuilder {
    product.parts = product.parts + "Tire\n"
    return self
  }
  func Construct() -> Product { 
    if product.parts.constainsString("Engine") == nil {
      preconditionFailure("Products must have an engine")
    }
    
    return product 
  }
}

The `Construct` method can either throw an exception or return `nil`, depending on the particular aesthetics desired.
