title=Closure-based State: Java
date=2016-04-03
type=pattern
tags=pattern implementation, structural, java
status=published
description=A Closure-based State implementation in Java.
~~~~~~

Although Java has only recently obtained function literals (lambdas) as a part of the formal language definition in Java8, [Closure-based State](../ClosureBasedState) has been something Java has been able to do for some time, so long as certain restrictions were obeyed (or worked around). Accordingly, we will split this implementation up into two parts: one Java8-and-beyond, and the other pre-Java8.

#### Java8
Java8 provides several different interfaces for capturing function instances, the core of which is the Function<T,R> interface, taking a parameter of type T and returning a value of type R. (All of this is in the [java.util.function](https://docs.oracle.com/javase/8/docs/api/java/util/function/package-summary.html) package, by the way.) Function literals can be captured into references using these interfaces, so creating a function-only version of the Closure-based State would look something like:

````java
public class CloState
{
  static class IntHolder {
    public int value;
  }
  
  public static void main(String... args) {
    Function<Integer, Integer> operation = 
      ( (Supplier<Function<Integer,Integer>>) ( () -> {
        IntHolder state = new IntHolder();
        state.value = 100;
        Function<Integer, Integer> func = (adj) -> {
          state.value += adj;
          return state.value;
        };
        return func;
      })).get();

    System.out.println(operation.apply(100));
  }
}
````

The `IntHolder` type is necessary because Java function literals, when they "close over" a bound variable (like "state"), require that the closed-over variable actually be `final` or, as of Java8, "effectively `final`" (meaning immutable). We could close over an `Integer` instance, but since the wrapper types are themselves immutable, we need a new "wrapper type" that allows for mutability; in this case, we just create a simple placeholder. (On a related note, it would be nice to be able to genericize this "Holder" type using parameterized types, but since Java generics simply type-erase into Object references we go right back to square one when doing so.)

Note also that while the inner function is a `Function<Integer,Integer>`, the outer wrapper is a `Supplier<Function<Integer,Integer>>`, which seems appropriate, since the outer wrapper is supplying an instance of that function type. (Pragmatically speaking, `Supplier` takes no paramters and returns a type, which fit the bill here.)

It is a shame that Java doesn't support direct invocation of the returned Function object, but it's not the end of the world.

#### Pre-Java8
Prior to Java8, similar kinds of results can be obtained using anonymous inner-class instances of an interface, and this actually has a benefit that the pure function-literal-based approach lacks, namely in that the state can be "carried along" in the anonymous inner-class instance, rather than being "closed over" by it:

````java
public class CloState
{
  public interface Function<T, R> {
    public R apply(T t);
  }

  public static void main(String... args) {
    Function<Integer,Integer> operation =
      (new Function<Void, Function<Integer,Integer>>() {
        public Function<Integer,Integer> apply(Void v) {
          Function<Integer,Integer> func = new Function<Integer,Integer>() {
            int state = 100;
            public Integer apply(Integer adj) {
              state += adj;
              return state;
            }
          };
          return func;
        }
      }).apply(null);
    System.out.println(operation.apply(100));
  }
}
````

(For pedagogical purposes, I kept the interface names the same as what we see in Java8; as such, should this code be used, make sure java.util.function is not imported in this compilation unit.)

Doing it this way doesn't exactly meet all of the desired consequences of Closure-based State, since now the state is a field of the object and therefore discoverable via Reflection, but it does simplify the picture somewhat and clearly demonstrate the relationship/nearness of Closure-based State and Strategy.

If the state moves to outside the inner Function implementation, then the same "effectively final" issues from the Java8 implementation will kick in.

#### Objects
Using Closure-based State in Java is actually easier than some other languages due to its ability to return anonymous inner-class implementations of either interfaces or classes. Consider the following Product class, deliberately marked abstract:

````java
public class CloState
{
  static public abstract class Product {
    public abstract int Operation(int adjust);
    
    public static Product New() {
      return new Product() {
        int state = 100;
        public int Operation(int adjust) {
          state += adjust;
          return state;
        }
      };
    }
  }
  
  public static void main(String... args) {
    Product p = Product.New();
    System.out.println(p.Operation(100));
  }
}
````

(Product is marked as a "static" class because it is embedded entirely inside of CloState for ease of compilation of the example. Normally it would be a top-level class.)

As in the pre-Java8 implementation, we choose to store the state as a field in the object, for simplicity's sake (to avoid the "effectively final" issue), which again makes it available via Reflection, but simplifies the overall picture. The New method acts as a [Constructor Function](../ConstructorFunction); the choice of names may be confusing to some and intuitive to others, but it's not important to the overall picture.

## Variations
It is possible to use Java's dynamic proxies to provide Closure-based State (or at least some form of it), but usually the point of the proxy is to "wrap" ([Decorator](../Decorator)-style) another object, and here there is no target object to enclose.

