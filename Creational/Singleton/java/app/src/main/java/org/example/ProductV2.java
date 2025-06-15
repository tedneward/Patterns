package org.example;

import java.util.concurrent.locks.Lock;
import java.util.concurrent.locks.ReentrantLock;

public class ProductV2 {
    private static ProductV2 instance;

    // Private constructor to prevent instantiation
    private ProductV2() {
        // Initialization code here
    }

    // Public method to provide access to the instance
    public synchronized static ProductV2 getInstance() {
        if (instance == null) {
            instance = new ProductV2();
        }
        return instance;
    }

    private int counter = 0;
    private Lock lock = new ReentrantLock();

    public void doSomething() {
        lock.lock();
        try {
            counter++;
            System.out.println("We are doing something with the Product, for the " + counter + "th time.");
        }
        finally {
            lock.unlock();
        }
    }
    public int getCounter() {
        return counter;
    }
}
