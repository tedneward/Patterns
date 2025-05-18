title=Strategy: Scala
date=2022-03-16
type=pattern
tags=pattern implementation, behavioral, scala
status=published
description=A Strategy implementation in Scala.
~~~~~~

## Variant: Function-as-Object

In pure functional languages, it's actually difficult to keep state and data tied together--in fact, part of the whole point of a functional language is to write functions that operate on data, ideally on lots of different kinds of data. "Therefore, create a function that acts like an object. Such a function carries the data it needs along with the expression that operates on the data. More importantly, an object encapsulates its data, ensuring that only the allowed operations are applied to them." In other words, by writing a function and keeping the data buried inside of it, we achieve the same kind of encapsulation that object-orientation has traditionally reserved for itself as its principal advantage. This is done via a closure, which is the next element in the language.

Similarly, Scala allows us to bind functions to names, too:

```
def functionAsObject() = {
  def withdraw(balance : Int, amount : Int) = {
    if (amount <= balance) balance - amount else throw new RuntimeException("Insufficient funds")
  }
  def deposit(balance: Int, amount : Int) = {
    balance + amount
  }
  def accrueInterest(balance : Int, rate : Float) = {
    balance + (balance * rate)
  }
}
```

Again, all of it is wrapped into a function for easier (on me, while I was experimenting with all of this) scoping.

