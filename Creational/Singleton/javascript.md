title=Singleton: Javascript
date=2016-03-26
type=pattern
tags=pattern implementation, creational, javascript
status=published
description=A Singleton implementation in Javascript.
~~~~~~

Javascript presents some challenges when working with [Singletons](../Singleton). Being a prototype-based language (meaning there are no classes), Singletons in Javascript are usually centered more around the conceptual contents of the object, not it's nominal type. This means that there's no concept of "static", either, since "statics" are essentially a synonym for "type" or "class" elements in the other object-oriented languages. In addition, Javascript objects are simply "bags" of name/value pairs with no access control, which means that in addition to the instance now having no real (language- or environment-enforced) access control, we may have to get a little creative just to help ensure that clients will always go to the right place to find the lone (intended) instance.

To get all of this right, we're going to have to walk through this one step at a time.

By the way, NodeJS has a number of conventions that will help enforce some of this, but let's go through this using just "stock" Javascript (ECMAScript 5), and then we'll examine how to do this using NodeJS. This will all change pretty significantly in ECMAScript/JavaScript 6, so we'll do that as a separate example.

We begin by taking a pretty normal Javascript object:

````javascript
var theInstance = { };
theInstance.state = 0;
theInstance.DoSomething = function() {
  this.state++;
  console.log("I'm doing something for the " + this.state + " time");
};

theInstance.DoSomething();
theInstance.DoSomething();
theInstance.DoSomething();
````

Nothing really exciting here, though certainly there are a few different ways we could've created this object, ultimately, so long as we end up with the above (an object with a field "state" and a method "DoSomething"), we're good.

#### Namespacing
First, and possibly most easily, Singleton "reduces the name space". Our object above is very likely to clash with other objects called the same, so it would be better if this is somehow tucked away behind a name that (more or less) is likely to be unique. In Javascript, which (until 6) has no packaging or module concept, this is done by simply layering the object inside another object, which thus acts as a scoping namespace:

````javascript
var NewardAssociates = { };
NewardAssociates.Util = { };
NewardAssociates.Util.Product = theInstance;

NewardAssociates.Util.Product.DoSomething();
````

Presumably nobody else will have a local var whose name is my company's name, so that will (hopefully) provide a scoping mechanism. On the one hand, this seems a little awkward on the client end. Fortunately, Javascript is pretty flexible here, and we can "reference" the deeper name by (again) making use of a var to reference the "nested namespaces":

````javascript
var NewardAssociates = { };
NewardAssociates.Util = { };
NewardAssociates.Util.Product = theInstance;

NewardAssociates.Util.Product.DoSomething();

var Product = NewardAssociates.Util.Product;
Product.DoSomething();
````

Feels a little silly in some respects to do this, but like a ground-hitched horse, sometimes just the idea of a restriction can be enough of a restriction to make a difference.

#### Access Control
The big problem with the above, of course, is that anyone can go get that state and muck around with it to their hearts' content. Singleton wants to set up some kind of restriction in place so that we ensure that clients are always working with the same instance---that implies some level of enforceable access control.

Some Javascript old-timers use another convention---the underscore-prefixed name---to imply to other programmers "Don't touch this!". Given the total lack of enforceability, however, and knowing that many developers would sell their own grandmother if it meant a quick hack would get their job done a lot faster than following the convention, it would be nice if we could restrict access a little bit more strongly than that.

Fortunately, we can use the "hanging enclosured state" idiom to provide this level of inaccessibility. (If you've not seen this before, make sure to check out 
[JavaScript: The Good Parts](http://www.amazon.com/JavaScript-Good-Parts-Douglas-Crockford/dp/0596517742) at some point, because that is essentially the canonical reference for "modern" JavaScript these days.) So the next step is to write a function literal that "encloses" a local state variable that remains in scope of the function literal but outside the object itself:

````javascript
var NewardAssociates = { };
NewardAssociates.Util = { };
NewardAssociates.Util.Product = { };
NewardAssociates.Util.Product.Instance = function() {
  var instance = theInstance;
  return function() {
    return instance;
  }
}();

var Product = NewardAssociates.Util.Product;
Product.Instance().DoSomething(); // state = 1
Product.Instance().DoSomething(); // state = 2
````

We can apply the same principle to the state itself, if necessary, but that's not really a concern of this particular pattern (see [State](../../structural/State), instead).

At this point, we have a reasonably functional Singleton.

*NOTES*:

* *It has no concurrency control.* Javascript lacks any sort of language-level concurrency control syntax or semantics. But NodeJS is "single-threaded" when executing our code, so in theory this isn't quite as much of a concern as it might be in other environments.
* *Eager or JIT?* It's always JIT; Javascript is an interpreted language (at least, in all of the currently popular environments, it is), so unless Javascript defines a language-level module facility that allows for initialization before the Instance() is called, it will have to be JIT. (Incidentally, both NodeJS and ES/JS 6 will provide exactly such facilities.)
* *Method or property?* Javascript has no concept of a property, per se, so if we want that "barrier" of access control in front of the Singleton instance, accessing the Singleton instance is always going to have to go through a method/function.
* *Enforcing Singleton-ness.* Good luck with that. In Javascript, an object is an object is an object, and since objects have no type and there is no access control, there is really nothing stopping any client from simply `var newInstance = {}` and copying over whatever state it can get its hands on.

#### Properties
Aliaksei Spenach points out in comments that later versions of ECMAScript 5/Javascript support 
an [explicit property notation](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Guide/Working_with_Objects#Defining_getters_and_setters):

````javascript
var o = {
  a: 7,
  get b() { 
    return this.a + 1;
  },
  set c(x) {
    this.a = x / 2
  }
};

console.log(o.a); // 7
console.log(o.b); // 8
o.c = 50;
console.log(o.a); // 25
````

This then allows the implementation to change to:

````javascript
var singleton = {
  _instance : {
    state : 0,
    doSomething : function() {
      this.state++;
      console.log("state: " + this.state);
    }
  },
  get instance() {
    return this._instance;
  }
}

singleton.instance.doSomething();
singleton.instance.doSomething();
singleton.instance.doSomething();
````

This, however, doesn't make the actual instance field any less accessible, so the above "hanging enclosured state" idiom is likely still necessary to the implementation. Once ECMAScript/JavaScript has some form of "proper" access control (properties, modules, etc), this will be less of an issue except in legacy codebases or those that need to execute in legacy environments.

## Implementation: NodeJS Javascript
Once we assume the NodeJS environment, a few options become available. NodeJS has established a convention around Javascript modules that allows us some more opportunities to better namespace and control access to the Singleton, although some of the other concerns (such as the relative inability to enforce Singleton-ness) are still in full force.

#### Namespacing
In NodeJS, we can use the "exports"/"require" facility to create a module, and thereby create a separate namespace in which the Singleton implementation and its details can reside:

````javascript
// In Product.js
exports.Product = { };

var Singleton = exports.Product;
Singleton.Instance = function() {
  var instance = {};
  return function() {
    return instance;
  }
}();

Singleton.Instance().state = 0;
Singleton.Instance().DoSomething = function() {
  this.state++;
  console.log("I'm doing something for the " + this.state + " time");
};
````

Which we can then reference using `require()` from client code:

````javascript
var Product = require("./Product").Product;
Product.Instance().DoSomething();
````

Notice that the NodeJS module uses the same "hanging enclosed state" idiom as before to avoid putting the instance into a accessible field inside the exported Product object.

Fortunately, NodeJS takes care to make sure that `require()` is persistent, so that multiple uses of it don't accidentally load multiple copies of the same module:

````javascript
var Product = require("./Product").Product;
Product.Instance().DoSomething(); // state = 1
var Product = require("./Product").Product;
Product.Instance().DoSomething(); // state = 2
````

The above code chooses to eagerly-instantiate the instance, but we can pretty easily flip it over to using a just-in-time instantiation:

````javascript
exports.Product = { };

var Singleton = exports.Product;
Singleton.Instance = function() {
  var instance = undefined;
  return function() {
    if (instance === undefined) {
      instance = {};
      instance.state = 0;
      instance.DoSomething = function() {
        this.state++;
        console.log("I'm doing something for the " + this.state + " time");
      };
    }
    return instance;
  }
}();
````

This has the (admittedly tiny) drawback of a performance hit on each call to Instance(); however, remember, this is Javascript, and if you're concerned about performance, boy are you already in trouble.

