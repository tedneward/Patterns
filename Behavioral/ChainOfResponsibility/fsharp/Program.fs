type Currency(amount: int) =
    member this.Amount = amount

module v2 =

    type Handler =
        abstract Dispense : Currency -> Currency

    type FiftyDollarDispenser() =
        interface Handler with
            member _.Dispense(amount : Currency) : Currency =
                if amount.Amount >= 50 then
                    let count = amount.Amount / 50
                    printfn "Dispensing %d fifty dollar bills" count
                    Currency(amount.Amount - (count * 50))
                else
                    amount

    type TwentyDollarDispenser() =
        interface Handler with
            member _.Dispense(amount : Currency) : Currency =
                if amount.Amount >= 20 then
                    let count = amount.Amount / 20
                    printfn "Dispensing %d twenty dollar bills" count
                    Currency(amount.Amount - (count * 20))
                else
                    amount

    type TenDollarDispenser() =
        interface Handler with
            member _.Dispense(amount : Currency) : Currency =
                if amount.Amount >= 10 then
                    let count = amount.Amount / 10
                    printfn "Dispensing %d ten dollar bills" count
                    Currency(amount.Amount - (count * 10))
                else
                    amount

    type ATM() =
        let chain : Handler list = [ 
            FiftyDollarDispenser(); 
            TwentyDollarDispenser();
            TenDollarDispenser()
        ]

        member this.Dispense(amount : Currency) =
            let mutable remaining = amount
            for handler in chain do
                remaining <- handler.Dispense(remaining)

            if remaining.Amount > 0 then
                printfn "Cannot dispense remaining amount: %d" remaining.Amount

module v3 =

    type ATM() =
        let chain : (Currency -> Currency) list = [ 
            fun amount -> 
                if amount.Amount >= 50 then
                    let count = amount.Amount / 50
                    printfn "Dispensing %d fifty dollar bills" count
                    [Currency(amount.Amount - (count * 50))]
                else
                    [amount]

            fun amount -> 
                if amount.Amount >= 20 then
                    let count = amount.Amount / 20
                    printfn "Dispensing %d twenty dollar bills" count
                    [Currency(amount.Amount - (count * 20))]
                else
                    [amount]

            fun amount -> 
                if amount.Amount >= 10 then
                    let count = amount.Amount / 10
                    printfn "Dispensing %d ten dollar bills" count
                    [Currency(amount.Amount - (count * 10))]
                else
                    [amount]
        ]

        member this.Dispense(amount : Currency) =
            let mutable remaining = amount
            for handler in chain do
                remaining <- handler.Dispense(remaining)

            if remaining.Amount > 0 then
                printfn "Cannot dispense remaining amount: %d" remaining.Amount


let atm = v3.ATM()
atm.Dispense(Currency(180))

