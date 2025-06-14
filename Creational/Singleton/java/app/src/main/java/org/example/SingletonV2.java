package org.example;

public class SingletonV2 {
    private static SingletonV2 instance;

    // Private constructor to prevent instantiation
    private SingletonV2() {
        // Initialization code here
    }

    // Public method to provide access to the instance
    public synchronized static SingletonV2 getInstance() {
        if (instance == null) {
            instance = new SingletonV2();
        }
        return instance;
    }

    private int counter = 0;

    public void doSomething() {
        counter++;
        System.out.println("We are doing something with the singleton, for the " + counter + "th time.");
    }
    public int getCounter() {
        return counter;
    }
}
