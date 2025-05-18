title=Constructor Function: Yeti
date=2022-03-16
type=pattern
tags=pattern implementation, creational, yeti
status=published
description=A Constructor Function implementation in Yeti.
~~~~~~
"You are creating a Function as Object using a Closure. How do you create instances of the object? [M]ake a function that returns your Function as Object. Give the function an Intention Revealing Name (Beck) such as make-object."

Same exercise--a function binding that returns a function, with the passed-in "balance" stored as a variable (var) inside the outer function scope, such that it is closed over by the inner function scope.

```
makeWithdraw =
  (do bal:
    var balance = bal;
    do amt:
      if amt <= balance then
        balance := balance - amt;
        balance
      else
        throw new RuntimeException("Insufficient funds")
      fi
    done;
  done;);

acctForEugene = makeWithdraw 100;
println (acctForEugene 10);   // 90
println (acctForEugene 10);   // 80
```
