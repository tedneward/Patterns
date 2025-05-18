package org.example;

public interface AccountFactory {
    public Savings openSavingsAccount(int initialAmount);
    public Checking openCheckingAccount(int initialAmount);
    public Investment openInvestmentAccount(int initialAmount);
}
