title=Singleton: Swift
date=2016-03-26
type=pattern
tags=pattern implementation, creational, swift
status=published
description=A Singleton implementation in Swift.
~~~~~~

Like most languages, Swift makes it pretty straightforward to code up a classic [Singleton](../Singleton):

````swift
import Foundation

public class Product
{
  private init() { }
  
  public static func Instance() -> Product {
    return instance
  }
  
  public static var TheInstance : Product {
    get { return instance }
  }

  public func DoSomething() {
    print("Doing something for the \(++self.state) time")
  }

  static let instance : Product = Product()
  var state = 0
}

Product.Instance().DoSomething()
Product.TheInstance.DoSomething()
Product.Instance().DoSomething()
````

This is an eagerly-instantiated Singleton, created whenever Swift initializes the static space for the class; this is described [here](https://developer.apple.com/swift/blog/?id=7) as being when the class is first referenced (a la how Java works). This is probably sufficient for most tasks, particularly given that (as of this writing) Swift apparently has no language-level thread-synchronization constructs.

Note that most Singletons will not have *both* a static property *and* method; this is simply here to demonstrate what each will look like.

(*NOTE*: As of this writing, Swift 2.2 will generate a warning around the "++self.state" in the above, warning that "++" is deprecated and will be removed in Swift 3.)

As noted earlier, Swift lacks any sort of language-level concurrency-protection mechanics, so make sure to build such semantics into the implementation of the Swift instance methods. 

## Additional notes
I am reasonably certain that there's a way to help deal with subclassed Singletons using the Swift/Objective-C "class" methods, which can be overridden (!) in subclasses. But I can't quite figure out when/where/how to make the implementation work offhand, nor do I really have a use-case around it yet. Not giving up on it yet, though.
