title=Closure-Based State: Yeti
date=2022-03-16
type=pattern
tags=pattern implementation, structural, yeti
status=published
description=A Strategy implementation in Yeti.
~~~~~~
"You are writing a function with a free variable. How do you bundle a function with a data value defined outside the procedure's body?" If the data value is defined inside the procedure, remember, it gets reset to the same value each time, and obviously this isn't going to track state at all well. "So you might try defining the balance outside the function." But that doesn't work, because now the value isn't encapsulated anymore. "Therefore, create the function in an environment where its free variables are bound to local variables."

This is something that O-O folks won't see right away, but it's a powerful mechanism for reuse. Traditional O-O says to tuck the encapsulated value (balance) away as a private field, but in environments like JavaScript, which lack any sort of formal access control, or in environments like the JVM or CLR, both of which offer a means by which to bypass access control directives (via the Reflection libraries in both), what's marked as "private" often isn't as private as we might want. By creating a local variable that's outside the scope of the returned function object but inside of the scope of the function returning the function (see where "balance" is declared in the JavaScript version, for example), the language or platform has to "close over" that variable (hence the name "closure"), thus making it accessible to the returned function for use, but effectively hidden away from any prying eyes that might want to screw with it outside of permitted access channels.

Yeti works just as any of the others have, since we can define a function literal that returns a function literal, so just like the JavaScript and Scala versions, we can bind a variable (as opposed to a value, which is immutable) just outside the inner function literal, and Yeti will "close over" that variable and use it for modifiable state:

```
withdraw = 
  (do:
    var balance = 100;
    do amt:
      if amt <= balance then
        balance := balance - amt;
        balance
      else
        throw new RuntimeException("Insufficient funds")
      fi
    done;
  done;) ();

println (withdraw 10);  // prints 90
println (withdraw 10);  // prints 80
println (withdraw 10);  // prints 70
```

