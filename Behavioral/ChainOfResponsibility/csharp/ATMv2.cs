using System;
using System.Collections.Generic;

public interface Dispenser
{
    Currency Dispense(Currency cur);
}

file class FiftyDollarDispenser : Dispenser
{
    public Currency Dispense(Currency cur)
    {
        if (cur.Amount >= 50)
        {
            int num = cur.Amount / 50;
            int remainder = cur.Amount % 50;

            Console.WriteLine("Dispensing {0} USD$50 note", num);
            return new Currency(remainder);
        }
        else
        {
            return cur;
        }
    }
}
file class TwentyDollarDispenser : Dispenser
{
    public Currency Dispense(Currency cur)
    {
        if (cur.Amount >= 20)
        {
            int num = cur.Amount / 20;
            int remainder = cur.Amount % 20;

            Console.WriteLine("Dispensing {0} USD$20 note", num);
            return new Currency(remainder);
        }
        else
        {
            return cur;
        }
    }
}
file class TenDollarDispenser : Dispenser
{
    public Currency Dispense(Currency cur)
    {
        if (cur.Amount >= 10)
        {
            int num = cur.Amount / 10;
            int remainder = cur.Amount % 10;

            Console.WriteLine("Dispensing {0} USD$10 note", num);
            return new Currency(remainder);
        }
        else
        {
            return cur;
        }
    }
}

public class ATMv2
{
    private List<Dispenser> chain = new List<Dispenser>();

    public ATMv2()
    {
        this.chain.Add(new FiftyDollarDispenser());
        this.chain.Add(new TwentyDollarDispenser());
        this.chain.Add(new TenDollarDispenser());
    }

    public void Dispense(Currency cur)
    {
        foreach (var dispenser in chain)
        {
            cur = dispenser.Dispense(cur);
        }
    }
}