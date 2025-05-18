title=Context Object
date=2022-02-25
type=pattern
tags=pattern, structural
status=published
description=Use an object to provide some degree of inference or reference about the environment in which another object or group of objects is operating. 
~~~~~~

***tl;dr*** Context Objects often provide a degree of scope for a group of objects operating within a larger space (such as an app server), and as such often serve as a means of access both outside of the scope (for those objects within it) and to the inside of the scope (for those objects outside of it) without violating encapsulation. Context objects can also provide some specific guarantees about those elements stored within the context, such as thread-safe access or transactional boundaries.

<!--more-->

We often see Context Objects used in HTTP middleware, where the HTTP request is captured as a single object containing not only all the headers (and their values) as well as any query parameters (and their values), but information about the request itself, such as the transport (http or https), the relative resource path, any fragments, and so on. 

## Problem
Frequently an operation, which consists fundamentally of inputs and a generated output, requires additional information by which to carry out its work. In some cases, this consists of out-of-band information, such as historical data, previous values, or quality-of-service data, which needs to travel with the operation regardless of its execution path within the system. The desire is to decouple the various participants working with the operation from having to know everything that is being "carried around" as part of the operation.

## Context
*(Irony....)*

Define a wrapper object that encapsulates all aspects of an operation, including details that may not be directly related to that operation. Context allows an object or graph of objects to be handled in a single logical unit, as part of a logical unit of work.

## Solution

To create a Context Object, define a wrapper object that encapsulates all aspects of an operation, including details that may not be directly related to that operation. Context allows an object or graph of objects to be handled in a single logical unit, as part of a logical unit of work.


Some questions arise out of this:


## Implementations


## Consequences
A Context Object tends to lead to several consequences:

## Relationships

Context Objects are often used to capture the parameters and other information passed along to an [Interceptor](../Interceptor/), either explicitly as part of the Interceptor interface or along a "side channel" if the signature of the Interceptor needs to conform to pre-existing descriptions.

Context is often the object passed along a [Chain of Responsibility](../../behavioral/ChainOfResponsibility/); each ConcreteHandler in the Chain examines the Context and potentially modifies it as necessary before handing it along to the next Handler in the Chain.

Context is also used as a wrapper for a [Command](../../behavioral/Command/) object, providing additional information beyond the Command itself. The key difference between the two is that Context provides out-of-band information that the Command object may not even know is there, for processing by others around the Command.

## Variations
A couple of different takes on the Context Object include:

* ***Thread-specific storage (POSA2 475).*** Not only the pattern documented in POSA2, but the language/platform-level support provided.

* ***Parameter Object (PEAA).*** Fowler's Parameter Object pattern describes the use of an object to capture a variety of required or optional parameters to a method call rather than directly-declared parameters on the function or method call, allowing for additional extensibility and flexibility over time. This is a very simple form of Context Object, where the context is the scope of that function call. Interestingly, if the context object is attached someplace beyond the thread stack, the parameter object no longer has to be explicitly passed down the call chain, which may make it easier to allow downstream recipients to access it.


## Known Uses
Several distributed communication toolkits make use of Context or something very similar to it. COM+, for example, uses the notion of Context as a interception barrier, allowing for a tightly-coupled graph of objects to be treated as an atomic unit, synchronizing multi-threaded calls into the context, also called an apartment. Transactions are traced as they flow through different parts of the system, such that each Context knows the transaction ID it was associated with and can allow that same transaction ID (the causality) to continue to flow through, thus avoiding self-deadlock.

Web Services also make use of Context, using the SOAP Message format as a mechanism in which out-of-band information, such as security headers and addressing information, can be conveyed without "polluting" the data stored in the message itself. WS-Security, WS-Transaction, WS-Routing, among others, are all examples of specifications that simply add headers to SOAP messages, so that other "processing nodes" in the Web service call chain can provide the appropriate semantics.

Similarly, more modern HTTP/middleware toolkits make use of Context, wherein the original request/response objects (corresponding to the incoming HTTP request and an object building up the HTTP response) are "implicitly available". Several of these same toolkits then provide [Interceptors](../Interceptor/) that take portions of the request (such as any Bearer Tokens) and use that to provide additional information about the request (such as the username making the request and/or permissions associated with that user).

---

The followup looked like this:

<blockquote>
<p>Wow--lots of you have posted comments about Context. Let's address some of them and see what comes up in the second iteration:  
<ul> <li> <p>Michael Earls wrote:  <blockquote>Very timely. I'm building a system right now that fits this pattern. We spent about five minutes determining what to call "it" (the little "black box" that holds the core command, entity, and metadata information). We settled on "nugget". Now there's prior art I can refer to. I'm using Context with WSE 2.0 and SOAP extensions for the pipeline in exactly the way you describe. Nice.</blockquote> and <blockquote>Another Related Pattern: Additionally, the Context may also be an container/extension/augmentation/decoration on the UnitOfWork (???).</blockquote>I suspect you're right--Context can be used to hold the information surrounding a UnitOfWork, including the transaction it's bound to (if the transaction is already opened). This is somewhat similar to what the MTS implementation does, if I'm not mistaken. <p></p> <li> <p>Kris wrote:  <blockquote>The HTTP pipeline in ASP.NET comes to mind, with the HttpContext being passed through for various things like session state, security, etc. One possible side effect that I can see (hopefully you can drop some thoughts on this one), is how to manage dependencies between the chain, as well as order of invocation of chain elements. The MS PAG stuff I believe talks about this somewhat with the Pipelines &amp; Filters pattern, but I'd love to hear your thoughts as well.</blockquote>The PAG stuff (Sandy Khaund's post) was part of what triggered this post in the first place, but I want to be careful not to rely too much on Microsoft prior art (WSE, Shadowfax, HttpContext, COM/MTS/COM+) since in many cases those systems were designed by people who had worked together before and/or shared ideas. The Rule of Three says that the pattern needs to be discovered "independently" of any other system, although with Google around these days that's becoming harder and harder to do. :-) As to managing dependencies between the chain, I think that's out of scope to Context itself--in fact, that raises another interesting pattern relationship, in that Context can be the thing operated upon by a Blackboard [POSA1 71]. Context doesn't care who interacts with it when, IMHO. <p></p> <li> <p>Dan Moore wrote:  <blockquote>Another context <i>(pun intended? --TKN)</i> I've read a lot about is the transactional context used in enterprise transaction processing systems. This entity contains information about the transaction, needed by various participants.</blockquote>Yep. Read on (Dan Malks' post). <p></p> <li> <p>Dan Malks wrote:  <blockquote>Hi Ted, Good start with your pattern writeup :) We document "Context Object" in our pattern catalog in our book "Core J2EE Patterns" 2nd ed., Alur, Crupi, Malks. I hope you'll find some interesting content in our writeup, so please feel free to have a look and let me know what you think. Thanks, Dan Malks</blockquote>Thanks, Dan. As soon as you posted I ran off and grabbed my copy of your book, and looked it up, and while I think there's definitely some overlap (boy what I wouldn't give to workshop this thing at PLOP this year), Context Object, given the protocol-independence focus that you guys gave it in Core J2EE, looks like a potential combination of Context and Chain of Responsibility. I wanted to elevate Context out of just the Chain of Responsibility usage, though, to create something that isn't "part of" another pattern--I'll leave it to you guys to decide whether Context makes sense in that regard. <p></p> <li> <p>Mark Levison wrote:  <blockquote>On related patterns: Context is what is passed into a Flyweight (alias Glyph's). We've been using Context for over two years on current project.</blockquote>Really? Wow; I never would have considered that, but of course it makes sense when you describe it that way. I'm not really keeping track, but I think we've reached the Rule of Three.
