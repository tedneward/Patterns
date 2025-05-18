<<<<<<< mine
A little over a decade ago, Eugene Wallingford wrote a paper for the PloP '99
conference, describing the [Envoy] pattern language, "a pattern language for
managing state in a functional program". It's a good read, but the
implementation language for the paper is Scheme--given that it's a Lisp
dialect, often isn't particularly obvious or easy to understand at first,
I thought it might be interesting (both for me and any readers that wanted
to follow along) to translate the implementation examples into a variety
of different languages. In this case, I thought it would be relatively easy
to do it in [Scala] and [F#], given their hybrid object-functional nature, but
it's also an interesting exercise to demonstrate it in [Javascript] (I'll
use NodeJS v0.8.15, running on my Mac, and Rhino, with the JVM), [Yeti]
(an ML dialect that runs on the JVM), [Jaskell] (a Haskell dialect that also
runs on the JVM), and, hey, what the heck, let's do it in C# while we're
at it, just so the .NET guys don't feel too badly outnumbered.

(I'm posting this now with the intent of filling in the Yeti, Jaskell, F# and
C# implementations later.)

Note that with lambdas coming in Java 8, it'll be possible to adapt this
pattern language to work with Java, too--I'll leave that as exercise to do
for myself (and update this blog entry) once I get a Java8 build on the
machine on which I'm writing this.

One reason for doing this in Yeti and Jaskell is to demonstrate the
original purpose of the Envoy pattern language--that we can achieve
object-like semantics even in languages that don't directly support object
semantics (like Scheme). But for the other languages, it's fair to ask why
anyone would bother doing this in languages that do directly support objects
(a la Scala, F#, etc), since it would seem a lot easier to just use the object features
directly. And, truth be told, it's true--when looking to model objects in a
language that has first-class support for objects, just use that support and
those features, and call it a day. The point of this exercise is, for me,
to exercise the functional features of those languages, and see exactly how
functional languages can provide some of the same benefits that an O-O
language enjoys, without having to use the O-O features directly. (There's
been a lot of people writing functional-isms in O-O languages, yours truly
included, so it seems a good exercise to flip that on its head.) This will
also help me figure out where/when/how to use these features, when the
need arises.

If you've not yet read the Envoy pattern language, take a moment and do that
now; I don't want to annoy Mr. Wallingford in any way by repeating his
prose here (not to mention that I'm going to have enough to do as it is
just translating the code into several different languages). But I will toss
in a brief summary of each of the elements in the pattern language, just so
we're all on the same page about what's happening in each of these code
samples.

Implementation notes
--------------------

These are a few notes for each of the implementation langauges.

### JavaScript ###
Because I want to be able to run the JavaScript code on either the Node
platform directly or on the Rhino engine (via the Java JDK "jrunscript"
command that installs on Java implementations starting with Java 6),
and because those two environments provide different mechanisms for
printing to the console ("console.log" in Node, and "println" in Rhino),
I create a top-level function "out" that aliases to one or the other of
those, depending on what's defined in the environment:

~~~~~ {#javascript-prelude .javascript .numberLines}
var out = (function() {
  if (typeof(console) !== "undefined" &&
      typeof(console.log) !== "undefined")
    return console.log
  else if (typeof(println) !== "undefined")
    return println
  else
    throw new Error("No idea what to use for output")
})();
~~~~~

(This actually gives away one of the punchlines in the first element of
the pattern language, Function as Object, below, because here we're pretty
clearly using "out" as a function-as-object.)

I used Rhino that ships with Java6, and node v0.8.15 for these.

### Scala ###
I used Scala v2.9.2 running on Java6 for this.

### Yeti ###
Yeti is an ML-based language that compiles to Java bytecode. Unlike Scala,
it's a functional-only language (well, sort of), with Hindley-Milner type
inference. As the [Yeti] home page describes, it supports polymorphic
structure and variant types, property fields, lazy lists, pattern-matching
on values, and a decent interop facility against Java code (meaning it can
call Java classes, as well as compile to classes to be called from Java if
desired.)

Yeti was at v0.9.7 at the time I wrote this, and again, running on the
Java6 VM.

Function as Object
------------------
In pure functional languages, it's actually difficult to keep state and data
tied together--in fact, part of the whole point of a functional language is
to write functions that operate on data, ideally on lots of different kinds
of data. "Therefore, create a function that acts like an object. Such a
function carries the data it needs along with the expression that operates
on the data. More importantly, an object encapsulates its data, ensuring
that only the allowed operations are applied to them." In other words, by
writing a function and keeping the data buried inside of it, we achieve the
same kind of encapsulation that object-orientation has traditionally
reserved for itself as its principal advantage. This is done via a closure,
which is the next element in the language.

### Scheme: ###
The original Scheme implementation looked like this:

~~~~~~ {#scheme-function-as-object .scheme .numberLines}
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
~~~~~~

There's a few things wrong with this approach, as Wallingford points
out, but to be faithful, recreating this in our target languages is pretty
straightforward: three functions, each of which operate on parameters
passed in. "You could create new accounts simply by binding values to
names. Operating on accounts involves passing the account to the appropriate
procedure and binding the new value as appropriate."

### JavaScript: ###
In JavaScript, we can bind function values to names just as we can in
Scheme, so it's not actually all that different, once you get past the
lack of parentheses and added curly braces. Thus, it looks like:

~~~~~ {#javascript-function-as-object .javascript .numberLines}
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
~~~~~

Note that I wrap all of it into its own function so as to give the whole
thing some scope--makes it easier to define in a single .js file and
execute.

### Scala: ###
Similarly, Scala allows us to bind functions to names, too:

~~~~~ {#scala-function-as-object .scala .numberLines}
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
~~~~~

Again, all of it is wrapped into a function for easier (on me, while I was
experimenting with all of this) scoping.

### F#: ###

~~~~~
~~~~~

### Yeti (ML): ###
Although Yeti supports a slightly more succinct syntax for defining a
function, I choose to use the syntax that more closely matches what we're
doing in the other examples--bind a function literal (do ... done;) to a
name (withdraw, deposit and accrueInterest). Again, since this is running
on top of the JVM, we have full access to the underlying Java library, which
means we can make use of RuntimeException again as a cheap way of signaling
a bad withdrawal.

~~~~~ {#yeti-function-as-object .ocaml .numberLines}
withdraw =
  do bal amt:
    if amt <= bal then
      bal - amt
    else
      throw new RuntimeException("Insufficient funds")
    fi
  done;

deposit =
  do bal amt:
    bal + amt
  done;

accrueInterest =
  do bal intRate:
    bal + (bal * intRate)
  done;

balance = 100;
println (withdraw balance 10)
~~~~~

### Jaskell (Haskell): ###

~~~~~
~~~~~

### C#: ###

~~~~~
~~~~~



Closure
-------
"You are writing a function with a free variable. How do you bundle a
function with a data value defined outside the procedure's body?" If the
data value is defined inside the procedure, remember, it gets reset to the
same value each time, and obviously this isn't going to track state at all
well. "So you might try defining the balance outside the function." But
that doesn't work, because now the value isn't encapsulated anymore.
"Therefore, create the function in an environment where its free variables
are bound to local variables."

This is something that O-O folks won't see right away, but it's a powerful
mechanism for reuse. Traditional O-O says to tuck the encapsulated value
(balance) away as a private field, but in environments like JavaScript,
which lack any sort of formal access control, or in environments like the
JVM or CLR, both of which offer a means by which to bypass access control
directives (via the Reflection libraries in both), what's marked as
"private" often isn't as private as we might want. By creating a local
variable that's outside the scope of the returned function object but
inside of the scope of the function returning the function (see where
"balance" is declared in the JavaScript version, for example), the language
or platform has to "close over" that variable (hence the name "closure"),
thus making it accessible to the returned function for use, but effectively
hidden away from any prying eyes that might want to screw with it outside
of permitted access channels.

### Scheme: ###
The only key thing to note here is that "withdraw" references a lambda,
a function literal in Scheme. We'll try to keep this flavor in the other
language implementations, just to be faithful:

~~~~~ {#scheme-closure .scheme .numberLines}
(define withdraw
  (let ((balance 100)) ;; balance is defined here,
    (lambda (amount)
      (if (>= balance amount) ;; so this reference is bound
        (begin
          (set! balance (- balance amount))
          balance)
        (error "Insufficient funds" balance)))
    ))
~~~~~

### JavaScript: ###
JavaScript is, surprisingly to some "old-school" JavaScript programmers, a
full-fledged member of the family of languages that support closures, so all
that's necessary here is to define a function that returns a function that
"closes over" the local variable "balance". But, in order to make sure that
balance isn't reset to its original value of 100 each time we call the
function, we have to actually invoke the outer function to return the inner
function, which is then bound to the name "withdraw"; that way, the variable
"balance" is initialized once, yet still referenced:

~~~~~ {#javascript-closure .javascript .numberLines}
(function() {
  out("closure ====================")

  var withdraw = function() {
    var balance = 100
    return function(amount) {
      if (balance >= amount) {
        balance -= amount
        return balance
      }
      else
        throw new Error("Insufficient funds")
    }
  }()
  out("withdraw 20 " + withdraw(20))
  out("withdraw 30 " + withdraw(30))
})()
~~~~~

### Scala: ###
We can do the same thing in Scala, and the syntax looks somewhat similar
to the JavaScript version--create a function literal, invoke it, and bind
the result to the name "withdraw", where the return is another anonymous
function literal:

~~~~~ {#scala-closure .scala .numberLines}
  def closure() = {
    val withdraw = (() => {
      var balance = 100
      (amount: Int) => {
        if (amount <= balance) {
          balance -= amount
          balance
        }
        else
          throw new RuntimeException("Insufficient funds")
      }
    })()
    println(withdraw(20))
    println(withdraw(20))
  }
~~~~~

### F#: ###

~~~~~
~~~~~

### Yeti (ML): ###
Yeti works just as any of the others have, since we can define a function
literal that returns a function literal, so just like the JavaScript and Scala
versions, we can bind a variable (as opposed to a value, which is immutable)
just outside the inner function literal, and Yeti will "close over" that
variable and use it for modifiable state:

~~~~~ {#yeti-closure .ocaml .numberLines}
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
~~~~~

### Jaskell (Haskell): ###

~~~~~
~~~~~

### C#: ###

~~~~~
~~~~~


Constructor Function
--------------------

"You are creating a Function as Object using a Closure. How do you create
instances of the object? [M]ake a function that returns your Function as
Object. Give the function an Intention Revealing Name (Beck) such as
make-object."

### Scheme: ###
Now things get more interesting, because the Scheme code is defining
"make-withdraw" to be a lambda that in turn nests a lambda inside of it.
This makes the syntax a little weird--since the returned value from
"make-withdraw" is a lambda, the bound lambda must be executed in order
to do the actual withdrawal.

~~~~~ {#scheme-constructor .scheme .numberLines}
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
~~~~~

### JavaScript: ###
It's pretty common in JavaScript to create a function that returns
a function, and that's the heart of what Constructor Function is
doing: returning a function:

~~~~~ {#javascript-constructor .javascript .numberLines}
(function() {
  out("constructorFunction ========")

  var makeWithdraw = function(balance) {
    return function(amount) {
      if (balance >= amount) {
        balance -= amount
        return balance
      }
      else
        throw new Error("Insufficient funds")
    }
  }
  var acctForEugene = makeWithdraw(100)
  out(acctForEugene(20))
  var acctForTed = makeWithdraw(1000)
  out(acctForTed(20))
})()
~~~~~

### Scala: ###
Ditto for Scala, though the idiom/pattern of function-literal-returning-
function-literal isn't always quite this obvious in Scala:

~~~~~ {#scala-constructor .scala .numberLines}
  def constructorFunction() = {
    def makeWithdraw(bal : Int) = {
      var balance = bal
      (amt : Int) => {
        if (balance >= amt) {
          balance = (balance - amt)
          balance
        }
        else
          throw new RuntimeException("Insufficient funds")
      }
    }
    val acctForEugene = makeWithdraw(100)
    println(acctForEugene(20))
    val acctForTed = makeWithdraw(1000)
    println(acctForTed(20))
  }
~~~~~

### F#: ###

~~~~~
~~~~~

### Yeti (ML): ###
Same exercise--a function binding that returns a function, with the passed-in
"balance" stored as a variable (var) inside the outer function scope, such
that it is closed over by the inner function scope.

~~~~~ {#yeti-constructor .ocaml .numberLines}
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
~~~~~

### Jaskell (Haskell): ###

~~~~~
~~~~~

### C#: ###

~~~~~
~~~~~




Method Selector
---------------

"You are creating a Function as Object using a Closure. A Constructor
Function creates new instances of the object. How do you provide shared
access to the closure's state?" After all, an account can do more than
just withdraw, but all of the operations on the account have to share
the same state--the account balance--without violating encapsulation.

### Scheme: ###
Again we see the nested lambdas, but now there's a third level of nesting;
the first invocation (make-account) returns a second invocation that will take
a single string, switch on the string, and return a third lambda that will
do the actual work of manipulating the balance.

~~~~~ {#scheme-method-selector .scheme .numberLines}
(define make-account
  (lambda (balance)
    (lambda (transaction)
      (case transaction
        ('withdraw
          (lambda (amount)
            (if (>= balance amount)
              (begin
                (set! balance (- balance amount)
                balance)
              (error "Insufficient funds" balance)))))
        ('deposit
          (lambda (amount)
            (set! balance (+ balance amount))
            balance))
        ('balance
          (lambda ()
            balance))
        (else
          (error "Unknown request -- ACCOUNT"
            transaction))))
  ))
(define account-for-eugene (make-account 100))
((account-for-eugene 'withdraw) 10)  => 90
((account-for-eugene 'withdraw) 10)  => 80
((account-for-eugene 'deposit) 100)  => 180
~~~~~

### JavaScript: ###
Doing this in JavaScript is, again, straightforward, though it does
seem a little too subtle for idiomatic JavaScript:

~~~~~ {#javascript-method-selector .javascript .numberLines}
(function() {
  out("methodSelector ========")

  var makeAccount = function(bal) {
    var balance = bal
    return function(transaction) {
      if (transaction === "withdraw") {
        return function(amount) {
          if (balance >= amount)
            return (balance = (balance - amount))
          else
            throw new Error("Insufficient funds")
        }
      }
      else if (transaction === "deposit") {
        return function(amount) {
          return (balance = (balance + amount))
        }
      }
      else if (transaction === "balance") {
        return function() {
          return balance
        }
      }
      else {
        throw new Error("Insufficient funds")
      }
    }
  }
  var acctForEugene = makeAccount(100)
  out(acctForEugene("withdraw")(20))
  out(acctForEugene("balance")())
})();
~~~~~

### Scala: ###
This style of interface--passing in a string and a variable list of
arguments--really isn't quite Scala's style, since (being a strongly-
typed language) it prefers to be able to compile-time-check as much
as it can, but that doesn't mean we can't build it when the need
and opportunity mesh:

~~~~~ {#scala-method-selector .scala .numberLines}
  def methodSelector() = {
    def makeAccount(bal : Int) = {
      var balance = bal
      (transaction : String) => {
        transaction match {
          case "withdraw" =>
            (amt : Int) => {
              if (balance >= amt) {
                balance = (balance - amt)
                balance
              }
              else
                throw new RuntimeException("Insufficient funds")
            }
          case "deposit" => {
            (amt : Int) => {
              balance += amt
              balance
            }
          }
          case _ =>
            throw new RuntimeException("Unknown request")
        }
      }
    }
    val acctForEugene = makeAccount(100)
    println(acctForEugene("deposit")(50))
    val acctForTed = makeAccount(100)
    println(acctForTed("withdraw")(50))
  }
~~~~~

### Yeti (ML): ###
Nothing new here: the makeAccount function now nests three function literals,
just like the JavaScript and Scala ones do. Like the other languages, we use
a pattern-match/switch-case construct to decide between the different action
strings ("deposit", "withdraw", "balance") and then return the appropriate
function literal for further execution. Note that Yeti, like JavaScript,
actually has a way of returning an "object" here (a structure, which is a
data type the contains one or more named fields, a la objects in JavaScript
or case classes in Scala), but since the goal is to remain as faithful as
possible to the original Scheme implementation, I stick with the more
"functional-only" approach.

~~~~~ {#yeti-method-selector .ocaml .numberLines}
makeAccount =
  (do bal:
    var balance = bal;
    do action:
      case action of
        "withdraw":
          do amt:
            if amt <= balance then
              balance := balance - amt;
              balance
            else
              throw new RuntimeException("Insufficient funds")
            fi
          done;
        "deposit":
          do amt:
            balance := balance + amt;
            balance;
          done;
        "balance":
          do:
            balance;
          done;
        _ : throw new RuntimeException("Unknown operation")
      esac
    done;
  done;);

acctForEugene = makeAccount 100;
println ((acctForEugene "withdraw") 20);
println ((acctForEugene "deposit") 20);
~~~~~

### Jaskell (Haskell): ###

~~~~~
~~~~~

### F#: ###

~~~~~
~~~~~

### C#: ###

~~~~~
~~~~~



Message-Passing Interface
-------------------------

"You have created a Method Selector for a Function as Object. You prefer to
use your object in code that has an object-oriented feel. How do you invoke
the methods of an object? [P]rovide a simple message-passing interface for
using the closure."

### Scheme: ###
Everything in a Lisp is a list, and the Scheme implementation uses that to
full effect by taking the argument list passed in to "send" and splits it up
into the object (the account), message (withdraw/deposit/etc), and the
arguments (if any) that are left.

~~~~~ {#scheme-message-passing-interface .scheme .numberLines}
(define send
  (lambda argument-list
    (let ((object  (car argument-list))
          (message (car (cdr argument-list)))
          (args    (cdr (cdr argument-list))))
      (apply (get-method object message) args))
  ))
(define get-method
  (lambda (object selector)
    (object selector)
  ))
(define account-for-eugene (make-account 100))
(send account-for-eugene 'withdraw 50)  => 50
(send account-for-eugene 'deposit 100)  => 150
(send account-for-eugene 'balance)      => 150
~~~~~

### JavaScript: ###
In JavaScript, peeling off the head and tail of the
arguments reference is trickier here, because unlike Scheme, JavaScript
sees "arguments" as an array, not a list. While I could've created "car"
and "cdr" functions in JavaScript to perform the relevant operations
on an array, it felt more idiomatic to provide a function "slice" to do
the "slicing" (which is actually a copy) of elements off the end of the
array instead. More importantly, "slice" is a primitive method on Array
objects in ECMAScript 5, though neither Node nor Rhino in Java 6
recognize it (I suspect because neither is a compliant ECMAScript 5
environment yet), and if this code ever gets run in a ECMAScript 5
world, then it would/should use that version, instead, since it'll likely
be faster than mine.

The other interesting tidbit in here is that when I wrote it the first
time, when doing a deposit, the "balance" became "8020", instead of the
mathematically-correct "100". JavaScript's "promiscuous typing" thought
that the "+" operator wanted to do a string concatenation, instead of
a mathematical add of two numbers, so I had to convince it that the
value coming out of arguments[1] was, in fact, a number, and the easiest
way (it seemed to me at the time) was to just do a quick redundant math
operation on it (multiply by 10, then divide by 10 again). There's likely
a more idiomatic way to do that, I suspect.

I also note that getMethod() in JavaScript is a bit unnecessary; we could
inline its functionality directly inside of send().

~~~~~ {#javascript-message-passing-interface .javascript .numberLines}
(function() {
  out("messagePassingInterface ========")

  var slice = function(src, start, end) {
    var returnVal = []
    var j = 0
    if (end === undefined)
      end = src.length
    for (var i = start; i < end; i++) {
      if (src.length > i)
        returnVal[j++] = src[i]
    }
    return returnVal;
  }

  var makeAccount = function(bal) {
    var balance = bal
    return function(transaction) {
      if (transaction === "withdraw") {
        return function(amount) {
          if (balance >= amount)
            return (balance = (balance - amount))
          else
            throw new Error("Insufficient funds")
        }
      }
      else if (transaction === "deposit") {
        return function(amount) {
          return (balance = (balance + (amount * 10.0 / 10.0)))
        }
      }
      else if (transaction === "balance") {
        return function() {
          return balance
        }
      }
      else {
        throw new Error("Insufficient funds")
      }
    }
  }
  var getMethod = function(object, selector) {
    return object(selector)
  }
  var send = function(object, message) {
    return (getMethod(object, message))(slice(arguments, 2))
  }
  var acctForEugene = makeAccount(100)
  out(send(acctForEugene, "withdraw", 20)) // 80
  out(send(acctForEugene, "balance"))      // 80
  out(send(acctForEugene, "deposit", 20))  // 100
  out(send(acctForEugene, "balance"))      // 100
})();
~~~~~

### Scala: ###
The Scala version of this follows the JavaScript version in that it
works off of a variable-argument list, but since Scala doesn't give
us the built-in "arguments" reference, we have to specify it at the
method declaration:

~~~~~ {#scala-message-passing-interface .scala .numberLines}
  def messagePassingInterface() = {
    def makeAccount(bal : Int) = {
      var balance = bal
      def send(key:String, args:Any*) = {
        key match {
          case "withdraw" => {
            val amt = args.head.asInstanceOf[Int]
            if (balance >= amt) {
              balance = (balance - amt)
              balance
            }
            else
              throw new RuntimeException("Insufficient funds")
          }
          case "deposit" => {
            val amt = args.head.asInstanceOf[Int]
            balance += amt
            balance
          }
        }
      }
      send _
    }
    val acctForEugene = makeAccount(100)
    println(acctForEugene("withdraw", 10))
  }
~~~~~

### Yeti (ML): ###
Unfortunately, while Yeti (like most functional languages) has a built-in list type,
it doesn't recognize arguments to a function as a list, so we either have to explicitly
put the arguments in, or we have to explicitly state that the arguments to the returned
function literal are a list. I choose the latter tactic, even though it's not the world's
most impressive syntax:

~~~~~ {#yeti-message-passing-interface .ocaml .numberLines}
makeAccount =
  (do bal:
    var balance = bal;
    do action:
      case action of
        "withdraw":
          do argList:
            amt = head argList;
            if amt <= balance then
              balance := balance - amt;
              balance;
            else
              throw new RuntimeException("Insufficient funds")
            fi
          done;
        "deposit":
          do argList:
            amt = head argList;
            balance := balance + amt;
            balance;
          done;
        "balance":
          do:
            balance;
          done;
        _ : throw new RuntimeException("Unknown operation")
      esac
    done;
  done;);

acctForEugene = makeAccount 100;
println  ((acctForEugene "withdraw")[20]);  // 80
~~~~~

If there's a way to get a Yeti function to accept a variable number of arguments, I've
not seen it in the language overview. I don't know if any ML-derivative has this, to be
honest. Of course, the other thing to do, since this is a statically-typed environment,
is to just return function literals that expect the proper number of arguments, which
will get us the compile-time safety that these languages are supposed to provide; the
below does exactly that--the last line will fail to compile if you uncomment it:

~~~~~ {#yeti-message-passing-interface2 .ocaml .numberLines}
makeAccount =
  (do bal:
    var balance = bal;
    do action:
      case action of
        "withdraw":
          do amt:
            if amt <= balance then
              balance := balance - amt;
              balance;
            else
              throw new RuntimeException("Insufficient funds")
            fi
          done;
        "deposit":
          do amt:
            balance := balance + amt;
            balance;
          done;
        "balance":
          do:
            balance;
          done;
        _ : throw new RuntimeException("Unknown operation")
      esac
    done;
  done;);

acctForEugene = makeAccount 100;
println ((acctForEugene "withdraw") 20);      // 80
println ((acctForEugene "balance") 0);        // 80
//println ((acctForEugene "withdraw") "fred");  // won't compile
~~~~~

(Truthfully, we should do this for the Scala version, too.) This choice is going to
cause us a little bit of heartache, though, because in order to use "balance", we have
to pass in a number--if we leave off the "_" in the function literal returned from the
"balance" arm of the selector, we don't need to pass "0" when we invoke it, but what's
returned isn't a number, but a function. I can't figure out how to make Yeti take that
function and just invoke it--the syntax guide doesn't seem to say out loud exactly how
I can invoke that function without having to pass in a number argument. If I'd left it
as taking a list, then I could pass an empty list and all would look consistent, if
a little weird.

### Jaskell (Haskell): ###

~~~~~
~~~~~

### F#: ###

~~~~~
~~~~~

### C#: ###

~~~~~
~~~~~



Generic Function
----------------
"You have created a Method Selector for a Function as Object. You want to
take full advantage of the tools available in your functional language. How
do you invoke the methods of an object? ... [P]rovide a simple interface to
the Method Selector that more closely follows the functional style."

### Scheme: ###
In the Scheme implementation, it's interesting that having written the
send function in the last element of the pattern language, we don't
really use it here, but instead just inline its functionality in each
of the named functions (which, in turn, take the argument list, peel off
the head of the argument list as the account object, and pass the remainder
of the arguments on to the selected function):

~~~~~
(define withdraw
  (lambda argument-list
    (let ((object (car argument-list))
          (withdraw-arguments (cdr argument-list)))
      (apply (object 'withdraw) withdraw-arguments)
    )))
(define deposit
  (lambda argument-list
    (let ((object (car arguments))
          (deposit-arguments (cdr arguments)))
      (apply (object 'deposit) deposit-arguments)
    )))
(define balance
  (lambda (object)
    (object 'balance)
  ))

(define account-for-eugene (make-account 100))
(withdraw account-for-eugene 10)
(map  (lambda (account) (deposit account 10)) account-for-eugene)
~~~~~

Interestingly enough, I sort of expected the Scheme version to use
"deposit" directly, rather than write a trampoline that calls "deposit",
since we could've avoided the Generic Function part of the language
just by using "send" directly, as well:

~~~~~
(map  (lambda (account) (send account 'deposit 10)) account-for-eugene)
~~~~~

And, to be honest, calling "map" on a single object doesn't really seem
to be a profoundly functional experience, so in my examples I'm going to
create a collection of accounts (called a "bank", naturally enough), and
map across that collection.

### JavaScript: ###
The JavaScript version of this is, again, pretty similar to the Scheme
version. Again, ECMAScript 5 environments are supposed to have a "map"
function natively built in, but previous environments don't, so I have
to write one to verify that we can, in fact, use the named functions
as the mapped operation. I also write a "map2", another version of map
that takes the function to apply to the collection but also takes any
additional arguments after that and passes them to the function being
applied across the collection; it allows me to use "deposit" directly,
instead of having to write a trampoline for it, and besides, it's trivial
to write in JavaScript:

~~~~~
(function() {
  out("genericFunction ==============")

  var slice = function(src, start, end) {
    var returnVal = []
    var j = 0
    if (end === undefined)
      end = src.length
    for (var i = start; i < end; i++) {
      if (src.length > i)
        returnVal[j++] = src[i]
    }
    return returnVal
  }

  var map = function(fn, src) {
    var retVal = []
    for (i in src)
      retVal[i] = fn(src[i])
    return retVal
  }
  var map2 = function(src, fn) {
    var retVal = []
    for (i in src)
      retVal[i] = fn(src[i], slice(arguments, 2))
    return retVal
  }

  var makeAccount = function(bal) {
    var balance = bal
    return function(transaction) {
      if (transaction === "withdraw") {
        return function(amount) {
          if (balance >= amount)
            return (balance = (balance - amount))
          else
            throw new Error("Insufficient funds")
        }
      }
      else if (transaction === "deposit") {
        return function(amount) {
          return (balance = (balance + (amount * 10.0 / 10.0)))
        }
      }
      else if (transaction === "balance") {
        return function() {
          return balance
        }
      }
      else {
        throw new Error("Insufficient funds")
      }
    }
  }
  var withdraw = function() {
    var object = arguments[0]
    var argumentList = slice(arguments, 1)
    return object("withdraw")(argumentList)
  }
  var deposit = function() {
    var object = arguments[0]
    var argumentList = slice(arguments, 1)
    return object("deposit")(argumentList)
  }
  var balance = function(object) {
    return object("balance")()
  }

  var acctForEugene = makeAccount(100)
  out(withdraw(acctForEugene, 20))
  out(deposit(acctForEugene, 20))

  var bank = [
    makeAccount(100),  // acctForEugene
    makeAccount(1000)  // acctForTed
  ]
  map(function(it) { deposit(it, 20) }, bank)
  out(balance(bank[0]))
  out(balance(bank[1]))

  map2(bank, deposit, 20)
  out(balance(bank[0]))
  out(balance(bank[1]))
})();
~~~~~

### Scala: ###
Scala, of course, has functional methods built onto its List type
(which we can use instead of an array, since Scala has much better
support for lists than arrays):

~~~~~
  def genericFunction() = {
    def makeAccount(bal : Int) = {
      var balance = bal
      def send(key:String, args:Any*) = {
        key match {
          case "withdraw" => {
            val amt = args.head.asInstanceOf[Int]
            if (balance >= amt) {
              balance = (balance - amt)
              balance
            }
            else
              throw new RuntimeException("Insufficient funds")
          }
          case "deposit" => {
            val amt = args.head.asInstanceOf[Int]
            balance += amt
            balance
          }
          case "balance" => {
            balance
          }
          case _ =>
            throw new RuntimeException("Unknown request")
        }
      }
      send _
    }
    def withdraw(account : (String, Any*) => Int, amount : Int) = {
      account("withdraw", amount)
    }
    def deposit(account : (String, Any*) => Int, amount : Int) = {
      account("deposit", amount)
    }
    def balance(account : (String, Any*) => Int) = {
      account("balance")
    }
    val accounts = List(makeAccount(100), makeAccount(200), makeAccount(300))
    accounts.foreach(withdraw(_, 20))
    accounts.foreach((in) => { println(balance(in)) })
  }
~~~~~

### Yeti (ML): ###
Writing this in Yeti/ML is definitely trickier than it was in JavaScript, despite the
built-in "map" and other functions, because getting the arguments to "trampoline" right
is a little harder. Fortunately, the generic method hides the "balance 0" weirdness from
the last pattern element, making it a tad easier to use:

~~~~~
makeAccount =
  (do bal:
    var balance = bal;
    do action:
      case action of
        "withdraw":
          do amt:
            if amt <= balance then
              balance := balance - amt;
              balance
            else
              throw new RuntimeException("Insufficient funds")
            fi
          done;
        "deposit":
          do amt:
            balance := balance + amt;
            balance
          done;
        "balance":
          do:
            balance
          done;
        _ : throw new RuntimeException("Unknown operation")
      esac
    done;
  done;);

withdraw =
  (do acct amt:
    (acct "withdraw") amt;
  done;);
deposit =
  (do acct amt:
    (acct "deposit") amt;
  done;);
balance =
  (do acct:
    acct "balance" 0;
  done;);

acctForEugene = makeAccount 100;
println (withdraw acctForEugene 20);      // 80
println (deposit acctForEugene 20);       // 100
println (balance acctForEugene);          // 100

accounts = [(makeAccount 100), (makeAccount 200), (makeAccount 300)];
balances = map (do acct: (deposit acct 20) done) accounts;
for accounts do acct: println(deposit acct 20) done;
~~~~~

Yeti complained if I didn't bind the result of the "map" call to a value, hence the
"balances" value there, even though the balances are actually also stored in the
relevant closures for each account. Note that the "for" line that follows it actually
does the same thing, and prints the results out, to boot. In fact, it's high time
people started to realize that the "for" loop in most imperative languages is just a
non-functional way of doing a "map" without yielding a value. Languages like Scala and
Yeti/ML essentially blur that line significantly enough to the point where we should
just eschew "for" altogether and use "map", if you ask me.

### Jaskell (Haskell): ###

~~~~~
~~~~~

### F#: ###

~~~~~
~~~~~

### C#: ###

~~~~~
~~~~~



Delegation
----------

"You are creating a Function as Object. How do you create a new object
that extends the behavior of an existing object? ... [U]se delegation. Make
a Function as Object that has an instance variable an instance of the
object you want to extend. Implement behaviors specific to the new object
as methods in a Method Selector. Pass all other messages onto the instance
variable."

Again, in a traditional O-O language, we'd just inherit, and in an object-
functional hybrid, we could do the same. There's no real point not to, to
be honest. But the interesting thing about this implementation is that it
demonstrates the runtime relationship between a JavaScript object and its
prototype: calling a function passing in the "derived" object causes the
"derived" to try its "base" (its prototype) in the event that the method
in question isn't defined on the "derived".

Note also that this particular trick is really only feasible because the
"object" presents a uniform interface: all interaction with the "object"
(whether it is a standard account or an interest-bearing one) is done
through the Method Selector mechanism, which allows for this extension
without having to modify any sort of base interface. This isn't so much
a knock on O-O as a whole as it is on statically-typed traditional O-O.

### Scheme: ###
This is pretty straightforward, if you understood the Message-Passing
Interface implementation of earlier.

~~~~~
(define make-interest-bearing-account
  (lambda (balance interest-rate)
    (let ((my-account (make-account balance)))
      (lambda (transaction)
        (case transaction
          ('accrue-interest
            (lambda ()
              ((my-account 'deposit)
                (* ((my-account 'balance))
                   interest-rate)) ))
        (else
          (my-account transaction))
        )))
  ))
(define account-for-eugene (make-interest-bearing-account 100 0.05))
((account-for-eugene 'balance))         => 100
((account-for-eugene 'deposit) 100)     => 200
((account-for-eugene 'balance))         => 200
((account-for-eugene 'accrue-interest)) => 210
((account-for-eugene 'balance))         => 210
~~~~~

### JavaScript: ###
Despite the fact that the JavaScript implementation just keeps getting
longer and longer, it's actually not that much harder to add in this
delegation functionality--again, as has been the case for a lot of the
JavaScript code, it's almost a direct one-to-one port from the Scheme:

~~~~~
(function() {
  out("delegation =======")

  var slice = function(src, start, end) {
    var returnVal = []
    var j = 0
    if (end === undefined)
      end = src.length
    for (var i = start; i < end; i++) {
      if (src.length > i)
        returnVal[j++] = src[i]
    }
    return returnVal
  }

  var makeAccount = function(bal) {
    var balance = bal
    return function(transaction) {
      if (transaction === "withdraw") {
        return function(amount) {
          if (balance >= amount)
            return (balance = (balance - amount))
          else
            throw new Error("Insufficient funds")
        }
      }
      else if (transaction === "deposit") {
        return function(amount) {
          return (balance = (balance + (amount * 10.0 / 10.0)))
        }
      }
      else if (transaction === "balance") {
        return function() {
          return balance
        }
      }
      else {
        throw new Error("Insufficient funds")
      }
    }
  }
  var makeInterestBearingAccount = function(bal, intRate) {
    var myAccount = makeAccount(bal)
    return function(transaction) {
      if (transaction === "accrueInterest") {
        return function() {
          var balance = myAccount("balance")()
          var interest = (balance * intRate)
          return myAccount("deposit")(interest)
        }
      }
      else
        return myAccount(transaction)
    }
  }

  var acctForEugene = makeInterestBearingAccount(100, 0.05)
  out(acctForEugene("balance")())
  out(acctForEugene("deposit")(20))
  out(acctForEugene("accrueInterest")())
  out(acctForEugene("balance")())
})();
~~~~~

### Scala: ###
The Scala version of this is tricky, because it relies on a very subtle
bit of Scala syntax; specifically, when we try to pass the "args" sequence
(which, in actual implementation, is a WrappedArray) from the
"makeInterestBearingAccount" function to the "makeAccount" function (by
which I mean, the functions returned from those two functions), if we don't
use the peculiar ": _*" syntax, Scala interprets "args" to be a single
parameter (a single parameter whose type is a collection), instead of
the intended "pass the arguments through" behavior. (If you're a Java
or C# developer, it's like having a varargs method calling another varargs
method, and passing the array of arguments from the first as an array
instead of each element on its own to form the array of arguments in the
second. Yeah, I know--it's a little brain-twisty.)

~~~~~
  def delegation() = {
    def makeAccount(bal : Int) = {
      var balance = bal
      def send(key:String, args:Any*) = {
        key match {
          case "withdraw" => {
            val amt = args.head.asInstanceOf[Int]
            if (balance >= amt) {
              balance = (balance - amt)
              balance
            }
            else
              throw new RuntimeException("Insufficient funds")
          }
          case "deposit" => {
            val amt = args.head.asInstanceOf[Int]
            balance += amt
            balance
          }
          case "balance" => {
            balance
          }
          case _ =>
            throw new RuntimeException("Unknown request")
        }
      }
      send _
    }
    def makeInterestBearingAccount(bal : Int, intRate : Double) = {
      val account = makeAccount(bal)
      def send(key: String, args:Any*) = {
        key match {
          case "accrueInterest" => {
            val amt = (int2float(account("balance")) * intRate).toInt
            account("deposit", amt)
          }
          case _ =>
            account(key, args : _*)
        }
      }
      send _
    }
    val acctForEugene = makeInterestBearingAccount(100, 0.05)
    println(acctForEugene("deposit", 20))
    println(acctForEugene("accrueInterest"))
    println(acctForEugene("balance"))
  }
~~~~~

### F#: ###

~~~~~
~~~~~

### Yeti (ML): ###
Since we didn't go down the path of trying to do the variable-argument list in Yeti,
we don't have the same problems the Scala version presented, and the generic methods
(the top-level "withdraw", "deposit" and "balance" functions) actually help hide the
syntactic weirdness that we ran into in the last pattern element:

~~~~~
makeAccount =
  (do bal:
    var balance = bal;
    do action:
      case action of
        "withdraw":
          do amt:
            if amt <= balance then
              balance := balance - amt;
              balance
            else
              throw new RuntimeException("Insufficient funds")
            fi
          done;
        "deposit":
          do amt:
            balance := balance + amt;
            balance
          done;
        "balance":
          do:
            balance
          done;
        _ : throw new RuntimeException("Unknown operation")
      esac
    done;
  done;);

withdraw =
  (do acct amt:
    (acct "withdraw") amt;
  done;);
deposit =
  (do acct amt:
    (acct "deposit") amt;
  done;);
balance =
  (do acct:
    acct "balance" 0;
  done;);

acctForEugene = makeAccount 100;
println (withdraw acctForEugene 20);      // 80
println (deposit acctForEugene 20);       // 100
println (balance acctForEugene);          // 100

accounts = [(makeAccount 100), (makeAccount 200), (makeAccount 300)];
balances = map (do acct: (deposit acct 20) done) accounts;
for accounts do acct: println(deposit acct 20) done;
~~~~~

Note the last two lines--the "for" construct in most imperative languages is actually
akin to the "map" construct in most functional languages, except that in the imperative
"for" there's no return value from the expression, and in a functional "map" there
(usually) is. This is why we have to bind the result from the "map" to a name, and we
don't have any results from the "for". (The "map" also insists on having a returned
value--a list of Unit isn't acceptable, which is what would be returned if we used
the "println" expression in the "map".)

### Jaskell (Haskell): ###

~~~~~
~~~~~

### C#: ###

~~~~~
~~~~~




Private Method
--------------
"You have created a Method Selector. How do you factor common behavior
out of the methods in the Method Selector? ... [D]efine the common code in
a Local Procedure (Wallingford). Invoke this procedure in place of the
duplicated code within the Method Selector."

### Scheme: ###

~~~~~
(define make-account
  (lambda (balance)
    (let ((transaction-log '())
      (log-transaction
        (lambda type amount)
          (set! transaction-log
                (cons (list type amount)
                      transaction-log)))) )
      (lamba (transaction)
        (case transaction
          ('withdraw
            (lambda (amount)
              (if (>= balance amount)
                (begin
                  (set! balance (- balance amount))
                  (log-transaction 'withdraw amount)
                  balance)
                (error "Insufficient funds" balance))))
          ('deposit
            (lambda (amount)
              (set! balance (+ balance amount))
              (log-transaction 'deposit amount)
              balance))
        ...))
    ))
~~~~~

### JavaScript: ###
Again, in JavaScript, we rely on the fact that anything declared inside
the "makeAccount" function but outside the function returned by "makeAccount"
is encapsulated, and create both the "transactionLog" (an array, since
JavaScript likes those better than lists) and the function to append to it
("logTransaction") within that "neutral zone". Just to prove that the transaction log
is being written, I add another method to the method selector table, "viewLog", to
return the contents of the transaction log.

~~~~~
(function() {
  out("privateMethod ===========")

  var makeAccount = function(bal) {
    var transactionLog = []
    var logTransaction = function(type, amount) {
      transactionLog.push("Action: " + type + " for " + amount)
    }

    var balance = bal
    return function(transaction) {
      if (transaction === "withdraw") {
        return function(amount) {
          if (balance >= amount) {
            logTransaction("withdraw", amount)
            return (balance = (balance - amount))
          }
          else
            throw new Error("Insufficient funds")
        }
      }
      else if (transaction === "deposit") {
        return function(amount) {
          logTransaction("deposit", amount)
          return (balance = (balance + (amount * 10.0 / 10.0)))
        }
      }
      else if (transaction === "balance") {
        return function() {
          logTransaction("balance", balance)
          return balance
        }
      }
      else if (transaction === "viewLog") {
        return function() {
          return (transactionLog)
        }
      }
      else {
        throw new Error("Insufficient funds")
      }
    }
  }
  var acctForEugene = makeAccount(100)
  out(acctForEugene("withdraw")(20))
  out(acctForEugene("balance")())
  out(acctForEugene("deposit")(20))
  out(acctForEugene("balance")())
  out(acctForEugene("viewLog")())
})();
~~~~~

### Scala: ###
The Scala version is also pretty straightforward--we've already seen that
Scala supports nested functions, so it is simply a matter of defining the
logTransaction() function and an empty List[String] in the same
"neutral zone" in which the "balance" variable lives. Instead of adding
a new selector to the list, I chose this time to just print the transaction
log as part of the "balance" operation.

~~~~~
  def privateMethod() = {
    def makeAccount(bal : Int) = {
      var balance = bal
      var transactionLog = List[String]()
      def logTransaction(action:String, amount:Int) = {
        val msg = ("Action: " + action + " for " + amount)
        transactionLog = transactionLog :+ msg
      }
      def send(key:String, args:Any*) = {
        key match {
          case "withdraw" => {
            val amt = args.head.asInstanceOf[Int]
            if (balance >= amt) {
              logTransaction("withdraw", amt)
              balance = (balance - amt)
              balance
            }
            else
              throw new RuntimeException("Insufficient funds")
          }
          case "deposit" => {
            val amt = args.head.asInstanceOf[Int]
            logTransaction("deposit", amt)
            balance += amt
            balance
          }
          case "balance" => {
            println(transactionLog)
            balance
          }
          case _ =>
            throw new RuntimeException("Unknown request")
        }
      }
      send _
    }
    val acctForEugene = makeAccount(100)
    println(acctForEugene("deposit", 20))
    println(acctForEugene("balance"))
  }
~~~~~

### F#: ###

~~~~~
~~~~~

### Yeti (ML): ###
The private method in Yeti is, again, just a nested function hiding out in the closure
that is returned by "makeAccount"; the fact that Yeti supports expressions embedded
inside of strings makes it easy to create the transaction log string:

~~~~~
makeAccount =
  (do bal:
    var balance = bal;
    var transactionLog is list<string> = [];
    logTransaction action amount =
      transactionLog := "Action: \(action) for \(amount)" :: transactionLog;
    do action:
      case action of
        "withdraw":
          do amt:
            if amt <= balance then
              logTransaction "withdraw" amt;
              balance := balance - amt;
              balance
            else
              throw new RuntimeException("Insufficient funds")
            fi
          done;
        "deposit":
          do amt:
            logTransaction "deposit" amt;
            balance := balance + amt;
            balance
          done;
        "balance":
          do:
            println transactionLog;
            balance
          done;
        _ : throw new RuntimeException("Unknown operation")
      esac
    done;
  done;);
~~~~~

### Jaskell (Haskell): ###

~~~~~
~~~~~

### C#: ###

~~~~~
~~~~~



Summary
-------

JavaScript is, of course, the de-facto golden child right now.

And Scala is, undoubtedly, one of my favorites. It's syntax is a little quirky in
places, but no more so than any other language I've used.

I like the Yeti code style and syntax, and could definitely see doing some
small projects in it, particularly some service-y kinds of things with it, a la
Web or REST services; the Yeti source code has some examples of how to create
(for example) servlets and WARs, and it's a nice syntax. I don't know that I'd want
to create a full-fledged MVC framework on top of Yeti, but as something that's
basically taking input, doing processing and sending back JSON or XML results,
it's not a bad approach. Considering you can also create classes in Yeti, which
puts it into the same grounds as F#, it's worth looking into if you've got some
ML in your background and want to go back to it while staying on top of the JVM.

Honestly, I don't really expect that anyone reading this piece is going to
immediately turn around, abandon all their domain objects, and take up this
approach as a replacement--in some cases, taking this "all functional"
style creates more angst than it really provides benefits--but we can use
parts of it to generate some really interesting new patterns.



[Envoy]: http://www.cs.uni.edu/~wallingf/patterns/envoy.pdf
[Scala]: http://www.typesafe.com
[C#]: http://en.wikipedia.org/wiki/C_Sharp_(programming_language)
[F#]: http://en.wikipedia.org/wiki/F_Sharp_(programming_language)
[Yeti]: http://mth.github.com/yeti/
[Jaskell]: http://jaskell.codehaus.org

=======
A little over a decade ago, Eugene Wallingford wrote a paper for the PloP '99
conference, describing the [Envoy] pattern language, "a pattern language for
managing state in a functional program". It's a good read, but the
implementation language for the paper is Scheme--given that it's a Lisp
dialect, often isn't particularly obvious or easy to understand at first,
I thought it might be interesting (both for me and any readers that wanted
to follow along) to translate the implementation examples into a variety
of different languages. In this case, I thought it would be relatively easy
to do it in [Scala] and [F#], given their hybrid object-functional nature, but
it's also an interesting exercise to demonstrate it in [Javascript] (I'll
use NodeJS v0.8.15, running on my Mac, and Rhino, with the JVM), [Yeti]
(an ML dialect that runs on the JVM), [Jaskell] (a Haskell dialect that also
runs on the JVM), and, hey, what the heck, let's do it in C# while we're
at it, just so the .NET guys don't feel too badly outnumbered.

(I'm posting this now with the intent of filling in the Yeti, Jaskell, F# and
C# implementations later.)

Note that with lambdas coming in Java 8, it'll be possible to adapt this
pattern language to work with Java, too--I'll leave that as exercise to do
for myself (and update this blog entry) once I get a Java8 build on the
machine on which I'm writing this.

One reason for doing this in Yeti and Jaskell is to demonstrate the
original purpose of the Envoy pattern language--that we can achieve
object-like semantics even in languages that don't directly support object
semantics (like Scheme). But for the other languages, it's fair to ask why
anyone would bother doing this in languages that do directly support objects
(a la Scala, F#, etc), since it would seem a lot easier to just use the object features 
directly. And, truth be told, it's true--when looking to model objects in a
language that has first-class support for objects, just use that support and
those features, and call it a day. The point of this exercise is, for me,
to exercise the functional features of those languages, and see exactly how
functional languages can provide some of the same benefits that an O-O
language enjoys, without having to use the O-O features directly. (There's
been a lot of people writing functional-isms in O-O languages, yours truly
included, so it seems a good exercise to flip that on its head.) This will
also help me figure out where/when/how to use these features, when the
need arises.

If you've not yet read the Envoy pattern language, take a moment and do that
now; I don't want to annoy Mr. Wallingford in any way by repeating his
prose here (not to mention that I'm going to have enough to do as it is
just translating the code into several different languages). But I will toss
in a brief summary of each of the elements in the pattern language, just so
we're all on the same page about what's happening in each of these code
samples.

Implementation notes
--------------------

These are a few notes for each of the implementation langauges.

### JavaScript ###
Because I want to be able to run the JavaScript code on either the Node
platform directly or on the Rhino engine (via the Java JDK "jrunscript"
command that installs on Java implementations starting with Java 6),
and because those two environments provide different mechanisms for
printing to the console ("console.log" in Node, and "println" in Rhino),
I create a top-level function "out" that aliases to one or the other of
those, depending on what's defined in the environment:

~~~~~
var out = (function() {
  if (typeof(console) !== "undefined" &&
      typeof(console.log) !== "undefined")
    return console.log
  else if (typeof(println) !== "undefined")
    return println
  else
    throw new Error("No idea what to use for output")
})();
~~~~~

(This actually gives away one of the punchlines in the first element of
the pattern language, Function as Object, below, because here we're pretty
clearly using "out" as a function-as-object.)

I used Rhino that ships with Java6, and node v0.8.15 for these.

### Scala ###
I used Scala v2.9.2 running on Java6 for this.

### Yeti ###
Yeti is an ML-based language that compiles to Java bytecode. Unlike Scala,
it's a functional-only language (well, sort of), with Hindley-Milner type
inference. As the [Yeti] home page describes, it supports polymorphic
structure and variant types, property fields, lazy lists, pattern-matching
on values, and a decent interop facility against Java code (meaning it can
call Java classes, as well as compile to classes to be called from Java if
desired.)

Yeti was at v0.9.7 at the time I wrote this, and again, running on the
Java6 VM.

### F# ###
I'm using the Visual Studio 2012 release to write the F# bits, which
corresponds to F# 3.0. As far as I can tell, there's nothing really all that
"3.0-specific" that I'm using, so it should work with F# 2.0, which shipped
with Visual Studio 2010, and there's nothing Windows-specific here either,
which means it should run fine on F# 3.0-on-Mono.

Note that, like what I'm doing with the JavaScript version, I'm binding each
of the pattern elements into a function for execution, thus creating a scope
block that is dissociated from the larger global scope:

~~~~
let example = fun () ->
    Console.WriteLine "Howdy world"
~~~~

If (like I tried once) we were to use the more naive approach:

~~~~
let example =
	Console.WriteLine "Howdy world"
~~~~

... then each of the functions is executed and the results bound to the name
described ("example", in this case) at the time the compiler sees it; in
other words, each is eagerly-evaluated, instead of waiting to be invoked
in the main entry point of the program later. By binding an anonymous
function literal, it essentially lazy-fies each of them, and won't execute
them until they are deliberately invoked in Main, as in:

~~~~
[<EntryPoint>]
let main argv = 
	example()
	// ... the others go here
    0 // return an integer exit code
~~~~

With the platform (and prelude) details out of the way, let's begin.

### C# ###
Wow, the C# version is going to be ugly. Let me explain what I mean.

Let's start with the syntax for an anonymous function literal (a lambda,
in C# parlance):

~~~~
() => { return 5; };
~~~~

This is a function that takes no arguments and yields an int. (Actually, 
to be exact, this is a delegate, since the lambda wouldn't
need an explicit return or the curly braces, since it's a 
single-expression block and the lambda assumes that the result of the
single expression should be implicitly returned.)

Ideally, we'd be able to capture this in an implicitly-typed local variable,
like so:

~~~~
var giveMeFive = () => { return 5; };
~~~~

But unfortunately, C# doesn't allow this, saying that it "Cannot assign
lambda to implicitly-typed local variable". (Doesn't get much more
straightforward than that when it comes to an error message.) So, we have
to explicitly-type the local variable, which is a Func<> of some type:

~~~~
Func<int> giveMeFive = () => { return 5; };
~~~~

Hold on to this thought, because things are going to get even uglier when
we want to invoke an anonymous block like this later (when we get into
the Closure parts of the pattern language).


Function as Object
------------------
In pure functional languages, it's actually difficult to keep state and data
tied together--in fact, part of the whole point of a functional language is
to write functions that operate on data, ideally on lots of different kinds
of data. "Therefore, create a function that acts like an object. Such a
function carries the data it needs along with the expression that operates
on the data. More importantly, an object encapsulates its data, ensuring
that only the allowed operations are applied to them." In other words, by
writing a function and keeping the data buried inside of it, we achieve the
same kind of encapsulation that object-orientation has traditionally
reserved for itself as its principal advantage. This is done via a closure,
which is the next element in the language.

### Scheme: ###
The original Scheme implementation looked like this:

~~~~~~
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
~~~~~~

There's a few things wrong with this approach, as Wallingford points
out, but to be faithful, recreating this in our target languages is pretty
straightforward: three functions, each of which operate on parameters
passed in. "You could create new accounts simply by binding values to
names. Operating on accounts involves passing the account to the appropriate
procedure and binding the new value as appropriate."

### JavaScript: ### 
In JavaScript, we can bind function values to names just as we can in 
Scheme, so it's not actually all that different, once you get past the
lack of parentheses and added curly braces. Thus, it looks like:

~~~~~
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
~~~~~ 

Note that I wrap all of it into its own function so as to give the whole
thing some scope--makes it easier to define in a single .js file and
execute.

### Scala: ###
Similarly, Scala allows us to bind functions to names, too:

~~~~~
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
~~~~~

Again, all of it is wrapped into a function for easier (on me, while I was
experimenting with all of this) scoping.

### F#: ###
F#, like most functional/object hybrid languages, also offers the ability
to bind functions to values, so this is also pretty straightforward. I choose
to just operate against the "global" balance value, rather than do the more
functional "pass the balance in" that the previous two use:

~~~~~
let functionAsObject = fun () ->
    let balance = ref 0
    let withdraw = 
        fun amt ->
            if amt <= !balance then
                balance := (!balance) - amt
                !balance
            else
                raise (Exception("Insufficient funds"))
    let deposit = 
        fun amt -> 
            balance := (!balance) + amt
            !balance
    let accrueInterest = 
        fun (intRate : float) -> 
            balance := (!balance) + (int (float !balance * intRate))
            !balance
    
    Console.WriteLine "=========> Function as Object"
    printfn "%d" (deposit 200)
    printfn "%d" (withdraw 50)
~~~~~

### Yeti (ML): ###
Although Yeti supports a slightly more succinct syntax for defining a
function, I choose to use the syntax that more closely matches what we're
doing in the other examples--bind a function literal (do ... done;) to a
name (withdraw, deposit and accrueInterest). Again, since this is running
on top of the JVM, we have full access to the underlying Java library, which
means we can make use of RuntimeException again as a cheap way of signaling
a bad withdrawal.

~~~~~
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
~~~~~

### Jaskell (Haskell): ###

~~~~~
~~~~~

### C#: ###
This is a little more verbose than some of the other versions we've
seen thus far, because C# lacks the type-inference that F# or Yeti
or Scala has, yet requires explicit typing (in some places) because
it is a statically-typed language. Again, because the language
explicitly forbids the assignment of a lambda/delegate to an
implicitly-typed local variable, the local names "withdraw", "deposit",
and "accrueInterest" have to be explicitly typed.

~~~~~
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
~~~~~

Notice that again, I choose to operate on the "global" variable
"balance", rather than pass it in. (It's fairly easy to imagine
how it would look if "balance" were passed in.)


Closure
-------
"You are writing a function with a free variable. How do you bundle a
function with a data value defined outside the procedure's body?" If the
data value is defined inside the procedure, remember, it gets reset to the
same value each time, and obviously this isn't going to track state at all
well. "So you might try defining the balance outside the function." But
that doesn't work, because now the value isn't encapsulated anymore.
"Therefore, create the function in an environment where its free variables
are bound to local variables."

This is something that O-O folks won't see right away, but it's a powerful
mechanism for reuse. Traditional O-O says to tuck the encapsulated value
(balance) away as a private field, but in environments like JavaScript,
which lack any sort of formal access control, or in environments like the
JVM or CLR, both of which offer a means by which to bypass access control
directives (via the Reflection libraries in both), what's marked as
"private" often isn't as private as we might want. By creating a local
variable that's outside the scope of the returned function object but
inside of the scope of the function returning the function (see where
"balance" is declared in the JavaScript version, for example), the language
or platform has to "close over" that variable (hence the name "closure"),
thus making it accessible to the returned function for use, but effectively
hidden away from any prying eyes that might want to screw with it outside
of permitted access channels.

### Scheme: ###
The only key thing to note here is that "withdraw" references a lambda,
a function literal in Scheme. We'll try to keep this flavor in the other
language implementations, just to be faithful:

~~~~~
(define withdraw
  (let ((balance 100)) ;; balance is defined here,
    (lambda (amount)
      (if (>= balance amount) ;; so this reference is bound
        (begin
          (set! balance (- balance amount))
          balance)
        (error "Insufficient funds" balance)))
    ))
~~~~~

### JavaScript: ###
JavaScript is, surprisingly to some "old-school" JavaScript programmers, a
full-fledged member of the family of languages that support closures, so all
that's necessary here is to define a function that returns a function that
"closes over" the local variable "balance". But, in order to make sure that
balance isn't reset to its original value of 100 each time we call the
function, we have to actually invoke the outer function to return the inner
function, which is then bound to the name "withdraw"; that way, the variable
"balance" is initialized once, yet still referenced:

~~~~~
(function() {
  out("closure ====================")

  var withdraw = function() {
    var balance = 100
    return function(amount) {
      if (balance >= amount) {
        balance -= amount
        return balance
      }
      else
        throw new Error("Insufficient funds")
    }
  }()
  out("withdraw 20 " + withdraw(20))
  out("withdraw 30 " + withdraw(30))
})()
~~~~~

### Scala: ###
We can do the same thing in Scala, and the syntax looks somewhat similar
to the JavaScript version--create a function literal, invoke it, and bind
the result to the name "withdraw", where the return is another anonymous
function literal:

~~~~~
  def closure() = {
    val withdraw = (() => {
      var balance = 100
      (amount: Int) => {
        if (amount <= balance) {
          balance -= amount
          balance
        }
        else
          throw new RuntimeException("Insufficient funds")
      }
    })()
    println(withdraw(20))
    println(withdraw(20))
  }
~~~~~

### F#: ###
The F# version gets interesting because when we try to do the same thing that
the JavaScript (or other languages) do--that is, "close over" a local variable
defined in the outer scope--the compiler immediately rejects that, saying
point-blank that the language does not support that, and to use "reference
variables" (the ref keyword) instead:

~~~~~
let closure = fun () ->
    let withdraw =
        let balance = ref 100
        fun amt ->
            if amt <= !balance then
                balance := (!balance) - amt
                !balance
            else
                raise (Exception("Insufficient funds"))

    Console.WriteLine "=========> Closure"
    printfn "%d" (withdraw 20)
    printfn "%d" (withdraw 30)
~~~~~

What essentially we're doing, then, is capturing a pointer/reference to balance,
and carrying that into the returned function literal, rather than letting the
language capture that for us. The ref is dereferenced using the "!" operator,
and assigned through using the ":=" operator, as can be seen above. Other than
that, this is pretty much identical to the other languages' versions.

By the way, it should be noted that F#'s "printfn" function is actually type-safe,
so attempts to write "printfn "%d" x" where "x" is a non-integer value will yield
a compile-time error. That's an incredibly spiffy feature, and I wish it were
something we could apply to our own F# APIs, but from what I understand from Don
Syme (the F# language creator), it's something that's baked into the compiler
somehow. :-/

### Yeti (ML): ###
Yeti works just as any of the others have, since we can define a function
literal that returns a function literal, so just like the JavaScript and Scala
versions, we can bind a variable (as opposed to a value, which is immutable)
just outside the inner function literal, and Yeti will "close over" that
variable and use it for modifiable state:

~~~~~
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
~~~~~

### Jaskell (Haskell): ###

~~~~~
~~~~~

### C#: ###
Brace yourself--things are about to get really ugly here. The other
versions suggest that we obtain encapsulation by capturing the "balance"
value inside an outer function scope which is then referenced from an
inner function scope, that inner function scope being the returned
function literal. But... C# doesn't let us invoke function literals
directly, except if they're cast to Func<> instances:

~~~~~
static void Closure()
{
    Func<int, int> withdraw = ((Func<Func<int, int>>)(() => {
        var balance = 100;
        Func<int, int> result = delegate(int amount)
        {
            if (balance >= amount)
            {
                balance -= amount;
                return balance;
            }
            else
                throw new Exception("Insufficient funds");
        };
        return result;
    }))();
    Console.WriteLine("=============> Closure");
    Console.WriteLine("{0}", withdraw(20));
}
~~~~~

Did all that make sense? It might be clearer if I go back to the version
I wrote that I had to use in order to figure all this out on my own:

~~~~
static void Closure()
{
    Func<Func<int, int>> withdrawMaker = (delegate {
        var balance = 100;
        Func<int, int> result = delegate(int amount)
        {
            if (balance >= amount)
            {
                balance -= amount;
                return balance;
            }
            else
                throw new Exception("Insufficient funds");
        };
        return result;
    });
    Func<int, int> withdraw = withdrawMaker();

    Console.WriteLine("=============> Closure");
    Console.WriteLine("{0}", withdraw(20));
}
~~~~

Why bother will all of this--why not just write it as a generalized
method like O-O folks have done since the beginning of time? Because
we want that "balance" tucked away somewhere where Reflection can't
find it. So the double-level of function indirection is necessary;
to cap things off, we don't want to have to write a one-use "maker"
function every time.

Constructor Function
--------------------

"You are creating a Function as Object using a Closure. How do you create
instances of the object? [M]ake a function that returns your Function as
Object. Give the function an Intention Revealing Name (Beck) such as
make-object."

### Scheme: ###
Now things get more interesting, because the Scheme code is defining
"make-withdraw" to be a lambda that in turn nests a lambda inside of it.
This makes the syntax a little weird--since the returned value from
"make-withdraw" is a lambda, the bound lambda must be executed in order
to do the actual withdrawal.

~~~~~
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
~~~~~

### JavaScript: ###
It's pretty common in JavaScript to create a function that returns
a function, and that's the heart of what Constructor Function is
doing: returning a function:

~~~~~
(function() {
  out("constructorFunction ========")

  var makeWithdraw = function(balance) {
    return function(amount) {
      if (balance >= amount) {
        balance -= amount
        return balance
      }
      else
        throw new Error("Insufficient funds")
    }
  }
  var acctForEugene = makeWithdraw(100)
  out(acctForEugene(20))
  var acctForTed = makeWithdraw(1000)
  out(acctForTed(20))
})()
~~~~~

### Scala: ###
Ditto for Scala, though the idiom/pattern of function-literal-returning-
function-literal isn't always quite this obvious in Scala:

~~~~~
  def constructorFunction() = {
    def makeWithdraw(bal : Int) = {
      var balance = bal
      (amt : Int) => {
        if (balance >= amt) {
          balance = (balance - amt) 
          balance
        }
        else 
          throw new RuntimeException("Insufficient funds")
      }
    }
    val acctForEugene = makeWithdraw(100)
    println(acctForEugene(20))
    val acctForTed = makeWithdraw(1000)
    println(acctForTed(20))
  }
~~~~~

### F#: ###
Really, not any different from the other languages: a function binding that
returns a function, with the passed-in "balance" captured as a reference (see
the earlier pattern element discussion for why it's a ref) inside the outer
function scope, and used from the inner function scope.

~~~~~
let constructorFunction = fun () ->
    let makeAccount =
        fun bal ->
            let balance = ref bal
            fun amt ->
                if amt <= !balance then
                    balance := (!balance) - amt
                    !balance
                else
                    raise (Exception("Insufficient funds"))                
            
    Console.WriteLine "=========> Constructor Function"
    let acctForEugene = makeAccount 100
    printfn "%d" (acctForEugene 20)
~~~~~

### Yeti (ML): ###
Same exercise--a function binding that returns a function, with the passed-in
"balance" stored as a variable (var) inside the outer function scope, such
that it is closed over by the inner function scope.

~~~~~
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
~~~~~

### Jaskell (Haskell): ###

~~~~~
~~~~~

### C#: ###
The constructor function must be explicitly typed, again, but we gain a
tiny bit of brevity by changing the "delegate" literals into (slightly)
shorter C# lambdas:

~~~~~
static void ConstructorFunction()
{
    Func<int,Func<int, int>> makeAccount = 
        ((Func<int,Func<int,int>>)( (bal) => {
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
~~~~~

Were it not for the implicitly-typed local variable declaration syntax
around "acctForEugene", it would be acutely obvious that "makeAccount"
isn't creating any kind of object at all, but a function to be executed.
Even so, the explicit typing requirement for the lambdas is kind of
annoying, and will only get worse as we move through the pattern language.


Method Selector
---------------

"You are creating a Function as Object using a Closure. A Constructor
Function creates new instances of the object. How do you provide shared
access to the closure's state?" After all, an account can do more than
just withdraw, but all of the operations on the account have to share
the same state--the account balance--without violating encapsulation.

### Scheme: ###
Again we see the nested lambdas, but now there's a third level of nesting;
the first invocation (make-account) returns a second invocation that will take
a single string, switch on the string, and return a third lambda that will
do the actual work of manipulating the balance.

~~~~~
(define make-account
  (lambda (balance)
    (lambda (transaction)
      (case transaction
        ('withdraw
          (lambda (amount)
            (if (>= balance amount)
              (begin
                (set! balance (- balance amount)
                balance)
              (error "Insufficient funds" balance)))))
        ('deposit
          (lambda (amount)
            (set! balance (+ balance amount))
            balance))
        ('balance
          (lambda ()
            balance))
        (else
          (error "Unknown request -- ACCOUNT"
            transaction))))
  ))
(define account-for-eugene (make-account 100))
((account-for-eugene 'withdraw) 10)  => 90
((account-for-eugene 'withdraw) 10)  => 80
((account-for-eugene 'deposit) 100)  => 180
~~~~~

### JavaScript: ###
Doing this in JavaScript is, again, straightforward, though it does
seem a little too subtle for idiomatic JavaScript:

~~~~~
(function() {
  out("methodSelector ========")

  var makeAccount = function(bal) {
    var balance = bal
    return function(transaction) {
      if (transaction === "withdraw") {
        return function(amount) {
          if (balance >= amount)
            return (balance = (balance - amount))
          else
            throw new Error("Insufficient funds")
        }
      }
      else if (transaction === "deposit") {
        return function(amount) {
          return (balance = (balance + amount))
        }
      }
      else if (transaction === "balance") {
        return function() {
          return balance
        }
      }
      else {
        throw new Error("Insufficient funds")
      }
    }
  }
  var acctForEugene = makeAccount(100)
  out(acctForEugene("withdraw")(20))
  out(acctForEugene("balance")())
})();
~~~~~

### Scala: ###
This style of interface--passing in a string and a variable list of
arguments--really isn't quite Scala's style, since (being a strongly-
typed language) it prefers to be able to compile-time-check as much
as it can, but that doesn't mean we can't build it when the need
and opportunity mesh:

~~~~~
  def methodSelector() = {
    def makeAccount(bal : Int) = {
      var balance = bal
      (transaction : String) => {
        transaction match {
          case "withdraw" =>
            (amt : Int) => {
              if (balance >= amt) {
                balance = (balance - amt) 
                balance
              }
              else 
                throw new RuntimeException("Insufficient funds")
            }
          case "deposit" => {
            (amt : Int) => {
              balance += amt
              balance
            }
          }
          case _ => 
            throw new RuntimeException("Unknown request")
        }
      }
    }
    val acctForEugene = makeAccount(100)
    println(acctForEugene("deposit")(50))
    val acctForTed = makeAccount(100)
    println(acctForTed("withdraw")(50))
  }
~~~~~

### F#: ###
This is, again, like the Yeti version and the Scala version, going to require
some sacrifice in terms of flexibility in order to stay true to the original
Scheme version--in F#, like in Scala and other statically-typed languages, we
have to make sure that all branches of a pattern-match yield the same type of
result, so the "balance" branch has to yield a function that takes a parameter
(and it must be of the same type of parameter as the other two branches), even
though "balance" never makes use of it. This also means that when calling the
return value from "makeAccount", even for balance, we have to pass along some
parameter that will be ignored.

~~~~~
let methodSelector = fun () ->
    let makeAccount =
        fun (bal : int) ->
            let balance = ref bal
            fun transaction ->
                match transaction with
                | "balance" ->
                    fun _ -> !balance
                | "deposit" ->
                    fun (amt : int) ->
                        balance := (!balance) + amt
                        !balance
                | "withdraw" ->
                    fun (amt : int) ->
                        if amt <= !balance then
                            balance := (!balance) - amt
                            !balance
                        else
                            raise (Exception("Insufficient funds"))
                | _ ->
                    raise (Exception("Unrecognized operation" + transaction))
                            
    Console.WriteLine "=========> Method Selector"
    let acctForEugene = makeAccount 100
    printfn "%d" ((acctForEugene "withdraw") 20)
    printfn "%d" ((acctForEugene "balance") 0)
~~~~~

We can address this required-uniformity-of-access a little bit more consistently
with the next pattern element, but whether it's an improvement is debatable.

### Yeti (ML): ###
Nothing new here: the makeAccount function now nests three function literals,
just like the JavaScript and Scala ones do. Like the other languages, we use
a pattern-match/switch-case construct to decide between the different action
strings ("deposit", "withdraw", "balance") and then return the appropriate
function literal for further execution. Note that Yeti, like JavaScript,
actually has a way of returning an "object" here (a structure, which is a
data type the contains one or more named fields, a la objects in JavaScript
or case classes in Scala), but since the goal is to remain as faithful as
possible to the original Scheme implementation, I stick with the more
"functional-only" approach.

~~~~~
makeAccount =
  (do bal:
    var balance = bal;
    do action:
      case action of
        "withdraw": 
          do amt:
            if amt <= balance then
              balance := balance - amt;
              balance
            else
              throw new RuntimeException("Insufficient funds")
            fi
          done;
        "deposit": 
          do amt: 
            balance := balance + amt;
            balance;
          done;
        "balance": 
          do: 
            balance; 
          done;
        _ : throw new RuntimeException("Unknown operation")
      esac
    done;
  done;);

acctForEugene = makeAccount 100;
println ((acctForEugene "withdraw") 20);
println ((acctForEugene "deposit") 20);
~~~~~

### Jaskell (Haskell): ###

~~~~~
~~~~~

### C#: ###
If you stopped reading right here, I wouldn't blame you; this is some ugly
C#, without question, particularly considering that there are other ways
to accomplishing this same effect without requiring quite so much nesting.

~~~~~
static void MethodSelector()
{
    Func<int, Func<string, Func<int, int>>> makeAccount =
        ((Func<int, Func<string, Func<int, int>>>)((bal) =>
        {
            var balance = bal;
            return (string transaction) =>
                {
                    switch (transaction)
                    {
                        case "deposit":
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
                        case "withdraw":
                            return (int amount) =>
                                {
                                    balance += amount;
                                    return balance;
                                };
                        case "balance":
                            return (int unused) =>
                                {
                                    return balance;
                                };
                        default:
                            throw new Exception("Illegal operation");
                    }
                };
        }));
    Console.WriteLine("=============> MethodSelector");
    var acctForEugene = makeAccount(100);
    Console.WriteLine("{0}", acctForEugene("deposit")(20));
    Console.WriteLine("{0}", acctForEugene("withdraw")(20));
    Console.WriteLine("{0}", acctForEugene("balance")(0));
}
~~~~~

Bear in mind, too, that there are some other ways to accomplish what the
C# code here tries to do, one using dynamic types (from 4.0):

~~~~~
static void MethodSelector2()
{
    Func<int, dynamic> makeAccount = (int bal) =>
    {
        var balance = bal;
        dynamic result = new System.Dynamic.ExpandoObject();
        result.withdraw = (Func<int, int>)((amount) => {
            if (balance >= amount)
            {
                balance -= amount;
                return balance;
            }
            else
                throw new Exception("Insufficient funds");
        });
        result.deposit = (Func<int, int>)((amount) =>
        {
            balance += amount;
            return balance;
        });
        result.balance = (Func<int>)(() => balance);
        return result;
    };

    Console.WriteLine("=============> MethodSelector2");
    var acctForEugene = makeAccount(100);
    Console.WriteLine("{0}", acctForEugene.deposit(20));
    Console.WriteLine("{0}", acctForEugene.balance());
    var acctForTed = makeAccount(100);
    Console.WriteLine("{0}", acctForTed.withdraw(10));
    Console.WriteLine("{0}", acctForTed.balance());
}
~~~~~

... or even using ye old plain ol' Dictionary type, taking a string
as a key and yielding Func<> as values for execution:

~~~~~
static void MethodSelector3()
{
    Func<int, Dictionary<string,Func<int,int>>> makeAccount = 
	(int bal) =>
    {
        var balance = bal;
        var result = new Dictionary<string, Func<int,int>>();
        result["withdraw"] = (Func<int, int>)((amount) =>
        {
            if (balance >= amount)
            {
                balance -= amount;
                return balance;
            }
            else
                throw new Exception("Insufficient funds");
        });
        result["deposit"] = (Func<int, int>)((amount) =>
        {
            balance += amount;
            return balance;
        });
        result["balance"] = (Func<int, int>)((unused) => balance);
        return result;
    };

    Console.WriteLine("=============> MethodSelector3");
    var acctForEugene = makeAccount(100);
    Console.WriteLine("{0}", acctForEugene["deposit"](20));
    Console.WriteLine("{0}", acctForEugene["balance"](0));
    var acctForTed = makeAccount(100);
    Console.WriteLine("{0}", acctForTed["withdraw"](10));
    Console.WriteLine("{0}", acctForTed["balance"](0));
}
~~~~~

The second of these two is closer to strict intent of Method Selector from the
Scheme example, but the first allows for flexible arity (numbers of parameters)
in the functions handed back when dereferenced (so that "balance" doesn't have
to take a bogus unused parameter). Frankly, had I to choose, I'd probably go
with the dynamic version, just because of that flexibility.

Message-Passing Interface
-------------------------

"You have created a Method Selector for a Function as Object. You prefer to
use your object in code that has an object-oriented feel. How do you invoke
the methods of an object? [P]rovide a simple message-passing interface for
using the closure."

### Scheme: ###
Everything in a Lisp is a list, and the Scheme implementation uses that to
full effect by taking the argument list passed in to "send" and splits it up
into the object (the account), message (withdraw/deposit/etc), and the
arguments (if any) that are left.

~~~~~
(define send
  (lambda argument-list
    (let ((object  (car argument-list))
          (message (car (cdr argument-list)))
          (args    (cdr (cdr argument-list))))
      (apply (get-method object message) args))
  ))
(define get-method
  (lambda (object selector)
    (object selector)
  ))
(define account-for-eugene (make-account 100))
(send account-for-eugene 'withdraw 50)  => 50
(send account-for-eugene 'deposit 100)  => 150
(send account-for-eugene 'balance)      => 150
~~~~~

### JavaScript: ###
In JavaScript, peeling off the head and tail of the
arguments reference is trickier here, because unlike Scheme, JavaScript
sees "arguments" as an array, not a list. While I could've created "car"
and "cdr" functions in JavaScript to perform the relevant operations
on an array, it felt more idiomatic to provide a function "slice" to do
the "slicing" (which is actually a copy) of elements off the end of the
array instead. More importantly, "slice" is a primitive method on Array
objects in ECMAScript 5, though neither Node nor Rhino in Java 6
recognize it (I suspect because neither is a compliant ECMAScript 5
environment yet), and if this code ever gets run in a ECMAScript 5
world, then it would/should use that version, instead, since it'll likely
be faster than mine.

The other interesting tidbit in here is that when I wrote it the first
time, when doing a deposit, the "balance" became "8020", instead of the
mathematically-correct "100". JavaScript's "promiscuous typing" thought
that the "+" operator wanted to do a string concatenation, instead of
a mathematical add of two numbers, so I had to convince it that the
value coming out of arguments[1] was, in fact, a number, and the easiest
way (it seemed to me at the time) was to just do a quick redundant math
operation on it (multiply by 10, then divide by 10 again). There's likely
a more idiomatic way to do that, I suspect.

I also note that getMethod() in JavaScript is a bit unnecessary; we could
inline its functionality directly inside of send().

~~~~~
(function() {
  out("messagePassingInterface ========")

  var slice = function(src, start, end) {
    var returnVal = []
    var j = 0
    if (end === undefined)
      end = src.length
    for (var i = start; i < end; i++) {
      if (src.length > i)
        returnVal[j++] = src[i]
    }
    return returnVal;
  }
  
  var makeAccount = function(bal) {
    var balance = bal
    return function(transaction) {
      if (transaction === "withdraw") {
        return function(amount) {
          if (balance >= amount)
            return (balance = (balance - amount))
          else
            throw new Error("Insufficient funds")
        }
      }
      else if (transaction === "deposit") {
        return function(amount) {
          return (balance = (balance + (amount * 10.0 / 10.0)))
        }
      }
      else if (transaction === "balance") {
        return function() {
          return balance
        }
      }
      else {
        throw new Error("Insufficient funds")
      }
    }
  }
  var getMethod = function(object, selector) {
    return object(selector)
  }
  var send = function(object, message) {
    return (getMethod(object, message))(slice(arguments, 2))
  }
  var acctForEugene = makeAccount(100)
  out(send(acctForEugene, "withdraw", 20)) // 80
  out(send(acctForEugene, "balance"))      // 80
  out(send(acctForEugene, "deposit", 20))  // 100
  out(send(acctForEugene, "balance"))      // 100
})();
~~~~~

### Scala: ###
The Scala version of this follows the JavaScript version in that it
works off of a variable-argument list, but since Scala doesn't give
us the built-in "arguments" reference, we have to specify it at the
method declaration:

~~~~~
  def messagePassingInterface() = {
    def makeAccount(bal : Int) = {
      var balance = bal
      def send(key:String, args:Any*) = {
        key match {
          case "withdraw" => {
            val amt = args.head.asInstanceOf[Int]
            if (balance >= amt) {
              balance = (balance - amt) 
              balance
            }
            else 
              throw new RuntimeException("Insufficient funds")
          }
          case "deposit" => {
            val amt = args.head.asInstanceOf[Int]
            balance += amt
            balance
          }
        }
      }
      send _
    }
    val acctForEugene = makeAccount(100)
    println(acctForEugene("withdraw", 10))
  }
~~~~~

### F#: ###
By now taking an "obj list" for the parameters, we unify all of the calls
to the account to take a consistent parameter list that still allows for
a flexible number of parameters, but.... It still requires that callers that
don't want to pass any arguments have to pass an empty list. And, on top of
that, it doesn't really feel "F#-ish".

~~~~~
let messagePassingInterface = fun () ->
    let makeAccount =
        fun (bal : int) ->
            let balance = ref bal
            fun transaction ->
                match transaction with
                | "balance" ->
                    fun _ -> !balance
                | "deposit" ->
                    fun (arglist : obj list) ->
                        let amt = arglist.Head :?> int
                        balance := (!balance) + amt
                        !balance
                | "withdraw" ->
                    fun (arglist : obj list) ->
                        let amt = arglist.Head :?> int
                        if amt <= !balance then
                            balance := (!balance) - amt
                            !balance
                        else
                            raise (Exception("Insufficient funds"))
                | _ ->
                    raise (Exception("Unrecognized operation" + transaction))
    let getMethod = fun (acct : string -> obj list -> int) selector -> acct selector
    let send = 
        fun (acct : string -> obj list -> int) (message : string) (arglist : obj list) ->
            (getMethod acct message)(arglist)

    Console.WriteLine "=========> Message Passing Interface"
    let acctForEugene = makeAccount 100
    printfn "%d" (send acctForEugene "withdraw" [20])
    printfn "%d" (send acctForEugene "balance" [])
~~~~~

Note that F# does have language facilities for allowing a [variable-argument
list](http://msdn.microsoft.com/en-us/library/dd233213.aspx) to be passed, 
but it only works on method members:

~~~~~
// From the MSDN documentation
open System

type X() =
    member this.F([<ParamArray>] args: Object[]) =
        for arg in args do
            printfn "%A" arg

[<EntryPoint>]
let main _ =
    // call a .NET method that takes a parameter array, passing values of various types
    Console.WriteLine("a {0} {1} {2} {3} {4}", 1, 10.0, "Hello world", 1u, true)

    let xobj = new X()
    // call an F# method that takes a parameter array, passing values of various types
    xobj.F("a", 1, 10.0, "Hello world", 1u, true)
    0
~~~~~

We could go back and rewrite all of the F# samples to be class member methods
(that is, return actual objects), but that sort of gets away from the spirit
of what the blog exercise is trying to do, so I'll leave that as an exercise
to the reader. (Which, by the way, is author-speak for "I'm feeling lazy and
I don't want to bother".)

### Yeti (ML): ###
Unfortunately, while Yeti (like most functional languages) has a built-in list type,
it doesn't recognize arguments to a function as a list, so we either have to explicitly
put the arguments in, or we have to explicitly state that the arguments to the returned
function literal are a list. I choose the latter tactic, even though it's not the world's
most impressive syntax:

~~~~~
makeAccount =
  (do bal:
    var balance = bal;
    do action:
      case action of
        "withdraw":
          do argList:
            amt = head argList;
            if amt <= balance then
              balance := balance - amt;
              balance;
            else
              throw new RuntimeException("Insufficient funds")
            fi
          done;
        "deposit": 
          do argList:
            amt = head argList; 
            balance := balance + amt;
            balance;
          done;
        "balance": 
          do: 
            balance; 
          done;
        _ : throw new RuntimeException("Unknown operation")
      esac
    done;
  done;);

acctForEugene = makeAccount 100;
println  ((acctForEugene "withdraw")[20]);  // 80
~~~~~

If there's a way to get a Yeti function to accept a variable number of arguments, I've
not seen it in the language overview. I don't know if any ML-derivative has this, to be
honest. Of course, the other thing to do, since this is a statically-typed environment,
is to just return function literals that expect the proper number of arguments, which
will get us the compile-time safety that these languages are supposed to provide; the
below does exactly that--the last line will fail to compile if you uncomment it:

~~~~~
makeAccount =
  (do bal:
    var balance = bal;
    do action:
      case action of
        "withdraw":
          do amt:
            if amt <= balance then
              balance := balance - amt;
              balance;
            else
              throw new RuntimeException("Insufficient funds")
            fi
          done;
        "deposit": 
          do amt:
            balance := balance + amt;
            balance;
          done;
        "balance": 
          do: 
            balance; 
          done;
        _ : throw new RuntimeException("Unknown operation")
      esac
    done;
  done;);

acctForEugene = makeAccount 100;
println ((acctForEugene "withdraw") 20);      // 80
println ((acctForEugene "balance") 0);        // 80
//println ((acctForEugene "withdraw") "fred");  // won't compile
~~~~~

(Truthfully, we should do this for the Scala version, too.) This choice is going to
cause us a little bit of heartache, though, because in order to use "balance", we have
to pass in a number--if we leave off the "_" in the function literal returned from the
"balance" arm of the selector, we don't need to pass "0" when we invoke it, but what's
returned isn't a number, but a function. I can't figure out how to make Yeti take that
function and just invoke it--the syntax guide doesn't seem to say out loud exactly how
I can invoke that function without having to pass in a number argument. If I'd left it
as taking a list, then I could pass an empty list and all would look consistent, if
a little weird.

(Note that this is deliberately opposite what I chose to do for the F# version.)

### Jaskell (Haskell): ###

~~~~~
~~~~~

### C#: ###

~~~~~
~~~~~



Generic Function
----------------
"You have created a Method Selector for a Function as Object. You want to
take full advantage of the tools available in your functional language. How
do you invoke the methods of an object? ... [P]rovide a simple interface to 
the Method Selector that more closely follows the functional style."

### Scheme: ###
In the Scheme implementation, it's interesting that having written the
send function in the last element of the pattern language, we don't
really use it here, but instead just inline its functionality in each
of the named functions (which, in turn, take the argument list, peel off
the head of the argument list as the account object, and pass the remainder
of the arguments on to the selected function):

~~~~~
(define withdraw
  (lambda argument-list
    (let ((object (car argument-list))
          (withdraw-arguments (cdr argument-list)))
      (apply (object 'withdraw) withdraw-arguments)
    )))
(define deposit
  (lambda argument-list
    (let ((object (car arguments))
          (deposit-arguments (cdr arguments)))
      (apply (object 'deposit) deposit-arguments)
    )))
(define balance
  (lambda (object)
    (object 'balance)
  ))
  
(define account-for-eugene (make-account 100))
(withdraw account-for-eugene 10)
(map  (lambda (account) (deposit account 10)) account-for-eugene)
~~~~~

Interestingly enough, I sort of expected the Scheme version to use
"deposit" directly, rather than write a trampoline that calls "deposit",
since we could've avoided the Generic Function part of the language
just by using "send" directly, as well:

~~~~~
(map  (lambda (account) (send account 'deposit 10)) account-for-eugene)
~~~~~

And, to be honest, calling "map" on a single object doesn't really seem
to be a profoundly functional experience, so in my examples I'm going to
create a collection of accounts (called a "bank", naturally enough), and
map across that collection.

### JavaScript: ###
The JavaScript version of this is, again, pretty similar to the Scheme
version. Again, ECMAScript 5 environments are supposed to have a "map"
function natively built in, but previous environments don't, so I have
to write one to verify that we can, in fact, use the named functions
as the mapped operation. I also write a "map2", another version of map
that takes the function to apply to the collection but also takes any
additional arguments after that and passes them to the function being
applied across the collection; it allows me to use "deposit" directly,
instead of having to write a trampoline for it, and besides, it's trivial
to write in JavaScript:

~~~~~
(function() {
  out("genericFunction ==============")

  var slice = function(src, start, end) {
    var returnVal = []
    var j = 0
    if (end === undefined)
      end = src.length
    for (var i = start; i < end; i++) {
      if (src.length > i)
        returnVal[j++] = src[i]
    }
    return returnVal
  }
  
  var map = function(fn, src) {
    var retVal = []
    for (i in src)
      retVal[i] = fn(src[i])
    return retVal
  }
  var map2 = function(src, fn) {
    var retVal = []
    for (i in src)
      retVal[i] = fn(src[i], slice(arguments, 2))
    return retVal
  }
  
  var makeAccount = function(bal) {
    var balance = bal
    return function(transaction) {
      if (transaction === "withdraw") {
        return function(amount) {
          if (balance >= amount)
            return (balance = (balance - amount))
          else
            throw new Error("Insufficient funds")
        }
      }
      else if (transaction === "deposit") {
        return function(amount) {
          return (balance = (balance + (amount * 10.0 / 10.0)))
        }
      }
      else if (transaction === "balance") {
        return function() {
          return balance
        }
      }
      else {
        throw new Error("Insufficient funds")
      }
    }
  }
  var withdraw = function() {
    var object = arguments[0]
    var argumentList = slice(arguments, 1)
    return object("withdraw")(argumentList)
  }
  var deposit = function() {
    var object = arguments[0]
    var argumentList = slice(arguments, 1)
    return object("deposit")(argumentList)
  }
  var balance = function(object) {
    return object("balance")()
  }

  var acctForEugene = makeAccount(100)
  out(withdraw(acctForEugene, 20))
  out(deposit(acctForEugene, 20))
  
  var bank = [
    makeAccount(100),  // acctForEugene
    makeAccount(1000)  // acctForTed
  ]
  map(function(it) { deposit(it, 20) }, bank)
  out(balance(bank[0]))
  out(balance(bank[1]))
  
  map2(bank, deposit, 20)
  out(balance(bank[0]))
  out(balance(bank[1]))
})();
~~~~~

### Scala: ###
Scala, of course, has functional methods built onto its List type
(which we can use instead of an array, since Scala has much better
support for lists than arrays):

~~~~~
  def genericFunction() = {
    def makeAccount(bal : Int) = {
      var balance = bal
      def send(key:String, args:Any*) = {
        key match {
          case "withdraw" => {
            val amt = args.head.asInstanceOf[Int]
            if (balance >= amt) {
              balance = (balance - amt) 
              balance
            }
            else 
              throw new RuntimeException("Insufficient funds")
          }
          case "deposit" => {
            val amt = args.head.asInstanceOf[Int]
            balance += amt
            balance
          }
          case "balance" => {
            balance
          }
          case _ =>
            throw new RuntimeException("Unknown request")
        }
      }
      send _
    }
    def withdraw(account : (String, Any*) => Int, amount : Int) = {
      account("withdraw", amount)
    }
    def deposit(account : (String, Any*) => Int, amount : Int) = {
      account("deposit", amount)
    }
    def balance(account : (String, Any*) => Int) = {
      account("balance")
    }
    val accounts = List(makeAccount(100), makeAccount(200), makeAccount(300))
    accounts.foreach(withdraw(_, 20))
    accounts.foreach((in) => { println(balance(in)) })
  }
~~~~~

### F#: ###
The F# version helps clean up some of the syntax a little, sort of:

~~~~~
let genericFunction = fun () ->
    let makeAccount =
        fun (bal : int) ->
            let balance = ref bal
            fun transaction ->
                match transaction with
                | "balance" ->
                    fun _ -> !balance
                | "deposit" ->
                    fun (arglist : obj list) ->
                        let amt = arglist.Head :?> int
                        balance := (!balance) + amt
                        !balance
                | "withdraw" ->
                    fun (arglist : obj list) ->
                        let amt = arglist.Head :?> int
                        if amt <= !balance then
                            balance := (!balance) - amt
                            !balance
                        else
                            raise (Exception("Insufficient funds"))
                | _ ->
                    raise (Exception("Unrecognized operation" + transaction))
    let deposit = 
        fun amt acct->
            acct "deposit" [amt :> obj]
    let withdraw =
        fun amt acct ->
            acct "withdraw" [amt :> obj]
    let balance =
        fun acct ->
            acct "balance" []

    Console.WriteLine "=========> Generic Function"
    let bank = [ makeAccount 100; makeAccount 200; makeAccount 300 ]
    let balances = List.map (fun it -> deposit 20 it) bank
    List.iter (fun it -> printfn "%d" it) balances
    let balances = List.map (deposit 20) bank
    List.iter (printfn "%d") balances
~~~~~

Notice that by putting the account counter-intuitively as the last parameter
to the generic "deposit" and "withdraw" functions, we can avoid having to write
the "trampoline" function that we would've had to write when using "map"; the
account gets curried from the List directly (as shown in the second example).
We could do the same thing in the Scala version, too, and then wouldn't have to
use the explicit "_" syntax that Scala provides. Of course, if the desire is
instead to pass the amount in a curried fashion, instead of the account, then
the original ordering of the parameters is better.

### Yeti (ML): ###
Writing this in Yeti/ML is definitely trickier than it was in JavaScript, despite the
built-in "map" and other functions, because getting the arguments to "trampoline" right
is a little harder. Fortunately, the generic method hides the "balance 0" weirdness from
the last pattern element, making it a tad easier to use:

~~~~~
makeAccount =
  (do bal:
    var balance = bal;
    do action:
      case action of
        "withdraw":
          do amt:
            if amt <= balance then
              balance := balance - amt;
              balance
            else
              throw new RuntimeException("Insufficient funds")
            fi
          done;
        "deposit": 
          do amt:
            balance := balance + amt;
            balance
          done;
        "balance": 
          do: 
            balance
          done;
        _ : throw new RuntimeException("Unknown operation")
      esac
    done;
  done;);

withdraw =
  (do acct amt:
    (acct "withdraw") amt;
  done;);
deposit =
  (do acct amt:
    (acct "deposit") amt;
  done;);
balance =
  (do acct:
    acct "balance" 0;
  done;);

acctForEugene = makeAccount 100;
println (withdraw acctForEugene 20);      // 80
println (deposit acctForEugene 20);       // 100
println (balance acctForEugene);          // 100

accounts = [(makeAccount 100), (makeAccount 200), (makeAccount 300)];
balances = map (do acct: (deposit acct 20) done) accounts;
for accounts do acct: println(deposit acct 20) done;
~~~~~

Yeti complained if I didn't bind the result of the "map" call to a value, hence the
"balances" value there, even though the balances are actually also stored in the
relevant closures for each account. Note that the "for" line that follows it actually
does the same thing, and prints the results out, to boot. In fact, it's high time
people started to realize that the "for" loop in most imperative languages is just a
non-functional way of doing a "map" without yielding a value. Languages like Scala and
Yeti/ML essentially blur that line significantly enough to the point where we should
just eschew "for" altogether and use "map", if you ask me.

### Jaskell (Haskell): ###

~~~~~
~~~~~

### C#: ###

~~~~~
~~~~~



Delegation
----------

"You are creating a Function as Object. How do you create a new object
that extends the behavior of an existing object? ... [U]se delegation. Make
a Function as Object that has an instance variable an instance of the
object you want to extend. Implement behaviors specific to the new object
as methods in a Method Selector. Pass all other messages onto the instance
variable."

Again, in a traditional O-O language, we'd just inherit, and in an object-
functional hybrid, we could do the same. There's no real point not to, to
be honest. But the interesting thing about this implementation is that it
demonstrates the runtime relationship between a JavaScript object and its
prototype: calling a function passing in the "derived" object causes the
"derived" to try its "base" (its prototype) in the event that the method
in question isn't defined on the "derived".

Note also that this particular trick is really only feasible because the
"object" presents a uniform interface: all interaction with the "object"
(whether it is a standard account or an interest-bearing one) is done 
through the Method Selector mechanism, which allows for this extension
without having to modify any sort of base interface. This isn't so much
a knock on O-O as a whole as it is on statically-typed traditional O-O.

### Scheme: ###
This is pretty straightforward, if you understood the Message-Passing
Interface implementation of earlier.

~~~~~
(define make-interest-bearing-account
  (lambda (balance interest-rate)
    (let ((my-account (make-account balance)))
      (lambda (transaction)
        (case transaction
          ('accrue-interest
            (lambda ()
              ((my-account 'deposit)
                (* ((my-account 'balance))
                   interest-rate)) ))
        (else
          (my-account transaction))
        )))
  ))
(define account-for-eugene (make-interest-bearing-account 100 0.05))
((account-for-eugene 'balance))         => 100
((account-for-eugene 'deposit) 100)     => 200
((account-for-eugene 'balance))         => 200
((account-for-eugene 'accrue-interest)) => 210
((account-for-eugene 'balance))         => 210
~~~~~

### JavaScript: ###
Despite the fact that the JavaScript implementation just keeps getting
longer and longer, it's actually not that much harder to add in this
delegation functionality--again, as has been the case for a lot of the
JavaScript code, it's almost a direct one-to-one port from the Scheme:

~~~~~
(function() {
  out("delegation =======")
  
  var slice = function(src, start, end) {
    var returnVal = []
    var j = 0
    if (end === undefined)
      end = src.length
    for (var i = start; i < end; i++) {
      if (src.length > i)
        returnVal[j++] = src[i]
    }
    return returnVal
  }
  
  var makeAccount = function(bal) {
    var balance = bal
    return function(transaction) {
      if (transaction === "withdraw") {
        return function(amount) {
          if (balance >= amount)
            return (balance = (balance - amount))
          else
            throw new Error("Insufficient funds")
        }
      }
      else if (transaction === "deposit") {
        return function(amount) {
          return (balance = (balance + (amount * 10.0 / 10.0)))
        }
      }
      else if (transaction === "balance") {
        return function() {
          return balance
        }
      }
      else {
        throw new Error("Insufficient funds")
      }
    }
  }
  var makeInterestBearingAccount = function(bal, intRate) {
    var myAccount = makeAccount(bal)
    return function(transaction) {
      if (transaction === "accrueInterest") {
        return function() {
          var balance = myAccount("balance")()
          var interest = (balance * intRate)
          return myAccount("deposit")(interest)
        }
      }
      else
        return myAccount(transaction)
    }
  }
  
  var acctForEugene = makeInterestBearingAccount(100, 0.05)
  out(acctForEugene("balance")())
  out(acctForEugene("deposit")(20))
  out(acctForEugene("accrueInterest")())
  out(acctForEugene("balance")())
})();
~~~~~

### Scala: ###
The Scala version of this is tricky, because it relies on a very subtle
bit of Scala syntax; specifically, when we try to pass the "args" sequence
(which, in actual implementation, is a WrappedArray) from the 
"makeInterestBearingAccount" function to the "makeAccount" function (by
which I mean, the functions returned from those two functions), if we don't
use the peculiar ": _*" syntax, Scala interprets "args" to be a single
parameter (a single parameter whose type is a collection), instead of
the intended "pass the arguments through" behavior. (If you're a Java
or C# developer, it's like having a varargs method calling another varargs
method, and passing the array of arguments from the first as an array
instead of each element on its own to form the array of arguments in the
second. Yeah, I know--it's a little brain-twisty.)

~~~~~
  def delegation() = {
    def makeAccount(bal : Int) = {
      var balance = bal
      def send(key:String, args:Any*) = {
        key match {
          case "withdraw" => {
            val amt = args.head.asInstanceOf[Int]
            if (balance >= amt) {
              balance = (balance - amt) 
              balance
            }
            else 
              throw new RuntimeException("Insufficient funds")
          }
          case "deposit" => {
            val amt = args.head.asInstanceOf[Int]
            balance += amt
            balance
          }
          case "balance" => {
            balance
          }
          case _ =>
            throw new RuntimeException("Unknown request")
        }
      }
      send _
    }
    def makeInterestBearingAccount(bal : Int, intRate : Double) = {
      val account = makeAccount(bal)
      def send(key: String, args:Any*) = {
        key match {
          case "accrueInterest" => {
            val amt = (int2float(account("balance")) * intRate).toInt
            account("deposit", amt)
          }
          case _ =>
            account(key, args : _*)
        }
      }
      send _
    }
    val acctForEugene = makeInterestBearingAccount(100, 0.05)
    println(acctForEugene("deposit", 20))
    println(acctForEugene("accrueInterest"))
    println(acctForEugene("balance"))
  }
~~~~~

### F#: ###
Aside from the aforementioned weirdness about the obj list as a generic
parameter mechanism, this is really straightforward:

~~~~~
let delegation = fun () ->
    let makeAccount =
        fun (bal : int) ->
            let balance = ref bal
            fun transaction ->
                match transaction with
                | "balance" ->
                    fun _ -> !balance
                | "deposit" ->
                    fun (arglist : obj list) ->
                        let amt = arglist.Head :?> int
                        balance := (!balance) + amt
                        !balance
                | "withdraw" ->
                    fun (arglist : obj list) ->
                        let amt = arglist.Head :?> int
                        if amt <= !balance then
                            balance := (!balance) - amt
                            !balance
                        else
                            raise (Exception("Insufficient funds"))
                | _ ->
                    raise (Exception("Unrecognized operation" + transaction))
    let makeInterestBearingAccount =
        fun (bal : int) (intRate : float) ->
            let account = makeAccount bal
            fun transaction ->
                match transaction with
                | "accrueInterest" ->
                    fun _ ->
                        let balance = (account "balance" [])
                        let interest : float = float balance * intRate 
                        account "deposit" [int interest]
                | _ -> account transaction
    Console.WriteLine "=========> Delegation"
    let acctForEugene = makeInterestBearingAccount 100 0.05
    printfn "%d" (acctForEugene "deposit" [20])
    printfn "%d" (acctForEugene "accrueInterest" [])
~~~~~

Note the explicit casts in the "accrueInterest" code: this is because F#, like a lot
of functional languages, won't do automatic type-promotion for you. So the "int"s
have to be explicitly converted to "float"s, and back again.

### Yeti (ML): ###
Since we didn't go down the path of trying to do the variable-argument list in Yeti,
we don't have the same problems the Scala version presented, and the generic methods
(the top-level "withdraw", "deposit" and "balance" functions) actually help hide the
syntactic weirdness that we ran into in the last pattern element:

~~~~~
makeAccount =
  (do bal:
    var balance = bal;
    do action:
      case action of
        "withdraw":
          do amt:
            if amt <= balance then
              balance := balance - amt;
              balance
            else
              throw new RuntimeException("Insufficient funds")
            fi
          done;
        "deposit": 
          do amt:
            balance := balance + amt;
            balance
          done;
        "balance": 
          do: 
            balance
          done;
        _ : throw new RuntimeException("Unknown operation")
      esac
    done;
  done;);

withdraw =
  (do acct amt:
    (acct "withdraw") amt;
  done;);
deposit =
  (do acct amt:
    (acct "deposit") amt;
  done;);
balance =
  (do acct:
    acct "balance" 0;
  done;);

acctForEugene = makeAccount 100;
println (withdraw acctForEugene 20);      // 80
println (deposit acctForEugene 20);       // 100
println (balance acctForEugene);          // 100

accounts = [(makeAccount 100), (makeAccount 200), (makeAccount 300)];
balances = map (do acct: (deposit acct 20) done) accounts;
for accounts do acct: println(deposit acct 20) done;
~~~~~

Note the last two lines--the "for" construct in most imperative languages is actually
akin to the "map" construct in most functional languages, except that in the imperative
"for" there's no return value from the expression, and in a functional "map" there
(usually) is. This is why we have to bind the result from the "map" to a name, and we
don't have any results from the "for". (The "map" also insists on having a returned
value--a list of Unit isn't acceptable, which is what would be returned if we used
the "println" expression in the "map".)

### Jaskell (Haskell): ###

~~~~~
~~~~~

### C#: ###

~~~~~
~~~~~




Private Method
--------------
"You have created a Method Selector. How do you factor common behavior
out of the methods in the Method Selector? ... [D]efine the common code in
a Local Procedure (Wallingford). Invoke this procedure in place of the
duplicated code within the Method Selector."

### Scheme: ###

~~~~~
(define make-account
  (lambda (balance)
    (let ((transaction-log '())
      (log-transaction
        (lambda type amount)
          (set! transaction-log
                (cons (list type amount)
                      transaction-log)))) )
      (lamba (transaction)
        (case transaction
          ('withdraw
            (lambda (amount)
              (if (>= balance amount)
                (begin
                  (set! balance (- balance amount))
                  (log-transaction 'withdraw amount)
                  balance)
                (error "Insufficient funds" balance))))
          ('deposit
            (lambda (amount)
              (set! balance (+ balance amount))
              (log-transaction 'deposit amount)
              balance))
        ...))
    ))
~~~~~

### JavaScript: ###
Again, in JavaScript, we rely on the fact that anything declared inside
the "makeAccount" function but outside the function returned by "makeAccount"
is encapsulated, and create both the "transactionLog" (an array, since
JavaScript likes those better than lists) and the function to append to it
("logTransaction") within that "neutral zone". Just to prove that the transaction log 
is being written, I add another method to the method selector table, "viewLog", to 
return the contents of the transaction log.

~~~~~
(function() {
  out("privateMethod ===========")
  
  var makeAccount = function(bal) {
    var transactionLog = []
    var logTransaction = function(type, amount) {
      transactionLog.push("Action: " + type + " for " + amount)
    }
    
    var balance = bal
    return function(transaction) {
      if (transaction === "withdraw") {
        return function(amount) {
          if (balance >= amount) {
            logTransaction("withdraw", amount)
            return (balance = (balance - amount))
          }
          else
            throw new Error("Insufficient funds")
        }
      }
      else if (transaction === "deposit") {
        return function(amount) {
          logTransaction("deposit", amount)
          return (balance = (balance + (amount * 10.0 / 10.0)))
        }
      }
      else if (transaction === "balance") {
        return function() {
          logTransaction("balance", balance)
          return balance
        }
      }
      else if (transaction === "viewLog") {
        return function() {
          return (transactionLog)
        }
      }
      else {
        throw new Error("Insufficient funds")
      }
    }
  }
  var acctForEugene = makeAccount(100)
  out(acctForEugene("withdraw")(20))
  out(acctForEugene("balance")())
  out(acctForEugene("deposit")(20))
  out(acctForEugene("balance")())
  out(acctForEugene("viewLog")())
})();
~~~~~

### Scala: ###
The Scala version is also pretty straightforward--we've already seen that
Scala supports nested functions, so it is simply a matter of defining the
logTransaction() function and an empty List[String] in the same 
"neutral zone" in which the "balance" variable lives. Instead of adding
a new selector to the list, I chose this time to just print the transaction
log as part of the "balance" operation.

~~~~~
  def privateMethod() = {
    def makeAccount(bal : Int) = {
      var balance = bal
      var transactionLog = List[String]()
      def logTransaction(action:String, amount:Int) = {
        val msg = ("Action: " + action + " for " + amount)
        transactionLog = transactionLog :+ msg
      }
      def send(key:String, args:Any*) = {
        key match {
          case "withdraw" => {
            val amt = args.head.asInstanceOf[Int]
            if (balance >= amt) {
              logTransaction("withdraw", amt)
              balance = (balance - amt) 
              balance
            }
            else 
              throw new RuntimeException("Insufficient funds")
          }
          case "deposit" => {
            val amt = args.head.asInstanceOf[Int]
            logTransaction("deposit", amt)
            balance += amt
            balance
          }
          case "balance" => {
            println(transactionLog)
            balance
          }
          case _ =>
            throw new RuntimeException("Unknown request")
        }
      }
      send _
    }
    val acctForEugene = makeAccount(100)
    println(acctForEugene("deposit", 20))
    println(acctForEugene("balance"))
  }
~~~~~

### F#: ###
Binding a local function is, by this point, somewhat trivial and
uninspiring, but it's just as easily done in F# as it is in any of the
other languages:

~~~~~
let privateMethod = fun () ->
    let makeAccount =
        fun (bal : int) ->
            let transactionLog = ref []
            let logTransaction act (amt : int) =
                let message = "Action: " + act + " for " + amt.ToString()
                transactionLog := List.append !transactionLog [message]
            let balance = ref bal
            fun transaction ->
                match transaction with
                | "balance" ->
                    fun _ -> 
                        List.iter (printfn "%s") !transactionLog
                        !balance
                | "deposit" ->
                    fun (arglist : obj list) ->
                        let amt = arglist.Head :?> int
                        balance := (!balance) + amt
                        logTransaction "deposit" amt
                        !balance
                | "withdraw" ->
                    fun (arglist : obj list) ->
                        let amt = arglist.Head :?> int
                        if amt <= !balance then
                            balance := (!balance) - amt
                            logTransaction "withdraw" amt
                            !balance
                        else
                            raise (Exception("Insufficient funds"))
                | _ ->
                    raise (Exception("Unrecognized operation" + transaction))
    Console.WriteLine "=========> Private Method"
    let acctForEugene = makeAccount 100
    printfn "%d" (acctForEugene "deposit" [20])
    printfn "%d" (acctForEugene "withdraw" [50])
    printfn "%d" (acctForEugene "balance" [])
~~~~~

### Yeti (ML): ###
The private method in Yeti is, again, just a nested function hiding out in the closure
that is returned by "makeAccount"; the fact that Yeti supports expressions embedded
inside of strings makes it easy to create the transaction log string:

~~~~~
makeAccount =
  (do bal:
    var balance = bal;
    var transactionLog is list<string> = [];
    logTransaction action amount = 
      transactionLog := "Action: \(action) for \(amount)" :: transactionLog;
    do action:
      case action of
        "withdraw":
          do amt:
            if amt <= balance then
              logTransaction "withdraw" amt;
              balance := balance - amt;
              balance
            else
              throw new RuntimeException("Insufficient funds")
            fi
          done;
        "deposit": 
          do amt:
            logTransaction "deposit" amt;
            balance := balance + amt;
            balance
          done;
        "balance": 
          do: 
            println transactionLog;
            balance
          done;
        _ : throw new RuntimeException("Unknown operation")
      esac
    done;
  done;);
~~~~~

### Jaskell (Haskell): ###

~~~~~
~~~~~

### C#: ###

~~~~~
~~~~~



Summary
-------

JavaScript is, of course, the de-facto golden child right now.

And Scala is, undoubtedly, one of my favorites. It's syntax is a little quirky in
places, but no more so than any other language I've used.

I like the Yeti code style and syntax, and could definitely see doing some
small projects in it, particularly some service-y kinds of things with it, a la
Web or REST services; the Yeti source code has some examples of how to create
(for example) servlets and WARs, and it's a nice syntax. I don't know that I'd want
to create a full-fledged MVC framework on top of Yeti, but as something that's
basically taking input, doing processing and sending back JSON or XML results,
it's not a bad approach. Considering you can also create classes in Yeti, which
puts it into the same grounds as F#, it's worth looking into if you've got some
ML in your background and want to go back to it while staying on top of the JVM.

The F# version is a nice mix of ML and objects, though the casting operators are
definitely a syntax that only a mother could love, and the distinction between
what is allowed on functions vs. methods (such as the parameter arrays) feels a
little arbitrary at times. (I'm sure there's good reasons for it, but it still feels
a little arbitrary, at least to me.) The "cannot close over local variables, use
refs instead" rule is also a little annoying, although it does make it explicitly
clear that now you're closing over a reference, not the actual value, so now the
"what happens if I modify the closed-over value" question becomes self-explanatory.
(This sometimes trips people up in other languages that don't make the by-value
or by-reference closing-over semantics explicit.)

Honestly, I don't really expect that anyone reading this piece is going to
immediately turn around, abandon all their domain objects, and take up this
approach as a replacement--in some cases, taking this "all functional"
style creates more angst than it really provides benefits--but we can use
parts of it to generate some really interesting new patterns.



[Envoy]: http://www.cs.uni.edu/~wallingf/patterns/envoy.pdf
[Scala]: http://www.typesafe.com
[C#]: http://en.wikipedia.org/wiki/C_Sharp_(programming_language)
[F#]: http://en.wikipedia.org/wiki/F_Sharp_(programming_language)
[Yeti]: http://mth.github.com/yeti/
[Jaskell]: http://jaskell.codehaus.org

>>>>>>> theirs
