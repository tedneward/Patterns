title=Blackboard
date=2022-03-05
type=pattern
tags=pattern, behavioral, architectural
status=published
description=Defines a mechanic by which individualized components each solve a portion of an otherwise non-deterministic solution to collectively come up with a solution.
~~~~~~

***tl;dr*** In the Blackboard pattern, several specialized components assemble their knowledge to bulld a possibly partial or approximate solution. The name 'blackboard' was chosen because it is reminiscent of the situation in which human experts sit in front of a real blackboard and  work together to solve a problem: Each expert separately evaluates the current state of the solution, and may go up to the blackboard at any time and add, change or delete information. Humans usually decide themselves who has the next access to the blackboard; in the pattern we describe, a moderator component decides the order in which components execute if more than one can make a contribution.

<!--more-->

The Blackboard pattern was originally documented in *Pattern-Oriented Software Architecture*, Vol 1, p. 71.

## Problem
"The Blackboard pattern tackles problems that do not have a feasible deterministic solution for the transformation of raw data into high level data structures, such as diagrams, tables or English phrases. Vision, image recognition, speech recognition and surveillance are examples of domains in which such problems occur." The Gang-of-Five (as the POSA1 authors were sometimes called) described Blackboard in terms that were very AI-centric, but I believe that Blackboard is applicable to a wide variety of problems well beyond the traditional AI space. 

"[These problems] are characterized by a problem that, when decomposed into subproblems, spans several fields of expertise. The solutions to the partial problems require different representations and paradigms. In many cases no predetermined strategy exists for how the 'partial problem solvers' should combine their knowledge. This is in contrast to functional decomposition, in which several solution steps are arranged so that the sequence of their activation is hard-coded." Consider business scenarios where a customer is entering a variety of data, from which we need to present a number of options for their consideration, such as insurance or financial products--a mortgage, for example, or health insurance. There may be a variety of potential solutions, and the choice of which solutions fit may well be based on only small parts of the data.

"In some of the above problem domains you may also have to work with uncertain or approximate knowledge. Each transformation step can also generate several alternative solutions. In such cases it is often enough to find an optimal solution for most cases, and a suboptimal solution, or no solution, for the rest. The limitations of a Blackboard system therefore have to be documented carefully, and if important decisions depend on its results, the results have to be verified." One of the areas of research in AI was around "fuzzy logic", in which solutions are based on data values that express a "range" of potential values--for example, in health care, we often want to determine if somebody is "obese", but there's not necessarily a sharp dividing line between "healthy" and "obese" (is 200 pounds healthy or obese? It depends!), and may have some overlap.

## Context

The following forces influence solutions to problems of this kind:

* A complete search of the solution space is not feasible in a reasonable time. For example, if you consider phrases of up to ten words using a vocabulary of a thousand words, the number of possible permutations of words is in the order of 1000^10.
* Since the domain is immature, you may need to experiment with different algorithms for the same subtask. For this reason, individual modules should be easily exchangeable.
* There are different algorithms that solve partial problems. For example, the detection of phonetic segments in the waveform is unrelated to the generation of phrases based on words and word sequences.
* Input, as well as intermediate and final results, have different representations, and the algorithms are implemented according to different paradigms.
* An algorithm usually works on the results of other algorithms.
* Uncertain data and approximate solutions are involved. For example, speech often includes pauses and extraneous sounds. These significantly distort the signal. The process of interpretation of the signal is also error-prone. Competing alternatives for a recognition target may occur at any stage of the process. For example, it is hard to distinguish between 'till' and 'tell'. The words 'two' and 'too' even have the same pronunciation, as do many others in English.
* Employing disjoint algorithms induces potential parallelism. If possible you should avoid a strictly sequential solution.

## Solution
The idea behind the Blackboard is a collection of independent components that work cooperatively on a common data structure. Each component is specialized for solving a particular part of the overall task, and all components work together on the solution. These specialized components are independent of each other: they do not call each other, nor is there a predetermined sequence for their activation. Instead, the direction taken by the system is mainly determined by the current state of progress. A central control component evaluates the current state of processing and coordinates the specialized components. This data-directed control regime is referred to as *opportunistic problem solving*. It makes experimentation with different algorithms possible, and allows experimentally-derived heuristics to control processing.

During the problem-solving process the system works with partial solutions that are combined, changed or rejected. Each of these solutions represents a partial problem and a certain stage of its solution, The set of all possible solutions is called the *solution space*, and is organized into levels of abstraction. The lowest level of solution consists of an internal representation of the input. Potential solutions of the overall system task are on the highest level.

Divide your system into a component called *blackboard*, a collection of *knowledge sources*, and a *control* component.

The blackboard is the central data store. Elements of the solution space and control data are stored here. We use the term vocabulary for the set of all data elements that can appear on the blackboard. The blackboard provides an interface that enables all knowledge sources to read from and write to it.

All elements of the solution space can appear on the blackboard. For solutions that are constructed during the problem solving process and put on the blackboard, we use the terms hypothesis or blackboard entry. Hypotheses rejected later in the process are removed from the blackboard.

A hypothesis usually has several attributes, for example its abstraction level, that is, its conceptual distance from the input. Hypotheses that have a low abstraction level have a representation that is still similar to input data representation, while hypotheses with the highest abstraction level are on the same abstraction level as the output. Other hypothesis attributes are the estimated degree of truth of the hypothesis or the time interval covered by the hypothesis.

It is often useful to specify relationships between hypotheses, such as 'part-of or 'in-support-of'.

Knowledge sources do not communicate directly-they only read from and write to the blackboard. They therefore have to understand the vocabulary of the blackboard. We explore the ramifications of this in the Implementation section.

Often a knowledge source operates on two levels of abstraction. If a knowledge source implements forward reasoning, a particular solution is transformed to a higher-level solution. A knowledge source that reasons backwards searches at a lower level for support for a solution, and may refer it back to a lower level if the reasoning did not give support for the solution. 

Each knowledge source is responsible for knowing the conditions under which it can contribute to a solution. Knowledge sources are therefore split into a condition-part and an action-part. The conditionpart evaluates the current state of the solution process, as written on the blackboard, to determine if it can make a contribution. The action-part produces a result that may cause a change to the blackboard's contents. 

The control component runs a loop that monitors the changes on the blackboard and decides what action to take next. It schedules knowledge source evaluations and activations according to a knowledge application strategy. The basis for this strategy is the data on the blackboard.

The strategy may rely on control knowledge sources. These special knowledge sources do not contribute directly to solutions on the blackboard, but perform calculations on which control decisions are made. Typical tasks are the estimation of the potential for progress, or the computational costs for execution of knowledge sources. Their results are called control data and are put on the blackboard as well.

Theoretically, it is possible that the blackboard can reach a state at which no knowledge source is applicable. In this case, the system fails to deliver a result. In practice, it is more likely that each reasoning step introduces several new hypotheses, and that the number of possible next steps 'explodes'. The problem is therefore to restrict the alternatives to be taken rather than to find an applicable knowledge source.

A special knowledge source or a procedure in the control component determines when the system should halt, and what the final result is. The system halts when an acceptable hypothesis is found, or when the space or time resources of the system are exhausted.

The blackboard component defines two procedures: `inspect` and `update`. Knowledge sources call `inspect` to check the current solutions on the blackboard, and `update` is used to make changes to the data on the blackboard.

## Implementation

* []

If the Blackboard has its own declarative language associated with it, it often becomes a *Rules Engine* (below).

## Relationships
Blackboards can sometimes be built out of a [Pipes and Filters](../PipesAndFilters/) approach, wherein each knowledge source is represented by a component in the pipeline, and the "board" of data being examined is passed explicitly to each one in sequential order. This inhibits the ability to operate in parallel, but does simplify the implementation since we no longer have to worry about concurrency issues. Additionally, if the pipeline is arranged in a circular pipeline (a ring), then it's easy to make multiple passes without having to wonder if each knowledge source has had a chance to examine the data.

## Consequences
A Blackboard approach tends to lead to several consequences:

* *Experimentation.* In domains in which no closed approach exists and a complete search of the solution space is not feasible, the Blackboard pattern makes experimentation with different algorithms possible, and also allows different control heuristics to be tried.
* *Support for changeability and maintainability.* The Blackboard supports changeability and maintainability because the individual knowledge sources, the control algorithm and the central data structure are strictly separated. However, all modules can communicate via the blackboard.
* *Reusable knowledge sources.* Knowledge sources are independent specialists for certain tasks. A Blackboard helps in making them reusable. The prerequisites for reuse are that knowledge source and the underlying Blackboard system understand the same protocol and data, or are close enough in this respect not to rule out adaptors for protocol or data.
* *Support for fault tolerance and robustness.* In a Blackboard all results are just hypotheses. Only those that are strongly supported by data and other hypotheses survive. This provides tolerance of noisy data and uncertain conclusions.
* *Difficulty of testing.* Since the computations of a Blackboard system do not follow a deterministic algorithm, its results are often not reproducible. In addition, wrong hypotheses are part of the solution process.
* *No good solution is guaranteed.* Usually Blackboard systems can solve only a certain percentage of their given tasks correctly.
* *Difficulty of establishing a good control strategy.* The control strategy cannot be designed in a straightforward way, and requires an experimental approach.
* *Low Emiency.* Blackboard systems suffer from computational overheads in rejecting wrong hypotheses. If no deterministk algorithm exists, however, low efficiency is the lesser of two evils when compared to no system at all.
* *High development effort.* Most Blackboard systems take years to evolve. We attribute this to the ill-structured problem domains and extensive trial-and-error programming when defining vocabulary, control strategies and knowledge sources.
* *No support for parallelism.* The Blackboard architecture does not prevent the use of a control strategy that exploits the potential parallelism of knowledge sources. It does not however provide for their parallel execution. Concurrent access to the central data on the blackboard must also be synchronized. 

## Variations
A couple of different takes on Blackboard include:

* *Production System* or *Rules Engine*: In this variant subroutines are represented as condition-action rules, and data is globally available in working memory. Condition-action rules consist of a left-hand side that specifies a condition, and a right-hand side that specifies an action. The action is executed only if the condition is satisfied and the rule is selected. The selection is made by a 'conflict resolution module'. A Blackboard system can be regarded as a radical extension of the original production system formalism: arbitrary programs are allowed for both sides of the rules, and the internal complexity of the working memory is increased. Complicated scheduling algorithms are used for conflict resolution. If the production system is generalized to any sort of domain, it is often called (and sold) as a "rules engine".

* *Repository*: This variant is a generalization of the Blackboard pattern. The central data structure of this variant is called a repository. In a Blackboard architecture the current state of the central data structure, in conjunction with the Control component, activates knowledge sources. In contrast, the Repository pattern does not specify an internal control. A repository architecture may be controlled by user input or by an external program. A traditional database, for example, can be considered as a repository. Application programs working on the database correspond to the knowledge sources in the Blackboard architecture. Programming environments are often organized as a  collection of tools together with a shared repository of programs and program fragments. Even applications that have been traditionally viewed as pipeline architectures, may be more accurately interpreted as repository systems.. .' Compilers, for example, have traditionally been described and sometimes also been implemented as pipelines. Modern compilers have a repository that holds shared information such as symbol tables and abstract syntax trees. The compilation phases correspond to knowledge sources operating on the repository.  This architecture enables incremental problem solving: The scanner reads an identifier that is not yet defined; the parser recognizes the syntactical unit described by the identifier; and the code generator then jumps in and creates the corresponding machine code, if any. 
