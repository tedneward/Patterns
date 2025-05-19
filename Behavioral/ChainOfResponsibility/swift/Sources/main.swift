struct Currency {
    let amount: Double
}

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

let atm = ATM()
atm.dispense(request: Currency(amount: 190))
