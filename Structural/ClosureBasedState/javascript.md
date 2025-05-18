title=Closure-based State: Javascript
date=2016-04-07
type=pattern
tags=pattern implementation, structural, csharp
status=published
description=A Closure-based State implementation in Javascript.
~~~~~~

As a language, JavaScript lacks a number of the traditional facilities that are present in an object-oriented language, such as access control, and so we have to take steps in order to provide the expected level of encapsulation. This is done by making use of [Closure-based State](../ClosureBasedState).

JavaScript is, surprisingly to some "old-school" JavaScript programmers, a full-fledged member of the family of languages that support closures, so all that's necessary here is to define a function that returns a function that "closes over" the local variable "state". But, in order to make sure that balance isn't reset to its original value of 100 each time we call the function, we have to actually invoke the outer function to return the inner function, which is then bound to the name "operation"; that way, the variable "state" is initialized once, yet still referenced:

````javascript
var operation = function() {
  var state = 100;
  return function(adjust) {
    state += adjust;
    return state;
  };
}();
````

Because Javascript objects are intrinsically [Dynamic Objects](../DynamicObject), it's also trivial to use Closure-based State to hold state outside of the object, without having to worry about the separation of interface from implementation that is necessary in other languages:

````javascript
var operation = function() {
  var privateState = {
    internalState : 100
  };
  var publicObject = { };
  publicObject.operation = function(adjust) {
    privateState.internalState += adjust;
    return privateState.internalState;
  };
  return publicObject;
}();
````

`privateState` can be expanded to hold whatever private state is necessary to support the operations on `publicObject`, and remain entirely inaccessible to clients making use of it.

#### Module implementation
When building NodeJS modules, it's quite common to use a variation of this idea to hold state behind the module object that is returned from the "exports" construct:

````javascript
// In state.js
var privateState = {
  state : 100
};

exports.operation = function(adjust) {
  privateState.state += adjust;
  return privateState.state;
};


// In app.js
var state = require('./state');

console.log(state.operation(100));
````

This serves the same basic purpose as Closure-based State, but at a larger scope, since now the module is effectively also a [Singleton](../Singleton).

