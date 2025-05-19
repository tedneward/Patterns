struct Currency {
    let amount: Double
}

struct v1 {
    protocol ATMHandler {
        
        var nextHandler: ATMHandler? { get set }
        
        func handle(request: Currency)
    }

    class ATM {
        private var chain: ATMHandler?

        init() {
            let fiftyDollarHandler = FiftyDollarHandler()
            let twentyDollarHandler = TwentyDollarHandler()
            let tenDollarHandler = TenDollarHandler()
            
            fiftyDollarHandler.nextHandler = twentyDollarHandler
            twentyDollarHandler.nextHandler = tenDollarHandler
            
            chain = fiftyDollarHandler
        }
        
        func dispense(request: Currency) {
            chain?.handle(request: request)
        }
    }

    class FiftyDollarHandler: ATMHandler {
        var nextHandler: ATMHandler?
        
        func handle(request: Currency) {
            let count = Int(request.amount / 50)
            if count > 0 {
                print("Dispensing \(count) $50 bill(s)")
            }
            let remainingAmount = request.amount - Double(count * 50)
            if remainingAmount > 0 {
                nextHandler?.handle(request: Currency(amount: remainingAmount))
            }
        }
    }
    class TwentyDollarHandler: ATMHandler {
        var nextHandler: ATMHandler?
        
        func handle(request: Currency) {
            let count = Int(request.amount / 20)
            if count > 0 {
                print("Dispensing \(count) $20 bill(s)")
            }
            let remainingAmount = request.amount - Double(count * 20)
            if remainingAmount > 0 {
                nextHandler?.handle(request: Currency(amount: remainingAmount))
            }
        }
    }
    class TenDollarHandler: ATMHandler {
        var nextHandler: ATMHandler?
        
        func handle(request: Currency) {
            let count = Int(request.amount / 10)
            if count > 0 {
                print("Dispensing \(count) $10 bill(s)")
            }
            let remainingAmount = request.amount - Double(count * 10)
            if remainingAmount > 0 {
                nextHandler?.handle(request: Currency(amount: remainingAmount))
            }
        }
    }

}

struct v2 {
    protocol ATMHandler {        
        func handle(request: Currency) -> Currency
    }

    class ATM {
        private var chain: [ATMHandler] = []

        init() {
            let fiftyDollarHandler = FiftyDollarHandler()
            let twentyDollarHandler = TwentyDollarHandler()
            let tenDollarHandler = TenDollarHandler()
            
            chain += [fiftyDollarHandler, twentyDollarHandler, tenDollarHandler]
        }
        
        func dispense(request: Currency) {
            var amount = request
            for handler in chain {
                amount = handler.handle(request: amount)
            }        
        }
    }

    class FiftyDollarHandler: ATMHandler {
        func handle(request: Currency) -> Currency{
            let count = Int(request.amount / 50)
            let remainder = request.amount - Double(count * 50)
            if count > 0 {
                print("Dispensing \(count) $50 bill(s)")
            }
            return Currency(amount: remainder)
        }
    }
    class TwentyDollarHandler: ATMHandler {
        func handle(request: Currency) -> Currency{
            let count = Int(request.amount / 20)
            let remainder = request.amount - Double(count * 50)
            if count > 0 {
                print("Dispensing \(count) $20 bill(s)")
            }
            return Currency(amount: remainder)
        }
    }
    class TenDollarHandler: ATMHandler {
        func handle(request: Currency) -> Currency {
            let count = Int(request.amount / 10)
            let remainder = request.amount - Double(count * 50)
            if count > 0 {
                print("Dispensing \(count) $10 bill(s)")
            }
            return Currency(amount: remainder)
        }
    }
}

struct v3 {
    typealias ATMHandler = (Currency) -> Currency

    class ATM {
        private var chain: [ATMHandler] = []

        init() {
            let fiftyDollarHandler: ATMHandler = { request in
                let count = Int(request.amount / 50)
                let remainder = request.amount - Double(count * 50)
                if count > 0 {
                    print("Dispensing \(count) $50 bill(s)")
                }
                return Currency(amount: remainder)
            }
            
            let twentyDollarHandler: ATMHandler = { request in
                let count = Int(request.amount / 20)
                let remainder = request.amount - Double(count * 20)
                if count > 0 {
                    print("Dispensing \(count) $20 bill(s)")
                }
                return Currency(amount: remainder)
            }
            
            let tenDollarHandler: ATMHandler = { request in
                let count = Int(request.amount / 10)
                let remainder = request.amount - Double(count * 10)
                if count > 0 {
                    print("Dispensing \(count) $10 bill(s)")
                }
                return Currency(amount: remainder)
            }
            
            chain += [fiftyDollarHandler, twentyDollarHandler, tenDollarHandler]
        }
        
        func dispense(request: Currency) {
            var amount = request
            for handler in chain {
                amount = handler(amount)
            }        
        }
    }
}

let atm = v3.ATM()
atm.dispense(request: Currency(amount: 190))
