title=Constructor Function: Java
date=2016-05-23
type=pattern
tags=pattern implementation, creational, java
status=published
description=A Constructor Function implementation in Java.
~~~~~~

The [Constructor Function](../ConstructorFunction) is not a pattern commonly used in Java, since most of the time the desire when holding state is to hold it as accessible member variables of an object, which Java supports directly via fields. Additionally, thanks to Java's "C-family" heritage, it directly supports encapsulatory access control (meaning we can annotate the field with "private" to prevent access to the field from outside the class). However, in some cases, this standard mode of access control is not enough, and/or we want to provide a slightly different means of construction for clients. This is when a Constructor Function can be of use, usually modeled as a static method (or methods) on the class itself.

The simplest form of a Constructor Function doesn't bother with any sort of closure-based state, and simply uses a static method to construct the object:

````java
public class Person
{
  public static Person newPerson(String fn, String ln, int a) {
    return new Person(fn, ln, a);
  }
  
  public String toString() {
    return "[Person firstName:" + firstName + 
      " lastName:" + lastName + 
      " age:" + age + "]";
  }
  
  private Person(String fn, String ln, int a) {
    firstName = fn;
    lastName = ln;
    age =a;
  }
  
  private String firstName;
  private String lastName;
  private int age;
}
````

The only real significant advantage here is the fact that construction details are now hidden behind an artificial interface, allowing for some slight decoupling of details, such as returning an interface instead of the actual object instance being constructed. This is often what Java patterns enthusiasts refer to when they refer to a "factory" pattern (without qualification).

There is also a slight advantage here in that should the Constructor Function need to throw an exception, handling exception details from a method call "feels" a little more normal than handling exception details from a constructor call; in practice, there is no real difference, but (anecdotally) many Java developers have expressed surprise that a constructor throwing an exception could be considered good design.

Note that this is also one case where Java's "lowercaseLeading" style of naming convention becomes a pain--ideally, it would be nice to call the static method "new", in order to better mimic the use of the keyword, but unfortunately, using lowercase makes it look like the keyword exactly. (C# developers would call it "New", which is close enough to the keyword to make it clear what's going on, yet doesn't conflict with the keyword.) A nit, but one in an annoying place.

### Lambdas and inner classes
Constructor Functions can also be Java lambdas, however, which is more in keeping with the concept of the Constructor Function pattern as it was originally envisioned in the functional-language world. When combined with interfaces, this looks like:

````java
// In Interface.java
public interface Interface {
  public int operation(int adjust);
}

// In Main.java
import java.util.function.*;

public class Main {
    public static void main(String[] args) {
        Function<Integer, Interface> creator = (Integer startingTotal) -> {
            return new Interface() {
                int total = startingTotal;

                @Override
                public int operation(int adjust) {
                    return (total += adjust);
                }
            };
        };

        Interface obj = creator.apply(100);
        System.out.println(obj.operation(10));
    }
}
````

The anonymous-inner-class-implementation feature of Java makes Constructor Function actually quite trivial to implement this way---the interface serves as the strongly-typed "surface" to the object, providing the compiler the hints it needs to compile with type safety, but the fact that the implementation is anonymous (as far as the compiler is concerned---of course, at runtime, it will have a classname, something like "Main$1", but this name is neither guaranteed nor really ever seen, except in Reflection scenarios). Note that the field doesn't even need to be marked as private, since it can never be referenced (except, again, through Reflection).

Of course, the same kind of results can be achieved using static methods instead of a lambda instance, if the Constructor Function is not stored somewhere with the intent of modifying/replacing it.

One significant advantage of this approach (over the [Closure-Based State](../ClosureBasedState) approach for objects returned from a Constructor Function) is that debugging is much more natural to the Java environment---the debugger simply displays the "total" field in the Variables or Watch window, since it's a normal field.

### Accessibility
As has been noted, however, the field still can be referenced via Reflection pretty easily:

````java
    public static void main(String[] args) throws Exception {
        Function<Integer, Interface> creator = (Integer startingTotal) -> {
            return new Interface() {
                int total = startingTotal;

                @Override
                public int operation(int adjust) {
                    return (total += adjust);
                }
            };
        };

        Interface obj = creator.apply(100);
        System.out.println(obj.operation(10));

        Field f = obj.getClass().getDeclaredField("total");
        System.out.println(f.get(obj)); // prints 110
    }
````

Even were this field declared "private", the Java developer need only call `setAccessible(true)` to gain access. (This will trigger a security check within the Java runtime security permissions system, but that will only ever have an effect if the JVM is running with Permissions "turned on", a la via the securitymanager system property.) Moving this code over to using Closure-based State will not eliminate the ability to find the field entirely, but will at least make it more difficult to chase down. However, doing this now runs into Java's "final or effectively final" requirement, in that enclosed bound variables (such as "total") need to be either final or "effectively final" (i.e., unmodified):

````java
    public static void main(String[] args) throws Exception {
        Function<Integer, Interface> creator = (Integer startingTotal) -> {
            Integer total = startingTotal;
            return new Interface() {
                @Override
                public int operation(int adjust) {
                    return (total += adjust); // *** WILL NOT COMPILE
                }
            };
        };

        Interface obj = creator.apply(100);
        System.out.println(obj.operation(10));

        Field f = obj.getClass().getDeclaredField("total");
        System.out.println(f.get(obj));
    }
````

The only real way around this is to create a "MutableInteger" variant of the `java.lang.Integer` class, hold that internally, and allow its integer contents to be adjusted:

````java
import java.util.function.*;
import java.lang.reflect.*;

class MutableInteger {
    public MutableInteger(int v) { value = v; }
    public int value;
}

public class Main {

    public static void main(String[] args) throws Exception {
        Function<Integer, Interface> creator = (Integer startingTotal) -> {
            MutableInteger total = new MutableInteger(startingTotal);
            return new Interface() {
                @Override
                public int operation(int adjust) {
                    return (total.value += adjust);
                }
            };
        };

        Interface obj = creator.apply(100);
        System.out.println(obj.operation(10));

        Field f = obj.getClass().getDeclaredField("total");
        System.out.println(f.get(obj));
    }
}
````

Now when run, the `getDeclaredField()` will throw a NoSuchFieldException. However, the field is still there, just under a new (obscured) name:

````java
        Interface obj = creator.apply(100);
        System.out.println(obj.operation(10));

        Field[] fields = obj.getClass().getDeclaredFields();
        for (Field f : fields) {
            System.out.println(f); 
                // prints "final MutableInteger Main$1.val$total"
        }
````

This means that the field will still be serialized (which may be a good thing or a bad thing, depending) and is still accessible, but it's clearly not a "direct" field of the class, which may be sufficient to hide it from some prying eyes.
