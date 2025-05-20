package org.example;

public class GuardedFluentBuilder {
    private Product product;
    public GuardedFluentBuilder begin() {
        product = new Product();
        return this;
    }
    public GuardedFluentBuilder engine() {
        product.parts.add("Engine");
        return this;
    }
    public GuardedFluentBuilder steeringWheel() {
        product.parts.add("Steering Wheel");
        return this;
    }
    public GuardedFluentBuilder tire() {
        product.parts.add("Tire");
        return this;
    }
    public Product build() throws IllegalStateException {
        if (product.parts.size() < 4)
            throw new IllegalStateException("Product must have at least 4 parts");
            
        if (product.parts.indexOf("Engine") < 0)
            throw new IllegalStateException("Product must have an Engine");
        
        return product;
    }
}

