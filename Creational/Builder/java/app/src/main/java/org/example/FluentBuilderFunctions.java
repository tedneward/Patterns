package org.example;

import java.util.function.*;

public class FluentBuilderFunctions {
    private Function<Product, Product> fn;

    public FluentBuilderFunctions begin() {
        fn = (ignored) -> { return new Product(); };

        return this;
    }

    public FluentBuilderFunctions engine() {
        fn = fn.andThen((product) -> {
            product.parts.add("Engine");
            return product;
        });
        return this;
    }

    public FluentBuilderFunctions steeringWheel() {
        fn = fn.andThen((product) -> {
            product.parts.add("Steering Wheel");
            return product;
        });
        return this;
    }
    public FluentBuilderFunctions tire() {
        fn = fn.andThen((product) -> {
            product.parts.add("Tire");
            return product;
        });
        return this;
    }
    public FluentBuilderFunctions tire(int numberOfTires) {
        fn = fn.andThen((product) -> {
            for (int i=0; i<numberOfTires; i++)
                product.parts.add("Tire");
            return product;
        });
        return this;
    }
    
    public Product end() {
        return fn.apply(null);
    }
}
