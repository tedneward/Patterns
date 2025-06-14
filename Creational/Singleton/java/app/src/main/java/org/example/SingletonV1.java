package org.example;

/**
 * Singleton class that ensures only one instance is created.
 * This class uses the eager initialization method to create the instance.
 */
public class SingletonV1 {
    private static SingletonV1 instance = new SingletonV1();
    private SingletonV1() {
        // Private constructor to prevent instantiation
    }
    public static SingletonV1 getInstance() {
        return instance;
    }

    private int counter = 0;
    public int getCounter() {
        return counter;
    }

    public void doSomething() {
        counter++;
        System.out.println("We are doing something with the singleton, for the " + counter + "th time.");
    }
}
