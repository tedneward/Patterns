title=Factory Method: Swift
date=2016-04-13
type=pattern
tags=pattern implementation, creational, swift
status=published
description=A FactoryMethod implementation in Swift.
~~~~~~

[Factory Method](../FactoryMethod) is a relatively straightforward implementation in Swift, as it is in many object-oriented languages. 

We begin with a Product, which is abstract, and a concrete realization of that Product:

````swift
class Product {
    func DoSomething() {
        preconditionFailure("Should never be invoked")
    }
}

class ConcreteProduct : Product {
    override func DoSomething() {
        print("ConcreteProduct did something\n")
    }
}
````

The inheritance relationship here is necessary in that the Factory Method assumes the type being constructed wants to vary, but without the clients having to know the exact type of object being constructed. In order for that to work in a strongly-typed O-O language (like Swift, or its kin Java, C# or C++), there needs to be a way to polymorphically reference the things being constructed, and the easiest way to do that is via inheritance. Product could, certainly, be a protocol, and if there is no shared implementation that the various Product subtypes want to share, it arguably should be a protocol. (As a matter of fact, it's likely best to assume that it should be a protocol until proven otherwise.)

The FactoryMethod then establishes a base class from which all Factory types will derive:

````swift
class Creator {
    func FactoryMethod() -> Product {
        preconditionFailure("Should never be invoked")
    }
}
class ConcreteCreator : Creator {
    override func FactoryMethod() -> Product {
        return ConcreteProduct()
    }
}
````

Again, it's possible that the Creator type could be a protocol, and, again, the decision would hinge on whether the Creator-derived types have common functionality they want or need to share. However, it's important that the ConcreteCreator types have some kind of signature relationship to one another, as part of the point of the pattern is that clients can use a Creator without having to worry about the actual Creator type being used to construct the Products (that the client also wants to remain somewhat ignorant of).

However, somewhere in the client code, a decision must be made as to the actual derived type of Creator that is used, since the Creator hierarchy doesn't actually make any decision until the `FactoryMethod` method is invoked:

````swift
let creator = ConcreteCreator()
let product = creator.FactoryMethod()
product.DoSomething()
````

If the actual type of Creator is something the client wants to avoid deciding, then this will probably want to be combined with other patterns.

#### Variation: Registry
In the scenario where we want a Registry, Swift can provide this in several forms. The easiest is to take the Creator/ConcreteCreator hierarchy and have the Registry hold instances of ConcreteCreator. However, we can also reduce the work required to participate in this system if the ConcreteCreator is reduced to a  [Constructor Function](../ConstructorFunction) that is passed in to the Registry. The Registry then decides (usually by means of a parameter passed into the factory method call) which of these Constructor Functions to use.

So, for example, if we have the following product hierarchy:

````swift
class PaperProduct : Product {
    override func DoSomething() {
        print("Paper paper paper")
    }
}
class SilverProduct : Product {
    override func DoSomething() {
        print("Silver all the way")
    }
}
class DiamondProduct : Product {
    override func DoSomething() {
        print("Diamonds are forever")
    }
}
````

Then we can set up the following Registry and FactoryMethod system:

````swift
class AnniversaryChooser {
    typealias RegEntry = (Range<Int>, () -> Product)
    static var registry = [RegEntry]()
    
    static func GetGift(years : Int) -> Product {
        for regEntry in registry {
            if regEntry.0.contains(years) {
                return regEntry.1()
            }
        }
        preconditionFailure("No case registered for \(years) case")
    }
    static func Register(years : Range<Int>, creator : () -> Product) {
        registry.append(years, creator)
    }
}
````

The key here will be when and how FactoryMethods are registered with the Registry. Ideally, this happens when the type is loaded into the process, but unfortunately Swift lacks any sort of "auto-load" facility within the language at present. In that case, something must deliberately call Register, such as some top-level code in the module when the module is first loaded:

````swift
AnniversaryChooser.Register(0...1,
                            creator: { () -> Product in return PaperProduct() } )
AnniversaryChooser.Register(2...24,
                            creator: { () -> Product in return SilverProduct() } )
AnniversaryChooser.Register(25...100, 
                            creator: { () -> Product in return DiamondProduct() } )

let gift = AnniversaryChooser.GetGift(1)
gift.DoSomething()
````

Note that here we use simple Range<Int> objects to provide the criteria by which the appropriate FactoryMethod ConstructorFunction is selected. For more generalized systems, this "decision-making" step could itself be a function, in which case we have generalized the Registry to a fairly generic implementation.

````swift
class Registry<Criteria, Product> {
    typealias RegEntry = ( (Criteria) -> Bool, () -> Product)
    var registry = [RegEntry]()
    
    func Register(decider: (Criteria) -> Bool, creator: () -> Product) {
        registry.append((decider, creator))
    }
    
    func Create(criteria: Criteria) -> Product {
        for regEntry in registry {
            if regEntry.0(criteria) {
                return regEntry.1()
            }
        }
        preconditionFailure("No case registered for \(criteria) case")
    }
}
````

This implementation, like the prior one, assumes that Creators have no other responsibilities beyond that of creation, and so generalizes down to simple functions for both the selection and construction process. In addition, unlike the earlier implementation, this one assumes that the Registry will want to be a constructed instance, rather than a set of static functions. This is necessary in Swift (as of this writing, 2.2) because Swift disallows statics in generic class declarations/implementations.
