package org.example;

public abstract class Savings {
    protected int balance = 0;

    public Savings(int initialAmount) {
        this.balance = initialAmount;
    }

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
