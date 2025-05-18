title=Strategy: Yeti
date=2022-03-16
type=pattern
tags=pattern implementation, behavioral, yeti
status=published
description=A Strategy implementation in Yeti.
~~~~~~

## Variant: Function-as-Object

In pure functional languages, it's actually difficult to keep state and data tied together--in fact, part of the whole point of a functional language is to write functions that operate on data, ideally on lots of different kinds of data. "Therefore, create a function that acts like an object. Such a function carries the data it needs along with the expression that operates on the data. More importantly, an object encapsulates its data, ensuring that only the allowed operations are applied to them." In other words, by writing a function and keeping the data buried inside of it, we achieve the same kind of encapsulation that object-orientation has traditionally reserved for itself as its principal advantage. This is done via a closure, which is the next element in the language.

Although Yeti supports a slightly more succinct syntax for defining a function, I choose to use the syntax that more closely matches what we're doing in the other examples--bind a function literal (do ... done;) to a name (withdraw, deposit and accrueInterest). Again, since this is running on top of the JVM, we have full access to the underlying Java library, which means we can make use of RuntimeException again as a cheap way of signaling a bad withdrawal.

```ml
withdraw = 
  do bal amt:
    if amt <= bal then
      bal - amt
    else
      throw new RuntimeException("Insufficient funds")
    fi
  done;

deposit =
  do bal amt: bal + amt done;
  
accrueInterest =
  do bal intRate:
    bal + (bal * intRate)
  done;

balance = 100;
println (withdraw balance 10)
```
