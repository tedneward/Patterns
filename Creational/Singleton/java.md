title=Singleton: Java
date=2016-03-26
type=pattern
tags=pattern implementation, creational, java
status=published
description=A Singleton implementation in Java.
~~~~~~

Like most object-oriented typed languages, Java makes it pretty straightforward to code up a classic [Singleton](../Singleton):

````java
public class Product
{
  private Product() { }
  
  public static Product Instance() {
    return instance;
  }
  private static Product instance = new Product();
  
  public void DoSomething() {
    state++;
    System.out.println("I'm doing something for the " + state + " time");
  }
  private int state = 0;
}

public class Main
{
  public static void main(String... args) {
    Product.Instance().DoSomething();
    Product.Instance().DoSomething();
    Product.Instance().DoSomething();
  }
}
````

Like most of its kin, Java Singletons are eagerly-initialized, in that the Singleton is initialized and ready as soon as the class is loaded, but like most dynamically-loaded runtimes (the JVM and the CLR are both such creatures), the class won't be loaded until it is explicitly referenced somehow. (Accessing the static method is one such way to force the class-load.)

Java will usually use a static method, since it lacks any sort of property syntax.

#### Scopes
It is important to note that due to the implementation of the Java Virtual Machine, using statics (as above) will not actually be scoped to the entire JVM/process, but only to the ClassLoader that loaded the class. This is discussed in more detail in a variety of different places, including books and papers that I have written, as well as numerous other sources.

#### Serialization
Note that Java is a language which supports an opt-in automatic object serialization mechanism, and as such, if the Singleton is marked Serializable, developers must take additional care to ensure that the Singleton, when deserialized, still remains the only instance. Joshua Bloch talks about this in Effective Java. However, it's fair to ask why a Singleton might need to be serialized in the first place; if it is still determined that the Singleton must be serialized, then the follow-up question must be, "What happens when Singleton A is deserialized into a JVM where Singleton B already exists?" This becomes an important question around any state held in the two Singletons (which by itself, being a contradiction in terms, should be a red flag to the entire conversation), and whether that state should be merged, replaced, or cast aside. Danger Will Robinson, danger.

#### Enums-as-Singletons
Sebastian Kaiser pointed out (in comments) that Joshua Bloch talked about using enums in Java for Singletons:

````java
public enum Product {
  INSTANCE;
  
  private int state = 0;
  
  public void DoSomething() {
    ++state;
    System.out.println("I did something for the " + state + " time");
  }
}

public class Main
{
  Product.INSTANCE.DoSomething();
}
````

Bloch writes:

> "This approach is functionally equivalent to the public field approach, except that it is more concise, provides the serialization machinery for free, and provides an ironclad guarantee against multiple instantiation, even in the face of sophisticated serialization or reflection attacks. While this approach has yet to be widely adopted, a single-element enum type is the best way to implement a singleton."

While I'm not entirely convinced it's "the best way" to do it, it certainly represents a pretty easy implementation. As near as I can tell, however, it doesn't support the inheritance note that the GOF originally pointed out, though whether that's a paramount concern remains, of course, context-dependent.

Technically, enums in Java are classes, but there reaches a point where the Singleton may be sophisticated enough that the simplicity of implementing the Singleton as an enum gets overweighed by the 'weirdness' of using an enum to do it; for example, if the enum requires any level of sophistication in its initializer, writing a private constructor for an enum is doable, but just doesn't feel idiomatically correct:

````java
public enum Product {
  INSTANCE;

  private Product() {
    state = 27;
  }
  
  private int state;
  
  public void DoSomething() {
    ++state;
    System.out.println("I did something for the " + state + " time");
  }
}
````

As a result, this could end up causing some angst among the Java developers on the team; again, this is entirely context-dependent. (Note that there shouldn't ever need to be any constructor that accepts anything other than void, since by definition the Singleton shouldn't need to accept state passed into it.)

There is also a slight disconnect here in the naming of the instance accessor; classic Singleton has always made this either a method or property (hence the leading-caps in some implementations), but here, since the Java naming conventions suggest that a constant should be in all-caps, Bloch chooses the latter syntax. It's unusual, generally, to think of Singletons as a constant (though it's not unreasonable to do so). This represents a bit of a disconnect with the intent of an enumeration, which is supposed to represent a "type with a bounded number of possible values" (such as Gender, or Boolean, or the various states that comprise the United States), not a type with only one applicable instance.

The enums approach also does not allow for some of the variations on Singletons, such as the Registry, or any sort of scope other than process-wide.

#### Concurrent access
The JVM being a multi-threaded platform, it should usually be assumed that the Singleton can and will be accessed from multiple threads (whether that was originally intended or not!). As a result, some level of concurrency-safety needs to be implemented on the instance; one such (potentially naive) way of doing so is to mark the relevant methods as "synchronized", though the preferable approach is to hide the concurrency-safety details a little bit more deeply:

````java
import java.util.concurrent.locks.*;

public class ThreadsafeProduct
{
  private ThreadsafeProduct() { }
  
  public static ThreadsafeProduct Instance() {
    return instance;
  }
  private static ThreadsafeProduct instance = new ThreadsafeProduct();
  
  public void DoSomething() {
    lock.lock();
    try {
      state++;
      System.out.println("I'm doing something for the " + state + " time");
    }
    finally {
      lock.unlock();
    }
  }
  private int state = 0;
  private Lock lock = new ReentrantLock();
}

public class Main
{
  public static void main(String... args) {
    ThreadsafeProduct.Instance().DoSomething();
    ThreadsafeProduct.Instance().DoSomething();
    ThreadsafeProduct.Instance().DoSomething();
  }
}
````

*NOTE*: This is not to imply that the above is a particularly good concurrent implementation; such a subject is well beyond the intent of this example, and interested parties are referred to [JCiP](http://www.amazon.com/Java-Concurrency-Practice-Brian-Goetz/dp/0321349601) for (much) more detail.

#### Thread-scoped Singleton
As it turns out, a related concurrency example of Singletons is also an example of "scoping" Singletons differently than traditionally assumed. Java makes available a `ThreadLocal` class that will implicitly store the value, but store separate values per Thread. The Javadocs for it show some basic usage:

````java
import java.util.concurrent.atomic.AtomicInteger;

public class ThreadId {
  // Atomic integer containing the next thread ID to be assigned
  private static final AtomicInteger nextId = new AtomicInteger(0);

  // Thread local variable containing each thread's ID
  private static final ThreadLocal<Integer> threadId =
    new ThreadLocal<Integer>() {
      @Override protected Integer initialValue() {
        return nextId.getAndIncrement();
    }
  };

  // Returns the current thread's unique ID, assigning it if necessary
  public static int get() {
    return threadId.get();
  }
}
````

Each time `ThreadId.get()` is called, the "thread-specific Singleton" thread ID will be returned.
