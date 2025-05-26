package org.example;

public class FluentBuilder {
    private Product product;
    public FluentBuilder begin() {
        product = new Product();
        return this;
    }
    public FluentBuilder engine() {
        product.parts.add("Engine");
        return this;
    }
    public FluentBuilder steeringWheel() {
        product.parts.add("Steering Wheel");
        return this;
    }
    public FluentBuilder tire() {
        product.parts.add("Tire");
        return this;
    }
    public Product build() {
        return product;
    }
}
