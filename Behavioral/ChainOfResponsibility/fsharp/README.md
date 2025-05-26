# Chain of Responsibility: F#
There are multiple implementations of the ATM in this directory.

* **v2** uses a custom interface for the dispensation call and the F#-standard `list` type to maintain the chain as a whole. However, since now each element doesn't decide whether to continue processing the request or not, we must "signal" the interim state of the request by returning some kind of indication whether more work is required or not. In this particular case, we key off of the `Currency` amount requested--as each stage in the chain dispenses cash, if it did not dispense the requested amount entirely, it returns the remaining amount that requires dispensation.





* **ATMv3** uses the standard `Func<>` type along with `List` to minimize the need for custom types entirely, but when we do this, we lose any ability to have more than one method on the interface, as well as to define top-level types that describe each different processing type more explicitly.

Note that in a more production-worthy implementation, the ATM types would handle the input more effectively, so that the `Program.Main` code would be simpler and not have to handle the "divisible by 10" requirement explicitly.

To run the code, use `dotnet run`. If necessary, run `dotnet restore` before doing "run" (although the restore *should* be done as part of the "run" if it has never been run before).
