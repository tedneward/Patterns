title=Constructor Function: Scala
date=2016-05-17
type=pattern
tags=pattern implementation, creational, scala
status=published
description=A Constructor Function implementation in Scala.
~~~~~~

Given that Scala is a functional language (object/functional, really), building a 
[Constructor Function](../ConstructorFunction) in Scala is actually pretty straightforward.

### Object method constructor
Scala allows us to create per-class methods on the class object, providing a convenient place on which to store methods designed to construct instances of the class. Scala traits are used to define the Interface, so as to better take advantage of the statically-typed nature of the compiler and provide compilation guarantees. In addition, because Scala supports nested classes (meaning, in this particular case, classes whose definition appears entirely inside a function definition), we can "hide" the implementation in much the same way that Java uses inner-class implementations, though we still do need to provide a name to the type.

~~~~scala
trait Interface {
  def operation(adjust: Int) : Int;
}

object Implementation {
  def construct(startingValue : Int) : Interface = {
    class Impl(var current : Int) extends Interface {
      def operation(adjust: Int) : Int = {
        current += adjust
        return current
      }
    }
    return new Impl(startingValue)
  }
}

object App {
  def main(args : Array[String]) = {
    val intf = Implementation.construct(100)
    System.out.println(intf.operation(10))
  }
}
~~~~

However, it can make more sense to hide the implementation entirely, and because Scala allows for both traits and class object definitions, we can define Interface to be both trait and class object. While at it, we can define the Interface class object to appear as a function object by defining the "construct" method as the function application method ("apply") instead as well:

~~~~scala
trait Interface {
  def operation(adjust: Int) : Int;
}

object Interface {
  def apply(startingValue : Int) : Interface = {
    class Impl(var current : Int) extends Interface {
      def operation(adjust: Int) : Int = {
        current += adjust
        return current
      }
    }
    return new Impl(startingValue)
  }
}

object App {
  def main(args : Array[String]) = {
    val intf = Interface(100)
    System.out.println(intf.operation(10))
  }
}
~~~~

This then looks like the Constructor Function from other functional langauges.

Because Scala is an object/functional hybrid language, it's relatively easy to use [Closure-based State](../../structural/ClosureBasedState) to hold the state outside of the actual object returned.

### Runtime replacement
However, the disadvantage to the above is that the Constructor Function cannot be replaced at runtime, since it is statitically defined as part of the class. Adjusting for this as a desired consequence is actually relatively simple in the Scala case: the Constructor Function is either captured as a lambda and used directly, or else hidden behind the construction method, depending on what the exact desired client construction semantics are:

~~~~scala
trait Interface {
  def operation(adjust: Int) : Int;
}

object Interface {
  var constructorFn = construct _
  
  def apply(startingValue: Int) : Interface = constructorFn(startingValue)
  
  def construct(startingValue : Int) : Interface = {
    class Impl(var current : Int) extends Interface {
      def operation(adjust: Int) : Int = {
        current += adjust
        return current
      }
    }
    return new Impl(startingValue)
  }
}

object App {
  def main(args : Array[String]) = {
    val intf = Interface(100)
    System.out.println(intf.operation(10))
    
    Interface.constructorFn = { startingValue =>
      class Impl(var current : Int) extends Interface {
        def operation(adjust: Int) : Int = {
          current += adjust * 2
          return current
        }
      }
      new Impl(startingValue)
    }

    val intf2 = Interface(100)
    System.out.println(intf2.operation(10))
  }
}
~~~~

Note the syntax for capturing the "construct" method inside of the Interface class object---Scala requires the use of the undescore to signify that we want to capture the function as a value, rather than invoke it. (The error message here can be a bit cryptic the first time you see it.)

### Parameterized construction
Scala uses type parameterized Constructor Functions throughout the runtime library to simplify the construction of several of its collection classes. For example, constructing a Map uses the class object's "apply" and the types passed in to determine what time of map to return:

~~~~scala
Welcome to Scala 2.11.8 (Java HotSpot(TM) 64-Bit Server VM, Java 1.8.0_20).
Type in expressions for evaluation. Or try :help.

scala> val capitals = Map("Tacoma" -> "WA")
capitals: scala.collection.immutable.Map[String,String] = Map(Tacoma -> WA)

scala> val pops = Map("Seattle" -> 5000000)
pops: scala.collection.immutable.Map[String,Int] = Map(Seattle -> 5000000)
~~~~

Here, the Map will obtain its key/value types out of the respective types of the pair (constructed using hte "->" operator) passed in. Syntactically, the client need not be aware of the actual types (or even the syntax) of the Map.

