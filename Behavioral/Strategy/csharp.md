title=Strategy: C#
date=2022-03-16
type=pattern
tags=pattern implementation, behavioral, csharp
status=published
description=A Strategy implementation in C#.
~~~~~~

## Variant: Function-as-Object
In pure functional languages, it's actually difficult to keep state and data tied together--in fact, part of the whole point of a functional language is to write functions that operate on data, ideally on lots of different kinds of data. "Therefore, create a function that acts like an object. Such a function carries the data it needs along with the expression that operates on the data. More importantly, an object encapsulates its data, ensuring that only the allowed operations are applied to them." In other words, by writing a function and keeping the data buried inside of it, we achieve the same kind of encapsulation that object-orientation has traditionally reserved for itself as its principal advantage. This is done via a closure, which is the next element in the language.

This is a little more verbose than some of the other versions we've seen thus far, because C# lacks the type-inference that F# or Yeti or Scala has, yet requires explicit typing (in some places) because it is a statically-typed language. Again, because the language explicitly forbids the assignment of a lambda/delegate to an implicitly-typed local variable, the local names "withdraw", "deposit", and "accrueInterest" have to be explicitly typed.

```
static void FunctionAsObject()
{
    var balance = 0;
    Func<int, int> withdraw = (amount) =>
    {
        if (amount <= balance)
        {
            balance = balance - amount;
            return balance;
        }
        else
            throw new Exception("Insufficient funds");
    };
    Func<int, int> deposit = (amount) => 
    {
        balance += amount; return balance;
    };
    Func<float, int> accrueInterest = (intRate) => 
    { 
        balance += (int)(intRate * balance); return balance; 
    };

    Console.WriteLine("=============> FunctionAsObject");
    Console.WriteLine("{0}", deposit(100));
    Console.WriteLine("{0}", withdraw(10));
}
```

Notice that again, I choose to operate on the "global" variable "balance", rather than pass it in. (It's fairly easy to imagine how it would look if "balance" were passed in.)

