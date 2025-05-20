package org.example;

public class ConcreteBuilder extends Builder {
    @Override
    public void buildPart() {
        product.parts.add("Part #" + (part++));
    }

    @Override
    public Product result() {
        return product;
    }

    private int part = 0;
    private Product product = new Product();
}
