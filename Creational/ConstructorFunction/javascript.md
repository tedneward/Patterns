title=Constructor Function: Javascript
date=2016-04-01
type=pattern
tags=pattern implementation, creational, javascript
status=published
description=A Constructor Function implementation in Javascript.
~~~~~~

(Comments and constructive criticism desired.)

Javascript provides a pretty reasonable implementation of a [Constructor Function](../ConstructorFunction); as a matter of fact, most of the time "modern Javascript" (as described by [Crockford](http://www.amazon.com/JavaScript-Good-Parts-Douglas-Crockford/dp/0596517742/)  prefers the Constructor Function, or some variant of it, over the more traditional object-oriented approach.

Constructor Functions are often used in conjunction with [Function Objects](../../structural/FunctionObjects) and/or [Closure-based State](../../structural/ClosureBasedState), but can easily be extended to a more traditional object-oriented environment. 

Within Javascript, we have essentially a combination of both of those worlds; we have a highly dynamic environment, like LISP, but it's got objects "built-in", except without some of the more traditional object features, and so on. As one colleague once put it, "You've got just enough of both worlds to make you think it's the best of each, and not enough of the worst of both worlds to make you give it up entirely."

Thus, if the goal is to create a Product instance whose state is properly protected away from prying eyes (namely, the client who receives the Product instance back from the Constructor Function), we can do this by making use of Closure-based State, like so:

````javascript
var creator = function(somestate) {
  var state = somestate;
  return function() {
    var product = {};
    product.Operation = function() {
      console.log("Doing Operation on " + state);
    };
    return product;
  }();
};

var product1 = creator(27);
product1.Operation();
var product2 = creator(48);
product2.Operation();
````

The client is oblivious to the actual work necessary to create the object; the only interface surfaced to the client is the Constructor Function, and how it works is entirely irrelevant/encapsulated.

However, a Constructor Function needs to be able to allow for variance in the objects returned without the client being aware of the actual work necessary to do so; in some cases, this may require Constructor Functions to make use of one another as a sort of "stepping stone" to the finished OtherProduct:

````javascript
var anotherCreator = function(somestate, someotherstate) {
  var other = someotherstate;
  return function() {
    var anotherProduct = creator(somestate);
    anotherProduct.OtherOp = function() {
      console.log("Doing OtherOperation on " + other);
    };
    return anotherProduct;
  }();
};

var product3 = anotherCreator(5, 22);
product3.Operation();
product3.OtherOp();
````

Obviously, nothing stops the second Creator from simply building the AnotherProduct to look identical to the Product and returning that, but this way any changes to the original Product object's surface area will be preserved in the returned AnotherProduct, without required change to AnotherCreator.

Note that "creator" and "anotherCreator" can both exist, or one can silently supplant the other, such as:

````javascript
// First creator
var creator = function(somestate) {
  var state = somestate;
  return function() {
    var product = {};
    product.Operation = function() {
      console.log("Doing Operation on " + state);
    };
    return product;
  }();
};

// Second creator
var origCreator = creator;
var creator = function(somestate, someotherstate) {
  var other = someotherstate;
  return function() {
    var anotherProduct = origCreator(somestate);
    anotherProduct.OtherOp = function() {
      console.log("Doing OtherOperation on " + other);
    };
    return anotherProduct;
  }();
};

var product4 = creator(55, 55);
product4.Operation();
````

Care must be taken to avoid infinite recursion; the original creator must be preserved and referenced through some other name, otherwise calling "creator" from within "creator" will unintentionally recurse.

#### Variant: Static constructor methods
Although Javascript lacks the formal notion of "type" or "class" (at least, up until ECMAScript 6), it can approximate the notion of a static constructor method from other languages with some slight-of-hand that is common to Javascript:

````javascript
// Basic Product "interface"
var Product = {
  "Operation" : function () {
    console.log("Shouldn't be here")
  },
  "CommonOperation" : function () {
    console.log("Common operation across all Products")
  }
};

Product.createProductOne = function() {
  var state = 100;
  var retProduct = Object.create(Product);
  retProduct.Operation = function() {
    state += 2;
    console.log("Product1 new state ",state);
  }
  return retProduct;
};
Product.createProductTwo = function() {
  var state = 666;
  var retProduct = Object.create(Product);
  retProduct.Operation = function() {
    console.log("Product2 never changes state ",state);
  }
  return retProduct;
};

var p1 = Product.createProductOne();
p1.CommonOperation();
p1.Operation();
var p2 = Product.createProductTwo();
p2.CommonOperation();
p2.Operation();
````

Here, we make use of the ECMAScript `Object.create()` function to establish the prototype link between the various "subtypes" of Product being returned to the Product object itself, so that Product can provide common behavior across all Products. Thus, Product serves as both an "abstract interface" for the Product family, as well as the lexical namespace for creating Product instances, such as what we would see in langauges that support static creator methods, a la C++, Java or C#.

(Note that this implementation makes use of [Closure-based State](../../structural/ClosureBasedState) to encapsulate the storage details of each Product away from prying eyes; this is not necessary to the ConstructorFunction, but is a common tie-in.)
