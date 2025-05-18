title=Factory Method: Java
date=2022-02-28
type=pattern
tags=pattern implementation, creational, java
status=published
description=A Factory Method implementation in Java.
~~~~~~

Java has long had a relationship with [Factory Method](../FactoryMethod), usually calling it by the more degenerative term "Factory" or "Factory pattern". (Technically, what Java calls a "Factory pattern" is typically one of [Builder](../Builder/), [Abstract Factory](../AbstractFactory/), or Factory Method, depending on what precisely looks to be varied and/or encapsulated.)

Java has a number of Factory Method implementations already in its standard library:

* `javax.xml.parsers.DocumentBuilderFactory`: `newInstance` creates a ConcreteProduct implementation of XML parsers.

* `javax.xml.transform.TransformerFactory`: Ditto; `newInstance` creates a new ConcreteProduct implementation of XSLT transformers.

* `javax.xml.xpath.XPathFactory`: And also ditto; `newInstance` creates a new ConcreteProduct implementation of XPath query navigators.

Frankly, all three of these were an awesome example of a case where the use of the pattern was wildly overblown, based on the thought process at the time (the '99/2000 timeframe) that Java developers would want to change up their XML parser implementation without having to change any code. That never really happened.

