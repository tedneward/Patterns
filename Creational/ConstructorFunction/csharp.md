title=Constructor Function: C#
date=2022-03-16
type=pattern
tags=pattern implementation, creational, csharp
status=published
description=A Constructor Function implementation in C#.
~~~~~~
"You are creating a Function as Object using a Closure. How do you create instances of the object? [M]ake a function that returns your Function as Object. Give the function an Intention Revealing Name (Beck) such as make-object."

The constructor function must be explicitly typed, again, but we gain a tiny bit of brevity by changing the "delegate" literals into (slightly) shorter C# lambdas:

```
static void ConstructorFunction()
{
    Func<int,Func<int, int> makeAccount = 
        ((Func<int,Func<int,int>)( (bal) => {
            var balance = bal;
            return (int amount) =>
            {
                if (balance >= amount)
                {
                    balance -= amount;
                    return balance;
                }
                else
                    throw new Exception("Insufficient funds");
            };
        }));

    Console.WriteLine("=============> Closure");
    var acctForEugene = makeAccount(100);
    Console.WriteLine("{0}", acctForEugene(20));
}
```

Were it not for the implicitly-typed local variable declaration syntax around "acctForEugene", it would be acutely obvious that "makeAccount" isn't creating any kind of object at all, but a function to be executed. Even so, the explicit typing requirement for the lambdas is kind of annoying, and will only get worse as we move through the pattern language.

