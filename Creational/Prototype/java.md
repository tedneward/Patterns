title=Prototype: Java
date=2022-03-09
type=pattern
tags=pattern implementation, creational, java
status=published
description=A Prototype implementation in Java.
~~~~~~

Java has a couple of approaches to doing a Prototype.

## First approach: Java Object Serialization
One easy way to do this (if a touch roundabout) is to mark the object(s) that are part of the Prototype to be Serializable, then serialize-and-deserialize the object. This gets us a perfect clone of the ConcretePrototype, albeit it at the expense of a memory buffer large enough to hold the serialized representation.

Let's examine a simple scenario in which we have Person objects that should be Prototype-based construction:
```java
class Person implements Serializable {
    public String firstName;
    public String lastName;
    public int age;

    public Person clone() {
        try {
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            ObjectOutputStream oos = new ObjectOutputStream(baos);
            oos.writeObject(this);
            ByteArrayInputStream bais = new ByteArrayInputStream(baos.toByteArray());
            ObjectInputStream ois = new ObjectInputStream(bais);
            return (Person)ois.readObject();
        }
        catch (ClassNotFoundException cnfEx) {
            // Um....
            cnfEx.printStackTrace();
        }
        catch (IOException ioEx) {
            // Um....
            ioEx.printStackTrace();
        }
        return null; // should never get here...
    }
}
```

(In this contrived example, fields are public for ease.) On its own, there doesn't seem to be much advantage to this approach as opposed to a traditional constructor:

```java
public class Driver {
    public static void main(String... args) {
        Person ted = new Person();
        ted.firstName = "Ted";
        ted.lastName = "Neward";
        ted.age = 50;

        Person otherTed = ted.clone();
        System.out.println(otherTed.firstName + " " + otherTed.lastName);
    }
}
```

However, if we torture the metaphor a little bit more, we realize that `Person`s aren't really cloned from individuals, they come from pairs, and the Prototype pattern can help model that fairly easily:

```java
class Pairing {
    public Person male;
    public Person female;

    public Person geneticCombinator() {
        Person baby = female.clone();
        baby.lastName = male.lastName();
        baby.age = 0;
        return baby;
    }
}
```

Obviously we're playing fast and loose with some facts and assumptions here (implying that babies are perfect genetic clones of their mothers but carry their fathers' last name), but the point hopefully comes through: the returned Person object is the byproduct of two different Person objects, rather than just a straightforward mirror image of one.

Although we don't write it above as such, the Serialziation-based approach has the major advantage of being applicable to any Serializable hierarchy anywhere in the Java ecosystem by writing a single type-paramterized function to do the serialization-deserialization steps in one place:

```java
class Prototyper {
    public static <T> T clone(T source) {
        try {
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            ObjectOutputStream oos = new ObjectOutputStream(baos);
            oos.writeObject(source);
            ByteArrayInputStream bais = new ByteArrayInputStream(baos.toByteArray());
            ObjectInputStream ois = new ObjectInputStream(bais);
            return (T)ois.readObject();
        }
        catch (ClassNotFoundException cnfEx) {
            // Um....
            cnfEx.printStackTrace();
        }
        catch (IOException ioEx) {
            // Um....
            ioEx.printStackTrace();
        }
        return null; // should never get here...
    } 
}

class Person {
    // ...
    public Person clone() {
        return Prototyper.clone(this);
    }
}
```

This adds a little type-safety to the cloning process as well. (Defensively, we probably should also assert that the type `T` is Serializable, and/or declare the `source` parameter to be of type `Serializable`, to ensure that objects looking to be cloned are able to do so using Serialization.)

A couple of consequences to this implementation come up:

* ***clone() is technically reserved for Cloneable-based deep copying.*** The `clone` method used above is inherited from `java.lang.Object` and was originally intended for use by Cloneable objects. It was quickly deprecated, but there could be some accidental conflicts that arise out of repurposing this method this way. An easy alternative is to use a synonym to "clone" for the method name, but the original GOF pattern uses the name "clone", and it's tempting to keep the name the same.

* ***Anything marked `transient` will not be a part of the cloned object.*** This is because the Serialization system is designed that way--so, naturally, if we are resting upon Serialization to do the cloning, we are subject to its rules and restrictions. Normally, a field is only marked as such because we don't want it traversing the network (perhaps it is PII or the channel is unencrypted), but here, transient can be used to keep per-object state from appearing in clones. It's a bit of a hack, though, and not entirely clear in its intent as to what's going on.

* ***Serialization is slow.*** Compared to simple object construction and initialization, serializing and deserializing is ridiculously slow, by orders of magnitude. This is partly because Serialization is a general-purpose mechanism, ensuring that any aliased objects in the graph are only captured once, as well as the act of transforming the data values into a binary byte format (and then back again).

* ***Serialization hooks will be honored, which might disrupt the intent of the Prototyping.*** Java Object Serialization has a number of "hook points" in which custom code can be executed to customize or extend the Java Object Serialization mechansim. Any of these could end up disrupting the creation of new objects by silently changing (or eliding) the values being cloned.

* ***Serialization neatly handles subclasses.*** If we create subclasses of Person (Student, Instructor, whatever), the Serialization mechanism has built-in rules and behavior to handle all possible situations. That makes this approach neatly maintainable in the face of evolution--we literally don't have to do anything, and this approach still works, so long as all subclasses obey the laws of Serialization, most notably, the one that says that any non-transient field reference type must also be Serializable.

## Second (non-)approach: Java Cloneable
The Cloneable interface and `clone` method from `java.lang.Object` would seem to be a viable approach, but Cloneable and `clone` have been deprecated since JDK 1.1. While technically still feasible (in theory, anyway--I haven't tried in decades), it's definitely not the preferable approach.

## Third approach: Reflection
In this scenario, we use Reflection to do the same thing the Serialization approach takes: scoop out all the fields of the object before instantiating a new one (likely via `Class.newInstance`) and filling in the fields.

```java
class ReflectivePrototyper {
    public static <T> T clone(T source) {
        Class tClass = source.getClass();
        T clone = null;
        try { clone = (T)tClass.newInstance(); } catch (Exception ex) { ex.printStackTrace(); }
        try {
            for (Field f : tClass.getDeclaredFields()) {
                f.setAccessible(true);
                f.set(clone, f.get(source));
            }
        }
        catch (Exception ex) { ex.printStackTrace(); }
        return clone;
    }
}
```

Using this approach has the advantage of allowing us to clone objects that aren't Serializable, and it can be used with objects that aren't deliberately designed to be cloned (which is both good and bad).

Note: The above approach does ***not*** capture subclasses, because the `getDeclaredFields` will only get the fields declared on that class, not any inherited from a base class. Therefore, to support subclasses, the field-for-loop would need to be embedded in another loop that goes to each superclass and repeats the field-for-loop (until reaching `java.lang.Object`).

Also, this relies on the code being able to have the necessary Reflection permissions to be able to access the private members of classes within this ProtectionDomain, so if any SecurityManager is instantiated or a Security policy is in place, there's a strong chance this approach will crash on takeoff.

And, like Serialization, it's slow. Reflection information is almost always stored in out-of-the-way places inside the virtual machine (because it's not accessed frequently), and there's a ton of type-checking that goes on inside the Reflection code to ensure a careless developer isn't breaking assumptions (like type-safety of fields and assignments to fields).

This does have the advantage of not requiring Serialization, however, and doesn't have the drawbacks of not being able to handle non-Serializable types and having to worry about `transient` field declarations.

## Fourth approach: By-hand cloning
In this scenario, we build the duplication mechanics directly into each class that seeks to be cloned:

```
class Person {
    // ...
    public Person clone() {
        Person clone = new Person();
        clone.firstName = this.firstName;
        clone.lastName = this.lastName;
        clone.age = this.age;
        return clone;
    }
}
```

Subclasses must either have access to the base class fields (via `protected`), or else must call the base class `clone`, or we have to rely on runtime type information to get the most-derived-type from the object at runtime and use that object's `Class` object to instantiate the appropriate type:

```java
class Person {
    public String firstName;
    public String lastName;
    public int age;

    public Person clone() {
        Person clone = null;
        try { clone = this.getClass().newInstance(); } catch (Exception ex) { ex.printStackTrace(); }
        clone.firstName = firstName;
        clone.lastName = lastName;
        clone.age = age;
        return clone;
    }
}

class Student extends Person {
    public String field;

    public Student clone() {
        Student clone = (Student)super.clone();
        clone.field = field;
        return clone;
    }
}

public class Driver {
    public static void main(String... args) {
        Person ted = new Person();
        ted.firstName = "Ted";
        ted.lastName = "Neward";
        ted.age = 50;

        Person otherTed = ted.clone();
        System.out.println(otherTed.firstName + " " + otherTed.lastName);

        Student fred = new Student();
        fred.firstName = "Fred";
        fred.lastName = "Flintstone";
        fred.age = 49;
        fred.field = "Geology";

        Student otherFred = fred.clone();
        System.out.println(otherFred.field);
    }
}
```

This does feel a little less heavy-handed than the Serialization approach, at the expense of having to create a new `clone` method for every subclass--which may actually turn out to be a benefit as well, since it allows developers to make deliberate and thoughtful choices about what fields should be cloned. (Note that "allowing developers to make deliberate and thoughtful choices" is also a general recipe for "requiring developers to be disciplined", which often doesn't work out well in the long run as cognitive load sets in.)
