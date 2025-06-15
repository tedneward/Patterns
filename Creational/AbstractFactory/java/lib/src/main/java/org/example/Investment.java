package org.example;

public abstract class Investment {
    protected int balance = 0;

    public Investment(int initialAmount) {
        this.balance = initialAmount;
    }
    public int deposit(int amount) {
        balance += amount;
        return balance;
    }
    public int accrueInterest() {
        balance += ((int)(balance * 0.30));
        return balance;
    }

    public abstract int calculateTax();
}
