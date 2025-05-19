# Chain of Responsibility: Swift
Uses command-line swift, and as such, should be available/accessible for execution from any OS that supports Swift at the command line (eg., specifically, Linux, though WSL may be a possibility too).

Like other languages, this implementation has multiple versions. In the first version, we manage the chain explicitly as part of the protocol. In the second version, however, we use an Array to hold the chain, which is distinctly different from other languages--this is because Swift has no built-in linked list collection class as part of Foundation.

To run, use `swift run` in this directory.
