module v1 =

type Handler() =
    abstract Dispense : Currency -> Currency

type ATM() =
    let chain : list Handler = [ FiftyDollarDispenser(); ]

    member this.Dispense(amount : Currency) =
        chain.Dispense(amount)


type FiftyDollarDispenser =
    interface Handler with
        member _.Dispense(amount : Currency) -> Currency =
            printfn("Hello from F#")

