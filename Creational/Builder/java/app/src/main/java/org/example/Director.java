package org.example;

public class Director {
    public Product construct() {
        for (int i = 0; i < 5; i++)
            builder.buildPart();
        
        return builder.result();
    }

    private Builder builder = new ConcreteBuilder();
}
