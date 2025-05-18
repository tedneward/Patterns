title=Constructor Function: Scala
date=2016-04-09
type=pattern
tags=pattern implementation, creational, swift
status=published
description=A Constructor Function implementation in Swift.
~~~~~~

Being a language that allows for standalone and top-level functions, Swift makes it fairly easy to implement a [Constructor Function](../ConstructorFunction) directly; we simply define a function that returns an instance of the object type desired. However, since the language is also strongly-typed (like its kin C#, Java or C++), Swift will also require some level of declaration of the "surface area" of the thing being returned. Most commonly this would be done via a protocol or base class:

````swift
protocol Interface {
    func Operation(adjust: Int) -> Int;
}

let creator = { (s : Int) -> Interface in
    class Implementation : Interface {
        private var state : Int
        init(state : Int) {
            self.state = state
        }
        func Operation(adjust: Int) -> Int {
            state = state + adjust
            return state
        }
    }
    return Implementation(state: s)
}
let obj = creator(100)
print(obj.Operation(100))
````

Swift's ability to allow types to be declared as nested inside of functions (something that it shares with C++ and Java) provides a highly effective form of encapsulation; without access to the type declaration, no client would ever be able to construct an instance of `Implementation`, and therefore this type is completely, entirely and wholly removed from the public view.

It is important to note that the Creator function need not be a name-bound closure; an explicit `func` implementation would work just as well, unless there is a strong desire to allow variance in the implementation of the Constructor Function at runtime. However, should that runtime-variance facility be desired, the Constructor Function must be declared a `var` and not a `let`:

````swift
var origCreator = creator
creator = { (s: Int) -> Interface in
    class AnotherImplementation: Interface {
        private let interface : Interface
        private var otherState : Int
        
        init(state: Int, otherState : Int) {
            self.interface = origCreator(state)
            self.otherState = otherState
        }
        
        func Operation(adjust: Int) -> Int {
            interface.Operation(adjust)
            otherState = otherState + adjust
            return otherState
        }
    }
    return AnotherImplementation(state: s, otherState: 500)
}
let obj2 = creator(100)
print(obj2.Operation(100))
````

Note that the original creator must be trapped into a variable so that it can be used within the new Construction Function implementation without creating an infinite recursion. This original creator should be tucked away someplace hidden if the details of this multiple-step operation is to remain properly encapsulated.

Note also that because Swift is strongly-typed, the signature of the replacement Constructor Function must match that of the original exactly, preventing any additional variables/paramters being passed in to initialize the "extended" type. This usually means that when using replaceable Constructor Functions like this, the objects being created will be [Decorators](../../structural/Decorator), [Composites](../../structural/Composite), [Proxies](../../structural/Proxy), and the like.

Syntactically, it may be more idiomatic Swift to name the Constructor Function name to one that is upper-case-leading, rather than the typical lower-case-leading form used by local variables ("Creator" instead of "creator"); this is an aesthetic choice, but it certainly has a number of reasons to consider it, particularly since the Swift language eschews the use of an "allocation keyword" (a la "new" in C# or Java). Thus, using upper-case-leading names ("Creator") will more effectively hide away the fact that this is a Constructor Function, and not a standard constructor, from the client (`let o = ObjectCreator()` instead of `let o = objectCreator()`).

#### Variant: Static constructor functions
If the type or subtypes of the product being created don't quite differ enough in surface interface to be easily recognized as different, or the client wants to construct different kinds of Products without having to know which subclass corresponds to which kind of Product, it can be helpful to use static methods to provide this syntactic clue:

````swift
class Product {
    func DoSomething() {
        preconditionFailure("Should never be invoked")
    }
    
    static func newProduct1() -> Product {
        return ConcreteProduct()
    }
    static func newProduct2() -> Product {
        return ConcreteProduct2()
    }
}

class ConcreteProduct : Product {
    override func DoSomething() {
        print("ConcreteProduct did something\n")
    }
}

class ConcreteProduct2 : Product {
    override func DoSomething() {
        print("ConcreteProduct 2 did something different\n")
    }
}

let p1 = Product.newProduct1()
p1.DoSomething()
let p2 = Product.newProduct2()
p2.DoSomething()
````

This has the immediate disadvantage that any new construction paths (such as added parameters to the initializers) will require modification in two places---Product and the appropriate derivative---but given that these are all tightly coupled via inheritance anyway, this shouldn't be too onderous. (One would need to check to see about any changes to Product if there are changes to derived classes from Product, anyway.)
