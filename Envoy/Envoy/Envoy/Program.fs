// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open System

let example = fun () ->
    Console.WriteLine "Howdy world"

let functionAsObject = fun () ->
    let balance = ref 0
    let withdraw = 
        fun amt ->
            if amt <= !balance then
                balance := (!balance) - amt
                !balance
            else
                raise (Exception("Insufficient funds"))
    let deposit = 
        fun amt -> 
            balance := (!balance) + amt
            !balance
    let accrueInterest = 
        fun (intRate : float) -> 
            balance := (!balance) + (int (float !balance * intRate))
            !balance
    
    Console.WriteLine "=========> Function as Object"
    printfn "%d" (deposit 200)
    printfn "%d" (withdraw 50)

let closure = fun () ->
    let withdraw =
        let balance = ref 100
        fun amt ->
            if amt <= !balance then
                balance := (!balance) - amt
                !balance
            else
                raise (Exception("Insufficient funds"))

    Console.WriteLine "=========> Closure"
    printfn "%d" (withdraw 20)
    printfn "%d" (withdraw 30)

let constructorFunction = fun () ->
    let makeAccount =
        fun bal ->
            let balance = ref bal
            fun amt ->
                if amt <= !balance then
                    balance := (!balance) - amt
                    !balance
                else
                    raise (Exception("Insufficient funds"))                
            
    Console.WriteLine "=========> Constructor Function"
    let acctForEugene = makeAccount 100
    printfn "%d" (acctForEugene 20)

let methodSelector = fun () ->
    let makeAccount =
        fun (bal : int) ->
            let balance = ref bal
            fun transaction ->
                match transaction with
                | "balance" ->
                    fun _ -> !balance
                | "deposit" ->
                    fun (amt : int) ->
                        balance := (!balance) + amt
                        !balance
                | "withdraw" ->
                    fun (amt : int) ->
                        if amt <= !balance then
                            balance := (!balance) - amt
                            !balance
                        else
                            raise (Exception("Insufficient funds"))
                | _ ->
                    raise (Exception("Unrecognized operation" + transaction))
                            
    Console.WriteLine "=========> Method Selector"
    let acctForEugene = makeAccount 100
    printfn "%d" ((acctForEugene "withdraw") 20)
    printfn "%d" ((acctForEugene "balance") 0)

let messagePassingInterface = fun () ->
    let makeAccount =
        fun (bal : int) ->
            let balance = ref bal
            fun transaction ->
                match transaction with
                | "balance" ->
                    fun _ -> !balance
                | "deposit" ->
                    fun (arglist : obj list) ->
                        let amt = arglist.Head :?> int
                        balance := (!balance) + amt
                        !balance
                | "withdraw" ->
                    fun (arglist : obj list) ->
                        let amt = arglist.Head :?> int
                        if amt <= !balance then
                            balance := (!balance) - amt
                            !balance
                        else
                            raise (Exception("Insufficient funds"))
                | _ ->
                    raise (Exception("Unrecognized operation" + transaction))
    let getMethod = fun (acct : string -> obj list -> int) selector -> acct selector
    let send = 
        fun (acct : string -> obj list -> int) (message : string) (arglist : obj list) ->
            (getMethod acct message)(arglist)

    Console.WriteLine "=========> Message Passing Interface"
    let acctForEugene = makeAccount 100
    printfn "%d" (send acctForEugene "withdraw" [20])
    printfn "%d" (send acctForEugene "balance" [])

let genericFunction = fun () ->
    let makeAccount =
        fun (bal : int) ->
            let balance = ref bal
            fun transaction ->
                match transaction with
                | "balance" ->
                    fun _ -> !balance
                | "deposit" ->
                    fun (arglist : obj list) ->
                        let amt = arglist.Head :?> int
                        balance := (!balance) + amt
                        !balance
                | "withdraw" ->
                    fun (arglist : obj list) ->
                        let amt = arglist.Head :?> int
                        if amt <= !balance then
                            balance := (!balance) - amt
                            !balance
                        else
                            raise (Exception("Insufficient funds"))
                | _ ->
                    raise (Exception("Unrecognized operation" + transaction))
    let deposit = 
        fun amt acct->
            acct "deposit" [amt :> obj]
    let withdraw =
        fun amt acct ->
            acct "withdraw" [amt :> obj]
    let balance =
        fun acct ->
            acct "balance" []

    Console.WriteLine "=========> Generic Function"
    let bank = [ makeAccount 100; makeAccount 200; makeAccount 300 ]
    let balances = List.map (fun it -> deposit 20 it) bank
    List.iter (fun it -> printfn "%d" it) balances
    let balances = List.map (deposit 20) bank
    List.iter (printfn "%d") balances

let delegation = fun () ->
    let makeAccount =
        fun (bal : int) ->
            let balance = ref bal
            fun transaction ->
                match transaction with
                | "balance" ->
                    fun _ -> !balance
                | "deposit" ->
                    fun (arglist : obj list) ->
                        let amt = arglist.Head :?> int
                        balance := (!balance) + amt
                        !balance
                | "withdraw" ->
                    fun (arglist : obj list) ->
                        let amt = arglist.Head :?> int
                        if amt <= !balance then
                            balance := (!balance) - amt
                            !balance
                        else
                            raise (Exception("Insufficient funds"))
                | _ ->
                    raise (Exception("Unrecognized operation" + transaction))
    let makeInterestBearingAccount =
        fun (bal : int) (intRate : float) ->
            let account = makeAccount bal
            fun transaction ->
                match transaction with
                | "accrueInterest" ->
                    fun _ ->
                        let balance = (account "balance" [])
                        let interest : float = float balance * intRate 
                        account "deposit" [int interest]
                | _ -> account transaction
    Console.WriteLine "=========> Delegation"
    let acctForEugene = makeInterestBearingAccount 100 0.05
    printfn "%d" (acctForEugene "deposit" [20])
    printfn "%d" (acctForEugene "accrueInterest" [])

let privateMethod = fun () ->
    let makeAccount =
        fun (bal : int) ->
            let transactionLog = ref []
            let logTransaction act (amt : int) =
                let message = "Action: " + act + " for " + amt.ToString()
                transactionLog := List.append !transactionLog [message]
            let balance = ref bal
            fun transaction ->
                match transaction with
                | "balance" ->
                    fun _ -> 
                        List.iter (printfn "%s") !transactionLog
                        !balance
                | "deposit" ->
                    fun (arglist : obj list) ->
                        let amt = arglist.Head :?> int
                        balance := (!balance) + amt
                        logTransaction "deposit" amt
                        !balance
                | "withdraw" ->
                    fun (arglist : obj list) ->
                        let amt = arglist.Head :?> int
                        if amt <= !balance then
                            balance := (!balance) - amt
                            logTransaction "withdraw" amt
                            !balance
                        else
                            raise (Exception("Insufficient funds"))
                | _ ->
                    raise (Exception("Unrecognized operation" + transaction))
    Console.WriteLine "=========> Private Method"
    let acctForEugene = makeAccount 100
    printfn "%d" (acctForEugene "deposit" [20])
    printfn "%d" (acctForEugene "withdraw" [50])
    printfn "%d" (acctForEugene "balance" [])

[<EntryPoint>]
let main argv = 
    functionAsObject()
    closure()
    constructorFunction()
    methodSelector()
    messagePassingInterface()
    genericFunction()
    delegation()
    privateMethod()

    0 // return an integer exit code
