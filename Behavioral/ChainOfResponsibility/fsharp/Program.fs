type Currency(amount: int) =
    member this.Amount = amount

open v1

let atm = v1.ATM()

// For more information see https://aka.ms/fsharp-console-apps
printfn "Hello from F#"

