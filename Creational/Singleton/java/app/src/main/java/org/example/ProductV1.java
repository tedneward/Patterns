package org.example;

/**
 * Singleton class that ensures only one instance is created.
 * This class uses the eager initialization method to create the instance.
 */
public class ProductV1 {
  private static Product instance = new Product();
  private Product() { }
  public static Product instance() {
    return instance;
  }
  
  private int state = 0;
  public int getState() {
    return state;
  }
  
  public void doSomething() {
    state++;
    System.out.println("I'm doing something for the " + state + " time");
  }
}
