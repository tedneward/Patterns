title=Abstract Factory: Java
date=2022-03-04
type=pattern
tags=pattern implementation, creational, java
status=published
description=An Abstract Factory implementation in Java.
~~~~~~

Java has long had a relationship with [Abstract Factory](../AbstractFactory), usually calling it by the more degenerative term "Factory" or "Factory pattern". (Technically, what Java calls a "Factory pattern" is typically one of [Builder](../Builder/), [Factory Method](../FactoryMethod), or Abstract Factory, depending on what precisely looks to be varied and/or encapsulated.)

The key difference here against the [Factory Method](../FactoryMethod/) lies in the size of the family of things to be created (the product family), and more importantly in the number of families--if there is only one family, then there's not much point to an Abstract Factory, and a Factory Method is probably a better option. In the GOF example, they discuss how one might build a cross-platform GUI toolkit, which (unironically) Java did with AWT (Abstract Windowing Toolkit). Java followed up with a similar approach for building the "look and feels" for the Swing toolkit that was AWT's spiritual successor.

Given that the GOF and Java both did GUI-based examples, let's try for an example that isn't a GUI toolkit. Contrary to what most people remember, the GOF uses a "Maze" example for AbstractFactory, offering up a MazeFactory base class for creating the different parts of a maze (rooms, walls, doors, etc) that is then customized by creating EnchantedMazeFactory and BombedMazeFactory for making enchanted mazes (rooms that cast a spell when you enter) and bomb-happy mazes (where rooms and walls have bombs). That seems a little hard to adjust to a more business-like setting.

In order to select an example, we need a product family that has two dimensions to it, one being a collection of cooperative/"adjacent" product types, the other being an implementation axis. Although the GOF implied that only one product family could be "alive" at a given time (again, given that they used a GUI framework example, one doesn't expect that any application would be running on both macOS and Windows at the same time, so how could two families be available at the same time?), there's really nothing stopping multiple product families from being in use at the same time.

In this case, I choose to create a bit of a spurious example, that of a (over-)simplified banking system that works internationally. Our bank offers Savings, Checking, and Investment accounts, in several different regions (US, Canada, and let's say Sweden just for kicks). Savings accounts can only do deposits and withdrawals, and calculate interest; Checking accounts can do deposits, withdrawals and account-to-account transfers, but calculate zero interest; and Investment accounts can only do deposits (you only "withdraw" when you close the account), and accrue interest as well. However, each country has different tax implications for each kind of account--the US charges no tax on any of them, Canada charges a 10% tax on the interest accrued, and Sweden, being a good socialist country, charges a 50% tax on the total value of the account.

*(DISCLAIMER: I'm making this up out of whole cloth; I am not a financial advisor, nor do I sell accounts. Additionally, note that good socialist countries also don't run GoFundMes to pay for health care. 'Nuff said on that one.)*

Having established our (all-too-brief) parallel product families, we can get to code. In this particular case, we have some established rules about accounts, so these AbstractProduct classes really need to be abstract base classes from which ConcreteProduct subclasses derive and refine the behavior (such as around taxes).

So, let's take a look at our AbstractProduct classes, Savings, Checking, and Investment:

```java
// in Savings.java
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

// in Checking.java
public abstract class Checking {
    private int balance = 0;

    public int deposit(int amount) {
        balance += amount;
        return balance;
    }
    public int withdraw(int amount) {
        if (amount > balance)
            throw new InsufficientFundsException(amount);

        balance -= amount;
        return balance;
    }
    public int cashCheck(int amount) {
        if (amount > balance) {
            balance -= 5000; // $50 US overdraft fee
            throw new InsufficientFundsException(amount);
        }

        balance -= amount;
        return balance;
    }

    public abstract int calculateTax();
}

// in Investment.java
public abstract class Investment {
    int balance = 0;

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
```

For those who are unfamiliar with the rules in Checking, it's because our bank provides "overdraft protection": we charge you a $50 fee for writing a check when you have insufficient funds in the account, and then don't let the check go through anyway. Somehow, this is considered a feature.

We can then use the AbstractFactory to construct a portfolio of accounts, splitting an initial seed amount among all three accounts, like so:

```java
public interface AccountFactory {
    public Savings openSavingsAccount(int initialAmount);
    public Checking openCheckingAccount(int initialAmount);
    public Investment openInvestmentAccount(int initialAmount);
}
```

This could also be an abstract base class, if there was specific behavior about construction that was common to all the different product families; that might be the case here, if we consider opening the account means it always has a balance of 0, and then we do an initial deposit into the account. This approach would also help keep the ConcreteProduct classes from needing parameterized constructors, simplifying things a little.

Using the AccountFactory then, is pretty straightforward:

```java
public class Driver {
    public static void buildAccountPortfolio(AccountFactory af, int totalAmount) {
        // Open a SavingsAccount, CheckingAccount, and InvestmentAccount
        af.openSavingsAccount(totalAmount / 3);
        af.openCheckingAccount(totalAmount / 3);
        af.openInvestmentAccount(totalAmount / 3);
    }
}
```

***(TODO: USAccountFactory, CanadianAccountFactory, SwedishAccountFactory, and concrete implementations of each)***

