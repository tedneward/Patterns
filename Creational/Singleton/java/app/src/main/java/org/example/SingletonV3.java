package org.example;

/**
 * Singleton class that uses the enum type to ensure only one instance is created.
 * This approach is thread-safe and provides a global point of access to the instance.
 */
public enum SingletonV3 {
    INSTANCE;

    private int counter = 0;

    // Method to perform some action
    public void doSomething() {
        counter++;
        System.out.println("We are doing something with the singleton, for the " + counter + "th time.");
    }

    // Method to get the current counter value
    public int getCounter() {
        return counter;
    }

}