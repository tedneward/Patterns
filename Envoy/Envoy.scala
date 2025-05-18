// This code basically explores Scala as an implementation
// language for the Envoy pattern documented by Eugene
// Wallingford (www.cs.uni.edu/~wallingf/patterns/envoy.pdf)
//

object Envoy {
  def main(args : Array[String]) = {
    val lambda = () => {
      println("Hello, lambda")
    }
    lambda()
  
    println("FunctionAsObject:")
    functionAsObject()
    println("=================")
    println("Closure:")
    closure()
    println("=================")
    println("ConstructorFunction:")
    constructorFunction()
    println("=================")
    println("MethodSelector:")
    methodSelector()
    println("=================")
    println("MessagePassingInterface:")
    messagePassingInterface()
    println("=================")
    println("GenericFunction:")
    genericFunction()
    println("=================")
    println("Delegation:")
    delegation()
    println("=================")
    println("PrivateMethod:")
    privateMethod()
  }
  
  def functionAsObject() = {
    def withdraw(balance : Int, amount : Int) = {
      if (amount <= balance) balance - amount else throw new RuntimeException("Insufficient funds")
    }
    def deposit(balance: Int, amount : Int) = {
      balance + amount
    }
    def accrueInterest(balance : Int, rate : Float) = {
      balance + (balance * rate)
    }
  }
  
  def closure() = {
    val withdraw = (() => {
      var balance = 100
      (amount: Int) => {
        if (amount <= balance) {
          balance -= amount
          balance
        }
        else
          throw new RuntimeException("Insufficient funds")
      }
    })()
    println(withdraw(20))
    println(withdraw(20))
  }
  
  def constructorFunction() = {
    def makeWithdraw(bal : Int) = {
      var balance = bal
      (amt : Int) => {
        if (balance >= amt) {
          balance = (balance - amt) 
          balance
        }
        else 
          throw new RuntimeException("Insufficient funds")
      }
    }
    val acctForEugene = makeWithdraw(100)
    println(acctForEugene(20))
    val acctForTed = makeWithdraw(1000)
    println(acctForTed(20))
  }
  
  def methodSelector() = {
    def makeAccount(bal : Int) = {
      var balance = bal
      (transaction : String) => {
        transaction match {
          case "withdraw" =>
            (amt : Int) => {
              if (balance >= amt) {
                balance = (balance - amt) 
                balance
              }
              else 
                throw new RuntimeException("Insufficient funds")
            }
          case "deposit" => {
            (amt : Int) => {
              balance += amt
              balance
            }
          }
          case _ => 
            throw new RuntimeException("Unknown request")
        }
      }
    }
    val acctForEugene = makeAccount(100)
    println(acctForEugene("deposit")(50))
    val acctForTed = makeAccount(100)
    println(acctForTed("withdraw")(50))
  }
  
  def messagePassingInterface() = {
    def makeAccount(bal : Int) = {
      var balance = bal
      def send(key:String, args:Any*) = {
        key match {
          case "withdraw" => {
            val amt = args.head.asInstanceOf[Int]
            if (balance >= amt) {
              balance = (balance - amt) 
              balance
            }
            else 
              throw new RuntimeException("Insufficient funds")
          }
          case "deposit" => {
            val amt = args.head.asInstanceOf[Int]
            balance += amt
            balance
          }
          case "balance" => {
            balance
          }
        }
      }
      send _
    }
    val acctForEugene = makeAccount(100)
    println(acctForEugene("withdraw", 10))
  }
  
  def genericFunction() = {
    def makeAccount(bal : Int) = {
      var balance = bal
      def send(key:String, args:Any*) = {
        key match {
          case "withdraw" => {
            val amt = args.head.asInstanceOf[Int]
            if (balance >= amt) {
              balance = (balance - amt) 
              balance
            }
            else 
              throw new RuntimeException("Insufficient funds")
          }
          case "deposit" => {
            val amt = args.head.asInstanceOf[Int]
            balance += amt
            balance
          }
          case "balance" => {
            balance
          }
          case _ =>
            throw new RuntimeException("Unknown request")
        }
      }
      send _
    }
    def withdraw(account : (String, Any*) => Int, amount : Int) = {
      account("withdraw", amount)
    }
    def deposit(account : (String, Any*) => Int, amount : Int) = {
      account("deposit", amount)
    }
    def balance(account : (String, Any*) => Int) = {
      account("balance")
    }
    val accounts = List(makeAccount(100), makeAccount(200), makeAccount(300))
    accounts.foreach(withdraw(_, 20))
    accounts.foreach((in) => { println(balance(in)) })
  }

  def delegation() = {
    def makeAccount(bal : Int) = {
      var balance = bal
      def send(key:String, args:Any*) = {
        key match {
          case "withdraw" => {
            val amt = args.head.asInstanceOf[Int]
            if (balance >= amt) {
              balance = (balance - amt) 
              balance
            }
            else 
              throw new RuntimeException("Insufficient funds")
          }
          case "deposit" => {
            val amt = args.head.asInstanceOf[Int]
            balance += amt
            balance
          }
          case "balance" => {
            balance
          }
          case _ =>
            throw new RuntimeException("Unknown request")
        }
      }
      send _
    }
    def makeInterestBearingAccount(bal : Int, intRate : Double) = {
      val account = makeAccount(bal)
      def send(key: String, args:Any*) = {
        key match {
          case "accrueInterest" => {
            val amt = (int2float(account("balance")) * intRate).toInt
            account("deposit", amt)
          }
          case _ =>
            account(key, args : _*)
        }
      }
      send _
    }
    val acctForEugene = makeInterestBearingAccount(100, 0.05)
    println(acctForEugene("deposit", 20))
    println(acctForEugene("accrueInterest"))
    println(acctForEugene("balance"))
  }

  def privateMethod() = {
    def makeAccount(bal : Int) = {
      var balance = bal
      var transactionLog = List[String]()
      def logTransaction(action:String, amount:Int) = {
        val msg = ("Action: " + action + " for " + amount)
        transactionLog = transactionLog :+ msg
      }
      def send(key:String, args:Any*) = {
        key match {
          case "withdraw" => {
            val amt = args.head.asInstanceOf[Int]
            if (balance >= amt) {
              logTransaction("withdraw", amt)
              balance = (balance - amt) 
              balance
            }
            else 
              throw new RuntimeException("Insufficient funds")
          }
          case "deposit" => {
            val amt = args.head.asInstanceOf[Int]
            logTransaction("deposit", amt)
            balance += amt
            balance
          }
          case "balance" => {
            println(transactionLog)
            balance
          }
          case _ =>
            throw new RuntimeException("Unknown request")
        }
      }
      send _
    }
    val acctForEugene = makeAccount(100)
    println(acctForEugene("deposit", 20))
    println(acctForEugene("balance"))
  }
  
}
