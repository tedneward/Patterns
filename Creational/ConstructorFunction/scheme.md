title=Constructor Function: Scheme
date=2022-03-16
type=pattern
tags=pattern implementation, creational, scheme
status=published
description=A Constructor Function implementation in Scheme.
~~~~~~
"You are creating a Function as Object using a Closure. How do you create instances of the object? [M]ake a function that returns your Function as Object. Give the function an Intention Revealing Name (Beck) such as make-object."

Now things get more interesting, because the Scheme code is defining "make-withdraw" to be a lambda that in turn nests a lambda inside of it. This makes the syntax a little weird--since the returned value from "make-withdraw" is a lambda, the bound lambda must be executed in order to do the actual withdrawal.

```
(define make-withdraw
  (lambda (balance)
    (lambda (amount)
      (if (>= balance amount)  ;; balance is still bound,
        (begin                 ;; but to a new object on each call
          (set! balance (- balance amount))
          balance)
        (error "Insufficient funds" balance)))
    ))
(define account-for-eugene (make-withdraw 100))
(account-for-eugene 20)    => 80
(define account-for-tom (make-withdraw 1000))
(account-for-tom 20)       => 980
```
