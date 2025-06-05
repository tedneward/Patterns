package org.example;

public abstract class Checking {
    protected int balance = 0;

    public Checking(int initialAmount) {
        this.balance = initialAmount;
    }

    public int deposit(int amount) {
        balance += amount;
        return balance;
    }
    public int withdraw(int amount) {
        if (amount > balance)
            throw new IllegalStateException("You are short by " + (balance - amount) + " dollars.");

        balance -= amount;
        return balance;
    }
    public int cashCheck(int amount) {
        if (amount > balance) {
            balance -= 5000; // $50 US overdraft fee
            throw new IllegalStateException("You are short by " + (balance - amount) + " dollars.");
        }

        balance -= amount;
        return balance;
    }

    public abstract int calculateTax();
}
