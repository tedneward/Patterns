package org.example;

public abstract class Savings {
    private int balance = 0;

    public int deposit(int amount) {
        balance += amount;
        return balance;
    }
    public int withdraw(int amount) {
        balance -= amount;
        return balance;
    }
    public int accrueInterest() {
        balance = balance + ((int)(balance * 0.10));
        return balance;
    }

    public abstract int calculateTax();
}
