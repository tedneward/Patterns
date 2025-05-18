# Pipes-and-Filters
The Pipes-and-Filters pattern ...

## Problem
Imagine you are building a system that must process or transform a stream of input data. Implementing such a system as a single component may not be feasible for several reasons: the system has to be built by several developers, the global system task decomposes naturally into several processing stages, and the requirements are likely to change.

You therefore plan for future flexibility by exchanging or reordering the processing steps. By incorporating such flexibility, it is possible to build a family of systems using existing processing components. The design of the system--especially the interconnection of processing steps--has to consider the following forces: 

* Future system enhancements should be possible by exchanging processing steps or by recombination of steps, even by users.
* Small processing steps are easier to reuse in different contexts than large components.
* Non-adjacent processing steps do not share information.
* Different sources of input data exist, such as a network connection or a hardware sensor providing temperature readings, for example.
* It should be possible to present or store final results in various ways.
* Explicit storage of intermediate results for further processing in files clutters directories and is error-prone, if done by users.
* You may not want to rule out multi-processing the steps, for example running them in parallel or quasi-parallel.

Whether a separation into processing steps is feasible strongly depends on the application domain and the problem to be solved. For example, an interactive, event-driven system does not split into sequential stages. 

## Context

Processing data streams. *(TODO: This needs more detail.)*

## Solution

The Pipes and Filters architectural pattern divides the task of a system into several sequential processing steps. These steps are connected by the data flow through the system-the output data of a step is the input to the subsequent step. Each processing step is implemented by a filter component. A filter consumes and delivers data incrementally--in contrast to consuming all its input before producing any output--to achieve low latency and enable real parallel processing. The input to the system is provided by a data source such as a text file. The output flows into a data sink such as a file, terminal, animation program and so on. The data source, the filters and the data sink are connected sequentially by pipes. Each pipe implements the data flow between adjacent processing steps. The sequence of filters combined by pipes is called a processing pipeline. 

Filter components are the processing units of the pipeline. A filter enriches, refines or transforms its input data. It enriches data by computing and adding information, refines data by concentrating or extracting information, and transforms data by delivering the data in some other representation. A concrete filter implementation may combine any of these three basic principles.

The activity of a filter can be triggered by several events:

* The subsequent pipeline element pulls output data from the filter.
* The previous pipeline element pushes new input data to the filter.
* Most commonly, the filter is active in a loop, pulling its input from and pushing its output down the pipeline.

The first two cases denote so-called passive filters, whereas the last case is an active filter. An active filter starts processing on its own as a separate program or thread. A passive filter component is activated by being called either as a function (pull) or as a procedure (push). 

*Pipes* denote the connections between filters, between the data source and the first filter, and between the last filter and the data sink. If two active components are joined, the pipe synchronizes them. This synchronization is done with a first-in- first-out buffer. If activity is controlled by one of the adjacent filters, the pipe can be implemented by a direct call from the active to the passive component. Direct calls make filter recombination harder, however. 

The *data source* represents the input to the system, and provides a sequence of data values of the same structure or type. Examples of such data sources are a file consisting of lines of text, or a sensor delivering a sequence of numbers. The data source of a pipeline can either actively push the data values to the first processing stage, or passively provide data when the first filter pulls.

The *data sink* collects the results from the end of the pipeline. Two variants of the data sink are possible. An active data sink pulls results out of the preceding processing stage, while a passive one allows the preceding filter to push or write the results into it. 

## Implementations

## Consequences
A Pipes-and-Filters approach tends to lead to several consequences:

* *No intermediate files necessary, but possible.* Computing results using separate programs is possible without pipes, by storing intermediate results in files. This approach clutters directories, and is error-prone if you have to set up your processing stages every time you run your system. In addition, it rules out incremental and parallel computation of results. Using Pipes and Filters removes the need for intermediate files, but allows you to investigate intermediate data by using a T-junction in your pipeline.

* *Flexibility by filter exchange.* Filters have a simple interface that allows their easy exchange within a processing pipeline. Even if filter components call each other directly in a push or pull fashion, exchanging a filter component is still straightforward. In our compiler example a scanner generated with lex can easily be replaced by a more efficient hand-coded function yylex that performs the same task. Filter exchange is generally not possible at run-time due to incremental computation in the pipeline.

* *Flexibility by recombination.* This major benefit, combined with the reusability of filter components, allows you to create new processing pipelines by rearranging filters or by adding new ones. A pipeline without a data source or sink can be embedded as a filter within a larger pipeline. You should aim to tune the system plafform or surrounding infrastructure to support this flexibility, such as is provided by the pipe mechanism and shells in UNIX.

* *Reuse of filter components.* Support for recombination leads to easy reuse of filter components. Reuse is further enhanced if you implement each filter as an active component, while the underlying plafform and shell allow easy end-user construction of pipelines.

* *Rapid prototyping of pipelines.* The preceding benefits make it easy to prototype a data-processing system from existing filters. After you have implemented the principal system function using a pipeline you can optimize it incrementally. You can do this, for example, by developing specific filters for time-critical processing stages, or by reimplementing the pipeline using more efficient pipe connections. Your prototype pipeline can however be the final system if it performs the required task adequately. Highly-flexible filters such as the UNIX tools sed and awk reinforce such a prototyping approach.

* *Efficiency by parallel processing.* It is possible to start active filter components in parallel in a multiprocessor system or a network. If each filter in a pipeline consumes and produces data incrementally they can perform their functions in parallel.

* *Sharing state information is expensive or inflxible.* If your processing stages need to share a large amount of global data, applying the Pipes and Filters pattern is either inefficient or does not provide the full benefits of the pattern. 

* Efficiency gain by parallel processing is often an illusion. This is for several reasons:

    * The cost for transferring data between filters may be relatively high compared to the cost of the computation carried out by a single filter. This is especially true for small filter components or pipelines using network connections.
    * Some filters consume all their input before producing any output, either because the task, such as sorting, requires it or because the filter is badly coded, for example by not using incremental processing when the application allows it.
    * Context-switching between threads or processes is generally an expensive operation on a single-processor machine.
    * Synchronization of filters via pipes may stop and start filters often, especially when a pipe has only a small buffer.

* *Data transformation overhead.* Using a single data type for all filter input and output to achieve highest flexibility results in data conversion overheads. Consider a system that performs numeric calculations and uses UNIX pipes. Such a system must convert ASCII characters to real numbers, and vice-versa, within each filter. A simple filter, such as one that adds two numbers, will spend most of its processing time doing format conversion.

* *Error handling.* Error handling is the Achilles' heel of the Pipes and Filters pattern. You should at least define a common strategy for error reporting and use it throughout your system. A concrete error recovery or error-handling strategy depends on the task you need to solve. If your intended pipeline is used in a 'mission-critical' system and restarting the pipeline or ignoring errors is not possible, you should consider structuring your system using alternative architectures such as [Layers](../../Structural/Layers/).

## Relationships

The [Layers](../../Structural/Layers/) pattern is better suited to systems that require reliable operation, because it is easier to implement error handling than with Pipes and Filters. However, Layers lacks support for the easy recombination and reuse of components that is the key feature of the Pipes and Filter pattern. 

Each processing component can often be its own [Strategy](../Strategy/).

It's relatively easy to see a Pipes and Filters pattern as a [Chain of Responsibility](../ChainOfResponsibility/), though the intent is somewhat different; in a CoR, the idea is to pass data through the pipe until one of the participants in the Chain acknowledges and handles the request, whereas in a P&F approach, the assumption is that every participant in the pipe wants/needs to see the data, often to manipulate it in some way.

[Blackboard](../Blackboard/) systems can be built as Pipes and Filters, where each participant in the decision-making process takes a turn at the data before passing it on to the next one in the chain.

## Variations
Variations on Pipes and Filters include...

