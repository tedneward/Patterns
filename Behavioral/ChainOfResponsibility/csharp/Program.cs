using System;

public class Currency
{
    public int Amount { get; set; }

    public Currency(int amt)
    {
        this.Amount = amt;
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        ATMv3 atm = new ATMv3();
        while (true)
        {
            int amount = 0;
            Console.Write("Enter amount to dispense (in multiples of 10): ");
            amount = Convert.ToInt32(Console.ReadLine());
            if (amount % 10 != 0)
            {
                Console.WriteLine("Amount should be in multiple of 10s.");
            }
            else
            {
                atm.Dispense(new Currency(amount));
            }
        }
    }
}


