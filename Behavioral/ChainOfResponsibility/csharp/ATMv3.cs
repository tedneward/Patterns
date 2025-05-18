using System;
using System.Collections.Generic;

public class ATMv3
{
    private List<Func<Currency, Currency>> chain;

    public ATMv3()
    {
        chain = new List<Func<Currency, Currency>>
        {
            (cur) => {
                if (cur.Amount >= 50)
                {
                    int num = cur.Amount / 50;
                    int remainder = cur.Amount % 50;

                    Console.WriteLine("Dispensing {0} USD$50 note", num);
                    return new Currency(remainder);
                }
                return cur;
            },
            (cur) => {
                if (cur.Amount >= 20)
                {
                    int num = cur.Amount / 20;
                    int remainder = cur.Amount % 20;

                    Console.WriteLine("Dispensing {0} USD$20 note", num);
                    return new Currency(remainder);
                }
                return cur;
            },
            (cur) => {
                if (cur.Amount >= 10)
                {
                    int num = cur.Amount / 10;
                    int remainder = cur.Amount % 10;

                    Console.WriteLine("Dispensing {0} USD$10 note", num);
                    return new Currency(remainder);
                }
                return cur;
            }
        };
    }

    public void Dispense(Currency cur)
    {
        foreach (var dispense in chain)
        {
            cur = dispense(cur);
            if (cur.Amount == 0)
                break;
        }
    }
}
