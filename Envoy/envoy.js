var out = (function() {
  if (typeof(console) !== "undefined" &&
      typeof(console.log) !== "undefined")
    return console.log
  else if (typeof(println) !== "undefined")
    return println
  else
    throw new Error("No idea what to use for output")
})();

(function() {
  out("function-as-object =========")
  
  var balance = 0
  var withdraw = function(amount) {
    if (amount <= balance)
      balance = balance - amount
    else
      throw new Error("Insufficient funds")
  }
  var deposit = function(amount) {
    balance += amount
  }
  var accrueInterest = function(interestRate) {
    balance += (balance * interestRate)
  }
})();

(function() {
  out("closure ====================")

  var withdraw = function() {
    var balance = 100
    return function(amount) {
      if (balance >= amount) {
        balance -= amount
        return balance
      }
      else
        throw new Error("Insufficient funds")
    }
  }()
  out("withdraw 20 " + withdraw(20))
  out("withdraw 30 " + withdraw(30))
})();

(function() {
  out("constructorFunction ========")

  var makeWithdraw = function(balance) {
    return function(amount) {
      if (balance >= amount) {
        balance -= amount
        return balance
      }
      else
        throw new Error("Insufficient funds")
    }
  }
  var acctForEugene = makeWithdraw(100)
  out(acctForEugene(20))
  var acctForTom = makeWithdraw(1000)
  out(acctForTom(20))
})();

(function() {
  out("methodSelector ========")

  var makeAccount = function(bal) {
    var balance = bal
    return function(transaction) {
      if (transaction === "withdraw") {
        return function(amount) {
          if (balance >= amount)
            return (balance = (balance - amount))
          else
            throw new Error("Insufficient funds")
        }
      }
      else if (transaction === "deposit") {
        return function(amount) {
          return (balance = (balance + amount))
        }
      }
      else if (transaction === "balance") {
        return function() {
          return balance
        }
      }
      else {
        throw new Error("Insufficient funds")
      }
    }
  }
  var acctForEugene = makeAccount(100)
  out(acctForEugene("withdraw")(20))
  out(acctForEugene("balance")())
  out(acctForEugene("deposit")(20))
  out(acctForEugene("balance")())
})();

(function() {
  out("messagePassingInterface ========")

  var slice = function(src, start, end) {
    var returnVal = []
    var j = 0
    if (end === undefined)
      end = src.length
    for (var i = start; i < end; i++) {
      if (src.length > i)
        returnVal[j++] = src[i]
    }
    return returnVal;
  }
  
  var makeAccount = function(bal) {
    var balance = bal
    return function(transaction) {
      if (transaction === "withdraw") {
        return function(amount) {
          if (balance >= amount)
            return (balance = (balance - amount))
          else
            throw new Error("Insufficient funds")
        }
      }
      else if (transaction === "deposit") {
        return function(amount) {
          return (balance = (balance + (amount * 10.0 / 10.0)))
        }
      }
      else if (transaction === "balance") {
        return function() {
          return balance
        }
      }
      else {
        throw new Error("Insufficient funds")
      }
    }
  }
  var getMethod = function(object, selector) {
    return object(selector)
  }
  var send = function(object, message) {
    return (getMethod(object, message))(slice(arguments, 2))
  }
  var acctForEugene = makeAccount(100)
  out(send(acctForEugene, "withdraw", 20))
  out(send(acctForEugene, "balance"))
  out(send(acctForEugene, "deposit", 20))
  out(send(acctForEugene, "balance"))
})();


(function() {
  out("genericFunction ==============")

  var slice = function(src, start, end) {
    var returnVal = []
    var j = 0
    if (end === undefined)
      end = src.length
    for (var i = start; i < end; i++) {
      if (src.length > i)
        returnVal[j++] = src[i]
    }
    return returnVal
  }
  
  var map = function(fn, src) {
    var retVal = []
    for (i in src)
      retVal[i] = fn(src[i])
    return retVal
  }
  var map2 = function(src, fn) {
    var retVal = []
    for (i in src)
      retVal[i] = fn(src[i], slice(arguments, 2))
    return retVal
  }
  
  var makeAccount = function(bal) {
    var balance = bal
    return function(transaction) {
      if (transaction === "withdraw") {
        return function(amount) {
          if (balance >= amount)
            return (balance = (balance - amount))
          else
            throw new Error("Insufficient funds")
        }
      }
      else if (transaction === "deposit") {
        return function(amount) {
          return (balance = (balance + (amount * 10.0 / 10.0)))
        }
      }
      else if (transaction === "balance") {
        return function() {
          return balance
        }
      }
      else {
        throw new Error("Insufficient funds")
      }
    }
  }
  var withdraw = function() {
    var object = arguments[0]
    var argumentList = slice(arguments, 1)
    return object("withdraw")(argumentList)
  }
  var deposit = function() {
    var object = arguments[0]
    var argumentList = slice(arguments, 1)
    return object("deposit")(argumentList)
  }
  var balance = function(object) {
    return object("balance")()
  }

  var acctForEugene = makeAccount(100)
  out(withdraw(acctForEugene, 20))
  out(deposit(acctForEugene, 20))
  
  var bank = [
    makeAccount(100),  // acctForEugene
    makeAccount(1000)  // acctForTed
  ]
  map(function(it) { deposit(it, 20) }, bank)
  out(balance(bank[0]))
  out(balance(bank[1]))
  
  map2(bank, deposit, 20)
  out(balance(bank[0]))
  out(balance(bank[1]))
})();

(function() {
  out("delegation =======")
  
  var slice = function(src, start, end) {
    var returnVal = []
    var j = 0
    if (end === undefined)
      end = src.length
    for (var i = start; i < end; i++) {
      if (src.length > i)
        returnVal[j++] = src[i]
    }
    return returnVal
  }
  
  var makeAccount = function(bal) {
    var balance = bal
    return function(transaction) {
      if (transaction === "withdraw") {
        return function(amount) {
          if (balance >= amount)
            return (balance = (balance - amount))
          else
            throw new Error("Insufficient funds")
        }
      }
      else if (transaction === "deposit") {
        return function(amount) {
          return (balance = (balance + (amount * 10.0 / 10.0)))
        }
      }
      else if (transaction === "balance") {
        return function() {
          return balance
        }
      }
      else {
        throw new Error("Insufficient funds")
      }
    }
  }
  var makeInterestBearingAccount = function(bal, intRate) {
    var myAccount = makeAccount(bal)
    return function(transaction) {
      if (transaction === "accrueInterest") {
        return function() {
          var balance = myAccount("balance")()
          var interest = (balance * intRate)
          return myAccount("deposit")(interest)
        }
      }
      else
        return myAccount(transaction)
    }
  }
  
  var acctForEugene = makeInterestBearingAccount(100, 0.05)
  out(acctForEugene("balance")())
  out(acctForEugene("deposit")(20))
  out(acctForEugene("accrueInterest")())
  out(acctForEugene("balance")())
})();

(function() {
  out("privateMethod ===========")
  
  var makeAccount = function(bal) {
    var transactionLog = []
    var logTransaction = function(type, amount) {
      transactionLog.push("Action: " + type + " for " + amount)
    }
    
    var balance = bal
    return function(transaction) {
      if (transaction === "withdraw") {
        return function(amount) {
          if (balance >= amount) {
            logTransaction("withdraw", amount)
            return (balance = (balance - amount))
          }
          else
            throw new Error("Insufficient funds")
        }
      }
      else if (transaction === "deposit") {
        return function(amount) {
          logTransaction("deposit", amount)
          return (balance = (balance + (amount * 10.0 / 10.0)))
        }
      }
      else if (transaction === "balance") {
        return function() {
          logTransaction("balance", balance)
          return balance
        }
      }
      else if (transaction === "viewLog") {
        return function() {
          return (transactionLog)
        }
      }
      else {
        throw new Error("Insufficient funds")
      }
    }
  }
  var acctForEugene = makeAccount(100)
  out(acctForEugene("withdraw")(20))
  out(acctForEugene("balance")())
  out(acctForEugene("deposit")(20))
  out(acctForEugene("balance")())
  out(acctForEugene("viewLog")())
})();




