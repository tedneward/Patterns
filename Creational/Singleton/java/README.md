# Singleton: Java
Like most object-oriented typed languages, Java makes it pretty straightforward to code up a classic [Singleton](../README.md):

````java
/**
 * Singleton class that ensures only one instance is created.
 * This class uses the eager initialization method to create the instance.
 */
public class ProductV1 {
  private static Product instance = new Product();
  private Product() { }
  public static Product instance() {
    return instance;
  }
  
  private int state = 0;
  public int getState() {
    return state;
  }
  
  public void doSomething() {
    state++;
    System.out.println("I'm doing something for the " + state + " time");
  }
}
````

Like most of its kin, the simplest Java Singletons are eagerly-initialized, in that the Singleton is initialized and ready as soon as the class is loaded, but like most dynamically-loaded runtimes (the JVM and the CLR are both such creatures), the class won't be loaded until it is explicitly referenced somehow. (Accessing the static method is one such way to force the class-load.)

Java will usually use a static method, since it lacks any sort of property syntax.

#### Scopes
It is important to note that due to the implementation of the Java Virtual Machine, using statics (as above) will not actually be scoped to the entire JVM/process, but only to the ClassLoader that loaded the class. This is discussed in more detail in a variety of different places, including books and papers that I have written, as well as numerous other sources.

#### Serialization
Note that Java is a language which supports an opt-in automatic object serialization mechanism, and as such, if the Singleton is marked Serializable, developers must take additional care to ensure that the Singleton, when deserialized, still remains the only instance. Joshua Bloch talks about this in Effective Java. However, it's fair to ask why a Singleton might need to be serialized in the first place; if it is still determined that the Singleton must be serialized, then the follow-up question must be, "What happens when Singleton A is deserialized into a JVM where Singleton B already exists?" This becomes an important question around any state held in the two Singletons (which by itself, being a contradiction in terms, should be a red flag to the entire conversation), and whether that state should be merged, replaced, or cast aside. *Danger Will Robinson, danger.*

#### Concurrent access
The JVM being a multi-threaded platform, it should usually be assumed that the Singleton can and will be accessed from multiple threads (whether that was originally intended or not!). As a result, some level of concurrency-safety needs to be implemented on the instance; one such (potentially naive) way of doing so is to mark the relevant methods as "synchronized", though the preferable approach is to hide the concurrency-safety details a little bit more deeply:

````java
package org.example;

import java.util.concurrent.locks.Lock;
import java.util.concurrent.locks.ReentrantLock;

public class ProductV2 {
    private static ProductV2 instance;

    // Private constructor to prevent instantiation
    private ProductV2() {
        // Initialization code here
    }

    // Public method to provide access to the instance
    public synchronized static ProductV2 getInstance() {
        if (instance == null) {
            instance = new ProductV2();
        }
        return instance;
    }

    private int counter = 0;
    private Lock lock = new ReentrantLock();

    public void doSomething() {
        lock.lock();
        try {
            counter++;
            System.out.println("We are doing something with the Product, for the " + counter + "th time.");
        }
        finally {
            lock.unlock();
        }
    }
    public int getCounter() {
        return counter;
    }
}
````

*NOTE*: This is not to imply that the above is a particularly good concurrent implementation; such a subject is well beyond the intent of this example, and interested parties are referred to [JCiP](http://www.amazon.com/Java-Concurrency-Practice-Brian-Goetz/dp/0321349601) for (much) more detail.

#### Enums-as-Singletons
Sebastian Kaiser pointed out (in comments) that Joshua Bloch talked about using enums in Java for Singletons:

````java
public enum ProductV3 {
    INSTANCE;

    private int counter = 0;

    // Method to perform some action
    public void doSomething() {
        counter++;
        System.out.println("We are doing something with the Product, for the " + counter + "th time.");
    }

    // Method to get the current counter value
    public int getCounter() {
        return counter;
    }

}
````

This version eschews the multi-threaded access of the V2 version, mostly out of simplicity. Note that initialization is now entirely up to the classloading and resolution rules around enums in the JVM, rather than being explicitly eager or lazy. (It doesn't take much research to realize that it's fundamentally an eagerly-initialized approach.)

Bloch writes:

> "This approach is functionally equivalent to the public field approach, except that it is more concise, provides the serialization machinery for free, and provides an ironclad guarantee against multiple instantiation, even in the face of sophisticated serialization or reflection attacks. While this approach has yet to be widely adopted, a single-element enum type is the best way to implement a singleton."

While I'm not entirely convinced it's "the best way" to do it, it certainly represents a pretty easy implementation. As near as I can tell, however, it doesn't support the inheritance note that the GOF originally pointed out, though whether that's a paramount concern remains, of course, context-dependent.

Bloch places great store by the need/desire to serialize the Singleton; I still remain unconvinced that this is a necessary feature, at least within most Singleton scenarios.

Technically, enums in Java are classes, but there reaches a point where the Singleton may be sophisticated enough that the simplicity of implementing the Singleton as an enum gets overweighed by the 'weirdness' of using an enum to do it; for example, if the enum requires any level of sophistication in its initializer, writing a private constructor for an enum is doable, but just doesn't feel idiomatically correct (and note that there shouldn't ever need to be any constructor that accepts anything other than void, since by definition the Singleton shouldn't need to accept state passed into it):

````java
public enum ProductV3 {
  INSTANCE;

  private ProductV3() {
    state = 27;
  }
  
  private int state;
  
  public void doSomething() {
    ++state;
    System.out.println("I did something for the " + state + " time");
  }
}
````

... but keep in mind that we could accomplish the same initialization *outside* the constructor:

````java
public enum ProductV3 {
  INSTANCE;

  private int state = 27
  
  public void doSomething() {
    ++state;
    System.out.println("I did something for the " + state + " time");
  }
}
````

... or even:

````java
public enum ProductV3 {
  INSTANCE;

  private int state;

  // This is an anonymous initializer block, and is compiled "into" any constructor
  // defined for this class
  {
    state = 27;
  }
  
  public void doSomething() {
    ++state;
    System.out.println("I did something for the " + state + " time");
  }
}
````

This "wait, enums can do this" kind of syntax could end up causing some angst among the Java developers on the team.

There is also a slight disconnect here in the naming of the instance accessor; classic Singleton has always made this either a method or property (hence the leading-caps in some implementations), but here, since the Java naming conventions suggest that a constant should be in all-caps, Bloch chooses the latter syntax. It's unusual, generally, to think of Singletons as a constant (though it's not unreasonable to do so). This represents a bit of a disconnect with the intent of an enumeration, which is supposed to represent a "type with a bounded number of possible values" (such as Gender, or Boolean, or the various states that comprise the United States), not a type with only one applicable instance.

The enums approach also does not allow for some of the variations on Singletons, such as the Registry, or any sort of scope other than process-wide.

#### Thread-scoped Singleton
As it turns out, a related concurrency example of Singletons is also an example of "scoping" Singletons differently than traditionally assumed. Java makes available a `ThreadLocal` class that will implicitly store the value, but store separate values per Thread. The Javadocs for it show some basic usage:

````java
import java.util.concurrent.atomic.AtomicInteger;

public class ThreadId {
  // Atomic integer containing the next thread ID to be assigned
  private static final AtomicInteger nextId = new AtomicInteger(0);

  // Thread local variable containing each thread's ID
  private static final ThreadLocal<Integer> threadedId =
    new ThreadLocal<Integer>() {
      @Override protected Integer initialValue() {
        return nextId.getAndIncrement();
    }
  };

  // Returns the current thread's singleton's unique ID, assigning it if necessary
  public static int get() {
    return threadedId.get();
  }
}
````

Each time `ThreadId.get()` is called, the "thread-specific Singleton" ID will be returned.
