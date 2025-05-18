title=Closure-Based State: Scheme
date=2022-03-16
type=pattern
tags=pattern implementation, structural, scheme
status=published
description=A Strategy implementation in Scheme.
~~~~~~
"You are writing a function with a free variable. How do you bundle a function with a data value defined outside the procedure's body?" If the data value is defined inside the procedure, remember, it gets reset to the same value each time, and obviously this isn't going to track state at all well. "So you might try defining the balance outside the function." But that doesn't work, because now the value isn't encapsulated anymore. "Therefore, create the function in an environment where its free variables are bound to local variables."

This is something that O-O folks won't see right away, but it's a powerful mechanism for reuse. Traditional O-O says to tuck the encapsulated value (balance) away as a private field, but in environments like JavaScript, which lack any sort of formal access control, or in environments like the JVM or CLR, both of which offer a means by which to bypass access control directives (via the Reflection libraries in both), what's marked as "private" often isn't as private as we might want. By creating a local variable that's outside the scope of the returned function object but inside of the scope of the function returning the function (see where "balance" is declared in the JavaScript version, for example), the language or platform has to "close over" that variable (hence the name "closure"), thus making it accessible to the returned function for use, but effectively hidden away from any prying eyes that might want to screw with it outside of permitted access channels.

The only key thing to note here is that "withdraw" references a lambda, a function literal in Scheme. We'll try to keep this flavor in the other language implementations, just to be faithful:

```
(define withdraw
  (let ((balance 100)) ;; balance is defined here,
    (lambda (amount)
      (if (>= balance amount) ;; so this reference is bound
        (begin
          (set! balance (- balance amount))
          balance)
        (error "Insufficient funds" balance)))
    ))
```
