title=Strategy: Javascript
date=2022-03-16
type=pattern
tags=pattern implementation, behavioral, javascript
status=published
description=A Strategy implementation in Javascript.
~~~~~~

## Variant: Function-as-Object

In pure functional languages, it's actually difficult to keep state and data tied together--in fact, part of the whole point of a functional language is to write functions that operate on data, ideally on lots of different kinds of data. "Therefore, create a function that acts like an object. Such a function carries the data it needs along with the expression that operates on the data. More importantly, an object encapsulates its data, ensuring that only the allowed operations are applied to them." In other words, by writing a function and keeping the data buried inside of it, we achieve the same kind of encapsulation that object-orientation has traditionally reserved for itself as its principal advantage. This is done via a closure, which is the next element in the language.

In JavaScript, we can bind function values to names just as we can in Scheme, so it's not actually all that different, once you get past the lack of parentheses and added curly braces. Thus, it looks like:

```javascript
(function() {
  out("function-as-object =========")
  
  var balance = 0
  var withdraw = function(amount) {
    if (amount <= balance)
      balance = balance - amount
    else
      throw new Error("Insufficient funds")
  }
  var deposit = function(amount) {
    balance += amount
  }
  var accrueInterest = function(interestRate) {
    balance += (balance * interestRate)
  }
})()
```

Note that I wrap all of it into its own function so as to give the whole thing some scope--makes it easier to define in a single .js file and execute.
