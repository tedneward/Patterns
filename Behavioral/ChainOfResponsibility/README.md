# Chain of Responsibility
Chain of Responsibility ...

## Problem
Avoid coupling the sender of a request to its receiver by giving more than one object a chance to handle the request. Chain the receiving objects and pass the request along the chain until an object handles it.

## Context
The GOF lists a couple of scenarios in which Chain makes sense:

* **More than one object may handle a request, and the handler isn't known *a priori*. The handler should be ascertained automatically.** Often the request will originate in one part of the system, but it needs to be target in a different part, and the two aren't coupled or connected enough to merit merging the two systems.

* **You want to issue a request to one of several objects without specifying the receiver explicitly.** We should probably amend that word "objects" to mean "targets" since the targets can be objects or functions, depending on the implementation language.

* **The set of objects that can handle a request should be specified dynamically.** Often we want to "wire up" the connections at runtime, rather than baking them in statically (at compile- or design-time), often because we may want to add to or remove from that set of request handlers. 

## Solution

Some questions arise out of this:

* **Seriality.** Can elements in the chain be processed simultaneously (as in, can a given node in the chain be a "split" or "tee" to two or more processing elements)? If so, can they be run in parallel? 

* **Awareness.** Are the elements in the chain aware of each other?

* **Early satisfaction.** If one element in the chain handles the request, do the others in the chain get a chance to do so as well, or is it "first-come, first-served" handling?

* **Concurrency.** Can the chain be invoked from multiple threads simultaneously? 

* **Chain creation/configuration.** Who is responsible for creating and configuring (registering each element) the chain? Does the chain make use of a standardized data structure (such as a common collection type like a LinkedList or Array) or does the chain manage each element itself directly?

* **Re-configuration.** Is the chain able to reconfigure itself after construction?

## Implementations

* The [C#](chsarp/) implementation demonstrates an ATM, wherein a given amount requested by the user must be dispensed in a combination of USD$50, USD$20, and USD$10 notes, using the minimum number of actual notes. (In other words, a request for USD$200 should dispense 4xUSD$50, rather than 10xUSD$20 or 20xUSD$10 notes.) The chain is made up of "dispenser" objects, each of whom have a single method.

## Consequences
A Chain of Responsibility tends to lead to several consequences:

* **Reduced coupling.** The pattern frees an object from knowing which other object handles a request. An object only has to know that a request will be handled "appropriately." Both the receiver and the sender have no explicit knowledge of each other, and an object in the chain doesn't have to know about the chain's structure.

    As a result, Chain of Responsibility can simplify object interconnections. Instead of objects maintaining references to all candidate receivers, they keep a single reference to their successor.

* **Added flexibility in assigning responsibilities to objects.** Chain of Responsibility gives you added flexibility in distributing responsibilities among objects. You can add or change responsibilities for handling a request by adding to or otherwise changing the chain at run-time. You can combine this with subclassing to specialize handlers statically.

* **Ordering dictates priority.** If an earlier element in the chain handles the request, subsequent elements in the chain won't ever see the request.

* **Receipt isn't guaranteed.** Since a request has no explicit receiver, there's no guarantee it'll be handled—-the request can fall off the end ofthe chain without ever being handled. A request can also go unhandled when the chain is not configured properly.

## Variations
A couple of different takes on the Chain of Responsibility include:

### Publish-Subscribe/"Pub-Sub" or Event Bus
When combined with a [Message-Passing Interface](../../Structural/MessagePassingInterface/), we often end up with what gets called a "publish-subscribe" system or communications backplane, particularly if the underlying delivery system is built on store-and-forward messaging systems (JMS, MSMQ, Tibco, Kafka, NATS, and so on). Often, "subscribers" to a queue/topic are a Chain of Responsibility, and each time a message is pushed into the messaging system, each subscriber receives it, independently of the other subscribers on the Chain.

When messages are being sent to a single message queue or topic with multiple registered handler/processors, we refer to that as an Event Bus. If the first interested handler/processor on the bus pulls the message off the bus, it is a Chain of Responsibility; if the messages continue down the bus so that all interested handler/processors get a chance to see it, it is more of an [Observer](../Observer/).

### Workflow
At its core, the Chain of Responsibility assumes that there is no "decisioning" nodes inside the chain that could take the processing down an entirely different path--it is a linear path that could stop processing early. If the elements in the chain can be bypassed or repeated based on the state of the request, then we have more of a "workflow" system than a singular chain.

Very often, Workflow systems will be highly reconfigurable, and often long-running (meaning the workflow will need to be persistable).

