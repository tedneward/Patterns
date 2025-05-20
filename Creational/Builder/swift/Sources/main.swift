class Product {
  var parts : String = ""
}

class Builder {
  func BuildPart() {
    preconditionFailure("Should never invoke this")
  }
  func GetResult() -> Product {
    preconditionFailure("Should never invoke this")
  }
}

class ConcreteBuilder : Builder {
  override func BuildPart() {
    part += 1
    product.parts = product.parts + "Adding part #\(part)\n"
  }
  override func GetResult() -> Product {
    return product
  }
  
  var part = 0
  var product = Product()
}

class Director {
  func Construct() -> Product {
    for _ in 1 ... 5 {
      builder.BuildPart()
    }
    
    return builder.GetResult()
  }
  
  let builder = ConcreteBuilder()
}

class FluentBuilder {
  var product = Product()
  func Begin() -> FluentBuilder {
    product = Product()
    return self
  }
  func Engine() -> FluentBuilder {
    product.parts = product.parts + "Engine\n"
    return self
  }
  func SteeringWheel() -> FluentBuilder {
    product.parts = product.parts + "SteeringWheel\n"
    return self
  }
  func Tire() -> FluentBuilder {
    product.parts = product.parts + "Tire\n"
    return self
  }
  func Construct() -> Product { return product }
}


class SteppedFluentBuilder {
  var steps : [(Product) -> Product] = []

  func Begin() -> SteppedFluentBuilder {
    steps.append({ignored in return Product()})
    return self
  }
  func Engine() -> SteppedFluentBuilder {
    steps.append({product in
      product.parts = product.parts + "Engine\n"
      return product
    })
    return self
  }
  func SteeringWheel() -> SteppedFluentBuilder {
    steps.append({product in
      product.parts = product.parts + "SteeringWheel\n"
      return product
    })
    return self
  }
  func Tire() -> SteppedFluentBuilder {
    steps.append({product in
      product.parts = product.parts + "Tire\n"
      return product
    })
    return self
  }
  func Construct() -> Product { 
    var working : Product = Product()
    for step in steps {
      working = step(working)
    }
    return working
  }
}

func compose<A, B, C>(_ f1: @escaping ((A) -> B), _ f2: @escaping ((B) -> C)) -> (A) -> C {
  return { a in f2(f1(a)) }
}

class FnFluentBuilder {
  var fn : (Product) -> Product = 
    { ignored in return Product() }

  func Begin() -> FnFluentBuilder {
    fn = { ignored in return Product() }
    return self
  }
  func Engine() -> FnFluentBuilder {
    fn = compose(fn, { product in
      product.parts = product.parts + "Engine\n"
      return product
    })
    return self
  }
  func SteeringWheel() -> FnFluentBuilder {
    fn = compose(fn, { product in
      product.parts = product.parts + "SteeringWheel\n"
      return product
    })
    return self
  }
  func Tire() -> FnFluentBuilder {
    fn = compose(fn, { product in
      product.parts = product.parts + "Tire\n"
      return product
    })
    return self
  }
  func Construct() -> Product { 
    return fn(Product())
  }
}

class GuardedFluentBuilder {
  var product = Product()
  func Begin() -> GuardedFluentBuilder {
    product = Product()
    return self
  }
  func Engine() -> GuardedFluentBuilder {
    product.parts = product.parts + "Engine\n"
    return self
  }
  func SteeringWheel() -> GuardedFluentBuilder {
    product.parts = product.parts + "SteeringWheel\n"
    return self
  }
  func Tire() -> GuardedFluentBuilder {
    product.parts = product.parts + "Tire\n"
    return self
  }
  func Construct() -> Product { 
    if !(product.parts.contains("Engine")) {
      preconditionFailure("Products must have an engine")
    }
    
    return product 
  }
}

// The Swift Programming Language
// https://docs.swift.org/swift-book

let director = Director()
let product = director.Construct()
print(product.parts)

let vehicleBuilder = FluentBuilder()
let motorcycle = vehicleBuilder.Begin()
  .Engine()
  .SteeringWheel()
  .Tire()
  .Tire()
  .Construct()
print(motorcycle.parts)
