title=Strategy: Scheme
date=2022-03-16
type=pattern
tags=pattern implementation, behavioral, scheme
status=published
description=A Strategy implementation in Scheme.
~~~~~~

## Variant: Function-as-Object

In pure functional languages, it's actually difficult to keep state and data tied together--in fact, part of the whole point of a functional language is to write functions that operate on data, ideally on lots of different kinds of data. "Therefore, create a function that acts like an object. Such a function carries the data it needs along with the expression that operates on the data. More importantly, an object encapsulates its data, ensuring that only the allowed operations are applied to them." In other words, by writing a function and keeping the data buried inside of it, we achieve the same kind of encapsulation that object-orientation has traditionally reserved for itself as its principal advantage. This is done via a closure, which is the next element in the language.

The original Scheme implementation looked like this:

```scheme
(define balance 0)
(define withdraw
  (lambda (balance amount)
    (if (<= amount balance)
      (- balance amount)
      (error "Insufficient funds" balance))
  ))
(define deposit
  (lambda (balance amount)
    (+ balance amount)
  ))
(define accrue-interest
  (lambda (balance interest-rate)
    (+ balance (* balance interest-rate))
  ))
```

There's a few things wrong with this approach, as Wallingford points out, but to be faithful, recreating this in our target languages is pretty straightforward: three functions, each of which operate on parameters passed in. "You could create new accounts simply by binding values to names. Operating on accounts involves passing the account to the appropriate procedure and binding the new value as appropriate."


