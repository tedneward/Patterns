title=Compositional Patterns
date=2022-03-06
type=patternsindex
tags=patterns
status=published
description=Patterns which are "built up" out of several different patterns, to make a new--and worth documenting and studying--pattern at a higher level of abstraction.
patternfilter=compositional
~~~~~~

***tl;dr*** For many years, I've looked at all the different "scope levels" that patterns seem to address, and wondered about their relationship, and it hit me just recently that many of the "architectural" or "distributed" patterns are, in fact, some combination of other patterns, allowing us to express the composed pattern at a higher level of abstraction. The GOF even allude to this in their first chapter, talking about how the classic Model-View-Controller of Smalltalk is actually an Observer, Composite, and Strategy pattern, composed in such a way as to produce a useful design (and one that has been revisisted/reinvented numerous times) but separate and distinct from each of those "foundational" patterns.

<!--more-->

Some of the patterns I expect to analyze and document here include:

* Model-View-Controller (from POSA1 and GOF intro)
* View Handler (from POSA1)
* Client-Dispatcher-Server (from POSA1)

*(TODO: Go through PEAA, EIP, maybe some of the Core J2EE Patterns, see what fits here)*
